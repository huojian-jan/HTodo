using System.Windows;

namespace Todo
{
    internal  class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var app = new Application();
            app.Resources.Add("bootstrapper",new Bootstrapper());
            app.Run();
        }
    }
}
