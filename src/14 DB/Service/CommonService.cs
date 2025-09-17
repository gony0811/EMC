namespace EGGPLANT
{
    public interface ICommonService
    {
        Task<IReadOnlyList<UnitDto>> GetUnits();
        Task<IReadOnlyList<ValueTypeDto>> GetValueTypes();

    }

    public class CommonService : BaseService, ICommonService
    {
        public CommonService(ISqliteConnectionFactory factory) : base(factory) { }

        public async Task<IReadOnlyList<UnitDto>> GetUnits()
        {
            using var conn = Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT UnitId, Name, Symbol FROM Unit ORDER BY Name;";

            var list = new List<UnitDto>();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new UnitDto(
                    rd.GetInt32(0),
                    rd.GetString(1),
                    rd.IsDBNull(2) ? "" : rd.GetString(2)
                ));
            }
            return list;
        }
        public async Task<IReadOnlyList<ValueTypeDto>> GetValueTypes()
        {
            using var conn = Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ValueTypeId, Name FROM ValueType ORDER BY Name;";

            var list = new List<ValueTypeDto>();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new ValueTypeDto(
                    rd.GetInt32(0),
                    rd.GetString(1)
                ));
            }
            return list;
        }
    }
}
