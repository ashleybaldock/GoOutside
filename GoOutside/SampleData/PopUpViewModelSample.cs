using System.Windows.Input;
using GoOutside.ViewModels;

namespace GoOutside.SampleData
{
    class PopUpViewModelSample : IPopUpViewModel
    {
        public bool Visible { get; set; }

        public double Top { get; set; }

        public PopUpViewModelSample()
        {
            Visible = true;
            Top = 200;
        }

        public ICommand DelayCommand { get { return null; } }
        public ICommand ShowPopUpCommand { get { return null; } }
        public ICommand HidePopUpCommand { get { return null; } }
    }
}
