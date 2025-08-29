using CommunityToolkit.Mvvm.ComponentModel;
using EGGPLANT;
using EGGPLANT.Models;

namespace EGGPLANT.ViewModels
{
    public partial class USubViewModel01n02 : ObservableObject
    {
        [ObservableProperty]
        private MotorStateStore motorState;

        [ObservableProperty]
        private double pitch;

        [ObservableProperty]
        private ManualOperationModel manualOperationModel = new ManualOperationModel();


        public USubViewModel01n02()
        {

        }
    }
}
