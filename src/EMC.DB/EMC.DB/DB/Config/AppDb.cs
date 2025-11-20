using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace EMC.DB
{
    public class AppDb : DbContext
    {
        public AppDb(DbContextOptions<AppDb> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);


            b.ApplyConfigurationsFromAssembly(typeof(AppDb).Assembly);
        }
    }
}
