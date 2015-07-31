using System.Windows.Input;
using System.Windows.Media;
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

        public Color LightColour { get; set; }
        public Color DarkColour { get; set; }
        public Color BackgroundColour { get; set; }
        public Color StemColour { get; set; }

        public TomatoColours.TomatoColourSet ColourSet { get; set; }

        public PomoViewModelSample()
        {
            Visible = true;
            TimerText = "25:00";
            LightColour = Color.FromArgb(255, 212, 0, 0);
            DarkColour = Color.FromArgb(255, 128, 0, 0);
            BackgroundColour = Color.FromArgb(255, 85, 0, 0);
            StemColour = Color.FromArgb(255, 204, 255, 0);
            ColourSet = TomatoColours.Work;
        }
    }
}