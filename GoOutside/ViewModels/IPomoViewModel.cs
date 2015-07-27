using System.Windows.Input;

namespace GoOutside.ViewModels
{
    public interface IPomoViewModel
    {
        double Height { get; }
        double Width { get; }
        bool Visible { get; set; }
        string TimerText { get; set; }

        ICommand OnMouseEnter { get; }
        ICommand OnMouseLeave { get; }
        ICommand OnMouseClick { get; }
        ICommand Show { get; }
        ICommand Hide { get; }
    }
}