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
        [TestCase(SessionSwitchReason.SessionUnlock)]
        [TestCase(SessionSwitchReason.SessionLogon)]
        public void SchedulerStarted_ForCorrectSystemEvents(SessionSwitchReason reason)
        {
            // Arrange
            var mockSystemEvents = new Mock<ISystemEvents>();
            var mockPeriodBetweenBreaks = new Mock<IPeriod>();
            var mockPeriodFactory = new Mock<IPeriodFactory>();
            mockPeriodFactory.Setup(m => m.PeriodBetweenBreaks()).Returns(mockPeriodBetweenBreaks.Object);

            var sessionTimer = new SessionTimer(mockSystemEvents.Object, mockPeriodFactory.Object);

            // Act
            var sessionSwitchEventArgs = new SessionSwitchEventArgs(reason);
            mockSystemEvents.Raise(m => m.SessionSwitch += null, sessionSwitchEventArgs);

            // Verify
            mockPeriodBetweenBreaks.Verify(m => m.Start(), Times.Once());
        }

        [TestCase(SessionSwitchReason.SessionLock)]
        [TestCase(SessionSwitchReason.SessionLogoff)]
        public void SchedulerStopped_ForCorrectSystemEvents(SessionSwitchReason reason)
        {
            // Arrange
            var mockSystemEvents = new Mock<ISystemEvents>();
            var mockPeriodBetweenBreaks = new Mock<IPeriod>();
            var mockPeriodFactory = new Mock<IPeriodFactory>();
            mockPeriodFactory.Setup(m => m.PeriodBetweenBreaks()).Returns(mockPeriodBetweenBreaks.Object);

            var sessionTimer = new SessionTimer(mockSystemEvents.Object, mockPeriodFactory.Object);

            // Act
            var sessionSwitchEventArgs = new SessionSwitchEventArgs(reason);
            mockSystemEvents.Raise(m => m.SessionSwitch += null, null, sessionSwitchEventArgs);

            // Verify
            mockPeriodBetweenBreaks.Verify(m => m.Stop(), Times.Once());
        }

        [Test]
        public void WhenSchedulerTimesOut_PeriodSinceBreakElapsedEventInvoked()
        {
            // Arrange
            var mockSystemEvents = new Mock<ISystemEvents>();
            var mockPeriodBetweenBreaks = new Mock<IPeriod>();
            var mockPeriodFactory = new Mock<IPeriodFactory>();
            mockPeriodFactory.Setup(m => m.PeriodBetweenBreaks()).Returns(mockPeriodBetweenBreaks.Object);
            var mockHandler = new Mock<PeriodSinceBreakElapsedEventHandler>();

            var sessionTimer = new SessionTimer(mockSystemEvents.Object, mockPeriodFactory.Object);
            sessionTimer.PeriodSinceBreakElapsed += mockHandler.Object;

            // Act
            mockPeriodBetweenBreaks.Raise(m => m.Elapsed += null, null, new PeriodElapsedEventArgs(DateTime.Now));

            // Verify
            mockHandler.Verify(m => m(sessionTimer, It.IsAny<PeriodSinceBreakElapsedEventArgs>()), Times.Once());
        }

    }
}
