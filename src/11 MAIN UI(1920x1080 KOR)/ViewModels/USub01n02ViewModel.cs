using CommunityToolkit.Mvvm.ComponentModel;
using EGGPLANT.Models;
using System.Collections.ObjectModel;

namespace EGGPLANT.ViewModels
{
    public partial class USub01n02ViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<MotorAxisViewModel> motorState = new ObservableCollection<MotorAxisViewModel>();

        public USub01n02ViewModel()
        {
            MotorState.Add(new MotorAxisViewModel("X"));
            MotorState.Add(new MotorAxisViewModel("Y"));
            MotorState.Add(new MotorAxisViewModel("Z"));
        }
    }
}
