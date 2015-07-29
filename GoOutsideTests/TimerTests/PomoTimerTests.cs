using System;
using System.Collections;
using GoOutside.Events;
using GoOutside.Scheduling;
using GoOutside.Timers;
using Moq;
using NUnit.Framework;

namespace GoOutsideTests.TimerTests
{
    [TestFixture]
    class PomoTimerTests
    {
        private Mock<IDispatcherTimer> _MockDispatcherTimer;
        private PomoTimer _PomoTimer;
        private Mock<ITimeProvider> _MockTimeProvider;
        private Mock<IConfiguration> _MockConfiguration;
        private Mock<PomoTimerTickEventHandler> _MockPomoTimerTickHandler;

        [SetUp]
        public void Create()
        {
            _MockConfiguration = new Mock<IConfiguration>();

            _MockDispatcherTimer = new Mock<IDispatcherTimer>();
            _MockDispatcherTimer.Setup(m => m.Start())
                .Callback(() => _MockDispatcherTimer.SetupGet(x => x.IsEnabled).Returns(true));
            _MockDispatcherTimer.Setup(m => m.Stop())
                .Callback(() => _MockDispatcherTimer.SetupGet(x => x.IsEnabled).Returns(false));

            _MockTimeProvider = new Mock<ITimeProvider>();
            _MockTimeProvider.Setup(m => m.CreateDispatcherTimer()).Returns(_MockDispatcherTimer.Object);

            _PomoTimer = new PomoTimer(_MockConfiguration.Object, _MockTimeProvider.Object);

            _MockPomoTimerTickHandler = new Mock<PomoTimerTickEventHandler>();
        }

        [Test]
        public void Create_SetsUpCorrectly()
        {
            _MockTimeProvider.Verify(m => m.CreateDispatcherTimer(), Times.Once);
            _MockDispatcherTimer.VerifySet(m => m.Interval = It.Is<TimeSpan>(x => x == TimeSpan.FromMilliseconds(250)));
            Assert.That(_PomoTimer.Running, Is.False);
        }

        [Test]
        public void Start_StartsDispatcherTimer()
        {
            _PomoTimer.Start();

            _MockDispatcherTimer.Verify(m => m.Start(), Times.Once);
        }

        [Test]
        public void Start_SetsRunning()
        {
            _PomoTimer.Start();

            Assert.That(_PomoTimer.Running, Is.True);
        }

        [Test]
        public void Stop_StopsDispatcherTimer()
        {
            _PomoTimer.Stop();

            _MockDispatcherTimer.Verify(m => m.Stop(), Times.Once);
        }

        [Test]
        public void Stop_SetsRunning()
        {
            _PomoTimer.Stop();

            Assert.That(_PomoTimer.Running, Is.False);
        }

        [Test]
        public void Stop_SendsStateChangedEvent()
        {
            var mockHandler = new Mock<PomoTimerStateChangeEventHandler>();
            _PomoTimer.StateChanged += mockHandler.Object;

            _PomoTimer.Start();
            _PomoTimer.Stop();

            mockHandler.Verify(m => m(_PomoTimer,
                It.Is<PomoTimerStateChangeEventArgs>(x => x.State == PomoTimerState.Disabled)), Times.Once);
        }

        [Test]
        public void Start_SendsStateChangedEvent()
        {
            var mockHandler = new Mock<PomoTimerStateChangeEventHandler>();
            _PomoTimer.StateChanged += mockHandler.Object;

            _PomoTimer.Start();

            mockHandler.Verify(m => m(_PomoTimer,
                It.Is<PomoTimerStateChangeEventArgs>(x => x.State == PomoTimerState.Work)), Times.Once);
        }

