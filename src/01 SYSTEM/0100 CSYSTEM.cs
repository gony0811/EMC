using Autofac;
using Autofac.Core;
using EGGPLANT._11_MAIN_UI_1920x1080_KOR_;
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
        }

        public void Show()
        {

        }

        public UDevHistory FDevHistory = App.Container.Resolve<UDevHistory>();
        public UMain FMain = App.Container.Resolve<UMain>();
        public USub01 FSub01 = App.Container.Resolve<USub01>();
        public USub01ViewModel FSub01ViewModel = App.Container.Resolve<USub01ViewModel>();
        public USub02 USub02 = App.Container.Resolve<USub02>();
        public USub03 USub03 = App.Container.Resolve<USub03>();
        public USub04 USub04 = App.Container.Resolve<USub04>();
        public Usub05 Usub05 = App.Container.Resolve<Usub05>();
        public USub06 Usub06 = App.Container.Resolve<USub06>();
        public USub07 Usub07 = App.Container.Resolve<USub07>();
        public USub08 Usub08 = App.Container.Resolve<USub08>();
        public USub09 Usub09 = App.Container.Resolve<USub09>();


        public CTrace DeviceLogTrace = App.Container.ResolveKeyed<CTrace>("DeviceLogTrace");
        public CTrace Trace = App.Container.ResolveKeyed<CTrace>("Trace");

        public void GoToSub01()
        {
            FMain.Dispatcher.Invoke(() => FMain.MainFrame.Navigate(FSub01));
        }
        public void GoToSub02() => FMain.MainFrame.Navigate(USub02);
        public void GoToSub03() => FMain.MainFrame.Navigate(USub03);
        public void GoToSub04() => FMain.MainFrame.Navigate(USub04);
        public void GoToSub05() => FMain.MainFrame.Navigate(Usub05);
        public void GoToSub06() => FMain.MainFrame.Navigate(Usub06);
        public void GoToSub07() => FMain.MainFrame.Navigate(Usub07);
        public void GoToSub08() => FMain.MainFrame.Navigate(Usub08);
        public void GoToSub09() => FMain.MainFrame.Navigate(Usub09);


    }
}