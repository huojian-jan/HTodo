using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace Todo.ViewModels.LogIn {
    public class LogInCarouselControlViewModel:Screen
    {

        public LogInCarouselControlViewModel()
        {
            Source = @"pack://application:,,,/Todo.Asset;component/Png/loginimage/loginhabit.png";
        }

        public string Source { get; set; }
    }
}
