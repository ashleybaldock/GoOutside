using System.Windows.Input;
using GoOutside.ViewModels;

namespace GoOutside.SampleData
{
    class PopUpViewModelSample : IPopUpViewModel
    {
        public bool Visible { get; set; }

        public double Top { get { return 0; } }
        public double Left { get { return 0; } }
        public double Height { get { return 200; } }
        public double Width { get { return 500; } }

        public PopUpViewModelSample()
        {
            Visible = true;
        }

        public ICommand DelayCommand { get { return null; } }
        public ICommand ShowPopUpCommand { get { return null; } }
        public ICommand HidePopUpCommand { get { return null; } }
    }
}
