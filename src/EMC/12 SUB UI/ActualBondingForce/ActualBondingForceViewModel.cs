using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;

namespace EMC
{
    public partial class ActualBondingForceViewModel : ObservableObject
    {
        // === Chart Data ===
        public ChartValues<MeasureModel> ForceValues { get; set; }
        public SeriesCollection SeriesCollection { get; set; }

        // === Configurable Properties ===
        [ObservableProperty] private double axisMin;
        [ObservableProperty] private double axisMax;
        [ObservableProperty] private double windowSize;   // 표시 구간 (ms)
        [ObservableProperty] private double minForce;
        [ObservableProperty] private double maxForce;
        [ObservableProperty] private double timeStep;     // X축 눈금 간격 (ms)
        [ObservableProperty] private double forceStep;    // Y축 눈금 간격 (N)
        [ObservableProperty] private string title;

        // === Constructor ===
        public ActualBondingForceViewModel(
            double windowSizeMs = 100,   // 기본: 1초
            double timeStepMs = 10,       // X축 간격: 10ms
            double forceStep = 2,         // Y축 간격: 2N
            double minForce = 0,
            double maxForce = 12,
            string title = "Bonding Force")
        {
            // 사용자 설정 반영
            WindowSize = windowSizeMs;
            TimeStep = timeStepMs;
            ForceStep = forceStep;
            MinForce = minForce;
            MaxForce = maxForce;
            Title = title;

            // Chart 매핑: X=Time(ms), Y=Force(N)
            var mapper = Mappers.Xy<MeasureModel>()
                .X(m => m.Time)
                .Y(m => m.Force);
            Charting.For<MeasureModel>(mapper);

            ForceValues = new ChartValues<MeasureModel>();

            var forceSeries = new LineSeries
            {
                Title = "",
                Values = ForceValues,
                StrokeThickness = 2,
                PointGeometrySize = 0,
                LineSmoothness = 0,
                Fill = System.Windows.Media.Brushes.Transparent
            };

            SeriesCollection = new SeriesCollection { forceSeries };

            AxisMin = 0;
            AxisMax = windowSizeMs;
        }

        // === 외부에서 데이터 추가 ===
        public void AddDataPoint(double timeMs, double force)
        {
            App.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                ForceValues.Add(new MeasureModel
                {
                    Time = timeMs,
                    Force = force
                });

                while (ForceValues.Count > 0 &&
                       ForceValues[0].Time < timeMs - WindowSize)
                {
                    ForceValues.RemoveAt(0);
                }

                AxisMax = timeMs;
                AxisMin = Math.Max(0, AxisMax - WindowSize);
            }));
        }
    }

    public class MeasureModel
    {
        public double Time { get; set; }  // ms 단위 (외부 제공)
        public double Force { get; set; } // N 단위
    }
}
