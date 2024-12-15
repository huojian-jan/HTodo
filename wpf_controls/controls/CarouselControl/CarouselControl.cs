using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace ControlToolKits.Controls
{
    [TemplatePart(Name = "PART_CurrentContent", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_NextContent", Type = typeof(ContentControl))]
    public class CarouselControl : ListBox
    {
        private readonly DispatcherTimer _dispatcherTimer;
        private ContentControl PART_CurrentContent;
        private ContentControl PART_NextContent;

        private int _preIndex = 0;
        private ThicknessAnimation _prev_Animation;
        private ThicknessAnimation _next_Animation;

        public CarouselControl()
        {
            SelectionChanged += CarouselControl_SelectionChanged;
            this.Loaded += CarouselControl_Loaded;
            this.Unloaded += CarouselControl_Unloaded;
            _dispatcherTimer = new DispatcherTimer();
        }

        public List<FrameworkElement> ContentItems { get; set; } = new List<FrameworkElement>();


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_CurrentContent = GetTemplateChild("PART_CurrentContent") as ContentControl;
            PART_NextContent = GetTemplateChild("PART_NextContent") as ContentControl;
            for (int i = 0; i < ContentItems.Count; i++)
            {
                Items.Add(new ListBoxItem());
            }

            SelectedIndex = 0;
        }


        private void CarouselControl_Loaded(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            _dispatcherTimer.Tick += (s, re) =>
            {
                _preIndex = SelectedIndex;
                var currentIndex = SelectedIndex + 1;
                if (currentIndex >= Items.Count)
                    SelectedIndex = 0;
                else
                    SelectedIndex = currentIndex;
            };
            _dispatcherTimer.Start();
        }

        private void CarouselControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Items.Count == 0)
                return;

            if (PART_CurrentContent.Content == null)
            {
                PART_CurrentContent.Content = ContentItems[SelectedIndex];
                return;
            }

            Roll();
        }

        private void CarouselControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Stop();
        }

        private void Roll()
        {
            _prev_Animation = new ThicknessAnimation();
            _prev_Animation.From = new Thickness(0, 0, 0, 0);
            _prev_Animation.AccelerationRatio = 0.3;
            _prev_Animation.DecelerationRatio = 0.3;
            _prev_Animation.To = new Thickness(-ActualWidth, 0, ActualWidth, 0);
            _prev_Animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));

            _next_Animation = new ThicknessAnimation();
            _next_Animation.AccelerationRatio = 0.3;
            _next_Animation.DecelerationRatio = 0.3;
            _next_Animation.From = new Thickness(ActualWidth, 0, -ActualWidth, 0);
            _next_Animation.To = new Thickness(0, 0, 0, 0);
            _next_Animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            PART_NextContent.Content = ContentItems[SelectedIndex];
            PART_CurrentContent.Content = ContentItems[_preIndex];

            PART_NextContent.BeginAnimation(MarginProperty, _next_Animation);

            PART_CurrentContent.BeginAnimation(MarginProperty, _prev_Animation);
        }
    }

}
