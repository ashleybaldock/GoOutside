using System.Windows.Input;

namespace GoOutside.ViewModels
{
    public interface IPopUpViewModel
    {
        bool Visible { get; set; }
        double Height { get; }
        double Width { get; }
        double Top { get; }
        double Left { get; }
        ICommand DelayCommand { get; }
        ICommand ShowPopUpCommand { get; }
        ICommand HidePopUpCommand { get; }
    }
}