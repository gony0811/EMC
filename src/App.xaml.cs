using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using System.IO;
using System.Threading.Tasks;

namespace EGGPLANT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Define a project name constant
        // This is used for mutex to ensure only one instance of the application runs
        static public string Project = "EGGPLANT";
        private static IContainer container = CStartUp.Build();

        static public IContainer Container { get => container; private set => container = value; }
        protected override async void OnStartup(StartupEventArgs e)
        {
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            base.OnStartup(e);

            // Initialize the main window
            var Mutex = new Mutex(false, Project);

            if (!Mutex.WaitOne(0, false))
            {
                // If another instance is already running, exit the application
                MessageBox.Show($"이미 해당 프로그램{Project}이 구동 중입니다.", "에러", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }
            
            var initWindow = Container.Resolve<UInitialize>();
            bool? result = initWindow.ShowDialog();

            //var userVM = Container.Resolve<UserViewModel>();
            //await userVM.InitializeAsync();

            if (result == true)
            {
                var scope = Container.BeginLifetimeScope();
                UMain mainWindow = Container.Resolve<UMain>();
                mainWindow.DataContext = Container.Resolve<UMainViewModel>();
                mainWindow.Closed += (_, __) => scope.Dispose();
                mainWindow.Show();
                this.ShutdownMode = ShutdownMode.OnMainWindowClose;
                Mutex.ReleaseMutex();
            }
            else
            {
                Shutdown();
            }

        }
    }

}
