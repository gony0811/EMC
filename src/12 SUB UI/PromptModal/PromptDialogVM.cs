using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace EGGPLANT
{
    public interface IDialogRequestClose<T>
    {
        event EventHandler<DialogCloseRequestedEventArgs<T>>? CloseRequested;
    }

    public readonly struct DialogResult<T>
    {
        public DialogResult(bool? dialogResult, T value) { DialogResultValue = dialogResult; Value = value; }
        public bool? DialogResultValue { get; }
        public T Value { get; }
    }
    public sealed class DialogCloseRequestedEventArgs<T> : EventArgs
    {
        public DialogCloseRequestedEventArgs(DialogResult<T> result) => Result = result;
        public DialogResult<T> Result { get; }
    }

    public abstract partial class PromptDialogVM<T> : ObservableObject, IDialogRequestClose<T?>
    {
        public event EventHandler<DialogCloseRequestedEventArgs<T?>>? CloseRequested;

        [ObservableProperty] private string title = string.Empty;
        [ObservableProperty] private T? value;
        [ObservableProperty] private bool isBusy;
        public ObservableCollection<string> Errors { get; } = new();

        protected PromptDialogVM(T? initial = default, string? title = null)
        {
            Value = initial;
            Title = title ?? string.Empty;
        }

        // OK: 검증 통과 시에만 결과 반환(닫기 요청)
        [RelayCommand(CanExecute = nameof(CanOk))]
        private async Task OkAsync(CancellationToken ct)
        {
            Errors.Clear();
            try
            {
                IsBusy = true;
                var errs = await ValidateForOkAsync(ct);
                if (errs.Count > 0)
                {
                    foreach (var e in errs) Errors.Add(e);
                    OnValidationFailed(errs);
                    return;
                }

                CloseRequested?.Invoke(this,
                    new DialogCloseRequestedEventArgs<T?>(new DialogResult<T?>(true, GetResult())));
            }
            finally { IsBusy = false; }
        }

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel()
            => CloseRequested?.Invoke(this,
                new DialogCloseRequestedEventArgs<T?>(new DialogResult<T?>(false, default)));

        // OK 버튼 활성화 조건(필요시 파생에서 확장)
        protected virtual bool CanOk() => !IsBusy;
        protected virtual bool CanCancel() => !IsBusy;

        // 결과 후처리(필요 시 파생에서 변환)
        protected virtual T? GetResult() => Value;

        // ✅ 유효성 검증 파이프라인: DataAnnotations + 커스텀(비동기)
        protected virtual async Task<IReadOnlyList<string>> ValidateForOkAsync(CancellationToken ct)
        {
            var list = new List<string>();
            if (Value is null)
            {
                list.Add("값이 비어 있습니다.");
                return list;
            }

            // (1) DataAnnotations (엔티티/Value에 [Required], [MaxLength], IValidatableObject 등)
            var ctx = new ValidationContext(Value);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(Value, ctx, results, validateAllProperties: true);
            list.AddRange(results.Select(r => r.ErrorMessage ?? r.ToString()));

            // (2) 파생 클래스의 추가/비동기 규칙
            var extra = await ValidateCoreAsync(Value, ct);
            if (extra != null && extra.Count > 0) list.AddRange(extra);

            return list;
        }

        /// 커스텀/비동기 검증 훅(예: 중복 검사, 범위 비교 등)
        protected virtual Task<IReadOnlyList<string>> ValidateCoreAsync(T value, CancellationToken ct)
            => Task.FromResult((IReadOnlyList<string>)Array.Empty<string>());

        /// 기본 에러 표시(원하면 override해서 UI에 표시)
        protected virtual void OnValidationFailed(IReadOnlyList<string> errors)
            => MessageBox.Show(string.Join(Environment.NewLine, errors), "입력 오류",
                               MessageBoxButton.OK, MessageBoxImage.Warning);

        /// Ok/Cancel 버튼 활성화 갱신이 필요할 때 호출
        protected void InvalidateButtons()
        {
            (OkCommand as IRelayCommand)?.NotifyCanExecuteChanged();
            (CancelCommand as IRelayCommand)?.NotifyCanExecuteChanged();
        }
    }
}
