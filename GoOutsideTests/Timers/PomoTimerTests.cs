using System;
using GoOutside.Timers;
using GoOutside.Timers.Events;
using Moq;
using NUnit.Framework;

namespace GoOutsideTests.Timers
{
    [TestFixture]
    class PomoTimerTests
    {
        private PomoTimer _PomoTimer;
        private Mock<ITimeProvider> _MockTimeProvider;
        private Mock<ICountdownTimer> _MockRestTimer;
        private Mock<ICountdownTimer> _MockWorkTimer;
        private Mock<PomoTimerStateChangeEventHandler> _MockStateChangeHandler;
        private Mock<PomoTimerTickEventHandler> _MockTickHandler;

        [SetUp]
        public void SetUp()
        {
            _MockStateChangeHandler = new Mock<PomoTimerStateChangeEventHandler>();
            _MockTickHandler = new Mock<PomoTimerTickEventHandler>();

            _MockWorkTimer = new Mock<ICountdownTimer>();
            _MockWorkTimer.Setup(m => m.Start())
                .Callback(() => _MockWorkTimer.SetupGet(x => x.Running).Returns(true));
            _MockWorkTimer.Setup(m => m.Stop())
                .Callback(() => _MockWorkTimer.SetupGet(x => x.Running).Returns(false));

            _MockRestTimer = new Mock<ICountdownTimer>();
            _MockRestTimer.Setup(m => m.Start())
                .Callback(() => _MockRestTimer.SetupGet(x => x.Running).Returns(true));
            _MockRestTimer.Setup(m => m.Stop())
                .Callback(() => _MockRestTimer.SetupGet(x => x.Running).Returns(false));

            _MockTimeProvider = new Mock<ITimeProvider>();
            _MockTimeProvider.Setup(
                m => m.CreateCountdownTimer(TimeSpan.FromMinutes(25), TimeSpan.FromMilliseconds(100)))
                .Returns(_MockWorkTimer.Object);
            _MockTimeProvider.Setup(
                m => m.CreateCountdownTimer(TimeSpan.FromMinutes(5), TimeSpan.FromMilliseconds(100)))
                .Returns(_MockRestTimer.Object);

            _PomoTimer = new PomoTimer(_MockTimeProvider.Object);
        }

        [Test]
        public void Create_SetsUpCorrectly()
        {
//            Assert.That(_PomoTimer.State, Is.EqualTo(PomoTimerState.Disabled));
        }

        [Test]
        public void Start_SendsStateChangedEvent()
        {
            AttachMockStateChangeHandler();

            _PomoTimer.Start();

            _MockStateChangeHandler.Verify(m => m(_PomoTimer,
                It.Is<PomoTimerStateChangeEventArgs>(
                    x => x.State == PomoTimerState.Work)), Times.Once);
        }

        [Test]
        public void Constructor_InitialisesState_ToDisabled()
        {
            AttachMockStateChangeHandler();

            _PomoTimer.Stop();

            _MockStateChangeHandler.Verify(m => m(_PomoTimer,
                It.Is<PomoTimerStateChangeEventArgs>(
                    x => x.State == PomoTimerState.Disabled)), Times.Never);
        }

        [Test]
        public void Stop_DoesNotSendStateChangedEvent_WhenStopped()
        {
            _PomoTimer.Start();
            _PomoTimer.Stop();

            AttachMockStateChangeHandler();

            _PomoTimer.Stop();

            _MockStateChangeHandler.Verify(m => m(_PomoTimer,
                It.Is<PomoTimerStateChangeEventArgs>(
                    x => x.State == PomoTimerState.Disabled)), Times.Never);
        }

        [Test]
        public void Stop_SendsStateChangedEvent_WhenStarted()
        {
            AttachMockStateChangeHandler();

            _PomoTimer.Start();
            _PomoTimer.Stop();

            _MockStateChangeHandler.Verify(m => m(_PomoTimer,
                It.Is<PomoTimerStateChangeEventArgs>(
                    x => x.State == PomoTimerState.Disabled)), Times.Once);
        }

        [Test]
        public void WhenWorkCountdown_DoneEventFired_SendsStateChangedEvent()
        {
            AttachMockStateChangeHandler();

            _MockWorkTimer.Raise(m => m.Done += null, _MockWorkTimer.Object, new EventArgs());

            _MockStateChangeHandler.Verify(m => m(_PomoTimer,
                It.Is<PomoTimerStateChangeEventArgs>(
                    x => x.State == PomoTimerState.Rest)), Times.Once);
        }

        [Test]
        public void WhenRestCountdown_DoneEventFired_SendsStateChangedEvent()
        {
            PutIntoRestPhase();
            AttachMockStateChangeHandler();

            _MockRestTimer.Raise(m => m.Done += null, _MockWorkTimer.Object, new EventArgs());

            _MockStateChangeHandler.Verify(m => m(_PomoTimer,
                It.Is<PomoTimerStateChangeEventArgs>(
                    x => x.State == PomoTimerState.Disabled)), Times.Once);
        }

        [Test]
        public void Start_StartsOnlyWorkTimer()
        {
            _PomoTimer.Start();

            _MockRestTimer.Verify(m => m.Start(), Times.Never);
            _MockWorkTimer.Verify(m => m.Start(), Times.Once);
        }

        [Test]
        public void Stop_StopsBothTimers()
        {
            _PomoTimer.Stop();

            _MockRestTimer.Verify(m => m.Stop(), Times.Once);
            _MockWorkTimer.Verify(m => m.Stop(), Times.Once);
        }

        [Test]
        public void WhenWorkCountdown_DoneEventFired_StartsRestTimer()
        {
            _PomoTimer.Start();
            _MockWorkTimer.Raise(m => m.Done += null, _MockWorkTimer.Object, new EventArgs());

            _MockRestTimer.Verify(m => m.Start(), Times.Once);
        }

        [TestCase(5)]
        [TestCase(25)]
        public void WhenWorkCountdown_TickEventFired_SendsTickEvent(int time)
        {
            var timeRemaining = TimeSpan.FromMinutes(time);
            AttachMockTickHandler();

            _MockWorkTimer.Raise(m => m.Tick += null, _MockWorkTimer.Object,
                new CountdownTickEventArgs(timeRemaining));

            _MockTickHandler.Verify(m => m(_PomoTimer,
                It.Is<CountdownTickEventArgs>(
                    x => x.TimeRemaining == timeRemaining)), Times.Once);
        }

        [TestCase(5)]
        [TestCase(25)]
        public void WhenRestCountdown_TickEventFired_SendsTickEvent(int time)
        {
            var timeRemaining = TimeSpan.FromMinutes(time);
            AttachMockTickHandler();

            _MockRestTimer.Raise(m => m.Tick += null, _MockRestTimer.Object,
                new CountdownTickEventArgs(timeRemaining));

            _MockTickHandler.Verify(m => m(_PomoTimer,
                It.Is<CountdownTickEventArgs>(
                    x => x.TimeRemaining == timeRemaining)), Times.Once);
        }

        private void AttachMockStateChangeHandler()
        {
            _PomoTimer.StateChanged += _MockStateChangeHandler.Object;
        }

        private void AttachMockTickHandler()
        {
            _PomoTimer.Tick += _MockTickHandler.Object;
        }

        private void PutIntoRestPhase()
        {
            _PomoTimer.Start();
            _MockWorkTimer.Raise(m => m.Done += null, _MockWorkTimer.Object, new EventArgs());
        }
    }
}