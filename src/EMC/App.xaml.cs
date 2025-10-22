using Autofac;
using System.Threading;
using System.Windows;

namespace EMC
{
    public partial class App : Application
    {

        static public string Project = "EGGPLANT";
        public static IContainer Container { get; private set; } = null;
        //private ILifetimeScope _initScope;
        //private ILifetimeScope _mainScope;
        private Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries_V2.Init();
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

            // ② DI 컨테이너 초기화
            Container = CStartUp.Build();
            // ③ 메인 윈도우 실행
            var mainWindow = Container.Resolve<UMain>();
            mainWindow.Show();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            try { _mutex?.ReleaseMutex(); } catch { /* ignore */ }
            _mutex?.Dispose();
            Container?.Dispose();
            base.OnExit(e);
        }
    }
}
