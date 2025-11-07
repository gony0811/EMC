using EPFramework.DB;
using EPFramework.IoC;

namespace EMC.DB
{
    [Service(Lifetime.Singleton)]
    public class PowerPMacRepository : DbRepository<PowerPMac, AppDb>
    {
        public PowerPMacRepository(AppDb context) : base(context)
        {
        }
    }
}
