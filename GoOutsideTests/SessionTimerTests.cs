using System;
using GoOutside.Events;
using GoOutside.Scheduling;
using GoOutside.Timers;
using Microsoft.Win32;
using Moq;
using NUnit.Framework;

namespace GoOutsideTests
{
    [TestFixture]
    public class SessionTimerTests
    {
        private Mock<ISystemEvents> _MockSystemEvents;
        private Mock<IPeriod> _MockPostponeBreakPeriod;
        private Mock<IPeriod> _MockPeriodBetweenBreaks;
        private Mock<IPeriodFactory> _MockPeriodFactory;
        private SessionTimer _SessionTimer;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _MockSystemEvents = new Mock<ISystemEvents>();
            _MockPostponeBreakPeriod = new Mock<IPeriod>();
            _MockPeriodBetweenBreaks = new Mock<IPeriod>();
            _MockPeriodFactory = new Mock<IPeriodFactory>();
            _MockPeriodFactory.Setup(m => m.PeriodBetweenBreaks()).Returns(_MockPeriodBetweenBreaks.Object);
            _MockPeriodFactory.Setup(m => m.PostponeBreakPeriod()).Returns(_MockPostponeBreakPeriod.Object);

            _SessionTimer = new SessionTimer(_MockSystemEvents.Object, _MockPeriodFactory.Object);
        }

        [TestCase(SessionSwitchReason.SessionUnlock)]
        [TestCase(SessionSwitchReason.SessionLogon)]
        public void BreakPeriodStarted_ForCorrectSystemEvents(SessionSwitchReason reason)
        {
            // Act
            var sessionSwitchEventArgs = new SessionSwitchEventArgs(reason);
            _MockSystemEvents.Raise(m => m.SessionSwitch += null, sessionSwitchEventArgs);

            // Verify
            _MockPeriodBetweenBreaks.Verify(m => m.Start(), Times.Once());
        }

        [TestCase(SessionSwitchReason.SessionLock)]
        [TestCase(SessionSwitchReason.SessionLogoff)]
        public void BreakPeriodStopped_ForCorrectSystemEvents(SessionSwitchReason reason)
        {
            // Act
            var sessionSwitchEventArgs = new SessionSwitchEventArgs(reason);
            _MockSystemEvents.Raise(m => m.SessionSwitch += null, null, sessionSwitchEventArgs);

            // Verify
            _MockPeriodBetweenBreaks.Verify(m => m.Stop(), Times.Once());
        }

        [Test]
        public void BreakNeededEventInvoked_WhenBreakPeriodTimesOut()
        {
            // Arrange
            var mockHandler = new Mock<BreakNeededEventHandler>();
            _SessionTimer.BreakNeeded += mockHandler.Object;

            // Act
            _MockPeriodBetweenBreaks.Raise(m => m.Elapsed += null, null, new PeriodElapsedEventArgs(DateTime.Now));

            // Verify
            mockHandler.Verify(m => m(_SessionTimer, It.IsAny<EventArgs>()), Times.Once());
        }

        [Test]
        public void PostponeBreakPeriodStarted_WhenPostponeBreakCalled()
        {
            // Act
            _SessionTimer.PostponeBreak();

            // Verify
            _MockPostponeBreakPeriod.Verify(m => m.Start(), Times.Once());
        }

        [TestCase(SessionSwitchReason.SessionLock)]
        [TestCase(SessionSwitchReason.SessionLogoff)]
        public void PostponeBreakPeriodStopped_ForCorrectSystemEvents(SessionSwitchReason reason)
        {
            // Act
            var sessionSwitchEventArgs = new SessionSwitchEventArgs(reason);
            _MockSystemEvents.Raise(m => m.SessionSwitch += null, null, sessionSwitchEventArgs);

            // Verify
            _MockPostponeBreakPeriod.Verify(m => m.Stop(), Times.Once());
        }

        [TestCase(SessionSwitchReason.SessionLock)]
        [TestCase(SessionSwitchReason.SessionLogoff)]
        public void BreakTakenEventInvoked_WhenSystemEventsOccur(SessionSwitchReason reason)
        {
            // Arrange
            var mockHandler = new Mock<BreakTakenEventHandler>();
            _SessionTimer.BreakTaken += mockHandler.Object;

            // Act
            var sessionSwitchEventArgs = new SessionSwitchEventArgs(reason);
            _MockSystemEvents.Raise(m => m.SessionSwitch += null, null, sessionSwitchEventArgs);

            // Verify
            mockHandler.Verify(m => m(_SessionTimer, It.IsAny<EventArgs>()), Times.Once());
        }

        [Test]
        public void BreakNeededEventInvoked_WhenPostponeBreakPeriodTimesOut()
        {
            // Arrange
            var mockHandler = new Mock<BreakNeededEventHandler>();
            _SessionTimer.BreakNeeded += mockHandler.Object;

            // Act
            _MockPostponeBreakPeriod.Raise(m => m.Elapsed += null, null, new PeriodElapsedEventArgs(DateTime.Now));

            // Verify
            mockHandler.Verify(m => m(_SessionTimer, It.IsAny<EventArgs>()), Times.Once());
        }
    }
}
