using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
#pragma warning disable CS8625 // null 가능 참조에 대한 역참조입니다.

namespace EGGPLANT
{
    static partial class CSYS
    {
        #region Assembly Version
        /// <summary>
        /// 컴파일한 날짜를 구한다.
        ///   단, AssemblyInfo.cs 파일에서 AssemblyVersion는 다음 형식으로 되어있어야만한다.
        ///   [assembly: AssemblyVersion("1.0.*")]
        /// </summary>        /// <returns        
        static public string GetBuildVersion()
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

        static private string FDirectory = "";

        static public string Directory { get { return FDirectory; } }

        static public void Initialize(string appPath, UMain AMain)
        {
            FDirectory = appPath;
            FMain = AMain;
        }

        static public void Show()
        {

        }

        static public UDevHistory FDevHistory = new UDevHistory();
        static public UMain FMain = null;
        static public USub01 FSub01 = new USub01();
        static public USub02 USub02 = new USub02();
        static public Usub05 Usub05 = new Usub05();

        static public void GoToSub01() => FMain.MainFrame.Navigate(FSub01);
        static public void GoToSub02() => FMain.MainFrame.Navigate(USub02);
        static public void GoToSub05() => FMain.MainFrame.Navigate(Usub05);
    }
}