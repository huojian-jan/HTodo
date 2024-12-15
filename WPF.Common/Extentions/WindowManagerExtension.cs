using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Caliburn.Micro;

namespace WPF.Common.Extentions
{
    public static class WindowManagerExtension
    {
        private static Window _window;
        public static void ShowStatelessWindow(this IWindowManager manager, object dataContext, IDictionary<string, object> viewSettings)
        {
            if (_window!=null)
            {
                manager.CloseStatelessWindow();
            }
            _window = new Window();
            _window.ResizeMode = ResizeMode.NoResize;
            _window.WindowStyle = WindowStyle.None;

            var effect = new DropShadowEffect();
            effect.ShadowDepth = 4;
            effect.BlurRadius = 8;
            effect.Color = Color.FromRgb(10, 13, 14);
            effect.Opacity = 0.15;

            var border = new Border();
            border.CornerRadius = new CornerRadius(8, 8, 8, 8);
            border.Effect = effect;
            var content = new ContentControl();
            var viewTypeName = dataContext.GetType().Name.Replace("ViewModel", "View");
            var viewType = dataContext.GetType().Assembly.GetTypes().FirstOrDefault(x => x.Name == viewTypeName);

            var view = ViewLocator.GetOrCreateViewType(viewType);
            ViewLocator.InitializeComponent(view);
            ViewModelBinder.Bind(dataContext, view,default);

            content.Content = view;
            border.Child = content;
            border.BorderThickness = new Thickness(1);
            border.CornerRadius = new CornerRadius(8);
            border.BorderBrush = new SolidColorBrush(Colors.White);
            border.Padding = new Thickness(3);
            _window.Background = new SolidColorBrush(Colors.Transparent);
            _window.Content = border;
            _window.ApplySettings(viewSettings);
            _window.Show();
        }

        public static void CloseStatelessWindow(this IWindowManager windowManager)
        {
            if (_window != null)
            {
                _window.DataContext = default;
                _window.Content = default;
                _window.Close();
                _window = default;
            }
        }
    }
}
