using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace EGGPLANT._12_SUB_UI
{
    public partial class MoterStateControlVM : ObservableObject
    {
        [ObservableProperty] private string title = "";

        [ObservableProperty] ObservableCollection<StateCellVM> motorStates = new();
        
        [ObservableProperty] string ep = "0.0";

        [ObservableProperty] string cp = "0.0";

        public MoterStateControlVM() { }

        public MoterStateControlVM(ObservableCollection<StateCellVM> motorStates, string ep, string cp)
        {
            MotorStates = motorStates;
            Ep = ep;
            Cp = cp;
        }

        /** 
         * Command 명령 
        **/
        [RelayCommand]
        private void Servo()
        {
            // TODO: 서보 동작
        }

        [RelayCommand]
        private void Home()
        {
            // TODO: HOME 동작 
        }



    }
}
