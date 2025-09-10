
using System.Xml.Linq;

namespace EGGPLANT
{
    // 공통으로 사용될 데이터 리스트
    public class CommonData
    {
        private bool _loaded;
        private readonly ICommonService commonService;

        public List<UnitDto> Units = new();

        public List<ValueTypeDto> Types = new();
        public CommonData(ICommonService service)
        {
            commonService = service;
        }

        public async Task EnsureLoadedAsync()
        {
            if (_loaded) return;

            Units.Clear();
            foreach (var u in await commonService.GetUnits()) Units.Add(u);

            Types.Clear();
            foreach (var t in await commonService.GetValueTypes()) Types.Add(t);

            _loaded = true;
        }

        public UnitDto? FindUnit(string name)
        {
            foreach (UnitDto dto in Units)
            {
                if (dto.Name.Equals(name))
                {
                    return dto;
                }
            }
            return null;
        }

        public ValueTypeDto? FindValueType(string name)
        {
            foreach(ValueTypeDto dto in Types)
            {
                if (dto.Name.Equals(name))
                {
                    return dto;
                }
            }
            return null;
        }
    }
}
