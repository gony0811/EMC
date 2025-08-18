using System.Windows.Controls;

namespace EGGPLANT
{
    /// <summary>
    /// USub01.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class USub01 : Page
    {
        public USub01()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            this.tbCurrentDevice.Content = "TEST DEVICE";
        }
    }
}
