
using EPFramework.IoC;
using EPFramework.DB;

namespace EMC.DB
{
    [Service(Lifetime.Scoped)]
    public class ValueTypeRepository : DbRepository<ValueTypeDef, AppDb>
    {
        public ValueTypeRepository(AppDb db) : base(db)
        {
        }
    }
}
