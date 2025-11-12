using System;
using System.Text;
using System.Windows;
using PropertyTools.Wpf;

namespace EMC
{
    public partial class ObjectInspectorWindow : Window
    {
        public object ResultObject { get; private set; }

        public ObjectInspectorWindow(object target)
        {
            InitializeComponent();
            DataContext = new { TargetObject = target };
        }

        // ✅ "저장" 버튼 클릭
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation 수행
                if (!ValidateObject())
                {
                    MessageBox.Show(this,
                        "입력한 값 중 유효하지 않은 항목이 있습니다.\n확인 후 다시 시도해주세요.",
                        "유효성 검사 오류",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return; // 🚫 저장 중단
                }

                // 정상이라면 결과 객체 반환
                ResultObject = ((dynamic)DataContext).TargetObject;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    $"저장 중 오류가 발생했습니다:\n{ex.Message}",
                    "오류",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ✅ 간단한 Validation (예: null/빈문자 확인)
        private bool ValidateObject()
        {
            var target = ((dynamic)DataContext).TargetObject;
            var type = target.GetType();

            foreach (var prop in type.GetProperties())
            {
                if (!prop.CanRead) continue;
                var value = prop.GetValue(target);

                // 예시: string 속성은 빈 값 불가
                if (prop.PropertyType == typeof(string) &&
                    string.IsNullOrWhiteSpace(value as string))
                    return false;

                // 예시: double 속성은 NaN 불가
                if (prop.PropertyType == typeof(double) &&
                    (double.IsNaN((double)value) || double.IsInfinity((double)value)))
                    return false;
            }

            return true;
        }
    }
}
