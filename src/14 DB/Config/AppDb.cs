using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Sqlite.Metadata.Internal;
using System.IO;
using System.Reflection.Emit;

namespace EGGPLANT
{
    public class AppDb : DbContext
    {
        public AppDb(DbContextOptions<AppDb> options) : base(options) { }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<PermissionCategory> PermissionCategory => Set<PermissionCategory>();
        public DbSet<Permission> Permission => Set<Permission>();
        public DbSet<RoleCategoryManage> RoleCategoryManage => Set<RoleCategoryManage>();
        public DbSet<Recipe> Recipes => Set<Recipe>();
        public DbSet<ValueTypeDef> ValueTypes => Set<ValueTypeDef>();
        public DbSet<Unit> Units => Set<Unit>();
        public DbSet<RecipeParam> RecipeParams => Set<RecipeParam>();


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
