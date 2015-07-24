using System;
using GoOutside;
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
            var mockScheduler = new Mock<IPeriod>();

            var sessionTimer = new SessionTimer(mockSystemEvents.Object, mockScheduler.Object);

            // Act
            var sessionSwitchEventArgs = new SessionSwitchEventArgs(reason);
            mockSystemEvents.Raise(m => m.SessionSwitch += null, sessionSwitchEventArgs);

            // Verify
            mockScheduler.Verify(m => m.Start(), Times.Once());
        }

        [TestCase(SessionSwitchReason.SessionLock)]
        [TestCase(SessionSwitchReason.SessionLogoff)]
        public void SchedulerStopped_ForCorrectSystemEvents(SessionSwitchReason reason)
        {
            // Arrange
            var mockSystemEvents = new Mock<ISystemEvents>();
            var mockScheduler = new Mock<IPeriod>();

            var sessionTimer = new SessionTimer(mockSystemEvents.Object, mockScheduler.Object);

            // Act
            var sessionSwitchEventArgs = new SessionSwitchEventArgs(reason);
            mockSystemEvents.Raise(m => m.SessionSwitch += null, null, sessionSwitchEventArgs);

            // Verify
            mockScheduler.Verify(m => m.Stop(), Times.Once());
        }

        [Test]
        public void WhenSchedulerTimesOut_PeriodSinceBreakElapsedEventInvoked()
        {
            // Arrange
            var mockSystemEvents = new Mock<ISystemEvents>();
            var mockScheduler = new Mock<IPeriod>();
            var mockHandler = new Mock<PeriodSinceBreakElapsedEventHandler>();

            var sessionTimer = new SessionTimer(mockSystemEvents.Object, mockScheduler.Object);
            sessionTimer.PeriodSinceBreakElapsed += mockHandler.Object;

            // Act
            mockScheduler.Raise(m => m.Elapsed += null, null, new PeriodElapsedEventArgs(DateTime.Now));

            // Verify
            mockHandler.Verify(m => m(sessionTimer, It.IsAny<PeriodSinceBreakElapsedEventArgs>()), Times.Once());
        }

    }
}
