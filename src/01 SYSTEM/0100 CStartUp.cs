using Autofac;
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
            builder.RegisterType<Usub05>().SingleInstance();
            builder.RegisterInstance<CTrace>(new CTrace("DeviceLogTrace")).Keyed<CTrace>("DeviceLogTrace");
            builder.RegisterInstance<CTrace>(new CTrace("Trace")).Keyed<CTrace>("Trace");
            builder.RegisterType<CProcessMap>().AsSelf().SingleInstance();
            return builder.Build();
        }

    }
}
