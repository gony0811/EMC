using EPFramework.IoC;
using System;
using System.Windows;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;

namespace EMC
{
    [View(Lifetime.Singleton)]
    public partial class USub03 : Page
    {
        public USub03(USub03ViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            
        }
        private void OnEngineerChange(object sender, RoutedEventArgs e)
        {
            ConfirmAndSavePassword("엔지니어");
        }

        private void OnAdminChange(object sender, RoutedEventArgs e)
        {
            ConfirmAndSavePassword("관리자");
        }

        private void ConfirmAndSavePassword(string roleLabel)
        {
            // 현재 윈도우를 Owner로 설정
            var owner = Window.GetWindow(this);

            // 1) 새 비밀번호 입력
            var dlg1 = new UPasswordNPad(owner, $"{roleLabel} 비밀번호 입력");
            if (dlg1.ShowDialog() != true) return; // 취소

            var pw1 = dlg1.Password;

            // 유효성 검사 실패시
            if (!IsValidPassword(pw1))
            {
                
            }

            // 2) 비밀번호 확인
            var dlg2 = new UPasswordNPad(owner, $"{roleLabel} 비밀번호 확인");
            if (dlg2.ShowDialog() != true) return; // 취소

            var pw2 = dlg2.Password;

            if (pw1 != pw2)
            {
                MessageBox.Show(owner, "비밀번호가 일치하지 않습니다. 다시 시도해 주세요.", "불일치",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3) 저장 로직 (실제 저장 처리는 프로젝트에 맞게 교체)
            try
            {
                SavePassword(roleLabel, pw1);
                MessageBox.Show(owner, "비밀번호가 변경되었습니다.", "완료",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(owner, $"저장 중 오류가 발생했습니다.\n{ex.Message}", "오류",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 비밀번호 규칙 (원하면 강화하세요)
        private bool IsValidPassword(string pw)
        {
            if (string.IsNullOrWhiteSpace(pw)) return false;
            return pw.Length >= 4; // 예: 최소 4자리
        }

        // 실제 저장 처리(예시)
        private void SavePassword(string roleLabel, string password)
        {
            // TODO: 역할별 저장 로직 연결
            // ex) _userService.UpdatePassword(roleLabel, password);
        }


    }


}

