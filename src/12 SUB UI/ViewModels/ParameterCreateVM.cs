using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace EGGPLANT
{
    public partial class ParameterCreateVM : ObservableObject
    {
        private readonly IRecipeService _svc;
        private readonly CommonData _common;
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
        public int RecipeId { get; }
        public List<UnitDto> Units { get; }
        public List<ValueTypeDto> ValueTypes { get; }
        public event Action<bool>? RequestClose; // true: OK, false: Cancel

        // 아이템들 
        [ObservableProperty] private string name = string.Empty;
        [ObservableProperty] private string value = string.Empty;
        [ObservableProperty] private string minimum = string.Empty;
        [ObservableProperty] private string maximum = string.Empty;

        [ObservableProperty] private UnitDto? selectedUnit;
        [ObservableProperty] private ValueTypeDto? selectedValueType;

        [ObservableProperty] private bool isActive = false;
        [ObservableProperty] private bool canSave = false;

        public ParameterCreateVM(IRecipeService svc, CommonData commonData, int recipeId)
        {
            _svc = svc;
            _common = commonData;
            RecipeId = recipeId;
            Units = _common.Units;
            ValueTypes = _common.Types;
        }

        public async Task InitAsync()
        {
            await _common.EnsureLoadedAsync();
            SelectedValueType ??= ValueTypes.FirstOrDefault();
            SelectedUnit ??= Units.FirstOrDefault();
        }

        [RelayCommand]
        public async Task CreateParameter()
        {
            // 1) 유효성 검사 + DTO 생성
            if (!TryBuildParameterDto(out var paramDto, out var err))
            {
                MessageBox.Show(err, "유효성 검사 실패",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2) 저장
            try
            {
                await _svc.CreateParamter(paramDto); 
                RequestClose?.Invoke(true);          
            }
            catch (Exception ex)
            {
                MessageBox.Show($"저장 중 오류가 발생했습니다.\n{ex.Message}",
                    "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        // 유효성 검사
        public bool TryBuildParameterDto(out RecipeParamDto dto, out string error)
        {
            dto = null!;
            var errors = new List<string>();

            // 1) 필수값 체크
            if (RecipeId <= 0) errors.Add("RecipeId가 유효하지 않습니다.");
            if (string.IsNullOrWhiteSpace(Name)) errors.Add("이름은 필수입니다.");
            if (string.IsNullOrWhiteSpace(Value)) errors.Add("값은 필수입니다.");
            if (SelectedValueType is null) errors.Add("값 타입을 선택하세요.");
            if (SelectedUnit is null) errors.Add("단위를 선택하세요."); // (DB는 NULL 허용이어도 규칙상 필수)

            if (errors.Count > 0)
            {
                error = string.Join("\n", errors);
                return false;
            }

            // 2) 타입별 유효성 검사 (INT / REAL / BOOL / TEXT 가정)
            var vt = SelectedValueType!.Name?.Trim().ToUpperInvariant();

            // 최소/최대가 입력되었다면 숫자로 파싱 시도 (REAL 기준)
            double? min = null, max = null;
            if (!string.IsNullOrWhiteSpace(Minimum))
            {
                if (double.TryParse(Minimum, NumberStyles.Float, Inv, out var d)) min = d;
                else errors.Add("최소 값이 숫자가 아닙니다.");
            }
            if (!string.IsNullOrWhiteSpace(Maximum))
            {
                if (double.TryParse(Maximum, NumberStyles.Float, Inv, out var d)) max = d;
                else errors.Add("최대 값이 숫자가 아닙니다.");
            }
            if (min.HasValue && max.HasValue && min > max)
                errors.Add("최소 값은 최대 값보다 클 수 없습니다.");

            // 값 파싱 및 범위 검사
            switch (vt)
            {
                case "INT":
                    {
                        if (!int.TryParse(Value, NumberStyles.Integer, Inv, out var v))
                            errors.Add("값은 정수여야 합니다.");

                        if (errors.Count == 0) // 파싱 성공 시 범위
                        {
                            if (min.HasValue && v < min) errors.Add($"값은 최소값 {min} 이상이어야 합니다.");
                            if (max.HasValue && v > max) errors.Add($"값은 최대값 {max} 이하여야 합니다.");
                        }
                        break;
                    }
                case "REAL":
                    {
                        if (!double.TryParse(Value, NumberStyles.Float, Inv, out var v))
                            errors.Add("값은 실수여야 합니다.");

                        if (errors.Count == 0)
                        {
                            if (min.HasValue && v < min) errors.Add($"값은 최소값 {min} 이상이어야 합니다.");
                            if (max.HasValue && v > max) errors.Add($"값은 최대값 {max} 이하여야 합니다.");
                        }
                        break;
                    }
                case "BOOL":
                    {
                        // true/false, 0/1 모두 허용
                        bool parsed = bool.TryParse(Value, out var b);
                        if (!parsed)
                        {
                            if (Value == "0")
                            {
                                b = false; parsed = true;
                            }
                            else if (Value == "1") {
                                b = true; 
                                parsed = true;
                            } 
                        }
                        if (!parsed) errors.Add("값은 true/false 또는 0/1 이어야 합니다.");
                        // 범위 개념 없음
                        break;
                    }
                case "TEXT":
                    {
                        // 텍스트는 추가 제약 없음. (min/max 입력돼 있어도 무시)
                        break;
                    }
                default:
                    errors.Add($"지원하지 않는 값 타입: {SelectedValueType!.Name}");
                    break;
            }

            if (errors.Count > 0)
            {
                error = string.Join("\n", errors);
                return false;
            }

            // 3) 통과 → DTO 작성
            dto = new RecipeParamDto
            {
                RecipeId = RecipeId,
                Name = Name.Trim(),
                Value = Value.Trim(),
                Minimum = string.IsNullOrWhiteSpace(Minimum) ? "" : Minimum.Trim(),
                Maximum = string.IsNullOrWhiteSpace(Maximum) ? "" : Maximum.Trim(),
                ValueType = SelectedValueType!.Name,  // 예: INT/REAL/BOOL/TEXT
                Unit = SelectedUnit!.Name        // 규칙상 필수
            };

            error = "";
            return true;
        }


    }
}
