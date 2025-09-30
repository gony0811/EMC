using CommunityToolkit.Mvvm.ComponentModel;
using Dapper;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
namespace EGGPLANT
{

    public partial class UserViewModel : ObservableObject
    {
        private readonly UserService _userService;
        private readonly ScreenService _screenService;
        public ObservableCollection<Authority> AuthorityList { get; } = new();


        public ObservableCollection<RoleScreensGroupVM> Groups { get; } = new();

        [ObservableProperty]
        private Authority? currentAuthority;

        public NavigationViewModel NavVM { get; }

        public UserViewModel(
            UserService userService,
            ScreenService screenService,
            NavigationViewModel navigationViewModel)
        {
            _userService = userService;
            _screenService = screenService;
            NavVM = navigationViewModel;
        }

        public async Task InitializeAsync()
        {
            var roles = await _userService.GetRoles();      // 사용 가능한 권한 리스트 불러오기

            AuthorityList.Clear();
            foreach (var r in roles)
            {
                if (r.Name.Equals("OPERATOR"))  // 초기 권한 부여 
                {
                    CurrentAuthority = Authority.of(r);
                    await ChangeAuthority(CurrentAuthority, "");
                }
                AuthorityList.Add(Authority.of(r));
            }
        }

        public async Task<bool> ChangeAuthority(Authority authority, string password, CancellationToken ct = default)
        {
            // 1) 인증 + 역할/권한 로드 (GetRoleAsync는 RolePermissions+Permission+Category를 eager-load)
            var role = await _userService.GetRoleAsync(authority.Name, password, ct);
            if (role is null) return false;
            var allowedScreenCodes = role.ScreenAccesses
                .Where(sa => sa.Screen != null && sa.Screen.IsEnabled)  
                .Select(sa => sa.Screen!.Code)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            CurrentAuthority = Authority.of(role);
            NavVM.ApplyScreens(allowedScreenCodes);
            await ManagedScreen(CurrentAuthority.Id);
            return true;
        }

        // 관리하는 스크린 화면들
        public async Task ManagedScreen(int managerRoleId, CancellationToken ct = default)
        {
            Groups.Clear();
            try
            {
                var data = await _userService.GetManagedRolesScreensAsync(managerRoleId, onlyEnabled: true, ct);
                foreach (var g in data) Groups.Add(new RoleScreensGroupVM(managerRoleId, _screenService, g));
            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
            
        }
    }


}
