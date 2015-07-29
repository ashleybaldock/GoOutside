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

        [SetUp]
        public void Create()
        {
            _MockConfiguration = new Mock<IConfiguration>();

            _MockDispatcherTimer = new Mock<IDispatcherTimer>();

            _MockTimeProvider = new Mock<ITimeProvider>();
            _MockTimeProvider.Setup(m => m.CreateDispatcherTimer()).Returns(_MockDispatcherTimer.Object);

            _PomoTimer = new PomoTimer(_MockConfiguration.Object, _MockTimeProvider.Object);
        }

        [Test]
        public void Create_SetsUpCorrectly()
        {
            _MockTimeProvider.Verify(m => m.CreateDispatcherTimer(), Times.Once);
            _MockDispatcherTimer.VerifySet(m => m.Interval = It.Is<TimeSpan>(x => x == TimeSpan.FromMilliseconds(250)));
        }

        [Test]
        public void Start_StartsDispatcherTimer()
        {
            _PomoTimer.Start();

            _MockDispatcherTimer.Verify(m => m.Start(), Times.Once);
        }

        [Test]
        public void Stop_StopsDispatcherTimer()
        {
            _PomoTimer.Stop();

            _MockDispatcherTimer.Verify(m => m.Stop(), Times.Once);
        }

        [TestCaseSource(typeof(TestCaseFactory), "TimerTickTestCases")]
        public void TickEventOccurs_WhenTimerStarted(
            TimeSpan pomoDuration, TimeSpan progress, TimeSpan expectedTimeRemaining)
        {
            var mockHandler = SetupAndExecuteTimerTest(pomoDuration, progress);

            // Verify
            mockHandler.Verify(m => m(_PomoTimer, It.Is<PomoTimerEventArgs>(
                x => x.TimeRemaining == expectedTimeRemaining)));
        }

        [Test]
        public void TickEventDoesNotOccur_WhenTimerNegative()
        {
            var mockHandler = SetupAndExecuteTimerTest(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(11));

            // Verify
            mockHandler.Verify(m => m(_PomoTimer, It.IsAny<PomoTimerEventArgs>()), Times.Never);
        }

        private Mock<PomoTimerTickEventHandler> SetupAndExecuteTimerTest(
            TimeSpan pomoDuration, TimeSpan progress)
        {
            // Arrange
            _MockConfiguration.SetupGet(m => m.PomoDuration).Returns(pomoDuration);

            var mockHandler = new Mock<PomoTimerTickEventHandler>();
            _PomoTimer.Tick += mockHandler.Object;

            _MockTimeProvider.SetupSequence(m => m.Now())
                .Returns(DateTime.MinValue)
                .Returns(DateTime.MinValue + progress);

            // Act
            _PomoTimer.Start();
            _MockDispatcherTimer.Raise(m => m.Tick += null, null, new EventArgs());

            return mockHandler;
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
