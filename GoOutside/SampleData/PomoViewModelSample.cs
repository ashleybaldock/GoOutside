using System.Windows.Input;
using GoOutside.ViewModels;

namespace GoOutside.SampleData
{
    class PomoViewModelSample : IPomoViewModel
    {
        public double Height { get { return 200; } }
        public double Width { get { return 200; } }
        public bool Visible { get; set; }
        public string TimerText { get; set; }
        public ICommand OnMouseEnter { get; private set; }
        public ICommand OnMouseLeave { get; private set; }
        public ICommand OnMouseClick { get; private set; }
        public ICommand Show { get; private set; }
        public ICommand Hide { get; private set; }

        public PomoViewModelSample()
        {
            Visible = true;
            TimerText = "25:00";
        }
    }
}