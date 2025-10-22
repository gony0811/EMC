
using EPFramework.IoC;
using System.Windows.Controls;

namespace EMC
{
    [View(Lifetime.Singleton)]
    public partial class USub02 : Page
    {
        public USub02(USub02ViewModel vm)

        {
            DataContext = vm;
            InitializeComponent();
        }

        //private void ParamGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        //{
        //    // "Value" 컬럼에서만 가로채기
        //    if ((e.Column.Header?.ToString() ?? "") != "Value")
        //        return;

        //    e.Cancel = true; // 기본 셀 편집 막고 모달로 대체

        //    if (e.Row?.Item is not ParameterModel item)
        //        return;

        //    // 모달 생성 및 초기값 설정
        //    var dlg = new UNmumPad
        //    {
        //        Owner = Window.GetWindow(this),
        //        CurrentValue = item.Value ?? "0",
        //        // 필요 시 최대/최소 설정 (없으면 생략/빈 문자열)
        //        MaximumValue = item.MaximumValue,
        //        MinimumValue = item.MinimumValue,
        //        // Step = 1.0,          // +/− 증감 단위
        //        // AllowDecimal = true, // 소수 허용
        //    };

        //    var ok = dlg.ShowDialog() == true;
        //    if (ok)
        //    {
        //        // 적용된 값 반영
        //        item.Value = dlg.CurrentValue;
        //        // 즉시 UI 갱신
        //        ParamGrid.CommitEdit(DataGridEditingUnit.Cell, true);
        //        ParamGrid.Items.Refresh();
        //    }
        //}
    }
}
