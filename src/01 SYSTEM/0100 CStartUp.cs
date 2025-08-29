using Autofac;
using EGGPLANT.Device.PowerPmac;
using EGGPLANT.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            builder.RegisterType<USub02>().SingleInstance();
            builder.RegisterType<USub02ViewModel>().InstancePerDependency();
            builder.RegisterType<USub05>().SingleInstance();
            builder.RegisterType<USub05ViewModel>().InstancePerDependency();
            builder.RegisterType<USub01n01>().SingleInstance();
            builder.RegisterType<USub01n02>().SingleInstance();
            builder.RegisterType<USub01n03>().SingleInstance();
            builder.RegisterType<USub01n04>().SingleInstance();
            builder.RegisterType<USub01n01ViewModel>().InstancePerDependency();
            builder.RegisterType<USub01n02ViewModel>().InstancePerDependency();
            builder.RegisterType<USub01n03ViewModel>().InstancePerDependency();
            builder.RegisterType<USub01n04ViewModel>().InstancePerDependency();

            builder.RegisterType<UError>().SingleInstance();

            builder.RegisterInstance<CTrace>(new CTrace("DeviceLogTrace")).Keyed<CTrace>("DeviceLogTrace");
            builder.RegisterInstance<CTrace>(new CTrace("Trace")).Keyed<CTrace>("Trace");
            builder.RegisterType<CProcessMap>().SingleInstance();

            builder.RegisterType<CErrorList>().SingleInstance();
            builder.RegisterType<CError>().SingleInstance();
            builder.RegisterType<CPmacMotion>().SingleInstance();
            builder.RegisterType<CExecute>().SingleInstance();
            return builder.Build();
        }

    }
}
