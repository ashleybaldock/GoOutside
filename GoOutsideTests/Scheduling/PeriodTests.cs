using System;
using System.Threading;
using GoOutside.Scheduling;
using Moq;
using NUnit.Framework;

namespace GoOutsideTests.Scheduling
{
    [TestFixture]
    class PeriodTests
    {
        [Test]
        public void CreatePeriodElapsedEventArgs_SetsSignalTime()
        {
            var datetime = DateTime.Now;
            var periodElapsedEventArgs = new PeriodElapsedEventArgs(datetime);

            Assert.That(periodElapsedEventArgs.SignalTime, Is.EqualTo(datetime));
        }

        [Test]
        public void PeriodOnTimerElapsedEvent_WiredCorrectly()
        {
            var mockHandler = new Mock<PeriodElapsedEventHandler>();
            var period = new Period();

            period.Elapsed += mockHandler.Object;

            period.Interval = 1;
            period.AutoReset = false;
            period.Start();

            Thread.Sleep(2);

            mockHandler.Verify(m => m(period, It.IsAny<PeriodElapsedEventArgs>()), Times.Once());
        }
    }
}
