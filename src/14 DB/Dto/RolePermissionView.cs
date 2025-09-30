using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;

namespace EGGPLANT
{
    public class RolePermissionRow
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = "";
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = "";
        public int Granted { get; set; }          // 0/1
        public int IsEnabled { get; set; }        // 0/1
    }

    public sealed record RoleCategoryGroup(
        int RoleId, string RoleName,
        int CategoryId, string CategoryName,
        List<RolePermissionRow> Items);

    
}
