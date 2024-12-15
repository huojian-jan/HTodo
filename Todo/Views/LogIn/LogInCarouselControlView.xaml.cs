using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Todo.Views.LogIn
{
    /// <summary>
    /// Interaction logic for LogInCarouselControlView.xaml
    /// </summary>
    public partial class LogInCarouselControlView : UserControl
    {
        public LogInCarouselControlView()
        {
            InitializeComponent();
            this.Loaded += LogInCarouselControlView_Loaded;
        }

        private void LogInCarouselControlView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Image_OnImageFailed(object? sender, ExceptionRoutedEventArgs e)
        {
            var image=sender as Image;
            var path = image.Source;
        }

        private void Image_OnInitialized(object? sender, EventArgs e)
        {
            var image=sender as Image;
            var path = image.Source;
        }

        private void Image_OnLoaded(object sender, RoutedEventArgs e)
        {
            var image=sender as Image;
            var path = image.Source;
        }

        private void Image_OnSourceUpdated(object? sender, DataTransferEventArgs e)
        {
            var image=sender as Image;
            var path = image.Source;
        }
    }
}
