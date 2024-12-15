using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using WPF.Common;

namespace Todo.ViewModels
{
    public class LeftTabBarMoreViewModel:Screen
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IWindowManager _windowManager;

        public BindableCollection<object> MoreViewButtons { get; set; } = new();

        public LeftTabBarMoreViewModel(IWindowManager windowManager,IViewModelFactory viewModelFactory)
        {
            _windowManager = windowManager;
            _viewModelFactory = viewModelFactory;
            Init();
        }


        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }

        private void Init()
        {
            for (var i = 0; i < 4; i++)
            {
                MoreViewButtons.Add(new object());
            }
        }

        public void Clicked(object dataContext)
        {

        }
    }
}
