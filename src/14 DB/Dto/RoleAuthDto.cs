namespace EGGPLANT
{
    public sealed class ManagedPermissionDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsEnabled { get; set; }
    }

    public sealed class AuthDto
    {
        public int PermissionId { get; set; }
        public string Name { get; set; } = "";
        public bool IsEnabled { get; set; } = false;
    }
}
