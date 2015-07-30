using System;
using GoOutside.Scheduling;
using GoOutside.Timers;
using GoOutside.Timers.Events;
using Moq;
using NUnit.Framework;

namespace GoOutsideTests.Timers
{
    [TestFixture]
    class CountdownTimerTests
    {
        private ICountdownTimer _CountdownTimer;
        private Mock<CountdownTimerTickEventHandler> _MockTickHandler;
        private Mock<CountdownDoneEventHandler> _MockDoneHandler;
        private Mock<ITimeProvider> _MockTimeProvider;
        private Mock<IDispatcherTimer> _MockDispatcherTimer;
        private readonly TimeSpan _IntervalTimeSpan = TimeSpan.FromMilliseconds(100);
        private readonly TimeSpan _DurationTimeSpan = TimeSpan.FromSeconds(10);

        [SetUp]
        public void SetUp()
        {
            _MockTickHandler = new Mock<CountdownTimerTickEventHandler>();
            _MockDoneHandler = new Mock<CountdownDoneEventHandler>();

            _MockDispatcherTimer = new Mock<IDispatcherTimer>();
            _MockDispatcherTimer.Setup(m => m.Start())
                .Callback(() => _MockDispatcherTimer.SetupGet(x => x.IsEnabled).Returns(true));
            _MockDispatcherTimer.Setup(m => m.Stop())
                .Callback(() => _MockDispatcherTimer.SetupGet(x => x.IsEnabled).Returns(false));

            _MockTimeProvider = new Mock<ITimeProvider>();
            _MockTimeProvider.Setup(m => m.CreateDispatcherTimer(_IntervalTimeSpan))
                .Returns(_MockDispatcherTimer.Object);

            CreateCountdownTimer(_DurationTimeSpan, _IntervalTimeSpan);
        }

        private void CreateCountdownTimer(TimeSpan duration, TimeSpan interval)
        {
            _CountdownTimer = new CountdownTimer(
                _MockTimeProvider.Object, duration, interval);
        }

        [Test]
        public void Start_StartsDispatcherTimers()
        {
            _CountdownTimer.Start();

            _MockDispatcherTimer.Verify(m => m.Start(), Times.Once);
        }

        [Test]
        public void Stop_StopsDispatcherTimers()
        {
            _CountdownTimer.Stop();

            _MockDispatcherTimer.Verify(m => m.Stop(), Times.Once);
        }

        [TestCaseSource("_TimerTickTestCases")]
        public void IntervalTimerTick_TriggersTickEvent_WithCorrectTimeRemainingMessage(
            TimeSpan timePassed, TimeSpan expectedTimeRemaining)
        {
            SetupTimerSequence(timePassed);
            AttachMockTickHandler();

            _CountdownTimer.Start();
            MockTick();

            _MockTickHandler.Verify(m => m(_CountdownTimer,
                It.Is<CountdownTickEventArgs>(x => x.TimeRemaining == expectedTimeRemaining)), Times.Once);
        }

        [Test]
        public void IntervalTimer_TickWithNoTimeRemaining_TriggersEventsInOrder()
        {
            var callOrder = 0;
            _MockTickHandler.Setup(m => m(_CountdownTimer,
                It.Is<CountdownTickEventArgs>(x => x.TimeRemaining == TimeSpan.Zero)))
                .Callback(() => Assert.That(callOrder++, Is.EqualTo(0)));
            _MockDoneHandler.Setup(m => m(_CountdownTimer,
                It.IsAny<EventArgs>()))
                .Callback(() => Assert.That(callOrder++, Is.EqualTo(1)));

            SetupTimerSequence(TimeSpan.FromSeconds(10));
            AttachMockTickHandler();
            AttachMockDoneHandler();

            _CountdownTimer.Start();
            MockTick();

            _MockTickHandler.Verify(m => m(_CountdownTimer,
                It.Is<CountdownTickEventArgs>(x => x.TimeRemaining == TimeSpan.Zero)), Times.Once);
            _MockDoneHandler.Verify(m => m(_CountdownTimer,
                It.IsAny<EventArgs>()), Times.Once);
        }

        [Test]
        public void IntervalTimer_TickWithNoTimeRemaining_StopsDispatcherTimer()
        {
            SetupTimerSequence(TimeSpan.FromSeconds(10));

            _CountdownTimer.Start();
            MockTick();

            _MockDispatcherTimer.Verify(m => m.Stop(), Times.Once);
        }

        [Test]
        public void Running_ReturnsTrueWhenDispatcherTimerRunning()
        {
            _MockDispatcherTimer.SetupGet(x => x.IsEnabled).Returns(true);

            Assert.That(_CountdownTimer.Running, Is.True);
        }

        [Test]
        public void Running_ReturnsFalseWhenDispatcherTimerNotRunning()
        {
            _MockDispatcherTimer.SetupGet(x => x.IsEnabled).Returns(false);

            Assert.That(_CountdownTimer.Running, Is.False);
        }

        private void MockTick()
        {
            _MockDispatcherTimer.Raise(m => m.Tick += null, _MockDispatcherTimer.Object, new EventArgs());
        }

        private void AttachMockTickHandler()
        {
            _CountdownTimer.Tick += _MockTickHandler.Object;
        }

        private void AttachMockDoneHandler()
        {
            _CountdownTimer.Done += _MockDoneHandler.Object;
        }

        private void SetupTimerSequence(TimeSpan duration1)
        {
            _MockTimeProvider.SetupSequence(m => m.Now())
                .Returns(DateTime.MinValue)
                .Returns(DateTime.MinValue + duration1);
        }

        private static readonly object[] _TimerTickTestCases =
        {
            // Duration is set to 10 seconds in setup
            new object[] { TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(10) },
            new object[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(9) },
            new object[] { TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(8) },
            new object[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5) },
            new object[] { TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(0) },
            new object[] { TimeSpan.FromSeconds(11), TimeSpan.FromSeconds(0) }
        };

    }
}
