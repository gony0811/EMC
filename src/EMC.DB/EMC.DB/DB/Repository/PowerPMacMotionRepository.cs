using EPFramework.DB;
using EPFramework.IoC;

namespace EMC.DB
{
    [Service(Lifetime.Singleton)]
    public class PowerPMacMotionRepository : DbRepository<PowerPMacMotion, AppDb>
    {
        public PowerPMacMotionRepository(AppDb context) : base(context)
        {
        }

    }
}
