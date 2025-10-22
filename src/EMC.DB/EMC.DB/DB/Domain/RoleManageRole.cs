namespace EMC.DB
{
    public class RoleManageRole
    {
        public int ManagerRoleId { get; set; }
        public int TargetRoleId { get; set; }
        public bool CanManage { get; set; } = true;

        public Role Manager { get; set; }
        public Role Target { get; set; }
    }
}
