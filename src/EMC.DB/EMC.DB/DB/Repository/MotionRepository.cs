using EPFramework.DB;
using EPFramework.IoC;

namespace EMC.DB
{
    [Service(Lifetime.Scoped)]
    public class MotionRepository : DbRepository<Motion, AppDb>
    {
        public MotionRepository(AppDb db) : base(db)
        {
        }
    
    }
}
