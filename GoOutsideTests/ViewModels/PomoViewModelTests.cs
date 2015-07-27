using System.Dynamic;
using GoOutside.ViewModels;
using NUnit.Framework;

namespace GoOutsideTests.ViewModels
{
    [TestFixture]
    class PomoViewModelTests
    {
        private IPomoViewModel _PomoViewModel;

        [SetUp]
        public void SetUp()
        {
            _PomoViewModel = new PomoViewModel();
        }

        [Test]
        public void CanGetHeight()
        {
            Assert.That(_PomoViewModel.Height, Is.EqualTo(200));
        }

        [Test]
        public void CanGetWidth()
        {
            Assert.That(_PomoViewModel.Width, Is.EqualTo(200));
        }

        [Test]
        public void CanGetVisibility()
        {
            Assert.That(_PomoViewModel.Visible, Is.True);
        }

        [Test]
        public void CanGetTimerText()
        {
            Assert.That(_PomoViewModel.TimerText, Is.EqualTo("25:00"));
        }
    }
}
