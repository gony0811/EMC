

using Microsoft.EntityFrameworkCore;
using System.Data.Entity;

namespace EGGPLANT
{
    public class ScreenService
    {
        private readonly AppDb _db;
        public ScreenService(AppDb db) => _db = db;

        public async Task<bool> SetGrantAsync(int managerRoleId, int targetRoleId, int screenId, bool grant, CancellationToken ct = default)
        {
            var canManage = await EntityFrameworkQueryableExtensions.AnyAsync(
                _db.RoleManageRole.Where(x => x.ManagerRoleId == managerRoleId && x.TargetRoleId == targetRoleId && x.CanManage),
                ct);
            if (!canManage) return false;

            // 비상승: 관리자도 해당 화면 보유 + 화면 활성
            var managerHas = await EntityFrameworkQueryableExtensions.AnyAsync(
                _db.RoleScreenAccess.Where(x => x.RoleId == managerRoleId && x.ScreenId == screenId && x.Granted && x.Screen!.IsEnabled),
                ct);
            if (!managerHas) return false;

            var sa = await EntityFrameworkQueryableExtensions.SingleOrDefaultAsync(
                _db.RoleScreenAccess.Where(x => x.RoleId == targetRoleId && x.ScreenId == screenId),
                ct);

            if (sa == null)
                _db.RoleScreenAccess.Add(new RoleScreenAccess { RoleId = targetRoleId, ScreenId = screenId, Granted = grant });
            else
                sa.Granted = grant;

            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
