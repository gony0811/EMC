using CommunityToolkit.Mvvm.ComponentModel;
using EGGPLANT._13_DataStore;
using EGGPLANT.Models;

namespace EGGPLANT.ViewModels
{
    public partial class USub01n02ViewModel : ObservableObject
    {
        [ObservableProperty]
        private MotorStateStore motorState;

        [ObservableProperty]
        private double pitch;

        [ObservableProperty]
        private ManualOperationModel manualOperationModel = new ManualOperationModel();


        public USub01n02ViewModel()
        {

        }
    }
}
