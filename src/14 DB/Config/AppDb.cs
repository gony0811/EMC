using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace EGGPLANT
{
    public class AppDb : DbContext
    {
        public AppDb(DbContextOptions<AppDb> options) : base(options) { }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Recipe> Recipes => Set<Recipe>();
        public DbSet<ValueTypeDef> ValueTypes => Set<ValueTypeDef>();
        public DbSet<Unit> Units => Set<Unit>();
        public DbSet<RecipeParam> RecipeParams => Set<RecipeParam>();
        public DbSet<RolePermissionRow> RolePermissionRows => Set<RolePermissionRow>();
        public DbSet<Screen> Screens => Set<Screen>();
        public DbSet<RoleScreenAccess> RoleScreenAccess => Set<RoleScreenAccess>();
        public DbSet<RoleManageRole> RoleManageRole => Set<RoleManageRole>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            // 현재 어셈블리의 모든 IEntityTypeConfiguration<T> 자동 적용
            b.ApplyConfigurationsFromAssembly(typeof(AppDb).Assembly);
        }

    }

    public sealed class DesignTimeAppDbFactory : IDesignTimeDbContextFactory<AppDb>
    {
        public AppDb CreateDbContext(string[] args)
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MyApp", "emc.db");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            var connStr = $"Data Source={dbPath};Cache=Shared;Pooling=True";
            var opts = new DbContextOptionsBuilder<AppDb>()
                .UseSqlite(connStr)
                .Options;

            return new AppDb(opts);
        }
    }

}
