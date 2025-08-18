using EGGPLANT._11_MAIN_UI_1920x1080_KOR_;

using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

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
        protected override void OnStartup(StartupEventArgs e)
        {
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            base.OnStartup(e);
            // Initialize the main window
            var Mutex = new System.Threading.Mutex(false, Project);

            if (!Mutex.WaitOne(0, false))
            {
                // If another instance is already running, exit the application
                MessageBox.Show($"이미 해당 프로그램{Project}이 구동 중입니다.", "에러", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }
            var initWindow = new UInitialize();
            bool? result = initWindow.ShowDialog();

            if (result == true)
            {
                UMain mainWindow = new UMain();
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
