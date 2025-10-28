using CommunityToolkit.Mvvm.ComponentModel;

namespace EMC
{
    public partial class DieViewModel : ObservableObject
    {
        [ObservableProperty] private string dieName;

        [ObservableProperty] private double x;
        [ObservableProperty] private double y;

        [ObservableProperty] private bool vacOnOff;
        [ObservableProperty] private bool airOnOff;

        public DieViewModel(string dieName, double x, double y, bool vacOnOff, bool airOnOff)
        {
            DieName = dieName;
            X = x;
            Y = y;
            VacOnOff = vacOnOff;
            AirOnOff = airOnOff;
        }
    }
}
