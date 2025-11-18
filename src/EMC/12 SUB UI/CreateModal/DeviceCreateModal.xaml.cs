using System.Windows;

namespace EMC
{
    public partial class DeviceCreateModal : Window
    {
        public DeviceCreateModal()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // DialogResult = true → ShowDialog()의 return 값이 true가 됨
            this.DialogResult = true;
            this.Close();
        }

        // 취소 버튼 클릭 → 단순 닫기
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;   // optional
            this.Close();
        }

        // 호출한 쪽에서 ViewModel을 가져갈 수 있도록 반환 메서드 제공
        public object GetViewModel()
        {
            return this.DataContext;
        }
    }
}
