using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace EGGPLANT
{
    public class RoleScreensGroupVM
    {
        public int RoleId { get; }
        public string RoleName { get; }
        public ObservableCollection<ScreenItemVM> Screens { get; } = new();
        public RoleScreensGroupVM(int managerRoleId, ScreenService service, RoleScreensGroupDto dto)
        {
            RoleId = dto.RoleId;
            RoleName = dto.RoleName;
            foreach (var s in dto.Screens)
            {
                Screens.Add(new ScreenItemVM(service, managerRoleId,
                    targetRoleId: dto.RoleId,
                    screenId: s.ScreenId, code: s.Code, name: s.Name,
                    granted: s.Granted, isEnabled: s.IsEnabled, canEdit: s.CanEdit));
            }
        }
    }

    public partial class ScreenItemVM : ObservableObject
    {
        private readonly ScreenService _service;
        private readonly int _managerRoleId;

        public int TargetRoleId { get; }
        public int ScreenId { get; }
        public string Code { get; }
        public string Name { get; }

        [ObservableProperty] private bool granted;
        public bool IsEnabled { get; }
        public bool CanEdit { get; }
        public bool CanToggle => IsEnabled && CanEdit && !Busy;

        [ObservableProperty] private bool busy;

        public ScreenItemVM(ScreenService service, int managerRoleId,
                            int targetRoleId, int screenId, string code, string name,
                            bool granted, bool isEnabled, bool canEdit)
        {
            _service = service;
            _managerRoleId = managerRoleId;

            TargetRoleId = targetRoleId;
            ScreenId = screenId;
            Code = code;
            Name = name;
            Granted = granted;
            IsEnabled = isEnabled;
            CanEdit = canEdit;
        }

        // ToggleButton에서 현재 체크값을 파라미터로 받음 (true/false)
        [RelayCommand]
        public async Task ToggleAsync()
        {
            if (!CanToggle) return;

            var desired = !Granted;

            Busy = true;
            try
            {
                var ok = await _service.SetGrantAsync(_managerRoleId, TargetRoleId, ScreenId, desired);
                if (ok) Granted = desired;
            }catch(Exception e )
            {
                MessageBox.Show(e.Message);
            }
            
            Busy = false;

            OnPropertyChanged(nameof(CanToggle));
        }
    }

}
