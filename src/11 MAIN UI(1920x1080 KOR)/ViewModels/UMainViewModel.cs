
using Autofac;
using Autofac.Features.Indexed;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace EGGPLANT
{
    public partial class UMainViewModel : ObservableObject
    {
        // 문자열 바인딩용 프로퍼티
        [ObservableProperty] private string? threadText;
        [ObservableProperty] private string? timeText;
        [ObservableProperty] private string? statusText;
        [ObservableProperty] private string? buildText;
        [ObservableProperty] private string? developerText;
        [ObservableProperty] private string? serialNumberText;

        // 메뉴 체크는 nullable 말고 bool 권장 (tri-state 방지)
        [ObservableProperty] private bool isMainChecked;
        [ObservableProperty] private bool isParamChecked;
        [ObservableProperty] private bool isUserChecked;
        [ObservableProperty] private bool isLogChecked;
        [ObservableProperty] private bool isErrorChecked;
        [ObservableProperty] private bool isManualChecked;
        [ObservableProperty] private bool isMotionChecked;
        [ObservableProperty] private bool isDirectIOChecked;
        [ObservableProperty] private bool isTowerLampChecked;
        [ObservableProperty] private Page? currentPage;

        [ObservableProperty] private ObservableCollection<string> logMessages = new();
        private readonly ILifetimeScope _scope; // Autofac 스코프

        [ObservableProperty]
        private NavigationViewModel navVM;

        [ObservableProperty]
        private UserViewModel userVM;
        public CSYS CSYS { get; }
        public CTrace Trace { get; }

        // 🔹 App.Container 사용 금지. DI로 받기.
        public UMainViewModel(
            NavigationViewModel navVM,
            UserViewModel userVM,
            ILifetimeScope scope,
            CSYS csys,
            IIndex<string, CTrace> traces) // keyed resolve는 IIndex로
        {
            NavVM = navVM;
            UserVM = userVM;
            _scope = scope;
            CSYS = csys;
            Trace = traces["Trace"]; // "Trace" 키로 주입

            ThreadText = "1 ms";
            TimeText = DateTime.Now.ToString("tt hh:mm:ss");
            StatusText = "설비 정지 상태 입니다.(설비 초기화 필요!)";
            BuildText = CSYS.GetBuildVersion();
            DeveloperText = "Developed by EGGPLANT";
            SerialNumberText = "SN : EGGPLANT ...";
            GoToSub01();
        }

        // View(UMain) Loaded 시점에 호출되게 연결
        public void OnLoaded()
        {
            CSYS.Initialize(AppDomain.CurrentDomain.BaseDirectory);
            SetMenu(Menu.Main);
            Trace.Trace("UMainViewModel", "UMainViewModel initialized.");
        }

        // 깔끔한 메뉴 토글 유틸
        private enum Menu { Main, Param, User, Log, Error, Manual, Motion, Sensor, TowerLamp }

        private void SetMenu(Menu m)
        {
            IsMainChecked = m == Menu.Main;
            IsParamChecked = m == Menu.Param;
            IsUserChecked = m == Menu.User;
            IsLogChecked = m == Menu.Log;
            IsErrorChecked = m == Menu.Error;
            IsManualChecked = m == Menu.Manual;
            IsMotionChecked = m == Menu.Motion;
            IsDirectIOChecked = m == Menu.Sensor;
            IsTowerLampChecked = m == Menu.TowerLamp;
        }

        [RelayCommand]
        private void NavigateClick(string tag)
        {
            if (!Enum.TryParse<Menu>(tag, true, out var m)) return;

            switch (m)
            {
                case Menu.Main: GoToSub01(); break;
                case Menu.Param:GoToSub02(); break;
                case Menu.User: GoToSub03(); break;
                case Menu.Log: GoToSub04(); break;
                case Menu.Error: GoToSub05(); break;
                case Menu.Manual: GoToSub06(); break;
                case Menu.Motion: GoToSub07(); break;
                case Menu.Sensor: GoToSub08(); break;
                case Menu.TowerLamp: GoToSub09(); break;
            }
            SetMenu(m);
        }

        private void GoToSub01() => CurrentPage = _scope.Resolve<USub01>();
        private void GoToSub02() => CurrentPage = _scope.Resolve<USub02>();
        private void GoToSub03() => CurrentPage = _scope.Resolve<USub03>();
        private void GoToSub04() => CurrentPage = _scope.Resolve<USub04>();
        private void GoToSub05() => CurrentPage = _scope.Resolve<USub05>();
        private void GoToSub06() => CurrentPage = _scope.Resolve<USub06>();
        private void GoToSub07() => CurrentPage = _scope.Resolve<USub07>();
        private void GoToSub08() => CurrentPage = _scope.Resolve<USub08>();
        private void GoToSub09() => CurrentPage = _scope.Resolve<USub09>();
    }
}
