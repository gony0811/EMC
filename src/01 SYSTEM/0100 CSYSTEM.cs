using Autofac;
using System.Windows.Controls;
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

        public UDevHistory FDevHistory = App.Container.Resolve<UDevHistory>();
        public CTrace DeviceLogTrace = App.Container.ResolveKeyed<CTrace>("DeviceLogTrace");
        public CTrace Trace = App.Container.ResolveKeyed<CTrace>("Trace");
    }
}