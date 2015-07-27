using System.Windows;

namespace GoOutside.Views
{
    public partial class Pomo : Window
    {
        public Pomo()
        {
            InitializeComponent();
            MouseLeftButtonDown += delegate { DragMove(); };
        }
    }
}
