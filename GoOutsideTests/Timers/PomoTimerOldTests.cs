using System;
using System.Collections;
using GoOutside.Configuration;
using GoOutside.Scheduling;
using GoOutside.Timers;
using GoOutside.Timers.Events;
using Moq;
using NUnit.Framework;

namespace GoOutsideTests.Timers
{
    [TestFixture]
    class PomoTimerOldTests
    {
        private Mock<IDispatcherTimer> _MockDispatcherTimer;
        private PomoTimerOld _PomoTimerOld;
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
//            _MockTimeProvider.Setup(m => m.CreateDispatcherTimer()).Returns(_MockDispatcherTimer.Object);

            _PomoTimerOld = new PomoTimerOld(_MockConfiguration.Object, _MockTimeProvider.Object);

            _MockPomoTimerTickHandler = new Mock<PomoTimerTickEventHandler>();
        }

        [Test]
        public void Create_SetsUpCorrectly()
        {
//            _MockTimeProvider.Verify(m => m.CreateDispatcherTimer(), Times.Once);
            _MockDispatcherTimer.VerifySet(m => m.Interval = It.Is<TimeSpan>(x => x == TimeSpan.FromMilliseconds(250)));
            Assert.That(_PomoTimerOld.Running, Is.False);
        }

        [Test]
        public void Start_StartsDispatcherTimer()
        {
            _PomoTimerOld.Start();

            _MockDispatcherTimer.Verify(m => m.Start(), Times.Once);
        }

        [Test]
        public void Start_SetsRunning()
        {
            _PomoTimerOld.Start();

            Assert.That(_PomoTimerOld.Running, Is.True);
        }

        [Test]
        public void Stop_StopsDispatcherTimer()
        {
            _PomoTimerOld.Stop();

            _MockDispatcherTimer.Verify(m => m.Stop(), Times.Once);
        }

        [Test]
        public void Stop_SetsRunning()
        {
            _PomoTimerOld.Stop();

            Assert.That(_PomoTimerOld.Running, Is.False);
        }

        [Test]
        public void Stop_SendsStateChangedEvent()
        {
            var mockHandler = new Mock<PomoTimerStateChangeEventHandler>();
            _PomoTimerOld.StateChanged += mockHandler.Object;

            _PomoTimerOld.Start();
            _PomoTimerOld.Stop();

            mockHandler.Verify(m => m(_PomoTimerOld,
                It.Is<PomoTimerStateChangeEventArgs>(x => x.State == PomoTimerState.Disabled)), Times.Once);
        }

        [Test]
        public void Start_SendsStateChangedEvent()
        {
            var mockHandler = new Mock<PomoTimerStateChangeEventHandler>();
            _PomoTimerOld.StateChanged += mockHandler.Object;

            _PomoTimerOld.Start();

            mockHandler.Verify(m => m(_PomoTimerOld,
                It.Is<PomoTimerStateChangeEventArgs>(x => x.State == PomoTimerState.Work)), Times.Once);
        }

        [Test]
        public void TickEvent_SendsStateChangedEvent_WhenTimeRemainingZero_AndStateIsWork()
        {
            var mockStateHandler = new Mock<PomoTimerStateChangeEventHandler>();
            SetupTimerStateChangeTest(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
            _PomoTimerOld.Start();
            _PomoTimerOld.StateChanged += mockStateHandler.Object;

            MockTick();

            mockStateHandler.Verify(m => m(_PomoTimerOld,
                It.Is<PomoTimerStateChangeEventArgs>(x => x.State == PomoTimerState.Rest)), Times.Once);
        }

        [TestCaseSource(typeof(TestCaseFactory), "TimerTickTestCases")]
        public void TickEventOccurs_WhenTimerStarted_AndStateIsWork(
            TimeSpan pomoDuration, TimeSpan progress, TimeSpan expectedTimeRemaining)
        {
            SetupConfiguration(pomoDuration, TimeSpan.FromSeconds(1));
            SetupTimerSequence(progress, TimeSpan.FromSeconds(1));

            _PomoTimerOld.Tick += _MockPomoTimerTickHandler.Object;

            _PomoTimerOld.Start();
            MockTick();

            // Verify
            _MockPomoTimerTickHandler.Verify(m => m(_PomoTimerOld, It.Is<CountdownTickEventArgs>(
                x => x.TimeRemaining == expectedTimeRemaining)));
        }

        private void MockTick()
        {
            _MockDispatcherTimer.Raise(m => m.Tick += null, null, new EventArgs());
        }

        private void PutTimerInRestPhase()
        {
            _PomoTimerOld.Start();
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
