using System;
using System.Security.Permissions;
using GoOutside.Events;
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
        public void OnCreate_VisibleSetToFalse()
        {
            Assert.That(_PopUpViewModel.Visible, Is.False);
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

        [Test]
        public void Visible_SetToTrue_WhenBreakNeededEventFired()
        {
            _PopUpViewModel.Visible = false;

            _MockSessionTimer.Raise(m => m.BreakNeeded += null, null, new EventArgs());

            Assert.That(_PopUpViewModel.Visible, Is.True);
        }

        [Test]
        public void Visible_SetToFalse_WhenBreakTakenEventFired()
        {
            _PopUpViewModel.Visible = true;

            _MockSessionTimer.Raise(m => m.BreakTaken += null, null, new EventArgs());

            Assert.That(_PopUpViewModel.Visible, Is.False);
        }
    }
}
