using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using GoOutside.Events;
using GoOutside.Timers;
using GoOutside.Timers.Events;
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
        public void CheckInitialValues()
        {
            Assert.That(_PomoViewModel.Height, Is.EqualTo(200));
            Assert.That(_PomoViewModel.Width, Is.EqualTo(200));
            Assert.That(_PomoViewModel.Visible, Is.True);
            Assert.That(_PomoViewModel.TimerText, Is.EqualTo("Start"));
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
            _MockPomoTimer.SetupGet(m => m.Running).Returns(false);

            _PomoViewModel.OnMouseClick.Execute(null);

            _MockPomoTimer.Verify(m => m.Start(), Times.Once);
            _MockPomoTimer.Verify(m => m.Stop(), Times.Never);
        }

        [Test]
        public void IfTimerStarted_OnMouseClick_StopsPomoTimer()
        {
            _MockPomoTimer.SetupGet(m => m.Running).Returns(true);

            _PomoViewModel.OnMouseClick.Execute(null);

            _MockPomoTimer.Verify(m => m.Start(), Times.Never);
            _MockPomoTimer.Verify(m => m.Stop(), Times.Once);
        }

        [Test]
        public void PomoTimerTick_UpdatesText()
        {
            _PomoViewModel.TimerText = "initialValue";
            _MockPomoTimer.SetupGet(m => m.Running).Returns(true);

            var timeRemaining = TimeSpan.FromMinutes(10);
            var pomoTimerEventArgs = new CountdownTickEventArgs(timeRemaining);
            _MockPomoTimer.Raise(m => m.Tick += null, null, pomoTimerEventArgs);

            Assert.That(_PomoViewModel.TimerText, Is.EqualTo("10:00"));
        }

        [Test]
        public void IfTimerStarted_OnMouseEnter_ShowsStop()
        {
            _PomoViewModel.TimerText = "initialValue";
            _MockPomoTimer.SetupGet(m => m.Running).Returns(true);

            _PomoViewModel.OnMouseEnter.Execute(null);

            Assert.That(_PomoViewModel.TimerText, Is.EqualTo("Cancel"));
        }

        [Test]
        public void IfTimerStopped_OnMouseEnter_ShowsStart()
        {
            _PomoViewModel.TimerText = "initialValue";
            _MockPomoTimer.SetupGet(m => m.Running).Returns(false);

            _PomoViewModel.OnMouseEnter.Execute(null);

            Assert.That(_PomoViewModel.TimerText, Is.EqualTo("Start"));
        }

        [Test]
        public void IfMouseEntered_PomoTimerTick_DoesNotUpdateText()
        {
            _PomoViewModel.TimerText = "initialValue";
            _MockPomoTimer.SetupGet(m => m.Running).Returns(true);
            var timeRemaining = TimeSpan.FromMinutes(10);
            var pomoTimerEventArgs = new CountdownTickEventArgs(timeRemaining);

            _PomoViewModel.OnMouseEnter.Execute(null);
            _MockPomoTimer.Raise(m => m.Tick += null, null, pomoTimerEventArgs);

            Assert.That(_PomoViewModel.TimerText, Is.EqualTo("Cancel"));
        }

        [Test]
        public void OnMouseLeave_EnablesTickUpdates()
        {
            _PomoViewModel.TimerText = "initialValue";
            _MockPomoTimer.SetupGet(m => m.Running).Returns(true);
            var timeRemaining = TimeSpan.FromMinutes(10);
            var pomoTimerEventArgs = new CountdownTickEventArgs(timeRemaining);
            _PomoViewModel.OnMouseEnter.Execute(null);

            _PomoViewModel.OnMouseLeave.Execute(null);
            _MockPomoTimer.Raise(m => m.Tick += null, null, pomoTimerEventArgs);

            Assert.That(_PomoViewModel.TimerText, Is.EqualTo("10:00"));
        }

        [Test]
        public void OnMouseLeave_IfTimerNotRunning_SetsTextToStart()
        {
            _PomoViewModel.TimerText = "initialValue";
            _MockPomoTimer.SetupGet(m => m.Running).Returns(false);

            _PomoViewModel.OnMouseLeave.Execute(null);

            Assert.That(_PomoViewModel.TimerText, Is.EqualTo("Start"));
        }

        [Test]
        public void OnCreate_ColourSetToDisabled()
        {
            Assert.That(_PomoViewModel.ColourSet, Is.EqualTo(TomatoColours.Disabled));
        }

        [TestCaseSource("_ColourChangeTestCases")]
        public void OnPomoTimerChangeState_ColoursSetCorrectly(
            PomoTimerState state,
            TomatoColours.TomatoColourSet initialColourSet,
            TomatoColours.TomatoColourSet expectedColourSet)
        {
            _PomoViewModel.ColourSet = TomatoColours.Disabled;
            var pomoTimerStateChangeEventArgs = new PomoTimerStateChangeEventArgs(state);

            _MockPomoTimer.Raise(m => m.StateChanged += null, null, pomoTimerStateChangeEventArgs);

            Assert.That(_PomoViewModel.ColourSet, Is.EqualTo(expectedColourSet));
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

        private static readonly object[] _ColourChangeTestCases =
        {
            new object[] { PomoTimerState.Disabled, TomatoColours.Work, TomatoColours.Disabled },
            new object[] { PomoTimerState.Disabled, TomatoColours.Rest, TomatoColours.Disabled },
            new object[] { PomoTimerState.Work, TomatoColours.Rest, TomatoColours.Work },
            new object[] { PomoTimerState.Work, TomatoColours.Disabled, TomatoColours.Work },
            new object[] { PomoTimerState.Rest, TomatoColours.Work, TomatoColours.Rest },
            new object[] { PomoTimerState.Rest, TomatoColours.Disabled, TomatoColours.Rest },
        };

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
                        PropertyName = "ColourSet",
                        SetupAction = m => m.ColourSet = TomatoColours.Work,
                        ChangeAction = m => m.ColourSet = TomatoColours.Rest
                    };
                }
            }

            public class PropertyChangeTestCase
            {
                public Action<PomoViewModel> SetupAction { get; set; }
                public Action<PomoViewModel> ChangeAction { get; set; }
                public string PropertyName { get; set; }
            }
        }
    }
}
