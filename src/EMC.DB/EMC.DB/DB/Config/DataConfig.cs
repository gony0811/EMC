using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EMC.DB
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> b)
        {
            b.ToTable("Roles");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
             .ValueGeneratedOnAdd()
             .HasAnnotation("Sqlite:Autoincrement", true);

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

        }
    }

    public class RoleScreenAccessConfig : IEntityTypeConfiguration<RoleScreenAccess>
    {
        public void Configure(EntityTypeBuilder<RoleScreenAccess> b)
        {
            b.ToTable("RoleScreenAccess");
            b.HasKey(x => new { x.RoleId, x.ScreenId });
            b.Property(x => x.Granted).HasDefaultValue(true);
            b.HasIndex(x => x.RoleId).HasName("IX_RoleScreenAccess_RoleId");
            b.HasIndex(x => x.ScreenId).HasName("IX_RoleScreenAccess_ScreenId");
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
             .HasAnnotation("Sqlite:Autoincrement", true);

            b.Property(x => x.Name).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();

            b.Property(x => x.IsActive).HasDefaultValue(false);

            b.Property(x => x.CreatedAt)
             .HasDefaultValueSql("strftime('%Y-%m-%dT%H:%M:%fZ','now')");

            b.HasIndex(x => x.IsActive)
             .HasFilter("IsActive = 1")
             .IsUnique()
             .HasName("UX_Recipes_OnlyOneActive");
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
             .HasAnnotation("Sqlite:Autoincrement", true);

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
             .HasAnnotation("Sqlite:Autoincrement", true);

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
             .HasAnnotation("Sqlite:Autoincrement", true);

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

    public sealed class MotionConfig : IEntityTypeConfiguration<Motion>
    {
        public void Configure(EntityTypeBuilder<Motion> e)
        {
            e.ToTable("Motions");
            e.HasKey(x => x.Id);

            e.Property(x => x.Name)
             .IsRequired()
             .HasMaxLength(100);

            e.HasIndex(x => x.Name).IsUnique();

            e.Property(x => x.Axis)
             .HasConversion<string>()
             .HasMaxLength(16);


            e.HasMany(x => x.Positions)
             .WithOne(p => p.Motion)
             .HasForeignKey(p => p.MotionId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public sealed class MotionPositionConfig : IEntityTypeConfiguration<MotionPosition>
    {
        public void Configure(EntityTypeBuilder<MotionPosition> e)
        {
            e.ToTable("MotionPositions");

            e.HasKey(x => x.Id);

            e.Property(x => x.Name)
             .IsRequired()
             .HasMaxLength(100);

            e.HasIndex(x => new { x.MotionId, x.Name })
             .IsUnique();

            e.Property(x => x.CurrentLocation).HasColumnType("REAL");
            e.Property(x => x.MinimumLocation).HasColumnType("REAL");
            e.Property(x => x.MaximumLocation).HasColumnType("REAL");
            e.Property(x => x.CurrentSpeed).HasColumnType("REAL");
            e.Property(x => x.MinimumSpeed).HasColumnType("INTEGER");
            e.Property(x => x.MaximumSpeed).HasColumnType("INTEGER");
        }
    }
    public sealed class AlarmConfig : IEntityTypeConfiguration<Alarm>
    {
        public void Configure(EntityTypeBuilder<Alarm> e)
        {
            e.ToTable("Alarms");
            e.HasKey(x => x.Id);


            e.Property(x => x.Id)
             .ValueGeneratedOnAdd()
             .HasAnnotation("Sqlite:Autoincrement", true);

            e.Property(x => x.Code)
            .IsRequired();                

            e.HasIndex(x => x.Code)
                .IsUnique();                   

            e.Property(x => x.Name).IsRequired().HasMaxLength(128);
            e.HasIndex(x => x.Name);

            e.Property(x => x.Description).HasMaxLength(512);
            e.Property(x => x.Action).HasMaxLength(512);

            // enum -> int 저장
            e.Property(x => x.Level)
             .HasConversion<int>()
             .HasColumnType("INTEGER")
             .HasDefaultValue(AlarmLevel.Light);

            e.Property(x => x.Status)
             .HasConversion<int>()
             .HasColumnType("INTEGER")
             .HasDefaultValue(AlarmStatus.RESET);

            e.Property(x => x.Enable)
             .HasConversion<int>()
             .HasColumnType("INTEGER")
             .HasDefaultValue(AlarmEnable.ENABLED);

            // 날짜/시간 (SQLite TEXT/ISO8601)
            e.Property(x => x.LastRaisedAt)
             .HasColumnType("TEXT");

            e.HasIndex(x => x.LastRaisedAt);
        }
    }

    public sealed class AlarmHistoryConfig : IEntityTypeConfiguration<AlarmHistory>
    {
        public void Configure(EntityTypeBuilder<AlarmHistory> e)
        {
            e.ToTable("AlarmHistories");
            e.HasKey(x => x.Id);

            e.Property(x => x.AlarmId).IsRequired();

            // enum -> int 저장
            e.Property(x => x.Level)
             .HasConversion<int>()
             .HasColumnType("INTEGER");

            e.Property(x => x.Status)
             .HasConversion<int>()
             .HasColumnType("INTEGER");

            e.Property(x => x.UpdateTime)
             .HasColumnType("TEXT")
             .IsRequired();

            e.HasIndex(x => x.UpdateTime);
            e.HasIndex(x => new { x.AlarmId, x.UpdateTime });

            e.HasOne<Alarm>()
             .WithMany()
             .HasForeignKey(x => x.AlarmId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
    public class DeviceConfig : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> e)
        {
            e.ToTable("Device");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            e.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            e.HasIndex(x => x.Name)
                .IsUnique();

            e.Property(x => x.Type)
                .HasConversion<string>()          // Enum을 string으로 저장
                .IsRequired();

            e.Property(x => x.InstanceName)
                .HasMaxLength(200)
                .IsRequired(false);

            e.Property(x => x.FileName)
                .HasMaxLength(200)
                .IsRequired(false);

            e.Property(x => x.IsUse)
                .HasMaxLength(20)
                .IsRequired(false);

            e.Property(x => x.Args)
                .HasMaxLength(1000)
                .IsRequired(false);

            e.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired(false);
        }
    }
}
