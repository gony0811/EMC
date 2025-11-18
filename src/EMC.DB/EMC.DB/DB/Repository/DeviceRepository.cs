using EPFramework.DB;
using EPFramework.IoC;

namespace EMC.DB
{
    [Service(Lifetime.Singleton)]
    public class DeviceRepository : DbRepository<Device, AppDb>
    {
        public DeviceRepository(AppDb context) : base(context)
        {
        }
    }
}
