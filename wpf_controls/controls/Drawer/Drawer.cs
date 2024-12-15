using ControlToolKits.Commands;
using ControlToolKits.Converters;
using ControlToolKits.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ControlToolKits.Controls
{
    [ContentProperty("Content")]
    public class Drawer : FrameworkElement
    {
        private Storyboard _storyboard;

        private AdornerContainer _container;

        private ContentControl _animationControl;

        private TranslateTransform _translateTransform;

        private double _animationLength;

        private string _animationPropertyName;

        private FrameworkElement _maskElement;

        private AdornerLayer _layer;

        private UIElement _contentElement;

        private Point _contentRenderTransformOrigin;

        private Thumb _thumb;

        static Drawer()
        {
            DataContextProperty.OverrideMetadata(typeof(Drawer), new FrameworkPropertyMetadata(DataContextPropertyChanged));
        }

        public Drawer()
        {
            Loaded += Drawer_Loaded;
            Unloaded += Drawer_Unloaded;
        }

        private void Drawer_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsOpen)
            {
                OnIsOpenChanged(true);
            }
        }

        private void Drawer_Unloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Drawer_Loaded;

            if (_maskElement != null)
            {
                _maskElement.PreviewMouseLeftButtonDown -= MaskElement_PreviewMouseLeftButtonDown;
            }

            if (_storyboard != null)
            {
                _storyboard.Completed -= Storyboard_Completed;
            }
            if (_thumb != null)
            {
                _thumb.DragDelta -= _thumb_DragDelta;
            }

            _container = null;
        }

        private static void DataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((Drawer)d).OnDataContextPropertyChanged(e);

        private void OnDataContextPropertyChanged(DependencyPropertyChangedEventArgs e) => UpdateDataContext(_animationControl, e.OldValue, e.NewValue);

        public static readonly RoutedEvent OpenedEvent =
            EventManager.RegisterRoutedEvent("Opened", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(Drawer));

        public event RoutedEventHandler Opened
        {
            add => AddHandler(OpenedEvent, value);
            remove => RemoveHandler(OpenedEvent, value);
        }

        public static readonly RoutedEvent ClosedEvent =
            EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(Drawer));

        public event RoutedEventHandler Closed
        {
            add => AddHandler(ClosedEvent, value);
            remove => RemoveHandler(ClosedEvent, value);
        }

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            "IsOpen", typeof(bool), typeof(Drawer), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsOpenChanged));

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (Drawer)d;
            ctl.OnIsOpenChanged((bool)e.NewValue);
        }

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public static readonly DependencyProperty MaskCanCloseProperty = DependencyProperty.Register(
            "MaskCanClose", typeof(bool), typeof(Drawer), new PropertyMetadata(true));

        public bool MaskCanClose
        {
            get => (bool)GetValue(MaskCanCloseProperty);
            set => SetValue(MaskCanCloseProperty, value);
        }

        public static readonly DependencyProperty ShowMaskProperty = DependencyProperty.Register(
            "ShowMask", typeof(bool), typeof(Drawer), new PropertyMetadata(true));

        public bool ShowMask
        {
            get => (bool)GetValue(ShowMaskProperty);
            set => SetValue(ShowMaskProperty, value);
        }

        public static readonly DependencyProperty DockProperty = DependencyProperty.Register(
            "Dock", typeof(Dock), typeof(Drawer), new PropertyMetadata(default(Dock)));

        public Dock Dock
        {
            get => (Dock)GetValue(DockProperty);
            set => SetValue(DockProperty, value);
        }

        public static readonly DependencyProperty ShowModeProperty = DependencyProperty.Register(
            "ShowMode", typeof(DrawerShowMode), typeof(Drawer), new PropertyMetadata(default(DrawerShowMode)));

        public DrawerShowMode ShowMode
        {
            get => (DrawerShowMode)GetValue(ShowModeProperty);
            set => SetValue(ShowModeProperty, value);
        }

        public static readonly DependencyProperty MaskBrushProperty = DependencyProperty.Register(
            "MaskBrush", typeof(Brush), typeof(Drawer), new PropertyMetadata(default(Brush)));

        public Brush MaskBrush
        {
            get => (Brush)GetValue(MaskBrushProperty);
            set => SetValue(MaskBrushProperty, value);
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(object), typeof(Drawer), new PropertyMetadata(default(object)));

        //写在Drawer内部的控件
        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public bool CanDrag
        {
            get { return (bool)GetValue(CanDragProperty); }
            set { SetValue(CanDragProperty, value); }
        }

        public static readonly DependencyProperty CanDragProperty =
            DependencyProperty.Register("CanDrag", typeof(bool), typeof(Drawer), new PropertyMetadata(false));

        private void CreateContainer()
        {
            _storyboard = new Storyboard();
            _storyboard.Completed += Storyboard_Completed;

            _translateTransform = new TranslateTransform();
            _animationControl = new ContentControl
            {
                Content = Content,
                RenderTransform = _translateTransform,
                DataContext = this
            };
            _animationControl.SetBinding(MinHeightProperty, new Binding("MinHeight")
            {
                Source = this,
            });
            _animationControl.SetBinding(MinWidthProperty, new Binding("MinWidth")
            {
                Source = this,
            });
            var panel = new Grid()
            {
                ClipToBounds = true,
            };

            var internalPanel = new Grid()
            {
                ClipToBounds = true,
            };
            if (ShowMask)
            {
                _maskElement = new Border
                {
                    Background = MaskBrush,
                    Opacity = 0,
                };
                _maskElement.PreviewMouseLeftButtonDown += MaskElement_PreviewMouseLeftButtonDown;
                panel.Children.Add(_maskElement);
            }

            _animationControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var size = _animationControl.DesiredSize;
            _animationControl.SetBinding(WidthProperty, new Binding("ActualWidth") { Source = _container });
            _animationControl.SetBinding(HeightProperty, new Binding("ActualHeight") { Source = _container });
            _animationControl.HorizontalAlignment = HorizontalAlignment.Stretch;
            _animationControl.VerticalAlignment = VerticalAlignment.Stretch;
            switch (Dock)
            {
                case Dock.Left:
                    internalPanel.HorizontalAlignment = HorizontalAlignment.Left;
                    internalPanel.VerticalAlignment = VerticalAlignment.Stretch;
                    _translateTransform.X = -size.Width;
                    _animationLength = -size.Width;
                    _animationPropertyName = "(UIElement.RenderTransform).(TranslateTransform.X)";
                    break;
                case Dock.Top:
                    internalPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                    internalPanel.VerticalAlignment = VerticalAlignment.Top;
                    _translateTransform.Y = -size.Height;
                    _animationLength = -size.Height;
                    _animationPropertyName = "(UIElement.RenderTransform).(TranslateTransform.Y)";
                    break;
                case Dock.Right:
                    internalPanel.HorizontalAlignment = HorizontalAlignment.Right;
                    internalPanel.VerticalAlignment = VerticalAlignment.Stretch;
                    _translateTransform.X = size.Width;
                    _animationLength = size.Width;
                    _animationPropertyName = "(UIElement.RenderTransform).(TranslateTransform.X)";
                    break;
                case Dock.Bottom:
                    internalPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                    internalPanel.VerticalAlignment = VerticalAlignment.Bottom;
                    _translateTransform.Y = size.Height;
                    _animationLength = size.Height;
                    _animationPropertyName = "(UIElement.RenderTransform).(TranslateTransform.Y)";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _animationControl.DataContext = DataContext;
            _animationControl.CommandBindings.Clear();
            _animationControl.CommandBindings.Add(new CommandBinding(ControlCommands.Close, (s, e) => SetCurrentValue(IsOpenProperty, false)));
            internalPanel.Children.Add(_animationControl);
            internalPanel.SetBinding(WidthProperty, new Binding("Width") { Source = this, Mode = BindingMode.TwoWay, Converter = new MathConverter(), ConverterParameter = "@VALUE+10" });
            internalPanel.SetBinding(HeightProperty, new Binding("Height") { Source = this, Mode = BindingMode.TwoWay, Converter = new MathConverter(), ConverterParameter = "@VALUE+10" });
            if (CanDrag)
            {
                _thumb = new Thumb()
                {
                    BorderThickness = new Thickness(0),
                    Cursor = (Dock == Dock.Left || Dock == Dock.Right) ? Cursors.SizeWE : Cursors.SizeNS,
                    Background = Brushes.Transparent,
                    Foreground = Brushes.Transparent,
                };
                if (Dock == Dock.Left)
                {
                    _thumb.Width = 5;
                    _thumb.Margin = new Thickness(0, 0, 10, 0);
                    _thumb.VerticalAlignment = VerticalAlignment.Stretch;
                    _thumb.HorizontalAlignment = HorizontalAlignment.Right;
                    _animationControl.Margin = new Thickness(0, 0, 10, 0);
                    internalPanel.SetBinding(MaxWidthProperty, new Binding("ActualWidth")
                    {
                        Source = panel,
                        Converter = new MathConverter(),
                        ConverterParameter = "@VALUE*0.8"
                    });
                }
                else if (Dock == Dock.Right)
                {
                    _thumb.Width = 5;
                    _thumb.Margin = new Thickness(10, 0, 0, 0);
                    _animationControl.Margin = new Thickness(10, 0, 0, 0);
                    _thumb.VerticalAlignment = VerticalAlignment.Stretch;
                    _thumb.HorizontalAlignment = HorizontalAlignment.Left;
                    internalPanel.SetBinding(MaxWidthProperty, new Binding("ActualWidth")
                    {
                        Source = panel,
                        Converter = new MathConverter(),
                        ConverterParameter = "@VALUE*0.8"
                    });
                }
                else if (Dock == Dock.Top)
                {
                    _thumb.Height = 5;
                    _thumb.Margin = new Thickness(0, 0, 0, 10);
                    _animationControl.Margin = new Thickness(0, 0, 0, 10);
                    _thumb.VerticalAlignment = VerticalAlignment.Bottom;
                    _thumb.HorizontalAlignment = HorizontalAlignment.Stretch;
                    internalPanel.SetBinding(MaxHeightProperty, new Binding("ActualHeight")
                    {
                        Source = panel,
                        Converter = new MathConverter(),
                        ConverterParameter = "@VALUE*0.8"
                    });
                }
                else
                {
                    _thumb.Height = 5;
                    _thumb.Margin = new Thickness(0, 10, 0, 0);
                    _animationControl.Margin = new Thickness(0, 10, 0, 0);
                    _thumb.VerticalAlignment = VerticalAlignment.Top;
                    _thumb.HorizontalAlignment = HorizontalAlignment.Stretch;
                    internalPanel.SetBinding(MaxHeightProperty, new Binding("ActualHeight")
                    {
                        Source = panel,
                        Converter = new MathConverter(),
                        ConverterParameter = "@VALUE*0.8"
                    });
                }
                //这里只create 一次因此不考虑解绑， 在unload 的时候解绑;
                _thumb.DragDelta += _thumb_DragDelta;
                internalPanel.Children.Add(_thumb);
            }
            panel.Children.Add(internalPanel);
            _container = new AdornerContainer(_layer)
            {
                Child = panel,
                ClipToBounds = true
            };
        }

        private void _thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Dock == Dock.Left || Dock == Dock.Right)
            {
                if (e.HorizontalChange != 0 && !double.IsNaN(e.HorizontalChange))
                {
                    var currentWidth = Dock == Dock.Left ? _animationControl.ActualWidth + e.HorizontalChange : _animationControl.ActualWidth - e.HorizontalChange;
                    if (currentWidth >= _animationControl.MinWidth && currentWidth <= _animationControl.MaxWidth)
                    {
                        Width = currentWidth;
                    }
                }
            }
            else
            {
                if (e.VerticalChange != 0 && !double.IsNaN(e.VerticalChange))
                {
                    var currentHeight = Dock == Dock.Top ? _animationControl.ActualHeight + e.VerticalChange : _animationControl.ActualHeight - e.VerticalChange;
                    if (currentHeight >= _animationControl.MinHeight && currentHeight <= _animationControl.MaxHeight)
                    {
                        Height = currentHeight;
                    }
                }
            }

        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            if (!IsOpen)
            {
                _contentElement.SetCurrentValue(RenderTransformOriginProperty, _contentRenderTransformOrigin);
                _layer.Remove(_container);
                RaiseEvent(new RoutedEventArgs(ClosedEvent, this));
            }
            else
            {
                RaiseEvent(new RoutedEventArgs(OpenedEvent, this));
            }
        }

        private void OnIsOpenChanged(bool isOpen)
        {
            //drawer空间是空时，不显示
            if (Content == null)
            {
                return;
            }

            AdornerDecorator decorator;
            var parent = WpfHelper.FindAncestor<DrawerContainer>(this);
            if (parent != null)
            {
                _contentElement = parent.Child;
                decorator = parent;
            }
            else
            {
                if (!WpfHelper.TryGetActiveWindow(out var window))
                    return;

                decorator = WpfHelper.FindChild<AdornerDecorator>(window);
                _contentElement = window.Content as UIElement;
            }

            if (_contentElement == null)
            {
                return;
            }

            _layer = decorator?.AdornerLayer;
            if (_layer == null)
            {
                return;
            }

            _contentRenderTransformOrigin = _contentElement.RenderTransformOrigin;

            if (_container == null)
            {
                CreateContainer();
            }

            switch (ShowMode)
            {
                case DrawerShowMode.Push:
                    ShowByPush(isOpen);
                    break;
                case DrawerShowMode.Press:
                    _contentElement.SetCurrentValue(RenderTransformOriginProperty, new Point(0.5, 0.5));
                    ShowByPress(isOpen);
                    break;
            }

            if (isOpen)
            {
                if (_maskElement != null)
                {
                    var maskAnimation = CreateAnimation(1);
                    Storyboard.SetTarget(maskAnimation, _maskElement);
                    Storyboard.SetTargetProperty(maskAnimation, new PropertyPath(OpacityProperty.Name));
                    _storyboard.Children.Add(maskAnimation);
                }

                var drawerAnimation = CreateAnimation(0);
                Storyboard.SetTarget(drawerAnimation, _animationControl);
                Storyboard.SetTargetProperty(drawerAnimation, new PropertyPath(_animationPropertyName));
                _storyboard.Children.Add(drawerAnimation);
                _layer.Remove(_container);
                _layer.Add(_container);
            }
            else
            {
                if (_maskElement != null)
                {
                    var maskAnimation = CreateAnimation(0);
                    Storyboard.SetTarget(maskAnimation, _maskElement);
                    Storyboard.SetTargetProperty(maskAnimation, new PropertyPath(OpacityProperty.Name));
                    _storyboard.Children.Add(maskAnimation);
                }

                var drawerAnimation = CreateAnimation(_animationLength);
                Storyboard.SetTarget(drawerAnimation, _animationControl);
                Storyboard.SetTargetProperty(drawerAnimation, new PropertyPath(_animationPropertyName));
                _storyboard.Children.Add(drawerAnimation);
            }
            _storyboard.Begin();
        }

        private void ShowByPush(bool isOpen)
        {
            string animationPropertyName;

            switch (Dock)
            {
                case Dock.Left:
                case Dock.Right:
                    animationPropertyName = "(UIElement.RenderTransform).(TranslateTransform.X)";
                    _contentElement.RenderTransform = new TranslateTransform
                    {
                        X = isOpen ? 0 : -_animationLength
                    };
                    break;
                case Dock.Top:
                case Dock.Bottom:
                    animationPropertyName = "(UIElement.RenderTransform).(TranslateTransform.Y)";
                    _contentElement.RenderTransform = new TranslateTransform
                    {
                        Y = isOpen ? 0 : -_animationLength
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var animation = isOpen ? CreateAnimation(-_animationLength) : CreateAnimation(0);
            Storyboard.SetTarget(animation, _contentElement);
            Storyboard.SetTargetProperty(animation, new PropertyPath(animationPropertyName));

            _storyboard.Children.Add(animation);
        }

        private void ShowByPress(bool isOpen)
        {
            _contentElement.RenderTransform = isOpen
                ? new ScaleTransform()
                : new ScaleTransform
                {
                    ScaleX = 0.9,
                    ScaleY = 0.9
                };

            var animationX = isOpen ? CreateAnimation(.9) : CreateAnimation(1);
            Storyboard.SetTarget(animationX, _contentElement);
            Storyboard.SetTargetProperty(animationX, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));

            _storyboard.Children.Add(animationX);

            var animationY = isOpen ? CreateAnimation(.9) : CreateAnimation(1);
            Storyboard.SetTarget(animationY, _contentElement);
            Storyboard.SetTargetProperty(animationY, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));

            _storyboard.Children.Add(animationY);
        }

        private void MaskElement_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MaskCanClose)
            {
                SetCurrentValue(IsOpenProperty, false);
            }
        }

        private void UpdateDataContext(FrameworkElement target, object oldValue, object newValue)
        {
            if (target == null || BindingOperations.GetBindingExpression(target, DataContextProperty) != null) return;
            if (ReferenceEquals(this, target.DataContext) || Equals(oldValue, target.DataContext))
            {
                target.DataContext = newValue ?? this;
            }
        }

        public static DoubleAnimation CreateAnimation(double toValue, double milliseconds = 200)
        {
            return new(toValue, new Duration(TimeSpan.FromMilliseconds(milliseconds)))
            {
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut }
            };
        }
    }

}
