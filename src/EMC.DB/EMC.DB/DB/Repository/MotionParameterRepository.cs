using EPFramework.DB;
using EPFramework.IoC;

namespace EMC.DB
{
    [Service(Lifetime.Singleton)]
    public class MotionParameterRepository : DbRepository<MotionParameter, AppDb>
    {
        public MotionParameterRepository(AppDb context) : base(context)
        {
        }
    }
}
