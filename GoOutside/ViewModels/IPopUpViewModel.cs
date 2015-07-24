using System.Windows.Input;

namespace GoOutside.ViewModels
{
    public interface IPopUpViewModel
    {
        bool Visible { get; set; }
        double Top { get; set; }
        ICommand DelayCommand { get; }
        ICommand ShowPopUpCommand { get; }
        ICommand HidePopUpCommand { get; }
    }
}