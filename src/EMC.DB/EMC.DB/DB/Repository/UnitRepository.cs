

using EPFramework.IoC;
using EPFramework.DB;

namespace EMC.DB
{
    [Service(Lifetime.Scoped)]
    public class UnitRepository : DbRepository<Unit, AppDb>
    {
        public UnitRepository(AppDb db) : base(db)
        {
        }
    }
}
