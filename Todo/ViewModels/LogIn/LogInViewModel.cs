using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Accessibility;
using Caliburn.Micro;
using WPF.Common;

namespace Todo.ViewModels.LogIn
{
    public class LogInViewModel:Screen
    {
        private readonly IViewModelFactory _viewModelFactory;
        public LogInViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory =viewModelFactory;

            LogInCarouselView = _viewModelFactory.Create<LogInCarouselControlViewModel>();
            ShowLogInView();
        }

        public LogInCarouselControlViewModel LogInCarouselView { get; set; }

        public bool IsLogInView { get; set; }

        public bool IsCreateAccountView { get; set; }

        public bool IsAccountLogInView { get; set; }

        public bool IsPreviousShow => IsCreateAccountView || IsAccountLogInView;

        public async Task CloseView()
        {
            await TryCloseAsync();
        }

        public void ForgetPassword()
        {
            MessageBox.Show("未实现");
        }

        public void CreateAccount()
        {
            ShowAccountCreateView();
        }

        public void AccoutLogIn()
        {
            ShowAccountLogInView();
        }

        public void BackToPrevieousView()
        {
            ShowLogInView();
        }

        public void LogIn()
        {
        }

        private void ShowLogInView()
        {
            IsLogInView = true;
            IsCreateAccountView = false;
            IsAccountLogInView=false;
            NotifyChangeOfVisibility();
        }

        private void ShowAccountLogInView()
        {
            IsLogInView = false;
            IsAccountLogInView = true;
            IsCreateAccountView = false;
            NotifyOfPropertyChange(nameof(IsLogInView));
            NotifyOfPropertyChange(nameof(IsAccountLogInView));
            NotifyOfPropertyChange(nameof(IsCreateAccountView));
            NotifyChangeOfVisibility();

        }

        private void ShowAccountCreateView()
        {
            IsLogInView = false;
            IsAccountLogInView =false;
            IsCreateAccountView = true;
            NotifyChangeOfVisibility();
        }

        private void NotifyChangeOfVisibility()
        {
            NotifyOfPropertyChange(nameof(IsPreviousShow));
            NotifyOfPropertyChange(nameof(IsLogInView));
            NotifyOfPropertyChange(nameof(IsAccountLogInView));
            NotifyOfPropertyChange(nameof(IsCreateAccountView));
        }
    }
}
