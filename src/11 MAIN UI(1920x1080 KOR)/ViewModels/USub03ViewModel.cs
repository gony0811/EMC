using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;

namespace EGGPLANT.ViewModels
{
    enum RoleId
    {
        OPERATOR,
        ENGINEER,
        ADMIN,
        SERVICE_ENGINEER
    }
    public partial class USub03ViewModel : ObservableObject
    {
        private readonly IAuthzService _authService;

        private readonly IMessenger _messenger;

        [ObservableProperty]
        private ObservableCollection<PermissionVM> operatorSettings = new(); // 작업자 설정

        [ObservableProperty]
        private ObservableCollection<PermissionVM> engineerSettings = new(); // 엔지니어 설정

        [ObservableProperty]
        private ObservableCollection<PermissionVM> userSettings = new(); // 사용자 설정

        [ObservableProperty]
        private ObservableCollection<PermissionVM> serviceEngineerOptions = new(); // 서비스 엔지니어 옵션 설정


        public USub03ViewModel(IAuthzService auth, IMessenger messenger)
        {
            _authService = auth;
            _messenger = messenger;
        }

        [RelayCommand]
        public async void EngineerSelect()
        {
            string roleID = RoleId.ENGINEER.ToString();
            bool result = await ConfirmPassword("엔지니어 비밀번호 입력", roleID);
            if(result)
            {
                _messenger.Send("ENGINEER");
            }
        }

        [RelayCommand]
        public async void AdminSelect()
        {
            string roleID = RoleId.ADMIN.ToString();
            bool result = await ConfirmPassword("관리자 비밀번호 입력", roleID);
            if (result)
            {
                _messenger.Send("ADMIN");
            }

        }

        [RelayCommand]
        public async void SESelect()
        {
            string roleID = RoleId.SERVICE_ENGINEER.ToString();
            bool result = await ConfirmPassword("서비스 엔지니어 비밀번호 입력", roleID);
            if (result)
            {
                _messenger.Send("SERVICE_ENGINEER");
            }
        }

        [RelayCommand]
        public async void OperatorSelect()
        {
            string roleID = RoleId.OPERATOR.ToString();
            
            IReadOnlyList <ManagedPermissionDto> dto = new List<ManagedPermissionDto>();
            VMPipeline(dto);
            _messenger.Send("OPERATOR");
            

        }

        private async Task<bool> ConfirmPassword(string roleLabel, string roleID)
        {
            // 현재 윈도우를 Owner로 설정
            var owner = System.Windows.Application.Current?.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.IsActive) ?? System.Windows.Application.Current?.MainWindow;

            var dlg1 = new UPasswordNPad(owner, $"{roleLabel} 비밀번호 입력");
            if (dlg1.ShowDialog() != true) return false; // 취소

            try
            {
                var data = await _authService.GetManageablePermissionsAsync(roleID, dlg1.Password);
                var list = data.ToArray();
                VMPipeline(list);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR");
                return false;
            }
            return true;
        }

        private void VMPipeline(IReadOnlyList<ManagedPermissionDto> list)
        {
            OperatorSettings.Clear();
            EngineerSettings.Clear();
            UserSettings.Clear();
            ServiceEngineerOptions.Clear();

            foreach (ManagedPermissionDto dto in list)
            {
                if (dto.CategoryName.Equals("권한 설정(작업자)"))
                {
                    OperatorSettings.Add(PermissionVM.of(dto));
                }

                else if (dto.CategoryName.Equals("권한 설정(엔지니어)"))
                {
                    EngineerSettings.Add(PermissionVM.of(dto));
                }

                else if (dto.CategoryName.Equals("사용자 옵션"))
                {
                    UserSettings.Add(PermissionVM.of(dto));
                }

                else if (dto.CategoryName.Equals("Service Engineer Option"))
                {
                    ServiceEngineerOptions.Add(PermissionVM.of(dto));
                }
            }
        }

        [RelayCommand]
        private async Task SetPermission(PermissionVM? item)
        {
            if (item is null) return;

            try
            {
                await _authService.UpdatePermissionAsync(item.Id, !item.IsEnabled);
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "저장 실패");
                return;
            }

            item.IsEnabled = !item.IsEnabled;
        }
    }
}