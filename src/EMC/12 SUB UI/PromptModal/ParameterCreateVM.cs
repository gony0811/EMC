
using EMC.DB;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace EMC
{
    public sealed class ParameterCreateVM : PromptDialogVM<RecipeParam>
    {
        public List<Unit> Units { get; }
        public List<ValueTypeDef> ValueTypes { get; }

        public ParameterCreateVM(List<Unit>? units, List<ValueTypeDef>? valueTypes,
                                 string? title = null, RecipeParam? initial = default)
            : base(initial ?? new RecipeParam(), title ?? "파라미터 생성")
        {
            Units = units ?? new List<Unit>();
            ValueTypes = valueTypes ?? new List<ValueTypeDef>();
        }

        protected override Task<IReadOnlyList<string>> ValidateCoreAsync(RecipeParam value, CancellationToken ct)
        {
            var errs = new List<string>();

            if (decimal.TryParse(value.Minimum, out var min) &&
                decimal.TryParse(value.Maximum, out var max) && min > max)
            {
                errs.Add("최소 값은 최대 값보다 클 수 없습니다.");
            }

            if (value.ValueTypeId <= 0 )
                errs.Add("값 타입을 선택하세요.");
            if (value.UnitId <= 0)
                errs.Add("단위를 선택하세요.");

            return Task.FromResult<IReadOnlyList<string>>(errs);
        }
    }

}
