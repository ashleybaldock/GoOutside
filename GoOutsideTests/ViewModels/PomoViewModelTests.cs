using System;
using System.ComponentModel;
using GoOutside.ViewModels;
using Moq;
using NUnit.Framework;

namespace GoOutsideTests.ViewModels
{
    [TestFixture]
    class PomoViewModelTests
    {
        private PomoViewModel _PomoViewModel;
        private Mock<IPomoTimer> _MockPomoTimer;

        [SetUp]
        public void SetUp()
        {
            _MockPomoTimer = new Mock<IPomoTimer>();
            _PomoViewModel = new PomoViewModel(_MockPomoTimer.Object);
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

        [Test]
        public void ChangeTimerTextProperty_SendsPropertyChangedEvent()
        {
            _PomoViewModel.TimerText = "startValue";
            var mockPropertyChangedDelegate = new Mock<PropertyChangedEventHandler>();
            _PomoViewModel.PropertyChanged += mockPropertyChangedDelegate.Object;

            _PomoViewModel.TimerText = "newValue";

            mockPropertyChangedDelegate.Verify(
                m => m(_PomoViewModel, It.Is<PropertyChangedEventArgs>(x => x.PropertyName == "TimerText")),
                Times.Once);
        }

        [Test]
        public void ChangeTimerTextProperty_OnlySendsPropertyChangedEvent_WhenPropertyChanges()
        {
            _PomoViewModel.TimerText = "sameValue";
            var mockPropertyChangedDelegate = new Mock<PropertyChangedEventHandler>();
            _PomoViewModel.PropertyChanged += mockPropertyChangedDelegate.Object;

            _PomoViewModel.TimerText = "sameValue";

            mockPropertyChangedDelegate.Verify(
                m => m(_PomoViewModel, It.Is<PropertyChangedEventArgs>(x => x.PropertyName == "TimerText")),
                Times.Never);
        }

        [Test]
        public void IfTimerNotStarted_OnMouseClick_StartsPomoTimer()
        {
            _MockPomoTimer.Setup(m => m.Running()).Returns(false);

            _PomoViewModel.OnMouseClick.Execute(null);

            _MockPomoTimer.Verify(m => m.Start(), Times.Once);
            _MockPomoTimer.Verify(m => m.Stop(), Times.Never);
        }

        [Test]
        public void IfTimerStarted_OnMouseClick_StopsPomoTimer()
        {
            _MockPomoTimer.Setup(m => m.Running()).Returns(true);

            _PomoViewModel.OnMouseClick.Execute(null);

            _MockPomoTimer.Verify(m => m.Start(), Times.Never);
            _MockPomoTimer.Verify(m => m.Stop(), Times.Once);
        }

        [Test]
        public void PomoTimerTick_UpdatesText()
        {
            _PomoViewModel.TimerText = "initialValue";
            _MockPomoTimer.Setup(m => m.Running()).Returns(true);

            var timeRemaining = TimeSpan.FromMinutes(10);
            var pomoTimerEventArgs = new PomoTimerEventArgs(timeRemaining);
            _MockPomoTimer.Raise(m => m.Tick += null, null, pomoTimerEventArgs);

            Assert.That(_PomoViewModel.TimerText, Is.EqualTo("10:00"));
        }

        [Test]
        public void IfTimerStarted_OnMouseEnter_ShowsStop()
        {
            _PomoViewModel.TimerText = "initialValue";
            _MockPomoTimer.Setup(m => m.Running()).Returns(true);

            _PomoViewModel.OnMouseEnter.Execute(null);

            Assert.That(_PomoViewModel.TimerText, Is.EqualTo("Cancel"));
        }

        [Test]
        public void IfTimerStopped_OnMouseEnter_ShowsStart()
        {
            _PomoViewModel.TimerText = "initialValue";
            _MockPomoTimer.Setup(m => m.Running()).Returns(false);

            _PomoViewModel.OnMouseEnter.Execute(null);

            Assert.That(_PomoViewModel.TimerText, Is.EqualTo("Start"));
        }

        [Test]
        public void IfMouseEntered_PomoTimerTick_DoesNotUpdateText()
        {
            _PomoViewModel.TimerText = "initialValue";
            _MockPomoTimer.Setup(m => m.Running()).Returns(true);
            var timeRemaining = TimeSpan.FromMinutes(10);
            var pomoTimerEventArgs = new PomoTimerEventArgs(timeRemaining);

            _PomoViewModel.OnMouseEnter.Execute(null);
            _MockPomoTimer.Raise(m => m.Tick += null, null, pomoTimerEventArgs);

            Assert.That(_PomoViewModel.TimerText, Is.EqualTo("Cancel"));
        }

        [Test]
        public void OnMouseLeave_EnablesTickUpdates()
        {
            _PomoViewModel.TimerText = "initialValue";
            _MockPomoTimer.Setup(m => m.Running()).Returns(true);
            var timeRemaining = TimeSpan.FromMinutes(10);
            var pomoTimerEventArgs = new PomoTimerEventArgs(timeRemaining);
            _PomoViewModel.OnMouseEnter.Execute(null);

            _PomoViewModel.OnMouseLeave.Execute(null);
            _MockPomoTimer.Raise(m => m.Tick += null, null, pomoTimerEventArgs);

            Assert.That(_PomoViewModel.TimerText, Is.EqualTo("10:00"));
        }

        [Test]
        public void OnMouseLeave_IfTimerNotRunning_SetsTextToStart()
        {
            _PomoViewModel.TimerText = "initialValue";
            _MockPomoTimer.Setup(m => m.Running()).Returns(false);

            _PomoViewModel.OnMouseLeave.Execute(null);

            Assert.That(_PomoViewModel.TimerText, Is.EqualTo("Start"));
        }
    }
}
