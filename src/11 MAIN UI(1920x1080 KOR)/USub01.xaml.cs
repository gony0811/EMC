using Autofac;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup.Localizer;

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

            // 시작 시 TAB1을 기본 선택하고 컨텐츠 로드
            Loaded += (_, __) =>
            {
                var first = TabPanel.Children.OfType<ToggleButton>()
                                             .FirstOrDefault(tb => (tb.Tag as string) == "Tab1");
                if (first != null) first.IsChecked = true;
            };
        }

        private void Tab_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not ToggleButton tb) return;

            // 1) 다른 토글 버튼은 해제 (라디오버튼처럼 동작)
            foreach (var other in TabPanel.Children.OfType<ToggleButton>())
                if (!ReferenceEquals(other, tb)) other.IsChecked = false;

            // 2) 태그에 따라 해당 UserControl 로드
            switch (tb.Tag as string)
            {
                case "Tab1":
                    ContentFrame.Content = App.Container.Resolve<USub01n01>();
                    break;
                case "Tab2":
                    ContentFrame.Content = App.Container.Resolve<USub01n02>();
                    break;
                case "Tab3":
                    ContentFrame.Content = App.Container.Resolve<USub01n03>();
                    break;
                case "Tab4":
                    ContentFrame.Content = App.Container.Resolve<USub01n04>();
                    break;
            }
        }
    }
}
