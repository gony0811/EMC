using Microsoft.EntityFrameworkCore;

namespace EGGPLANT
{
    public class UserService 
    {
        private readonly IDbRepository<Role> _roleRepo;
        private readonly AppDb _db;
        
        public UserService(IDbRepository<Role> roleRepo, AppDb db)
        {
            _roleRepo = roleRepo;
            _db = db;
        }

        public async Task<Role?> GetRoleAsync(string roleName, string password, CancellationToken ct = default)
        {
            var role = await _db.Roles
                .Where(r => r.IsActive && r.Name == roleName && r.Password == password)

                // 화면 접근권한(부여된 것만) + 화면 엔티티
                .Include(r => r.ScreenAccesses.Where(sa => sa.Granted))
                    .ThenInclude(sa => sa.Screen)

                // 내가 관리할 수 있는 대상 역할 목록(선택)
                .Include(r => r.ManageTargets.Where(m => m.CanManage))
                    .ThenInclude(m => m.Target)

                .AsSplitQuery()   // 카테시안 폭발 방지
                .AsNoTracking()   // 읽기 전용
                .SingleOrDefaultAsync(ct);

            return role;
        }

        public async Task<IReadOnlyList<Role?>> GetRoles(CancellationToken ct = default)
        {
            var result = await _roleRepo.ListAsync(ct:ct);
            return result;
        }

        /// <summary>
        /// managerRoleId가 관리중인 역할들의 화면 리스트를 역할별로 그룹화해서 반환
        /// </summary>
        /// 

        public async Task<List<RoleScreensGroupDto>> GetManagedRolesScreensAsync(
            int managerRoleId,
            bool onlyEnabled = true,    // 전역 비활성 스크린 숨김 여부
            CancellationToken ct = default)
        {
            // 0) 관리자(배우)가 보유한 스크린 집합 → 비상승 판단용
            var managerScreenIds = await _db.RoleScreenAccess
                .Where(sa => sa.RoleId == managerRoleId && sa.Granted && sa.Screen!.IsEnabled)
                .Select(sa => sa.ScreenId)
                .Distinct()
                .ToListAsync(ct);

            // 1) 내가 관리할 수 있는 대상 역할 Id 들
            var targetRoleIds = await _db.RoleManageRole
                .Where(m => m.ManagerRoleId == managerRoleId && m.CanManage)
                .Select(m => m.TargetRoleId)
                .Distinct()
                .ToListAsync(ct);

            if (targetRoleIds.Count == 0) return new();

            // 2) LEFT JOIN: 대상 역할 × (필터된) 모든 스크린
            var flat = await (
                from t in _db.Roles
                where targetRoleIds.Contains(t.Id) && t.IsActive

                from s in _db.Screens
                where !onlyEnabled || s.IsEnabled

                join sa0 in _db.RoleScreenAccess on new { RoleId = t.Id, ScreenId = s.Id }
                    equals new { sa0.RoleId, sa0.ScreenId } into grp
                from sa in grp.DefaultIfEmpty() // ← 미부여면 null

                orderby t.Name, s.DisplayOrder, s.Name
                select new RoleScreenFlat(
                    t.Id, t.Name,
                    s.Id, s.Code, s.Name,
                    sa != null && sa.Granted,   // Granted: null → false
                    s.IsEnabled,
                    managerScreenIds.Contains(s.Id) 
                )
            )
            .AsNoTracking()
            .ToListAsync(ct);

            // 3) 역할별 그룹으로 변환
            var groups = flat
                .GroupBy(f => new { f.RoleId, f.RoleName })
                .OrderBy(g => g.Key.RoleName)
                .Select(g => new RoleScreensGroupDto(
                    g.Key.RoleId,
                    g.Key.RoleName,
                    g.Select(f => new ScreenItemDto(f.ScreenId, f.Code, f.Name, f.Granted, f.IsEnabled, f.CanEdit))
                     .ToList()
                ))
                .ToList();

            return groups;
        }
    }
}
