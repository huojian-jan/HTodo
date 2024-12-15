using Autofac;
using Caliburn.Micro;
using System.Windows;
using ControlToolKits.Controls;

namespace WPF.Common
{
    public class HuojianBootstrapperBase : BootstrapperBase, IServiceProvider, ISupportChildScope
    {
        private readonly Stack<ILifetimeScope> _parentScopeStack = new Stack<ILifetimeScope>();
        protected ILifetimeScope _scope;

        protected override void StartDesignTime()
        {
            //TODO,注释掉的原因看影刀的注释
            //base.StartDesignTime();
        }

        protected override void Configure()
        {
            base.Configure();

            var builder = new ContainerBuilder();
            BuildContainer(builder);

            _scope = builder.Build();
            _parentScopeStack.Push(_scope);
        }

        protected virtual void InitUIResource(string[] resources)
        {
            CarouselControl control;
            foreach (var resource in resources)
            {
                Application.Resources.MergedDictionaries.Add(new ResourceDictionary
                {
                    Source = new Uri(resource, UriKind.RelativeOrAbsolute)
                });
            }
        }

        protected virtual void BuildContainer(ContainerBuilder builder)
        {
            builder.RegisterType<EventAggregator>()
                .As<IEventAggregator>()
                .SingleInstance();

            // 注册窗口处理器
            builder.RegisterType<RichWindowManager>()
                .As<IWindowManager>()
                .SingleInstance();

            // 注册布局管理器
            //builder.RegisterInstance(new LayoutManager(""))
            //    .AsSelf();

            builder.RegisterType<DefaultViewModelFactory>()
                .As<IViewModelFactory>()
                .SingleInstance();

            builder.RegisterInstance(this)
                .As<ISupportChildScope>()
                .As<IServiceProvider>();
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            var type = typeof(IEnumerable<>).MakeGenericType(service);
            return _scope.Resolve(type) as IEnumerable<object>;
        }

        protected override object GetInstance(Type service, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                if (_scope.IsRegistered(service))
                    return _scope.Resolve(service);
            }
            else
            {
                if (_scope.IsRegisteredWithKey(key, service))
                    return _scope.ResolveKeyed(key, service);
            }

            var msgFormat = "Could not locate any instances of contract {0}.";
            var msg = string.Format(msgFormat, key ?? service.Name);
            throw new Exception(msg);
        }

        protected override void BuildUp(object instance)
        {
            _scope.InjectProperties(instance);
        }

        public object GetService(Type sericeType)
        {
            return _scope.Resolve(sericeType);
        }

        public ILifetimeScope BeginChildScope(Action<ContainerBuilder> configurationAction)
        {
            _scope = _scope.BeginLifetimeScope(configurationAction);
            _parentScopeStack.Push(_scope);
            return _scope;
        }

        public void EndChildScope(ILifetimeScope lifetimeScope)
        {
            if (!_parentScopeStack.Contains(lifetimeScope))
                return;

            while (true)
            {
                var scope = _parentScopeStack.Pop();
                scope.Dispose();
                if (scope == lifetimeScope)
                {
                    _scope = _parentScopeStack.Peek();
                    break;
                }
            }

            GC.Collect();
        }
    }

    public interface ISupportChildScope
    {
        ILifetimeScope BeginChildScope(Action<ContainerBuilder> configurationAction);

        void EndChildScope(ILifetimeScope lifetimeScope);
    }

    public class DefaultViewModelFactory : IViewModelFactory
    {
        private readonly IServiceProvider _serviceLocator;

        public DefaultViewModelFactory(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _serviceLocator = serviceProvider;
        }

        public T Create<T>() where T : class
        {
            return (T)_serviceLocator.GetService(typeof(T));
        }

        public object Create(Type type)
        {
            return _serviceLocator.GetService(type);
        }
    }


    public interface IViewModelFactory
    {
        T Create<T>() where T : class;
        Object Create(Type tye);

    }
}
