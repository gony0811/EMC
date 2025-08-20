using Autofac;
using Autofac.Core;
using EGGPLANT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS8625 // null 가능 참조에 대한 역참조입니다.

namespace EGGPLANT
{
    public partial class CSYS
    {
        #region Assembly Version
        /// <summary>
        /// 컴파일한 날짜를 구한다.
        ///   단, AssemblyInfo.cs 파일에서 AssemblyVersion는 다음 형식으로 되어있어야만한다.
        ///   [assembly: AssemblyVersion("1.0.*")]
        /// </summary>        /// <returns        
        public static string GetBuildVersion()
        {
            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = executingAssembly?.GetName().Version;

            if (version == null)
                return "Unknown";

            DateTime buildDate = new DateTime(2000, 1, 1)
                .AddDays(version.Build)
                .AddSeconds(version.Revision * 2);

            return buildDate.ToString("yy.MM.dd");
        }
        #endregion

        private string FDirectory = "";

        public string Directory { get { return FDirectory; } }

        public void Initialize(string appPath)
        {
            FDirectory = appPath;
            FSub01.DataContext = FSub01ViewModel;
            FSub01n01.DataContext = FSub01n01ViewModel;
            FSub01n02.DataContext = FSub01n02ViewModel;
            FSub01n03.DataContext = FSub01n03ViewModel;
            FSub01n04.DataContext = FSub01n04ViewModel;
            FSub02.DataContext = FSub02ViewModel;
        }

        public void Show()
        {

        }

        public UDevHistory FDevHistory = App.Container.Resolve<UDevHistory>();
        public UMain FMain = App.Container.Resolve<UMain>();
        public USub01 FSub01 = App.Container.Resolve<USub01>();
        public USub01ViewModel FSub01ViewModel = App.Container.Resolve<USub01ViewModel>();
        public USub01n01 FSub01n01 = App.Container.Resolve<USub01n01>();
        public USub01n01ViewModel FSub01n01ViewModel = App.Container.Resolve<USub01n01ViewModel>();
        public USub01n02 FSub01n02 = App.Container.Resolve<USub01n02>();
        public USub01n02ViewModel FSub01n02ViewModel = App.Container.Resolve<USub01n02ViewModel>();
        public USub01n03 FSub01n03 = App.Container.Resolve<USub01n03>();
        public USub01n03ViewModel FSub01n03ViewModel = App.Container.Resolve<USub01n03ViewModel>();
        public USub01n04 FSub01n04 = App.Container.Resolve<USub01n04>();
        public USub01n04ViewModel FSub01n04ViewModel = App.Container.Resolve<USub01n04ViewModel>();
        public USub02ViewModel FSub02ViewModel = App.Container.Resolve<USub02ViewModel>();
        public USub02 FSub02 = App.Container.Resolve<USub02>();
        public USub05 FSub05 = App.Container.Resolve<USub05>();

        public UError FError = App.Container.Resolve<UError>();

        public CTrace DeviceLogTrace = App.Container.ResolveKeyed<CTrace>("DeviceLogTrace");
        public CTrace Trace = App.Container.ResolveKeyed<CTrace>("Trace");

        public void GoToSub01() => FMain.MainFrame.Navigate(FSub01);
        public void GoToSub02() => FMain.MainFrame.Navigate(FSub02);
        public void GoToSub05() => FMain.MainFrame.Navigate(FSub05);
    }
}