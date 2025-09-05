using System.Data.SQLite;
using System.Globalization;

namespace EGGPLANT
{
    public interface IRecipeService
    {
        // Recipe List 조회
        Task<IReadOnlyList<RecipeDto>> GetRecipes();

        // Recipe 생성
        Task CreateRecipe(RecipeDto dto);

        // Recipe 삭제 
        Task DeleteRecipe(int id);

        // Recipe 업데이트
        Task UpdateRecipe(RecipeDto dto);

        // 레시피 파라미터 조회
        Task<IReadOnlyList<RecipeParamDto>> GetParameters(int recipeId);

        // 레시피 파라미터 생성
        Task CreateParamter(RecipeParamDto dto);

        // 레시피 파라미터 삭제
        Task DeleteParamter(int id);

        // 레시피 파라미터 업데이트
        Task UpdateParamter(RecipeParamDto dto);
    }
    public sealed class RecipeService : BaseService, IRecipeService
    {
        
        public RecipeService(ISqliteConnectionFactory factory) : base(factory) { }


        public async Task<IReadOnlyList<RecipeDto>> GetRecipes()
        {
            using var conn = Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                                SELECT RecipeId, Name, IsActive, CreatedAt, IFNULL(UpdatedAt,'') AS UpdatedAt
                                FROM Recipes
                                ORDER BY RecipeId;";
            var list = new List<RecipeDto>();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new RecipeDto
                {
                    RecipeId = Convert.ToInt32(rd["RecipeId"], CultureInfo.InvariantCulture),
                    Name = rd["Name"] as string ?? "",
                    IsActive = Convert.ToInt32(rd["IsActive"], CultureInfo.InvariantCulture) == 1,
                    CreatedAt = rd["CreatedAt"] as string ?? "",
                    UpdatedAt = rd["UpdatedAt"] as string ?? ""
                });
            }
            return list;
        }
        public async Task CreateRecipe(RecipeDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            using var conn = Open();
            using var tx = conn.BeginTransaction();

            // 활성 레시피 단 1개 유지: 새 레시피를 active로 만들면 기존 active는 0으로
            if (dto.IsActive)
            {
                using var deactivate = conn.CreateCommand();
                deactivate.Transaction = tx;
                deactivate.CommandText = "UPDATE Recipes SET IsActive=0 WHERE IsActive=1;";
                await deactivate.ExecuteNonQueryAsync();
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"INSERT INTO Recipes(Name, IsActive) VALUES(@name, @active);";
                cmd.Parameters.AddWithValue("@name", dto.Name ?? "");
                cmd.Parameters.AddWithValue("@active", dto.IsActive ? 1 : 0);
                await cmd.ExecuteNonQueryAsync();
            }

            tx.Commit();
        }

        public async Task UpdateRecipe(RecipeDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            using var conn = Open();
            using var tx = conn.BeginTransaction();

            if (dto.IsActive)
            {
                using var deactivate = conn.CreateCommand();
                deactivate.Transaction = tx;
                deactivate.CommandText = "UPDATE Recipes SET IsActive=0 WHERE IsActive=1 AND RecipeId<>@id;";
                deactivate.Parameters.AddWithValue("@id", dto.RecipeId);
                await deactivate.ExecuteNonQueryAsync();
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"UPDATE Recipes SET Name=@name, IsActive=@active WHERE RecipeId=@id;";
                cmd.Parameters.AddWithValue("@name", dto.Name ?? "");
                cmd.Parameters.AddWithValue("@active", dto.IsActive ? 1 : 0);
                cmd.Parameters.AddWithValue("@id", dto.RecipeId);
                var rows = await cmd.ExecuteNonQueryAsync();
                if (rows == 0) throw new KeyNotFoundException($"Recipe {dto.RecipeId} not found.");
            }

            tx.Commit();
        }

        public async Task DeleteRecipe(int id)
        {
            using var conn = Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Recipes WHERE RecipeId=@id;";
            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        // ------------------------------------------------------------------------------------
        // RecipeParam
        // ------------------------------------------------------------------------------------
        public async Task<IReadOnlyList<RecipeParamDto>> GetParameters(int recipeId)
        {
            using var conn = Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT
                  rp.RecipeId,
                  rp.ParameterId,
                  rp.Name,
                  rp.Value,
                  IFNULL(rp.Maximum,'') AS Maximum,
                  IFNULL(rp.Minimum,'') AS Minimum,
                  vt.Name AS ValueType,
                  IFNULL(u.Name,'') AS Unit
                FROM RecipeParam rp
                JOIN ValueType vt ON vt.ValueTypeId = rp.ValueTypeId
                LEFT JOIN Unit u  ON u.UnitId       = rp.UnitId
                WHERE rp.RecipeId = @rid
                ORDER BY rp.ParameterId;";
            cmd.Parameters.AddWithValue("@rid", recipeId);

            var list = new List<RecipeParamDto>();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new RecipeParamDto
                {
                    RecipeId = Convert.ToInt32(rd["RecipeId"], CultureInfo.InvariantCulture),
                    ParameterId = Convert.ToInt32(rd["ParameterId"], CultureInfo.InvariantCulture),
                    Name = rd["Name"] as string ?? "",
                    Value = rd["Value"] as string ?? "",
                    Maximum = rd["Maximum"] as string ?? "",
                    Minimum = rd["Minimum"] as string ?? "",
                    ValueType = rd["ValueType"] as string ?? "",
                    Unit = rd["Unit"] as string ?? ""
                });
            }
            return list;
        }

        public async Task CreateParamter(RecipeParamDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            using var conn = Open();
            using var tx = conn.BeginTransaction();

            // ValueType/Unit ID 확보
            int valueTypeId = await EnsureValueTypeIdAsync(conn, tx, dto.ValueType);
            int? unitId = await EnsureUnitIdAsync(conn, tx, dto.Unit);

            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"
                    INSERT INTO RecipeParam
                            (RecipeId, ParameterId, Name, Value, Maximum, Minimum, ValueTypeId, UnitId, Description)
                            VALUES
                            (@rid, COALESCE(@pid, (SELECT IFNULL(MAX(ParameterId)+1,1)
                                             FROM RecipeParam)),
                             @name, @val, @max, @min, @vtId, @unitId, NULL);";
                cmd.Parameters.AddWithValue("@pid", (object?)dto.ParameterId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@rid", dto.RecipeId);
                cmd.Parameters.AddWithValue("@name", dto.Name ?? "");
                cmd.Parameters.AddWithValue("@val", dto.Value ?? "");
                cmd.Parameters.AddWithValue("@max", string.IsNullOrWhiteSpace(dto.Maximum) ? (object)DBNull.Value : dto.Maximum);
                cmd.Parameters.AddWithValue("@min", string.IsNullOrWhiteSpace(dto.Minimum) ? (object)DBNull.Value : dto.Minimum);
                cmd.Parameters.AddWithValue("@vtId", valueTypeId);
                if (unitId.HasValue) cmd.Parameters.AddWithValue("@unitId", unitId.Value);
                else cmd.Parameters.AddWithValue("@unitId", DBNull.Value);
                await cmd.ExecuteNonQueryAsync();
            }

            tx.Commit();
        }

        public async Task UpdateParamter(RecipeParamDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));
            using var conn = Open();
            using var tx = conn.BeginTransaction();

            int valueTypeId = await EnsureValueTypeIdAsync(conn, tx, dto.ValueType);
            int? unitId = await EnsureUnitIdAsync(conn, tx, dto.Unit);

            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"
                    UPDATE RecipeParam
                       SET Name=@name,
                           Value=@val,
                           Maximum=@max,
                           Minimum=@min,
                           ValueTypeId=@vtid,
                           UnitId=@uid
                     WHERE RecipeId=@rid AND ParameterId=@pid;";
                cmd.Parameters.AddWithValue("@name", dto.Name ?? "");
                cmd.Parameters.AddWithValue("@val", dto.Value ?? "");
                cmd.Parameters.AddWithValue("@max", (object?)dto.Maximum ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@min", (object?)dto.Minimum ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@vtid", valueTypeId);
                if (unitId.HasValue) cmd.Parameters.AddWithValue("@uid", unitId.Value);
                else cmd.Parameters.AddWithValue("@uid", DBNull.Value);
                cmd.Parameters.AddWithValue("@rid", dto.RecipeId);
                cmd.Parameters.AddWithValue("@pid", dto.ParameterId);

                var rows = await cmd.ExecuteNonQueryAsync();
                if (rows == 0) throw new KeyNotFoundException($"RecipeParam (RecipeId={dto.RecipeId}, ParameterId={dto.ParameterId}) not found.");
            }

            tx.Commit();
        }

        public async Task DeleteParamter(int id)
        {
            using var conn = Open();
            using var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM RecipeParam WHERE ParameterId=@pid;"; // 전체 레시피에서 해당 파라미터 ID 삭제
                cmd.Parameters.AddWithValue("@pid", id);
            await cmd.ExecuteNonQueryAsync();
        }

        // ------------------------------------------------------------------------------------
        // helpers
        // ------------------------------------------------------------------------------------
        private static async Task<int> EnsureValueTypeIdAsync(SQLiteConnection conn, SQLiteTransaction tx, string? valueTypeName)
        {
            var name = string.IsNullOrWhiteSpace(valueTypeName) ? "TEXT" : valueTypeName.Trim();

            using (var sel = conn.CreateCommand())
            {
                sel.Transaction = tx;
                sel.CommandText = "SELECT ValueTypeId FROM ValueType WHERE Name=@n LIMIT 1;";
                sel.Parameters.AddWithValue("@n", name);
                var found = await sel.ExecuteScalarAsync();
                if (found != null && found != DBNull.Value)
                    return Convert.ToInt32(found, CultureInfo.InvariantCulture);
            }

            using (var ins = conn.CreateCommand())
            {
                ins.Transaction = tx;
                ins.CommandText = "INSERT OR IGNORE INTO ValueType(Name) VALUES(@n);";
                ins.Parameters.AddWithValue("@n", name);
                await ins.ExecuteNonQueryAsync();
            }

            using (var sel2 = conn.CreateCommand())
            {
                sel2.Transaction = tx;
                sel2.CommandText = "SELECT ValueTypeId FROM ValueType WHERE Name=@n LIMIT 1;";
                sel2.Parameters.AddWithValue("@n", name);
                var id = await sel2.ExecuteScalarAsync();
                return Convert.ToInt32(id, CultureInfo.InvariantCulture);
            }
        }

        private static async Task<int?> EnsureUnitIdAsync(SQLiteConnection conn, SQLiteTransaction tx, string? unitName)
        {
            if (string.IsNullOrWhiteSpace(unitName))
                return null;

            var name = unitName.Trim();

            using (var sel = conn.CreateCommand())
            {
                sel.Transaction = tx;
                sel.CommandText = "SELECT UnitId FROM Unit WHERE Name=@n LIMIT 1;";
                sel.Parameters.AddWithValue("@n", name);
                var found = await sel.ExecuteScalarAsync();
                if (found != null && found != DBNull.Value)
                    return Convert.ToInt32(found, CultureInfo.InvariantCulture);
            }

            using (var ins = conn.CreateCommand())
            {
                ins.Transaction = tx;
                ins.CommandText = "INSERT OR IGNORE INTO Unit(Name, Symbol) VALUES(@n, NULL);";
                ins.Parameters.AddWithValue("@n", name);
                await ins.ExecuteNonQueryAsync();
            }

            using (var sel2 = conn.CreateCommand())
            {
                sel2.Transaction = tx;
                sel2.CommandText = "SELECT UnitId FROM Unit WHERE Name=@n LIMIT 1;";
                sel2.Parameters.AddWithValue("@n", name);
                var id = await sel2.ExecuteScalarAsync();
                return Convert.ToInt32(id, CultureInfo.InvariantCulture);
            }
        }

        private static async Task<int> NextParameterIdAsync(SQLiteConnection conn, SQLiteTransaction tx, int recipeId)
        {
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = "SELECT IFNULL(MAX(ParameterId)+1, 1) FROM RecipeParam WHERE RecipeId=@rid;";
            cmd.Parameters.AddWithValue("@rid", recipeId);
            var v = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(v, CultureInfo.InvariantCulture);
        }

    }
}
