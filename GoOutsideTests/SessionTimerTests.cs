using GoOutside;
using GoOutside.Events;
using Microsoft.Win32;
using Moq;
using NUnit.Framework;

namespace GoOutsideTests
{
    [TestFixture]
    public class SessionTimerTests
    {
        [Test]
        public void Create()
        {
            // Arrange
            var mockSystemEvents = new Mock<ISystemEvents>();
            var sessionTimer = new SessionTimer(mockSystemEvents.Object);

            var sessionSwitchEventArgs = new SessionSwitchEventArgs(SessionSwitchReason.SessionLogoff);

            // Act
            mockSystemEvents.Raise(m => m.SessionSwitch += null, sessionSwitchEventArgs);

            // Verify
        }
    }
}
