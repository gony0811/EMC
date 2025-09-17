﻿using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using DataFormats = System.Windows.DataFormats;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;
using DataObject = System.Windows.DataObject;
namespace EGGPLANT
{
    public static class NumericTextBox
    {
        // 켜기/끄기
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled", typeof(bool), typeof(NumericTextBox),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static void SetIsEnabled(DependencyObject d, bool v) => d.SetValue(IsEnabledProperty, v);
        public static bool GetIsEnabled(DependencyObject d) => (bool)d.GetValue(IsEnabledProperty);

        // 소수 허용 여부
        public static readonly DependencyProperty AllowDecimalProperty =
            DependencyProperty.RegisterAttached(
                "AllowDecimal", typeof(bool), typeof(NumericTextBox),
                new PropertyMetadata(false));

        public static void SetAllowDecimal(DependencyObject d, bool v) => d.SetValue(AllowDecimalProperty, v);
        public static bool GetAllowDecimal(DependencyObject d) => (bool)d.GetValue(AllowDecimalProperty);

        // 음수 허용 여부
        public static readonly DependencyProperty AllowNegativeProperty =
            DependencyProperty.RegisterAttached(
                "AllowNegative", typeof(bool), typeof(NumericTextBox),
                new PropertyMetadata(false));

        public static void SetAllowNegative(DependencyObject d, bool v) => d.SetValue(AllowNegativeProperty, v);
        public static bool GetAllowNegative(DependencyObject d) => (bool)d.GetValue(AllowNegativeProperty);

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox tb) return;

            if ((bool)e.NewValue)
            {
                tb.PreviewTextInput += OnPreviewTextInput;
                tb.PreviewKeyDown += OnPreviewKeyDown;
                DataObject.AddPastingHandler(tb, OnPaste);
            }
            else
            {
                tb.PreviewTextInput -= OnPreviewTextInput;
                tb.PreviewKeyDown -= OnPreviewKeyDown;
                DataObject.RemovePastingHandler(tb, OnPaste);
            }
        }

        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // 스페이스 금지, 나머지 편집 키/이동 키 허용
            if (e.Key == Key.Space) { e.Handled = true; return; }
        }

        private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is not TextBox tb) return;

            bool allowDecimal = GetAllowDecimal(tb);
            bool allowNegative = GetAllowNegative(tb);

            string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var regex = BuildRegex(allowDecimal, allowNegative, sep);

            // 현재 텍스트에 입력 예정 문자를 적용한 "가정 텍스트"로 유효성 검사
            string next = GetProposedText(tb, e.Text);
            e.Handled = !regex.IsMatch(next);
        }

        private static void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (sender is not TextBox tb) return;
            if (!e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText)) { e.CancelCommand(); return; }

            string paste = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string ?? "";
            string next = GetProposedText(tb, paste);

            bool allowDecimal = GetAllowDecimal(tb);
            bool allowNegative = GetAllowNegative(tb);
            string sep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var regex = BuildRegex(allowDecimal, allowNegative, sep);

            if (!regex.IsMatch(next)) e.CancelCommand();
        }

        private static string GetProposedText(TextBox tb, string input)
        {
            int selStart = tb.SelectionStart;
            int selLen = tb.SelectionLength;

            string before = tb.Text[..selStart];
            string after = tb.Text[(selStart + selLen)..];

            return before + input + after;
        }

        private static Regex BuildRegex(bool allowDecimal, bool allowNegative, string decimalSep)
        {
            // 빈 문자열은 허용(사용자가 지우는 중)
            // 정수: ^-?\d*$
            // 실수: ^-?\d*(\.\d*)?$  (현재 문화권 소수점 사용)
            string sign = allowNegative ? "-?" : "";
            string core = allowDecimal ? $@"\d*({Regex.Escape(decimalSep)}\d*)?" : @"\d*";
            return new Regex($@"^{sign}{core}$");
        }
    }
}