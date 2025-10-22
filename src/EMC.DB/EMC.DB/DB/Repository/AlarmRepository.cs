using EPFramework.DB;
using EPFramework.IoC;

namespace EMC.DB
{
    [Service(Lifetime.Scoped)]
    public class AlarmRepository : DbRepository<Alarm, AppDb>
    {
        public AlarmRepository(AppDb db) : base(db)
        {
        }
    }
}
