using EGGPLANT;
using EGGPLANT.Models;
using EGGPLANT.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EGGPLANT
{
    /// <summary>
    /// USub02.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class USub02 : Page
    {
        public USub02(USub02ViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
           
        }
        private void ParamGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            // "Value" 컬럼에서만 가로채기
            if ((e.Column.Header?.ToString() ?? "") != "Value")
                return;

            e.Cancel = true; // 기본 셀 편집 막고 모달로 대체

            if (e.Row?.Item is not ParameterVM item)
                return;

            // 모달 생성 및 초기값 설정
            var dlg = new UNmumPad
            {
                Owner = Window.GetWindow(this),
                CurrentValue = item.Value ?? "0",
                // 필요 시 최대/최소 설정 (없으면 생략/빈 문자열)
                MaximumValue = item.Maximum,
                MinimumValue = item.Minimum,
                // Step = 1.0,          // +/− 증감 단위
                // AllowDecimal = true, // 소수 허용
            };

            var ok = dlg.ShowDialog() == true;
            if (ok)
            {
                // 적용된 값 반영
                item.Value = dlg.CurrentValue;
                // 즉시 UI 갱신
                ParamGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                ParamGrid.Items.Refresh();
            }
        }

        //private void OnRowClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (sender is DataGridRow row && DataContext is USub02ViewModel vm)
        //    {
        //        var item = row.DataContext; // 현재 행의 아이템(RecipeVM)
        //        if (vm.RecipeClickCommand.CanExecute(item))
        //        {
        //            vm.RecipeClickCommand.Execute(item);
        //            e.Handled = true; 
        //        }
        //    }
        //}

    }
}
