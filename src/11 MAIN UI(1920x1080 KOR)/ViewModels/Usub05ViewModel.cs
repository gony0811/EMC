using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace EGGPLANT
{
    /** 에러 목록과 관련된 뷰모델 정의 클래스
     *  에러 번호 및 내용, 원인과 조치 방법에 대해 메모
     *  정의된 에러가 발생시 부저가 발생하며 부저음을 설정할 수 있음
     * **/
    public partial class USub05ViewModel : ObservableObject
    {
        private readonly ErrorService _errorService;

        [ObservableProperty]
        private ObservableCollection<Error> errorList = new ObservableCollection<Error>();

        [ObservableProperty]
        private ObservableCollection<Buzzer> buzzerList = new ObservableCollection<Buzzer>();

        [ObservableProperty] private Error selectedError;


        public USub05ViewModel(ErrorService errorService)
        {
            _errorService = errorService;
            Initialize();
        }

        // 생성 
        [RelayCommand]
        public void CreateError()
        {
            Error error = new Error();
            ErrorList.Add(error);
            _ = _errorService.AddAsync(error);
        }

        // 저장
        [RelayCommand]
        public async Task SaveError(CancellationToken ct = default)
        {
            CommitPendingEdits(ErrorList);

            try
            {
                await _errorService.SaveAsync(ct);
                AlertModal.Ask(GetOwnerWindow(), "저장", "저장되었습니다.");
            }catch(Exception e)
            {
                AlertModal.Ask(GetOwnerWindow(), "저장실패", "저장에 실패했습니다.");
            }
            
        }

        // 되돌리기
        [RelayCommand]
        public async Task Discard(CancellationToken ct = default)
        {
            CommitPendingEdits(ErrorList);
            await _errorService.DiscardChangesAsync(ct);

            var fresh = await _errorService.GetListAsync(ct);
            ErrorList.Clear();
            foreach (var e in fresh) ErrorList.Add(e);
        }

        // 초기화 
        public async Task Initialize()
        {
            try {
                var errorList = await _errorService.GetListAsync();

                ErrorList.Clear();
                ErrorList = new ObservableCollection<Error>(errorList);
            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private static void CommitPendingEdits(IList<Error> items)
        {
            var view = CollectionViewSource.GetDefaultView(items);
            if (view is IEditableCollectionView v)
            {
                if (v.IsEditingItem) v.CommitEdit();
                if (v.IsAddingNew) v.CommitNew();
            }
        }

        private static Window? GetOwnerWindow() =>
            Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
            ?? Application.Current?.MainWindow;
    }
}
