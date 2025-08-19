using CommunityToolkit.Mvvm.ComponentModel;
using EGGPLANT.Models;
using System.Collections.ObjectModel;

namespace EGGPLANT.ViewModels
{
    public partial class USubViewModel01n02 : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<MotorAxisViewModel> motorState = new ObservableCollection<MotorAxisViewModel>();

        public USubViewModel01n02()
        {
            MotorState.Add(new MotorAxisViewModel("X"));
            MotorState.Add(new MotorAxisViewModel("Y"));
            MotorState.Add(new MotorAxisViewModel("Z"));
        }
    }
}
