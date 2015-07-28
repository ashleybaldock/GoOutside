using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using GoOutside.Events;
using GoOutside.Timers;
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

        [Test]
        public void OnCreate_ColoursSetToGrey()
        {
            Assert.That(_PomoViewModel.LightColour, Is.EqualTo(Color.FromArgb(255, 212, 212, 212)));
            Assert.That(_PomoViewModel.DarkColour, Is.EqualTo(Color.FromArgb(255, 128, 128, 128)));
            Assert.That(_PomoViewModel.BackgroundColour, Is.EqualTo(Color.FromArgb(255, 85, 85, 85)));
        }

        [TestCaseSource(typeof(TestCaseFactory), "ColourChangeTestCases")]
        public void OnPomoTimerChangeState_ColoursSetCorrectly(
            PomoTimerState state,
            Color lightColour, Color darkColour, Color backgroundColour)
        {
            _PomoViewModel.LightColour = Colors.Black;
            _PomoViewModel.DarkColour = Colors.Black;
            _PomoViewModel.BackgroundColour = Colors.Black;
            var pomoTimerStateChangeEventArgs = new PomoTimerStateChangeEventArgs(state);

            _MockPomoTimer.Raise(m => m.StateChanged += null, null, pomoTimerStateChangeEventArgs);

            Assert.That(_PomoViewModel.LightColour, Is.EqualTo(lightColour));
            Assert.That(_PomoViewModel.DarkColour, Is.EqualTo(darkColour));
            Assert.That(_PomoViewModel.BackgroundColour, Is.EqualTo(backgroundColour));
        }

        [TestCaseSource(typeof(TestCaseFactory), "PropertyChangeTestCases")]
        public void ChangeProperty_SendsPropertyChangedEvent(
            TestCaseFactory.PropertyChangeTestCase testCase)
        {
            testCase.SetupAction(_PomoViewModel);
            var mockPropertyChangedDelegate = new Mock<PropertyChangedEventHandler>();
            _PomoViewModel.PropertyChanged += mockPropertyChangedDelegate.Object;

            testCase.ChangeAction(_PomoViewModel);

            mockPropertyChangedDelegate.Verify(
                m => m(_PomoViewModel, It.Is<PropertyChangedEventArgs>(x => x.PropertyName == testCase.PropertyName)),
                Times.Once);
        }

        [TestCaseSource(typeof(TestCaseFactory), "PropertyChangeTestCases")]
        public void UnchangedProperty_DoesNotSendPropertyChangedEvent(
            TestCaseFactory.PropertyChangeTestCase testCase)
        {
            testCase.SetupAction(_PomoViewModel);
            var mockPropertyChangedDelegate = new Mock<PropertyChangedEventHandler>();
            _PomoViewModel.PropertyChanged += mockPropertyChangedDelegate.Object;

            // "Change" to same value as setup
            testCase.SetupAction(_PomoViewModel);

            mockPropertyChangedDelegate.Verify(
                m => m(_PomoViewModel, It.Is<PropertyChangedEventArgs>(x => x.PropertyName == testCase.PropertyName)),
                Times.Never);
        }

        public class TestCaseFactory
        {
            public static IEnumerable<PropertyChangeTestCase> PropertyChangeTestCases
            {
                get
                {
                    yield return new PropertyChangeTestCase
                    {
                        PropertyName = "Visible",
                        SetupAction = m => m.Visible = false,
                        ChangeAction = m => m.Visible = true
                    };
                    yield return new PropertyChangeTestCase
                    {
                        PropertyName = "TimerText",
                        SetupAction = m => m.TimerText = "startValue",
                        ChangeAction = m => m.TimerText = "newValue"
                    };
                    yield return new PropertyChangeTestCase
                    {
                        PropertyName = "LightColour",
                        SetupAction = m => m.LightColour = Colors.Black,
                        ChangeAction = m => m.LightColour = Colors.White
                    };
                    yield return new PropertyChangeTestCase
                    {
                        PropertyName = "DarkColour",
                        SetupAction = m => m.DarkColour = Colors.Black,
                        ChangeAction = m => m.DarkColour = Colors.White
                    };
                    yield return new PropertyChangeTestCase
                    {
                        PropertyName = "BackgroundColour",
                        SetupAction = m => m.BackgroundColour = Colors.Black,
                        ChangeAction = m => m.BackgroundColour = Colors.White
                    };
                }
            }

            public class PropertyChangeTestCase
            {
                public Action<PomoViewModel> SetupAction { get; set; }
                public Action<PomoViewModel> ChangeAction { get; set; }
                public string PropertyName { get; set; }
            }

            public static IEnumerable ColourChangeTestCases
            {
                get
                {
                    yield return new object[]
                    {
                        PomoTimerState.Disabled,
                        Color.FromArgb(255, 212, 212, 212),
                        Color.FromArgb(255, 128, 128, 128),
                        Color.FromArgb(255, 85, 85, 85)
                    };
                    yield return new object[]
                    {
                        PomoTimerState.Work,
                        Color.FromArgb(255, 212, 0, 0),
                        Color.FromArgb(255, 128, 0, 0),
                        Color.FromArgb(255, 85, 0, 0)
                    };
                    yield return new object[]
                    {
                        PomoTimerState.Rest,
                        Color.FromArgb(255, 0, 212, 0),
                        Color.FromArgb(255, 0, 128, 0),
                        Color.FromArgb(255, 0, 85, 0)
                    };
                }
            }
        }
    }
}
