namespace GoOutside.ViewModels
{
    public class PomoViewModel : IPomoViewModel
    {
        public PomoViewModel()
        {
            Visible = true;
            TimerText = "25:00";
        }

        public double Height { get { return 200; } }
        public double Width { get { return 200; } }
        public bool Visible { get; set; }
        public string TimerText { get; set; }
    }
}