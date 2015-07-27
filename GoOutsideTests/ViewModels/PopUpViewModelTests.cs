using System.Security.Permissions;
using GoOutside.Timers;
using GoOutside.ViewModels;
using Moq;
using NUnit.Framework;

namespace GoOutsideTests.ViewModels
{
    [TestFixture]
    class PopUpViewModelTests
    {
        private Mock<IDispatcher> _MockDispatcher;
        private Mock<ISessionTimer> _MockSessionTimer;
        private PopUpViewModel _PopUpViewModel;

        [SetUp]
        public void SetUp()
        {
            _MockDispatcher = new Mock<IDispatcher>();
            _MockSessionTimer = new Mock<ISessionTimer>();

            _PopUpViewModel = new PopUpViewModel(_MockDispatcher.Object, _MockSessionTimer.Object);
        }

        [Test]
        public void Height_ReturnsCorrectly()
        {
            Assert.That(_PopUpViewModel.Height, Is.EqualTo(200));
        }

        [Test]
        public void Width_ReturnsCorrectly()
        {
            Assert.That(_PopUpViewModel.Width, Is.EqualTo(500));
        }

        [Test]
        public void DelayCommand_PostponesBreak()
        {
            _PopUpViewModel.DelayCommand.Execute(null);

            _MockSessionTimer.Verify(m => m.PostponeBreak(), Times.Once());
        }
    }
}
