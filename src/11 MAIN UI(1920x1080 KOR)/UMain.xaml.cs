using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace EGGPLANT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UMain : MahApps.Metro.Controls.MetroWindow
    {
        public UMain()
        {
            InitializeComponent();     
            CSYS.Initialize(AppDomain.CurrentDomain.BaseDirectory, this);
            Init();
        }

        private void Init()
        {
            DataContext = new UMainViewModel();

            this.tbThread.Text = "1ms";
            this.tbTime.Text = "오전 00:00:00";
            this.tbUser.Text = "SERVICE ENGINEER";
            this.tbBuild.Text = "Build 0000.00.00";
            this.tbDeveloper.Text = "Developed by EGGPLANT";
            this.tbStatus.Text = "설비 정지 상태 입니다.(설비 초기화 필요!)";
            this.tbSerialNumber.Text = "SN : EGGPLANT Copyright(c) EGGPLANT Corp. All rights reserved.";

        }
    }
}