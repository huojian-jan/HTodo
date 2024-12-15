using System.Printing;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Todo.Common;
using Todo.Message;
using Todo.Model;
using Todo.Views;
using WPF.Common;
using WPF.Common.Extentions;

namespace Todo.ViewModels
{
    public class LeftTabBarViewModel : Screen,IHandle<MainWindowMouseDownMessage>
    {
        private readonly IWindowManager _windowManager;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IEventAggregator _eventAggregator;

        public BindableCollection<LeftTabBarButton> LeftBarButtons { get; set; } = new();
        public BindableCollection<LeftTabBarButton> FunctionButtons { get; set; } = new();

        public LeftTabBarViewModel(IWindowManager windowManager,IViewModelFactory viewModelFactory, IEventAggregator eventAggregator)
        {
            Init();
            _windowManager = windowManager;
            _viewModelFactory = viewModelFactory;
            MoreView = _viewModelFactory.Create<LeftTabBarMoreViewModel>();
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnUIThread(this);
        }

        private void Init()
        {
            InitUpButtons();
            InitFunctionButtons();
        }

        private void InitUpButtons()
        {
            var task = new LeftTabBarButton()
            {
                Name = Constants.LeftTabBar_TaskName,
                Icon = Constants.LeftTabBar_TaskIcon,
                IsActive = false,
                ToolTips = Constants.LeftTabBar_TaskName
            };

            var focus = new LeftTabBarButton()
            {
                Name = Constants.LeftTabBar_FocusName,
                Icon = Constants.LeftTabBar_FocusIcon,
                IsActive = false,
                ToolTips = Constants.LeftTabBar_FocusName
            };

            var calendar = new LeftTabBarButton()
            {
                Name = Constants.LeftTabBar_CalendarName,
                Icon = Constants.LeftTabBar_CalendarIcon,
                IsActive = false,
                ToolTips = Constants.LeftTabBar_CalendarName
            };

            var behavior = new LeftTabBarButton()
            {
                Name = Constants.LeftTabBar_BehaviorName,
                Icon = Constants.LeftTabBar_BehaviroIcon,
                IsActive = false,
                ToolTips = Constants.LeftTabBar_BehaviorName
            };

            var search = new LeftTabBarButton()
            {
                Name = Constants.LeftTabBar_SearchName,
                Icon = Constants.LeftTabBar_SearchIcon,
                IsActive = false,
                ToolTips = Constants.LeftTabBar_SearchName
            };

            var quadrants = new LeftTabBarButton()
            {
                Name = Constants.LeftTabBar_QuadrantsName,
                Icon = Constants.LeftTabBar_QuadrantsIcon,
                IsActive = false,
                ToolTips = Constants.LeftTabBar_QuadrantsName
            };

            LeftBarButtons.Add(task);
            LeftBarButtons.Add(focus);
            LeftBarButtons.Add(calendar);
            LeftBarButtons.Add(behavior);
            LeftBarButtons.Add(search);
            LeftBarButtons.Add(quadrants);
        }

        private void InitFunctionButtons()
        {
            var sync = new LeftTabBarButton()
            {
                Name = Constants.LeftTabFunctionBtn_SyncName,
                Icon = Constants.LeftTabFunctionBtn_SyncIcon,
                IsActive = false,
                ToolTips = Constants.LeftTabFunctionBtn_SyncName
            };

            var notice = new LeftTabBarButton()
            {
                Name = Constants.LeftTabFunctionBtn_NoticeName,
                Icon = Constants.LeftTabFunctionBtn_NoticeIcon,
                IsActive = false,
                ToolTips = Constants.LeftTabFunctionBtn_NoticeName
            };
            
            var more = new LeftTabBarButton()
            {
                Name = Constants.LeftTabFunctionBtn_MoreName,
                Icon = Constants.LeftTabFunctionBtn_MoreIcon,
                IsActive = false,
                ToolTips = Constants.LeftTabFunctionBtn_MoreName
            };

            FunctionButtons.Add(sync);
            FunctionButtons.Add(notice);
            FunctionButtons.Add(more);
        }

        public void ActiveItemChanged(LeftTabBarButton context,MouseButtonEventArgs args)
        {
            args.Handled=true;
            LeftBarButtons.ToList().ForEach(x => x.IsActive = false);
            context.IsActive = true;

            if (context.Name == Constants.LeftTabFunctionBtn_SyncName ||
                context.Name == Constants.LeftTabFunctionBtn_NoticeName ||
                context.Name == Constants.LeftTabFunctionBtn_MoreName)
            {
                HandleFunctionButtons(context,args);
                return;
            }
            LeftTabClickChanged?.Invoke(new LeftTabClickChangedEventArgs(context.Name));
        }

        public LeftTabBarMoreViewModel MoreView { get; set; }

        private void HandleFunctionButtons(LeftTabBarButton context, MouseButtonEventArgs args)
        {
            if (context.Name == Constants.LeftTabFunctionBtn_SyncName)
            {
                Sync(context);
            }else if (context.Name == Constants.LeftTabFunctionBtn_NoticeName)
            {
                ShowNoticeView(args);
            }
            else if(context.Name== Constants.LeftTabFunctionBtn_MoreName)
            {
                ShowMoreView(args);
            }
            else
            {
                throw new InvalidOperationException("位置的功能按钮");
            }
        }

        private void ShowMoreView(MouseButtonEventArgs args)
        {
            var curView = GetView() as UserControl;
            var clickedPosition = args.GetPosition(curView);
            var viewSettings = new Dictionary<string, object>()
            {
                {"Width",143},
                {"Height",250},
                {"Left",curView.ActualWidth+7},
                {"Top",clickedPosition.Y-250+20}
            };
            _windowManager.ShowStatelessWindow(MoreView, viewSettings);
        }

        private void ShowNoticeView(MouseButtonEventArgs args)
        {
            //340,450
            var curView = GetView() as UserControl;
            var clickedPosition = args.GetPosition(curView);
            var vm = _viewModelFactory.Create<LeftButtonNoticeViewModel>();
            var viewSettings = new Dictionary<string, object>()
            {
                {"Width",340},
                {"Height",450},
                {"Left",curView.ActualWidth+7},
                {"Top",clickedPosition.Y-450+20}
            };
            _windowManager.ShowStatelessWindow(vm, viewSettings);
        }

        private void Sync(LeftTabBarButton context)
        {
            FunctionButtons.ToList().ForEach(x => x.IsRotate = false);
            context.IsRotate = true;
            FunctionButtons.FirstOrDefault(x=>x.Name==context.Name).IsRotate=true;
        }

        public event Action<LeftTabClickChangedEventArgs> LeftTabClickChanged;
        public Task HandleAsync(MainWindowMouseDownMessage message, CancellationToken cancellationToken)
        {
            _windowManager.CloseStatelessWindow();
            return Task.CompletedTask;
        }
    }

    public class LeftTabClickChangedEventArgs : EventArgs
    {
        public string Name { get; set; }

        public LeftTabClickChangedEventArgs(string name)
        {
            Name = name;
        }
    }
}
