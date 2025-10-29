
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EPFramework.IoC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace EMC
{

    [ViewModel(Lifetime.Scoped)]
    public partial class StepTabViewModel : ObservableObject
    {
        // 테스트용 데이터 저장소 (임시)
        private readonly Dictionary<string, int> _axisValues = new Dictionary<string, int>();
        public TableOffset WTableOffset { get; } = new TableOffset("W-Table");
        public TableOffset DTableOffset { get; } = new TableOffset("D-Table");
        public TableOffset PTableOffset { get; } = new TableOffset("P-Table");
        public TableOffset BHeadOffset { get; } = new TableOffset("B-Head");

        public StepSequenceViewModel Step1 { get; } = new StepSequenceViewModel(1);
        public StepSequenceViewModel Step2 { get; } = new StepSequenceViewModel(2);
        public StepSequenceViewModel Step3 { get; } = new StepSequenceViewModel(3);
        public StepSequenceViewModel Step4 { get; } = new StepSequenceViewModel(4); 

        public ObservableCollection<StepSequenceViewModel> DStepList { get; }
        public DieTableViewModel DieTable { get; }

        public ObservableCollection<ParameterCellViewModel> BondingParameterList { get; } = new ObservableCollection<ParameterCellViewModel>();
        public ObservableCollection<ParameterCellViewModel> BondHeadParameterList { get; } = new ObservableCollection<ParameterCellViewModel>();

        [ObservableProperty] private ActualBondingForceViewModel actualBondingForce;

        public StepTabViewModel()
        {
            _axisValues["Y"] = 10;
            _axisValues["D"] = 20;
            _axisValues["X"] = 30;
            _axisValues["Z1"] = 40;
            _axisValues["Z2"] = 50;

            // === 임시 연결 ===
            WTableOffset.Add(new TableAxisOffset("Y", () => _axisValues["Y"], a => _axisValues["Y"] = a, 0, 100));
            WTableOffset.Add(new TableAxisOffset("D", () => _axisValues["D"], a => _axisValues["D"] = a, 0, 100));

            DTableOffset.Add(new TableAxisOffset("Y", () => _axisValues["Y"], a => _axisValues["Y"] = a, 0, 100));
            PTableOffset.Add(new TableAxisOffset("Y", () => _axisValues["Y"], a => _axisValues["Y"] = a, 0, 100));

            BHeadOffset.Add(new TableAxisOffset("X", () => _axisValues["X"], a => _axisValues["X"] = a, 0, 100));
            BHeadOffset.Add(new TableAxisOffset("D", () => _axisValues["D"], a => _axisValues["D"] = a, 0, 100));
            BHeadOffset.Add(new TableAxisOffset("Z1", () => _axisValues["Z1"], a => _axisValues["Z1"] = a, 0, 100));
            BHeadOffset.Add(new TableAxisOffset("Z2", () => _axisValues["Z2"], a => _axisValues["Z2"] = a, 0, 100));


            // STEP 1 _ SquenceList
            Step1.AddSequenceItem(new SequenceItem("Wafer Center로 이동"));
            Step1.AddSequenceItem(new SequenceItem("Wafer Macro θ Align"));
            Step1.AddSequenceItem(new SequenceItem("Wafer Micro θ Align"));
            Step1.AddSequenceItem(new SequenceItem("Wafer Macro 5 Point Align"));
            Step1.AddSequenceItem(new SequenceItem("Piezo 위치 조정"));


            Step2.AddSequenceItem(new SequenceItem("Carrier Reading"));
            Step2.AddSequenceItem(new SequenceItem("Die Macro θ Align"));
            Step2.AddSequenceItem(new SequenceItem("Die Micro θ Align"));
            Step2.AddSequenceItem(new SequenceItem("Piezo 위치 조정"));
            Step2.AddSequenceItem(new SequenceItem("Die Pick Up"));

            Step3.AddSequenceItem(new SequenceItem("Fiducial Mark Macro 인식(L)"));
            Step3.AddSequenceItem(new SequenceItem("Fiducial Mark Micro 인식(L)"));
            Step3.AddSequenceItem(new SequenceItem("Fiducial Mark Macro 인식(R)"));
            Step3.AddSequenceItem(new SequenceItem("Fiducial Mark Micro 인식(R)"));
            Step3.AddSequenceItem(new SequenceItem("Fiducial Mark ㅁ 계산"));
            Step3.AddSequenceItem(new SequenceItem("Die Mark Macro 인식(L)"));
            Step3.AddSequenceItem(new SequenceItem("Die Mark Micro 인식(L)"));
            Step3.AddSequenceItem(new SequenceItem("Die Mark Macro 인식(R)"));
            Step3.AddSequenceItem(new SequenceItem("Die Mark Micro 인식(R)"));
            Step3.AddSequenceItem(new SequenceItem("Die Mark ㅁ 계산"));

            Step4.AddSequenceItem(new SequenceItem("Head Mark, Wafer Logic Mark 인식"));
            Step4.AddSequenceItem(new SequenceItem("Die Align θ 확인"));
            Step4.AddSequenceItem(new SequenceItem("Piezo 위치 조정"));
            Step4.AddSequenceItem(new SequenceItem("Bonding(가압)"));

            DStepList = new ObservableCollection<StepSequenceViewModel>
            {
                Step2,
                Step3
            };

            DieTable = new DieTableViewModel(DStepList);

            BondingParameterList.Add(new ParameterCellViewModel(1, "Pick Up force", 20, true, "Kgf", ValueType.Integer, ""));
            BondingParameterList.Add(new ParameterCellViewModel(2, "Pick Up time", 100, true, "msec", ValueType.Integer, ""));
            BondingParameterList.Add(new ParameterCellViewModel(3, "Bonding force", 20, true, "Kgf", ValueType.Integer, ""));
            BondingParameterList.Add(new ParameterCellViewModel(4, "Bonding time", 100, true, "msec", ValueType.Integer, ""));
            BondingParameterList.Add(new ParameterCellViewModel(5, "Bonding accuracy", 500, true, "nm", ValueType.Integer, ""));
            BondingParameterList.Add(new ParameterCellViewModel(6, "Tact time", 500, true, "UPH", ValueType.Integer, ""));
            BondingParameterList.Add(new ParameterCellViewModel(7, "ISO-S", 20, true, "", ValueType.String, ""));

            BondHeadParameterList.Add(new ParameterCellViewModel(1, "Head to W-Table 평탄도 setting", 20, true, "Kgf", ValueType.Integer, ""));
            BondHeadParameterList.Add(new ParameterCellViewModel(2, "Base bon level", 0, true, "msec", ValueType.Integer, ""));
            BondHeadParameterList.Add(new ParameterCellViewModel(3, "Search level", null, false, "", ValueType.Integer, "사용자 입력값"));
            BondHeadParameterList.Add(new ParameterCellViewModel(4, "Search speed", null, false, "", ValueType.Integer, "사용자 입력값"));
            BondHeadParameterList.Add(new ParameterCellViewModel(5, "Search force", null, false, "", ValueType.Integer, "사용자 입력값"));
            BondHeadParameterList.Add(new ParameterCellViewModel(6, "Dwell time", null, false, "", ValueType.Integer, "사용자 입력값"));
            BondHeadParameterList.Add(new ParameterCellViewModel(7, "Self alignment calibration", 0, true, "", ValueType.Integer, "Frequency 입력"));
            BondHeadParameterList.Add(new ParameterCellViewModel(7, "Collet life time", 0, true, "", ValueType.Integer, "Bonding 횟수 입력"));
            BondHeadParameterList.Add(new ParameterCellViewModel(7, "Piezo Actuator z축 이동 값", 0, true, "", ValueType.Integer, "z축 이동값 입력 [nm]"));
            ActualBondingForce = new ActualBondingForceViewModel(
                title: "Actual Bonding Force"     // 차트 제목
            );

        }



        [RelayCommand]
        public async Task Test()
        {
            // 그래프 초기화
            ActualBondingForce.ForceValues.Clear();
            ActualBondingForce.AxisMin = 0;
            ActualBondingForce.AxisMax = ActualBondingForce.WindowSize;

            double time = 0;
            double duration = 10_000; // 10초 (ms)
            double step = 10;          // 10ms 간격

            var rand = new Random();

            // 10초 동안 데이터 추가
            while (time < duration)
            {
                // Force = 난수 or sin파형 테스트
                double force = rand.NextDouble() * 12;

                ActualBondingForce.AddDataPoint(time, force);

                await Task.Delay((int)step);
                time += step;
            }
        }
    }
    public partial class TableOffset : ObservableObject
    {
        public string Name { get; set; }

        public ObservableCollection<TableAxisOffset> AxisOffsetList { get; } = new ObservableCollection<TableAxisOffset>();


        public TableOffset(string name)
        {
            Name = name;
        }

        public void Add(TableAxisOffset tableAxisOffset)
        {
            AxisOffsetList.Add(tableAxisOffset);
        }
    }

    public partial class TableAxisOffset : ObservableObject
    {
        public string OffsetAxis { get; private set; }

        [ObservableProperty] private int minimum;
        [ObservableProperty] private int maximum;

        private readonly Func<int> _getter;
        private readonly Action<int> _setter;
        public int Value
        {
            get => _getter();
            set
            {
                _setter(value);
                OnPropertyChanged(nameof(Value));
            }
        }

        public TableAxisOffset(string offsetAxis, Func<int> getter, Action<int> setter, int minimum, int maximum)
        {
            OffsetAxis = offsetAxis;
            _getter = getter;
            _setter = setter;
            Minimum = minimum;
            Maximum = maximum;
        }
    }

}