        [Test]
        public void TickEvent_SendsStateChangedEvent_WhenTimeRemainingZero_AndStateIsWork()
        {
            var mockStateHandler = new Mock<PomoTimerStateChangeEventHandler>();
            SetupTimerStateChangeTest(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
            _PomoTimer.Start();
            _PomoTimer.StateChanged += mockStateHandler.Object;

            MockTick();

            mockStateHandler.Verify(m => m(_PomoTimer,
                It.Is<PomoTimerStateChangeEventArgs>(x => x.State == PomoTimerState.Rest)), Times.Once);
        }

        [Test]
        public void TickEvent_SendsStateChangedEvent_WhenTimeRemainingZero_AndStateIsRest()
        {
            var mockStateHandler = new Mock<PomoTimerStateChangeEventHandler>();
            SetupTimerStateChangeTest(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
            PutTimerInRestPhase();
            _PomoTimer.StateChanged += mockStateHandler.Object;

            MockTick();

            mockStateHandler.Verify(m => m(_PomoTimer,
                It.Is<PomoTimerStateChangeEventArgs>(x => x.State == PomoTimerState.Disabled)), Times.Once);
        }

        [Test]
        public void TickEvent_StopsTimer_WhenTimeRemainingZero_AndStateIsRest()
        {
            var mockStateHandler = new Mock<PomoTimerStateChangeEventHandler>();
            SetupTimerStateChangeTest(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
            PutTimerInRestPhase();
            _PomoTimer.StateChanged += mockStateHandler.Object;

            MockTick();

            _MockDispatcherTimer.Verify(m => m.Stop(), Times.Once);
        }

        [TestCaseSource(typeof(TestCaseFactory), "TimerTickTestCases")]
        public void TickEventOccurs_WhenTimerStarted_AndStateIsWork(
            TimeSpan pomoDuration, TimeSpan progress, TimeSpan expectedTimeRemaining)
        {
            SetupConfiguration(pomoDuration, TimeSpan.FromSeconds(1));
            SetupTimerSequence(progress, TimeSpan.FromSeconds(1));

            _PomoTimer.Tick += _MockPomoTimerTickHandler.Object;

            _PomoTimer.Start();
            MockTick();

            // Verify
            _MockPomoTimerTickHandler.Verify(m => m(_PomoTimer, It.Is<PomoTimerEventArgs>(
                x => x.TimeRemaining == expectedTimeRemaining)));
        }

        [Test]
        public void TickEventDoesNotOccur_WhenTimerNegative_AndStateIsRest()
        {
            var pomoDuration = TimeSpan.FromSeconds(10);
            var pomoBreakDuration = TimeSpan.FromSeconds(5);
            SetupConfiguration(pomoDuration, pomoBreakDuration);
            SetupTimerSequence(pomoDuration, pomoBreakDuration + TimeSpan.FromSeconds(1));
            PutTimerInRestPhase();

            _PomoTimer.Tick += _MockPomoTimerTickHandler.Object;

            MockTick();

            // Verify
            _MockPomoTimerTickHandler.Verify(m => m(_PomoTimer, It.IsAny<PomoTimerEventArgs>()), Times.Never);
        }

        [TestCaseSource(typeof(TestCaseFactory), "TimerTickTestCases")]
        public void TickEventOccurs_WhenTimerStarted_AndStateIsRest(
            TimeSpan pomoBreakDuration, TimeSpan progress, TimeSpan expectedTimeRemaining)
        {
            var pomoDuration = TimeSpan.FromSeconds(20);
            SetupConfiguration(pomoDuration, pomoBreakDuration);
            SetupTimerSequence(pomoDuration, progress);
            PutTimerInRestPhase();

            _PomoTimer.Tick += _MockPomoTimerTickHandler.Object;

            // Verify
            _MockPomoTimerTickHandler.Verify(m => m(_PomoTimer, It.Is<PomoTimerEventArgs>(
                x => x.TimeRemaining == expectedTimeRemaining)));
        }

        private void MockTick()
        {
            _MockDispatcherTimer.Raise(m => m.Tick += null, null, new EventArgs());
        }

        private void PutTimerInRestPhase()
        {
            _PomoTimer.Start();
            MockTick();
        }

        private void SetupTimerStateChangeTest(TimeSpan pomoDuration, TimeSpan pomoBreakDuration)
        {
            SetupConfiguration(pomoDuration, pomoBreakDuration);
            SetupTimerSequence(pomoDuration, pomoBreakDuration);
        }

        private void SetupConfiguration(TimeSpan pomoDuration, TimeSpan pomoBreakDuration)
        {
            _MockConfiguration.SetupGet(m => m.PomoDuration).Returns(pomoDuration);
            _MockConfiguration.SetupGet(m => m.PomoBreakDuration).Returns(pomoBreakDuration);
        }

        private void SetupTimerSequence(TimeSpan duration1, TimeSpan duration2)
        {
            _MockTimeProvider.SetupSequence(m => m.Now())
                .Returns(DateTime.MinValue)
                .Returns(DateTime.MinValue + duration1)
                .Returns(DateTime.MinValue + duration1 + duration2);
        }

        private class TestCaseFactory
        {
            public static IEnumerable TimerTickTestCases
            {
                get
                {
                    yield return new object[]
                    {
                        TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(9)
                    };
                    yield return new object[]
                    {
                        TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(8)
                    };
                    yield return new object[]
                    {
                        TimeSpan.FromSeconds(25), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(20)
                    };
                }
            }
        }
    }
}
