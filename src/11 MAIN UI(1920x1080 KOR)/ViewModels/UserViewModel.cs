namespace EGGPLANT
{
    // UserViewModel
    using CommunityToolkit.Mvvm.ComponentModel;
    using System.Collections.ObjectModel;

    public partial class UserViewModel : ObservableObject
    {
        private readonly UserService _userService;

        // 👇 바인딩 가능한 공개 속성 + ObservableCollection
        public ObservableCollection<Authority> AuthorityList { get; } = new();

        [ObservableProperty]
        private Authority? currentAuthority;

        [ObservableProperty]
        private bool[] menuVisibility = { true, false, true, false, false, false, false, false, false };

        public UserViewModel(UserService userService) => _userService = userService;

        public async Task InitializeAsync()
        {
            var roles = await _userService.GetRoles();
            // UI 스레드에서 추가 (필요시 Dispatcher 사용)
            AuthorityList.Clear();
            foreach (var r in roles)
            {
                if (r.Name.Equals("OPERATOR"))  // 급한대로 일단.. 임시사용
                {
                    CurrentAuthority = Authority.of(r);
                }
                AuthorityList.Add(Authority.of(r));
            }
                
        }

        public async Task<bool> ChangeAuthority(Authority authority, string password)
        {
            var role = await _userService.GetRole(authority.Name, password);
            if (role is null) return false;
            CurrentAuthority = Authority.of(role);
            return true;
        }
    }


}
