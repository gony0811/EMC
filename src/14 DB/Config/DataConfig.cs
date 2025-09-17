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
            b.Property(x => x.Description).HasMaxLength(200);
            b.Property(x => x.Password).HasMaxLength(200).IsRequired();
            b.Property(x => x.IsActive).HasDefaultValue(true);

            // 필요 시 체크 제약
            b.ToTable(t => t.HasCheckConstraint("CK_Roles_IsActive_01", "IsActive IN (0,1)"));
        }
    }

    public class PermissionCategoryConfig : IEntityTypeConfiguration<PermissionCategory>
    {
        public void Configure(EntityTypeBuilder<PermissionCategory> b)
        {
            b.ToTable("PermissionCategory");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired();
            b.Property(x => x.DisplayOrder).HasDefaultValue(0);
            b.HasIndex(x => x.Name).IsUnique();
        }
    }

    public class PermissionConfig : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> b)
        {
            b.ToTable("Permission");
            b.HasKey(x => x.Id);

            b.Property(x => x.Name).IsRequired();
            b.Property(x => x.IsEnabled).HasDefaultValue(false);

            b.HasOne(x => x.Category)
             .WithMany(c => c.Permissions)
             .HasForeignKey(x => x.CategoryId)
             .OnDelete(DeleteBehavior.Restrict); // 스키마는 ON DELETE RESTRICT

            b.HasIndex(x => new { x.CategoryId, x.Name }).IsUnique();

            b.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Permission_IsEnabled_01", "IsEnabled IN (0,1)");
            });
        }
    }

    public class RoleCategoryManageConfig : IEntityTypeConfiguration<RoleCategoryManage>
    {
        public void Configure(EntityTypeBuilder<RoleCategoryManage> b)
        {
            b.ToTable("RoleCategoryManage");

            // 복합 PK
            b.HasKey(x => new { x.RoleId, x.CategoryId });

            b.Property(x => x.CanManage).HasDefaultValue(true);

            b.HasOne(x => x.Role)
             .WithMany(r => r.CategoryManages)
             .HasForeignKey(x => x.RoleId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Category)
             .WithMany(c => c.RoleManages)
             .HasForeignKey(x => x.CategoryId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => x.RoleId).HasDatabaseName("IX_RoleCategoryManage_RoleId");
            b.HasIndex(x => x.CategoryId).HasDatabaseName("IX_RoleCategoryManage_CategoryId");

            b.ToTable(t =>
            {
                t.HasCheckConstraint("CK_RCM_CanManage_01", "CanManage IN (0,1)");
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
}
