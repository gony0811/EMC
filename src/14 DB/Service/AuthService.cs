namespace EGGPLANT
{

    // 권한 조회 서비스
    public interface IAuthzService
    {
        Task<IReadOnlyList<ManagedPermissionDto>> GetManageablePermissionsAsync(string roleId, string plain);
        Task<IReadOnlyList<AuthDto>> GetAuthsAsync(string categoryName);
        Task<bool> UpdatePermissionAsync(int permissionId, bool on);
    }

    public sealed class AuthzService : IAuthzService
    {
        private readonly ISqliteConnectionFactory _factory;
        public AuthzService(ISqliteConnectionFactory factory) => _factory = factory;


        // 비밀번호 조회 후 일치시 관리 할 수 있는 권한 목록 반환
        public async Task<IReadOnlyList<ManagedPermissionDto>> GetManageablePermissionsAsync(string roleId, string plain)
        {
            using var conn = _factory.CreateOpen();
            using var tx = conn.BeginTransaction();
            // 1) 비밀번호 검증
            var hashCmd = conn.CreateCommand();
            hashCmd.CommandText = "SELECT Password FROM Roles WHERE RoleId=@r AND IsActive=1 LIMIT 1;";
            hashCmd.Parameters.AddWithValue("@r", roleId);
            var hash = (string?)await hashCmd.ExecuteScalarAsync() ?? "";

            if (!hash.Equals(plain))
                throw new UnauthorizedAccessException("RoleId 또는 비밀번호가 올바르지 않습니다.");

            // 암호화 로직
            //if (string.IsNullOrEmpty(hash) || !BCrypt.Net.BCrypt.Verify(plain, hash))
            //    throw new UnauthorizedAccessException("RoleId 또는 비밀번호가 올바르지 않습니다.");

            // 2) 권한 목록 조회
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                                SELECT
                                  pc.CategoryId,
                                  pc.Name           AS CategoryName,
                                  p.PermissionId,
                                  p.Name            AS PermissionName,
                                  IFNULL(p.Description,'') AS Description,
                                  p.IsEnabled
                                FROM RoleCategoryManage rcm
                                JOIN PermissionCategory pc ON pc.CategoryId = rcm.CategoryId
                                JOIN Permission         p  ON p.CategoryId  = pc.CategoryId
                                WHERE rcm.RoleId = @r AND rcm.CanManage = 1
                                ORDER BY pc.DisplayOrder, pc.Name, p.Name;";
            cmd.Parameters.AddWithValue("@r", roleId);

            var list = new List<ManagedPermissionDto>();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new ManagedPermissionDto
                {
                    CategoryId = rd.GetInt32(0),
                    CategoryName = rd.GetString(1),
                    PermissionId = rd.GetInt32(2),
                    PermissionName = rd.GetString(3),
                    Description = rd.GetString(4),
                    IsEnabled = rd.GetInt32(5) == 1
                });
            }
            tx.Commit();
            return list;
        }

        // 비밀번호 조회 후 권한 목록 반환 
        public async Task<IReadOnlyList<AuthDto>> GetAuthsAsync(string categoryName)
        {
            using var conn = _factory.CreateOpen();
            using var tx = conn.BeginTransaction();
            // 1) 권한 목록 조회
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                                SELECt p.PermissionId, p.Name, p.ISEnabled
                                FROM Permission p
                                JOIN PermissionCategory pc ON p.CategoryId = pc.CategoryId
                                WHERE pc.name = @r;
                            ";

            cmd.Parameters.AddWithValue("@r", categoryName);

            var list = new List<AuthDto>();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new AuthDto
                {
                    PermissionId = rd.GetInt32(0),
                    Name = rd.GetString(1),
                    IsEnabled = rd.GetInt32(2) == 1
                });
            }
            tx.Commit();
            return list;
        }


        // 
        public async Task<bool> UpdatePermissionAsync(int permissionId, bool on)
        {
            using var conn = _factory.CreateOpen();
            using var tx = conn.BeginTransaction();

            bool result = false;
            // 1) 권한 목록 조회
            var cmd = conn.CreateCommand();
            cmd.CommandText =
                @"
                    UPDATE Permission 
                    SET IsEnabled = @on
                    WHERE PermissionId = @id
                ";
            cmd.Parameters.AddWithValue("@on", on ? 1 : 0);
            cmd.Parameters.AddWithValue("@id", permissionId);
            var rows = await cmd.ExecuteNonQueryAsync();

            result = rows > 0;
            tx.Commit();
            return result;
        }
    }
}
