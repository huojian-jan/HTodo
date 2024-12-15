using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Message;
using WPF.Common;

namespace Todo.ViewModels
{
    public class MainViewModel:Screen
    {
        private readonly IViewModelFactory _viewmodelFactoty;
        private readonly IEventAggregator _eventAggregator;
        public MainViewModel(IViewModelFactory viewModelFactory, IEventAggregator eventAggregator)
        {
            _viewmodelFactoty = viewModelFactory;

            LeftTabBar = _viewmodelFactoty.Create<LeftTabBarViewModel>();
            TaskView = _viewmodelFactoty.Create<TaskViewModel>();
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
        }


        public LeftTabBarViewModel LeftTabBar { get; set; }

        public TaskViewModel TaskView { get; set; }

        public async Task PreviewMouseDown()
        {
           await _eventAggregator.PublishOnUIThreadAsync(new MainWindowMouseDownMessage());
        }
    }
}
