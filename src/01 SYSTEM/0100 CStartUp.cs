using Autofac;
using EGGPLANT._11_MAIN_UI_1920x1080_KOR_;
using EGGPLANT._13_DataStore;
using EGGPLANT.ViewModels;

namespace EGGPLANT
{
    public static class CStartUp
    {
        public static IContainer Build()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<UInitialize>().SingleInstance();
            builder.RegisterType<CSYS>().AsSelf().SingleInstance();
            builder.RegisterType<UDevHistory>().SingleInstance();
            builder.RegisterType<UMain>().SingleInstance();
            builder.RegisterType<UMainViewModel>().InstancePerDependency();

            builder.RegisterType<USub01>().SingleInstance();
            builder.RegisterType<USub01ViewModel>().InstancePerDependency();

            builder.RegisterType<Usub01n01>().SingleInstance();

            builder.RegisterType<Usub01n02>().SingleInstance();
            builder.RegisterType<USubViewModel01n02>().InstancePerDependency();

            //builder.RegisterType<Usub01n02>().AsSelf();
            //builder.RegisterType<USubViewModel01n02>().AsSelf().InstancePerDependency();

            builder.RegisterType<Usub01n03>().SingleInstance();
            builder.RegisterType<Usub01n04>().SingleInstance();

            builder.RegisterType<USub02>().SingleInstance();
            builder.RegisterType<USub03>().SingleInstance();
            builder.RegisterType<USub04>().SingleInstance();
            builder.RegisterType<USub06>().SingleInstance();
            builder.RegisterType<Usub05>().SingleInstance();
            builder.RegisterType<USub07>().SingleInstance();
            builder.RegisterType<USub08>().SingleInstance();
            builder.RegisterType<USub09>().SingleInstance();

            builder.RegisterInstance<CTrace>(new CTrace("DeviceLogTrace")).Keyed<CTrace>("DeviceLogTrace");
            builder.RegisterInstance<CTrace>(new CTrace("Trace")).Keyed<CTrace>("Trace");
            builder.RegisterType<CProcessMap>().AsSelf().SingleInstance();

            // 스토어 - 전역 공유 

            builder.RegisterType<MotorStateStore>().AsSelf().SingleInstance();      // 모터 상태 정보 

            return builder.Build();
        }

    }
}
