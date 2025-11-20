
using EMC.DB;
using EPFramework.DB;
using EPFramework.IoC;

namespace EMC
{
    [Service(Lifetime.Singleton)]
    public class MotionRepository : DbRepository<MotionEntity, AppDb>
    {
        public MotionRepository(AppDb context) : base(context)
        {
        }
    }
}
