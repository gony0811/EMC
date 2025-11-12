using EPFramework.DB;
using EPFramework.IoC;

namespace EMC.DB
{
    [Service(Lifetime.Singleton)]
    public class MotionPositionRepository : DbRepository<MotionPosition, AppDb>
    {
        public MotionPositionRepository(AppDb context) : base(context)
        {
        }
    }
}
