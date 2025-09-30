using Autofac;
using EGGPLANT.Device.PowerPmac;
using EGGPLANT.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace EGGPLANT
{
    public static class CStartUp
    {
        public static IContainer Build()
        {
            var builder = new ContainerBuilder();

            // === 공통 DB 경로 & 연결문자열 ===
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MyApp", "emc.db");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
            var connStr = $"Data Source={dbPath};Cache=Shared;Pooling=True";

            // === DbContext (창/화면 스코프 단위) ===
            builder.Register(ctx =>
                {
                    var opts = new DbContextOptionsBuilder<AppDb>()
                        .UseSqlite(connStr)
                        .Options;
                    return new AppDb(opts);
                })
                .AsSelf()
                .InstancePerLifetimeScope();

            // === 가벼운 인-스코프 팩토리(Func<AppDb>) ===
            builder.Register<Func<AppDb>>(ctx =>
            {
                var scope = ctx.Resolve<ILifetimeScope>();
                return () => scope.Resolve<AppDb>();
            });

            // === 스코프-소유 런타임 팩토리(IAppDbFactory) ===
            builder.RegisterType<AppDbFactory>()
                   .As<IAppDbFactory>()
                   .SingleInstance();

            // === 컨테이너 빌드 직후 1회 DB 초기화 ===
            builder.RegisterBuildCallback(c =>
            {
                using var s = c.BeginLifetimeScope();
                var db = s.Resolve<AppDb>();
                db.Database.Migrate(); // 마이그레이션 적용(없으면 생성)

                db.Database.OpenConnection();
                try
                {
                    db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
                    db.Database.ExecuteSqlRaw("PRAGMA foreign_keys=ON;");
                }
                finally { db.Database.CloseConnection(); }
            });
            builder.RegisterGeneric(typeof(DbRepository<>))
               .As(typeof(IDbRepository<>))
               .InstancePerLifetimeScope();
            builder.RegisterType<RecipeService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ParameterService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ErrorService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ScreenService>().AsSelf().InstancePerLifetimeScope();

            builder.RegisterType<UInitialize>()
                 .AsSelf()
                 .InstancePerDependency()
                 .OnActivated(e =>
                 {
                     if (e.Instance.DataContext == null)
                         e.Instance.DataContext = e.Context.Resolve<UInitializeViewModel>();
                 });
            builder.RegisterType<UInitializeViewModel>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<CSYS>().AsSelf().SingleInstance();
            builder.RegisterType<UDevHistory>().SingleInstance();
            builder.RegisterType<UMain>().SingleInstance();
            builder.RegisterType<UMainViewModel>().InstancePerDependency();

            builder.RegisterType<USub01>().SingleInstance();
            builder.RegisterType<USub01ViewModel>().InstancePerDependency();
            

            builder.RegisterType<USub01n01>().SingleInstance();
            builder.RegisterType<USub01n01ViewModel>().InstancePerDependency();
            builder.RegisterType<USub01n02>().SingleInstance();
            builder.RegisterType<USub01n02ViewModel>().InstancePerDependency();
            //builder.RegisterType<Usub01n02>().AsSelf();
            //builder.RegisterType<USubViewModel01n02>().AsSelf().InstancePerDependency();

            builder.RegisterType<USub01n03>().SingleInstance();
            builder.RegisterType<USub01n03ViewModel>().InstancePerDependency();
            builder.RegisterType<USub01n04>().SingleInstance();
            builder.RegisterType<USub01n04ViewModel>().InstancePerDependency();

            builder.RegisterType<USub02>().SingleInstance();
            builder.RegisterType<USub02ViewModel>().InstancePerDependency();
            builder.RegisterType<USub03>().SingleInstance();
            builder.RegisterType<USub03ViewModel>().InstancePerDependency();
            builder.RegisterType<USub04>().SingleInstance();
            builder.RegisterType<USub05>().SingleInstance();
            builder.RegisterType<USub05ViewModel>().InstancePerDependency();
            builder.RegisterType<USub06>().SingleInstance();
            builder.RegisterType<USub07>().SingleInstance();

            builder.RegisterType<USub08>().SingleInstance();
            builder.RegisterType<USub09>().SingleInstance();

            builder.RegisterInstance<CTrace>(new CTrace("DeviceLogTrace")).Keyed<CTrace>("DeviceLogTrace");
            builder.RegisterInstance<CTrace>(new CTrace("Trace")).Keyed<CTrace>("Trace");
            builder.RegisterType<CProcessMap>().SingleInstance();

            builder.RegisterType<CErrorList>().SingleInstance();
            builder.RegisterType<CError>().SingleInstance();
            builder.RegisterType<CPmacMotion>().SingleInstance();
            builder.RegisterType<CExecute>().SingleInstance();

            builder.RegisterType<MotorStateStore>().SingleInstance();

            builder.RegisterType<RecipeCreateVM>().InstancePerDependency();
            builder.RegisterType<RecipeCreateWindow>().InstancePerDependency();
            builder.RegisterType<UserViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<NavigationViewModel>().SingleInstance();

            return builder.Build();
        }

    }
}
