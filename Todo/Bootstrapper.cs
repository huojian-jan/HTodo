using System.Collections.Generic;
using Autofac;
using System.Windows;
using System.Windows.Navigation;
using ControlToolKits.Controls;
using Todo.ViewModels;
using Todo.ViewModels.LogIn;
using WPF.Common;
using WPF.Common.Extentions;

namespace Todo
{
    public class Bootstrapper:HuojianBootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            base.Configure();
        }


        protected override void BuildContainer(ContainerBuilder builder)
        {
            base.BuildContainer(builder);
            builder.RegisterViewModels(GetType().Assembly);
        }

        protected async override void OnStartup(object sender, StartupEventArgs e)
        {
            CarouselControl control;
            var resources = GetAllResources();
            InitUIResource(resources.ToArray());

            await DisplayRootViewForAsync<MainViewModel>();
        }

        private List<string> GetAllResources()
        {
            var res=new List<string>();
            //根目录
            //res.Add("pack://application:,,,/Todo;component/resource/tasklist.xaml");
            //res.Add("pack://application:,,,/Todo;component/resource/colorresource.xaml");
            //res.Add("pack://application:,,,/Todo;component/resource/icons_dark.xaml");
            //res.Add("pack://application:,,,/Todo;component/resource/icons_light.xaml");
            //res.Add("pack://application:,,,/Todo;component/resource/icons_pink.xaml");
            //res.Add("pack://application:,,,/Todo;component/resource/pomoicons.xaml");
            //res.Add("pack://application:,,,/Todo;component/resource/sidebar.xaml");
            res.Add("pack://application:,,,/Todo;component/resource/svgresource.xaml");
            res.Add("pack://application:,,,/ControlToolKits;component/themes/Generic.xaml");
            //res.Add("pack://application:,,,/Todo;component/resource/tasklist.xaml");
            //res.Add("pack://application:,,,/Todo;component/resource/widgetstyle.xaml");



            //font resource
            //res.Add("pack://application:,,,/Todo;component/resource/fontresource/font_large.xaml");
            //res.Add("pack://application:,,,/Todo;component/resource/fontresource/font_middle.xaml");
            //res.Add("pack://application:,,,/Todo;component/resource/fontresource/font_normal.xaml");
            //res.Add("pack://application:,,,/Todo;component/resource/fontresource/fontfamily975maru.xaml");
            //res.Add("pack://application:,,,/Todo;component/resource/fontresource/fontfamilynormal.xaml");
            //res.Add("pack://application:,,,/Todo;component/resource/fontresource/fontfamilysourcehansans.xaml");
            //res.Add("pack://application:,,,/Todo;component/resource/fontresource/fontfamilyyahei.xaml");
            //res.Add("pack://application:,,,/Todo;component/resource/fontresource/fontfamilyyozai.xaml");

            //string resource
            //res.Add("pack://application:,,,/Todo;component/resource/widgetstyle.xaml");


            //themes
            //res.Add("pack://application:,,,/Todo;component/resource/widgetstyle.xaml");


            return res;
        }
    }
}
