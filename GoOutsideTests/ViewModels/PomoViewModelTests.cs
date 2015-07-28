using System.ComponentModel;
using System.Dynamic;
using System.Security.Permissions;
using GoOutside.ViewModels;
using Moq;
using NUnit.Framework;

namespace GoOutsideTests.ViewModels
{
    [TestFixture]
    class PomoViewModelTests
    {
        private PomoViewModel _PomoViewModel;

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

        [TestCase(false)]
        [TestCase(true)]
        public void Show_SetsVisible_ToTrue(bool initialState)
        {
            _PomoViewModel.Visible = initialState;

            _PomoViewModel.Show.Execute(null);

            Assert.That(_PomoViewModel.Visible, Is.True);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Hide_SetsVisible_ToFalse(bool initialState)
        {
            _PomoViewModel.Visible = initialState;

            _PomoViewModel.Hide.Execute(null);

            Assert.That(_PomoViewModel.Visible, Is.False);
        }

        [TestCase(false, false)]
        [TestCase(true, true)]
        public void CanHide_OnlyWhenVisible(bool initialState, bool expected)
        {
            _PomoViewModel.Visible = initialState;

            Assert.That(_PomoViewModel.Hide.CanExecute(null), Is.EqualTo(expected));
        }

        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CanShow_OnlyWhenNotVisible(bool initialState, bool expected)
        {
            _PomoViewModel.Visible = initialState;

            Assert.That(_PomoViewModel.Show.CanExecute(null), Is.EqualTo(expected));
        }

        [Test]
        public void ChangeVisibleProperty_SendsPropertyChangedEvent()
        {
            _PomoViewModel.Visible = true;
            var mockPropertyChangedDelegate = new Mock<PropertyChangedEventHandler>();
            _PomoViewModel.PropertyChanged += mockPropertyChangedDelegate.Object;

            _PomoViewModel.Visible = false;

            mockPropertyChangedDelegate.Verify(
                m => m(_PomoViewModel, It.Is<PropertyChangedEventArgs>(x => x.PropertyName == "Visible")),
                Times.Once);
        }

        [Test]
        public void ChangeVisibleProperty_OnlySendsPropertyChangedEvent_WhenPropertyChanges()
        {
            _PomoViewModel.Visible = true;
            var mockPropertyChangedDelegate = new Mock<PropertyChangedEventHandler>();
            _PomoViewModel.PropertyChanged += mockPropertyChangedDelegate.Object;

            _PomoViewModel.Visible = true;

            mockPropertyChangedDelegate.Verify(
                m => m(_PomoViewModel, It.Is<PropertyChangedEventArgs>(x => x.PropertyName == "Visible")),
                Times.Never);
        }
    }
}
