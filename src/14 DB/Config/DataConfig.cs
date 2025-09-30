using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Sqlite.Metadata.Internal;
using System.Reflection.Emit;


namespace EGGPLANT
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> b)
        {
            b.ToTable("Roles");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
             .ValueGeneratedOnAdd()
             .HasAnnotation(SqliteAnnotationNames.Autoincrement, true);

            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();
            b.Property(x => x.Description).HasMaxLength(200);
            b.Property(x => x.Password).HasMaxLength(200).IsRequired();
            b.Property(x => x.IsActive).HasDefaultValue(true);

            b.HasMany(x => x.ScreenAccesses)
             .WithOne(sa => sa.Role)
             .HasForeignKey(sa => sa.RoleId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.ManageTargets)
             .WithOne(m => m.Manager)
             .HasForeignKey(m => m.ManagerRoleId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.ManagedBy)
             .WithOne(m => m.Target)
             .HasForeignKey(m => m.TargetRoleId)
             .OnDelete(DeleteBehavior.Cascade);

            // 필요 시 체크 제약
            b.ToTable(t => t.HasCheckConstraint("CK_Roles_IsActive_01", "IsActive IN (0,1)"));
        }
    }

    public class ScreenConfig : IEntityTypeConfiguration<Screen>
    {
        public void Configure(EntityTypeBuilder<Screen> b)
        {
            b.ToTable("Screens");
            b.HasKey(x => x.Id);
            b.Property(x => x.Code).HasMaxLength(100).IsRequired();
            b.HasIndex(x => x.Code).IsUnique();
            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
            b.Property(x => x.Path).HasMaxLength(200);
            b.Property(x => x.DisplayOrder).HasDefaultValue(0);
            b.Property(x => x.IsEnabled).HasDefaultValue(true);

            b.HasMany(x => x.AccessBy)
             .WithOne(sa => sa.Screen)
             .HasForeignKey(sa => sa.ScreenId)
             .OnDelete(DeleteBehavior.Cascade);

            b.ToTable(t => t.HasCheckConstraint("CK_Screens_IsEnabled_01", "IsEnabled IN (0,1)"));
        }
    }
    public class RoleScreenAccessConfig : IEntityTypeConfiguration<RoleScreenAccess>
    {
        public void Configure(EntityTypeBuilder<RoleScreenAccess> b)
        {
            b.ToTable("RoleScreenAccess");
            b.HasKey(x => new { x.RoleId, x.ScreenId });
            b.Property(x => x.Granted).HasDefaultValue(true);

            b.HasIndex(x => x.RoleId).HasDatabaseName("IX_RoleScreenAccess_RoleId");
            b.HasIndex(x => x.ScreenId).HasDatabaseName("IX_RoleScreenAccess_ScreenId");

            b.ToTable(t => t.HasCheckConstraint("CK_RoleScreenAccess_Granted_01", "Granted IN (0,1)"));
        }
    }

    public class RoleManageRoleConfig : IEntityTypeConfiguration<RoleManageRole>
    {
        public void Configure(EntityTypeBuilder<RoleManageRole> b)
        {
            b.ToTable("RoleManageRole");
            b.HasKey(x => new { x.ManagerRoleId, x.TargetRoleId });
            b.Property(x => x.CanManage).HasDefaultValue(true);

            b.HasOne(x => x.Manager)
             .WithMany(r => r.ManageTargets)
             .HasForeignKey(x => x.ManagerRoleId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Target)
             .WithMany(r => r.ManagedBy)
             .HasForeignKey(x => x.TargetRoleId)
             .OnDelete(DeleteBehavior.Cascade);

            b.ToTable(t =>
            {
                t.HasCheckConstraint("CK_RMR_CanManage_01", "CanManage IN (0,1)");
                t.HasCheckConstraint("CK_RMR_Self_01", "ManagerRoleId <> TargetRoleId");
            });
        }
    }

    public class RecipeConfig : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> b)
        {
            b.ToTable("Recipes");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
             .ValueGeneratedOnAdd()
             .HasAnnotation(SqliteAnnotationNames.Autoincrement, true);


            b.Property(x => x.Name).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();

            b.Property(x => x.IsActive).HasDefaultValue(false);

            // CreatedAt 기본값(DB)
            b.Property(x => x.CreatedAt)
             .HasDefaultValueSql("strftime('%Y-%m-%dT%H:%M:%fZ','now')");

            // "활성 레시피는 최대 1개" — 부분(필터) 유니크 인덱스
            b.HasIndex(x => x.IsActive)
             .HasFilter("IsActive = 1")
             .IsUnique()
             .HasDatabaseName("UX_Recipes_OnlyOneActive");
        }
    }

    public class ValueTypeDefConfig : IEntityTypeConfiguration<ValueTypeDef>
    {
        public void Configure(EntityTypeBuilder<ValueTypeDef> b)
        {
            b.ToTable("ValueType");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .HasAnnotation(SqliteAnnotationNames.Autoincrement, true);
            b.Property(x => x.Name).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();


        }
    }

    public class UnitConfig : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> b)
        {
            b.ToTable("Unit");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .HasAnnotation(SqliteAnnotationNames.Autoincrement, true);
            b.Property(x => x.Name).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();
        }
    }

    public class RecipeParamConfig : IEntityTypeConfiguration<RecipeParam>
    {
        public void Configure(EntityTypeBuilder<RecipeParam> b)
        {
            b.ToTable("RecipeParam");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .HasAnnotation(SqliteAnnotationNames.Autoincrement, true);
            b.Property(x => x.Name).IsRequired();
            b.Property(x => x.Value).IsRequired();

            b.HasOne(x => x.Recipe)
             .WithMany(r => r.Params)
             .HasForeignKey(x => x.RecipeId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.ValueType)
             .WithMany()
             .HasForeignKey(x => x.ValueTypeId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Unit)
             .WithMany()
             .HasForeignKey(x => x.UnitId)
             .OnDelete(DeleteBehavior.SetNull);
        }
    }

    public class ErrorConfig : IEntityTypeConfiguration<Error>
    {
        public void Configure(EntityTypeBuilder<Error> b)
        {
            b.ToTable("Errors");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
             .ValueGeneratedOnAdd()
             .HasAnnotation(SqliteAnnotationNames.Autoincrement, true);

            b.Property(x => x.Number).IsRequired();
            b.HasIndex(x => x.Number).IsUnique();

        }
    }

    public class RolePermissionRowConfig : IEntityTypeConfiguration<RolePermissionRow>
    {
        public void Configure(EntityTypeBuilder<RolePermissionRow> b)
        {
            b.HasNoKey();          
            b.ToView(null);
        }
    }
}
