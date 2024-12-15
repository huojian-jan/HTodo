using System.Windows;
using System.Windows.Input;

namespace Todo.Views.LogIn
{
    /// <summary>
    /// Interaction logic for LogIn.xaml
    /// </summary>
    public partial class LogInView : Window
    {
        public LogInView()
        {
            InitializeComponent();
        }

        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
