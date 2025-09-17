using Autofac;
using CommunityToolkit.Mvvm.Messaging;
using EGGPLANT.Device.PowerPmac;
using EGGPLANT.ViewModels;
using System.Data.SQLite;

namespace EGGPLANT
{
    public static class CStartUp
    {
        public static IContainer Build()
        {
            // DB Setting
            // DB가 없으면 파일+테이블을 한 번만 생성
            DB.EnsureCreatedOnce();

            var builder = new ContainerBuilder();

            var cs = new SQLiteConnectionStringBuilder
            {
                DataSource = DB.DbPath,
                Version = 3,
                ForeignKeys = true
            }.ToString();

            builder.Register(_ => new SqliteConnectionFactory(cs))
                .As<ISqliteConnectionFactory>()
                .SingleInstance();

            builder.RegisterType<CommonService>()
              .As<ICommonService>()
              .InstancePerLifetimeScope();

            builder.RegisterType<AuthzService>()
              .As<IAuthzService>()
              .InstancePerLifetimeScope();

            builder.RegisterType<RecipeService>()
              .As<IRecipeService>()
              .InstancePerLifetimeScope();

            // DB Setting End

            // 느슨한 결합
            builder.RegisterInstance(WeakReferenceMessenger.Default)
               .As<IMessenger>()
               .SingleInstance();

            builder.RegisterType<UInitialize>().SingleInstance();
            builder.RegisterType<UInitializeViewModel>().InstancePerDependency();
            builder.RegisterType<CommonData>().SingleInstance();
            builder.RegisterType<CSYS>().AsSelf().SingleInstance();
            builder.RegisterType<UDevHistory>().SingleInstance();
            builder.RegisterType<UMain>().SingleInstance();
            builder.RegisterType<UMainViewModel>().AsSelf().SingleInstance();

            builder.RegisterType<USub01>().SingleInstance();
            builder.RegisterType<USub01ViewModel>().InstancePerDependency();

            builder.RegisterType<USub01n01>().SingleInstance();
            builder.RegisterType<USub01n01ViewModel>().InstancePerDependency();
            builder.RegisterType<USub01n02>().SingleInstance();
            builder.RegisterType<USub01n02ViewModel>().InstancePerDependency();
            //builder.RegisterType<Usub01n02>().AsSelf();
            //builder.RegisterType<USubViewModel01n02>().AsSelf().InstancePerDependency();

            builder.RegisterType<USub01n03>().SingleInstance();
            builder.RegisterType<USub01n03ViewModel>().AsSelf();

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

            builder.RegisterType<RecipeCreateViewModel>().InstancePerDependency();
            builder.RegisterType<RecipeCreateWindow>().InstancePerDependency();

            builder.RegisterType<ParameterCreateVM>().InstancePerDependency();
            builder.RegisterType<ParameterCreateWindow>().InstancePerDependency();

            return builder.Build();
        }

    }
}
