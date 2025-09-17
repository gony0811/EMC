using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using Application = System.Windows.Application;

namespace EGGPLANT.ViewModels
{
    public partial class USub03ViewModel : ObservableObject
    {
        public UserViewModel UserVM { get; }
        public USub03ViewModel(UserViewModel userViewModel)
        {
            UserVM = userViewModel;
            _ = UserVM.InitializeAsync();
        }

        [RelayCommand]
        public async Task ChangeUser(Authority authority)
        {
            var currentUser = UserVM.CurrentAuthority;
            var owner = GetOwnerWindow();
            if (currentUser.Id != authority.Id)
            {
                var dlg1 = new UPasswordNPad(owner, $"{authority.Name} 비밀번호 입력");
                if (dlg1.ShowDialog() != true) return; // 취소
                var pw = dlg1.Password;

                var result = await UserVM.ChangeAuthority(authority, pw);
                if(result)
                {
                    AlertModal.Ask(owner, "변경", "사용자가 변경되었습니다");
                }else
                {
                    AlertModal.Ask(owner, "인증 실패", "비밀번호가 틀렸습니다");
                }
            }
        }
        
        private static Window? GetOwnerWindow() =>
            Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
            ?? Application.Current?.MainWindow;
    }
}