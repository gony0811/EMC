using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace EGGPLANT
{
    public partial class UMainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string authority;

        [ObservableProperty]
        private string? threadText;

        [ObservableProperty]
        private string? timeText;

        [ObservableProperty]
        private string? userText;

        [ObservableProperty]
        private string? statusText;

        [ObservableProperty]
        private string? buildText;

        [ObservableProperty]
        private string? developerText;

        [ObservableProperty]
        private string? serialNumberText;

        [ObservableProperty]
        private bool? isMainChecked;
        [ObservableProperty]
        private bool? isParamChecked;
        [ObservableProperty]
        private bool? isUserChecked;
        [ObservableProperty]
        private bool? isLogChecked;
        [ObservableProperty]
        private bool? isErrorChecked;
        [ObservableProperty]
        private bool? isManualChecked;
        [ObservableProperty]
        private bool? isMotionChecked;
        [ObservableProperty]
        private bool? isDirectIOChecked;
        [ObservableProperty]
        private bool? isTowerLampChecked;

        [ObservableProperty]
        private bool useMain;
        [ObservableProperty]
        private bool useParam;
        [ObservableProperty]
        private bool useUser;
        [ObservableProperty]
        private bool useLog;
        [ObservableProperty]
        private bool useError;
        [ObservableProperty]
        private bool? useManual;
        [ObservableProperty]
        private bool? useMotion;
        [ObservableProperty]
        private bool useDirectIO;
        [ObservableProperty]
        private bool useTowerLamp;

        [ObservableProperty]
        private ObservableCollection<string> logMessages = new ObservableCollection<string>();

        private readonly IAuthzService _authService;
        private readonly IMessenger _messenger;

        // public CSYS CSYS { get; } = App.Container?.Resolve<CSYS>();

        // 수정 코드
        public CSYS CSYS { get; } = App.Container?.Resolve<CSYS>() ?? throw new InvalidOperationException("CSYS 인스턴스를 생성할 수 없습니다. App.Container가 null이거나 등록되지 않았습니다.");

        public CTrace Trace { get; } = App.Container?.ResolveKeyed<CTrace>("Trace") ?? throw new InvalidOperationException("Trace 인스턴스를 생성할 수 없습니다. App.Container가 null이거나 등록되지 않았습니다.");

        public UMainViewModel(IAuthzService auth, IMessenger messenger)
        {
            _authService = auth;
            _messenger = messenger;

            _messenger.Register<string>(this, (r, msg) => SetView(msg));

            // 처음 시작은 Operator 권한으로 시작
            Authority = "OPERATOR";
            SetView(Authority);
            // Initialize properties with default values
            ThreadText = "1ms";
            TimeText = "오전 00:00:00";
            UserText = Authority;
            StatusText = "설비 정지 상태 입니다.(설비 초기화 필요!)";
            BuildText = "Build 0000.00.00";
            DeveloperText = "Developed by EGGPLANT";
            SerialNumberText = "SN : EGGPLANT Copyright(c) EGGPLANT Corp. All rights reserved.";
            
            CSYS.Initialize(AppDomain.CurrentDomain.BaseDirectory);
            Init();
        }

        private void Init()
        {
            // Initialize any additional properties or perform setup tasks here
            IsMainChecked = true;
            IsParamChecked = false;
            IsUserChecked = false;
            IsLogChecked = false;
            IsErrorChecked = false;
            IsManualChecked = false;
            IsMotionChecked = false;
            IsDirectIOChecked = false;
            IsTowerLampChecked = false;
            // Navigate to the initial page
            CSYS.GoToSub01();
            Trace.Trace("UMainViewModel", "UMainViewModel initialized successfully.");
        }

        public void SetView(string authority)
        {
            if (authority.Equals("ADMIN") || authority.Equals("SERVICE_ENGINEER"))
            {
                UseMain = true;
                UseParam = true;
                UseUser = true;
                UseLog = true;
                UseError = true;
                UseManual = true;
                UseMotion = true;
                UseDirectIO = true;
                UseTowerLamp = true;
            }
            else if (authority.Equals("OPERATOR"))
            {
                var categoryName = "권한 설정(작업자)";
                ChangedUseView(categoryName);
            }
            else if (authority.Equals("ENGINEER"))
            {
                var categoryName = "권한 설정(엔지니어)";
                ChangedUseView(categoryName);
            }
            else
            {
                UseMain = false;
                UseParam = false;
                UseUser = false;
                UseLog = false;
                UseError = false;
                UseManual = false;
                UseMotion = false;
                UseDirectIO = false;
                UseTowerLamp = false;
            }

            Authority = authority;
        }

        private async void ChangedUseView(string categoryName)
        {
            IReadOnlyList<AuthDto> result = await _authService.GetAuthsAsync(categoryName);
            foreach (AuthDto dto in result)
            {
                if (dto.Name == "경광등 설정화면")
                {
                    UseTowerLamp = dto.IsEnabled;
                }
                else if (dto.Name == "로그 화면")
                {
                    UseLog = dto.IsEnabled;
                }
                else if (dto.Name == "모터화면")
                {
                    UseMotion = dto.IsEnabled;
                }
                else if (dto.Name == "센서화면")
                {
                    UseDirectIO = dto.IsEnabled;
                }
                else if (dto.Name == "수동조작 화면")
                {
                    UseManual = dto.IsEnabled;
                }
                else if (dto.Name == "에러 목록 화면")
                {
                    UseError = dto.IsEnabled;
                }else if (dto.Name == "파라미터 설정화면")
                {
                    UseParam = dto.IsEnabled;
                }
            }
        }

        [RelayCommand]
        private void NavigateClick(string tag)
        {
            switch(tag)
            {
                case "MAIN":
                    CSYS.GoToSub01();
                    IsMainChecked = true;
                    IsParamChecked = false;
                    IsUserChecked = false;
                    IsLogChecked = false;
                    IsErrorChecked = false;
                    IsManualChecked = false;
                    IsMotionChecked = false;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = false;
                    break;
                case "PARAM":
                    CSYS.GoToSub02();
                    IsMainChecked = false;
                    IsParamChecked = true;
                    IsUserChecked = false;
                    IsLogChecked = false;
                    IsErrorChecked = false;
                    IsManualChecked = false;
                    IsMotionChecked = false;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = false;
                    break;
                case "USER":
                    CSYS.GoToSub03();
                    IsMainChecked = false;
                    IsParamChecked = false;
                    IsUserChecked = true;
                    IsLogChecked = false;
                    IsErrorChecked = false;
                    IsManualChecked = false;
                    IsMotionChecked = false;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = false;
                    break;
                case "LOG":
                    CSYS.GoToSub04();
                    IsMainChecked = false;
                    IsParamChecked = false;
                    IsUserChecked = false;
                    IsLogChecked = true;
                    IsErrorChecked = false;
                    IsManualChecked = false;
                    IsMotionChecked = false;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = false;
                    break;
                case "ERROR":
                    CSYS.GoToSub05();
                    IsMainChecked = false;
                    IsParamChecked = false;
                    IsUserChecked = false;
                    IsLogChecked = false;
                    IsErrorChecked = true;
                    IsManualChecked = false;
                    IsMotionChecked = false;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = false;
                    break;
                case "MANUAL":
                    CSYS.GoToSub06();
                    IsMainChecked = false;
                    IsParamChecked = false;
                    IsUserChecked = false;
                    IsLogChecked = false;
                    IsErrorChecked = false;
                    IsManualChecked = true;
                    IsMotionChecked = false;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = false;
                    break;
                case "MOTION":
                    CSYS.GoToSub07();
                    IsMainChecked = false;
                    IsParamChecked = false;
                    IsUserChecked = false;
                    IsLogChecked = false;
                    IsErrorChecked = false;
                    IsManualChecked = false;
                    IsMotionChecked = true;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = false;
                    break;
                case "SENSOR":
                    CSYS.GoToSub08();
                    IsMainChecked = false;
                    IsParamChecked = false;
                    IsUserChecked = false;
                    IsLogChecked = false;
                    IsErrorChecked = false;
                    IsManualChecked = false;
                    IsMotionChecked = false;
                    IsDirectIOChecked = true;
                    IsTowerLampChecked = false;
                    break;
                case "TOWERLAMP":
                    CSYS.GoToSub09();
                    IsMainChecked = false;
                    IsParamChecked = false;
                    IsUserChecked = false;
                    IsLogChecked = false;
                    IsErrorChecked = false;
                    IsManualChecked = false;
                    IsMotionChecked = false;
                    IsDirectIOChecked = false;
                    IsTowerLampChecked = true;
                    break;
                default:
                    throw new ArgumentException("Invalid tag for navigation");
            }
            // Navigate to the main page or perform any action needed
        }


        public void Receive(string m) => SetView(m);


    }
}
