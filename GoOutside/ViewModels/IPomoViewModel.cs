namespace GoOutside.ViewModels
{
    public interface IPomoViewModel
    {
        double Height { get; }
        double Width { get; }
        bool Visible { get; set; }
        string TimerText { get; set; }
    }
}