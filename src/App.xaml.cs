using Autofac;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace EGGPLANT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    public partial class App : Application
    {

        static public string Project = "EGGPLANT";
        private static IContainer _container = CStartUp.Build();
        static public IContainer Container { get => _container; private set => _container = value; }
        private ILifetimeScope? _initScope;
        private ILifetimeScope? _mainScope;
        private Mutex? _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            base.OnStartup(e);

            // ① 단일 실행 보장: 뮤텍스는 필드로 잡고, 종료 때까지 유지(+Release는 OnExit에서)
            _mutex = new Mutex(true, Project, out bool isNew);
            if (!isNew)
            {
                MessageBox.Show($"이미 {Project} 이(가) 실행 중입니다.", "에러",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            // ② 초기화 창용 스코프
            _initScope = _container.BeginLifetimeScope();

            UInitialize initWindow;
            try
            {
                initWindow = _initScope.Resolve<UInitialize>();
            }
            catch (Autofac.Core.DependencyResolutionException ex)
            {
                MessageBox.Show(ex.ToString(), "DI 오류");
                Shutdown();
                return;
            }

            var result = initWindow.ShowDialog();

            // 초기화 창 닫혔으니 스코프 정리
            _initScope.Dispose();
            _initScope = null;

            if (result == true)
            {
                // ③ 메인 창용 스코프
                _mainScope = _container.BeginLifetimeScope();

                var mainWindow = _mainScope.Resolve<UMain>();

                MainWindow = mainWindow;
                mainWindow.Show();

                ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            else
            {
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _mainScope?.Dispose();
            _container.Dispose();
            _mutex?.ReleaseMutex();   // ← 여기서 해제
            _mutex?.Dispose();
            base.OnExit(e);
        }
    }

    //public partial class App : Application
    //{
    //    // Define a project name constant
    //    // This is used for mutex to ensure only one instance of the application runs
    //    static public string Project = "EGGPLANT";
    //    private static IContainer container = CStartUp.Build();

    //    static public IContainer Container { get => container; private set => container = value; }
    //    protected override void OnStartup(StartupEventArgs e)
    //    {
    //        this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
    //        base.OnStartup(e);
    //        // Initialize the main window
    //        var Mutex = new Mutex(false, Project);

    //        if (!Mutex.WaitOne(0, false))
    //        {
    //            // If another instance is already running, exit the application
    //            MessageBox.Show($"이미 해당 프로그램{Project}이 구동 중입니다.", "에러", MessageBoxButton.OK, MessageBoxImage.Error);
    //            Current.Shutdown();
    //            return;
    //        }
            

    //        var initWindow = Container.Resolve<UInitialize>();
    //        bool? result = initWindow.ShowDialog();

    //        if (result == true)
    //        {
    //            UMain mainWindow = Container.Resolve<UMain>();
    //            mainWindow.DataContext = Container.Resolve<UMainViewModel>();
    //            mainWindow.Show();
    //            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
    //            Mutex.ReleaseMutex();
    //        }
    //        else
    //        {
    //            Shutdown();
    //        }

    //    }
    //}

}
