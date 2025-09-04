using CommunityToolkit.Mvvm.ComponentModel;

namespace EGGPLANT{
    public partial class PermissionVM : ObservableObject
    {
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private string permissionName;

        [ObservableProperty]
        private bool isEnabled;

        [ObservableProperty]
        public int categoryId;

        public PermissionVM()
        {
        }

        private PermissionVM(ManagedPermissionDto dto)
        {
            this.Id = dto.PermissionId;
            this.PermissionName = dto.PermissionName;
            this.IsEnabled = dto.IsEnabled;
            this.CategoryId = dto.CategoryId;
        }

        public static PermissionVM of(ManagedPermissionDto dto)
        {
            return new PermissionVM(dto);
        }
    }
}
