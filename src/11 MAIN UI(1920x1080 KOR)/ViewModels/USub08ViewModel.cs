using CommunityToolkit.Mvvm.ComponentModel;
using EGGPLANT.Models;
using System.Collections.ObjectModel;

namespace EGGPLANT.ViewModels
{
    public partial class USub08ViewModel : ObservableObject 
    {
        [ObservableProperty]
        private ObservableCollection<SensorIoItemViewModel> inputIoItems = new ObservableCollection<SensorIoItemViewModel>();

        [ObservableProperty]
        private ObservableCollection<SensorIoItemViewModel> outputIoItems = new ObservableCollection<SensorIoItemViewModel>();

        public USub08ViewModel()
        {
            // Sensor Item 정리
            // 입력 신호 샘플  
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));
            InputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: true));


            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
            OutputIoItems.Add(new SensorIoItemViewModel("센서 G", isChecked: true, isReadOnly: false));
        }
    }
}
