using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EPFramework.IoC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EMC
{
    [ViewModel(Lifetime.Transient)]
    public partial class USub08ViewModel : ObservableObject
    {
        private readonly PowerPmacManager PowerPmacManager;
        public ObservableCollection<PowerPmacDevice> PowerPMacList { get; }

        [ObservableProperty] private PowerPmacDevice selectedPMac;
        [ObservableProperty] private PowerPmacMotion selectedMotion;

        public USub08ViewModel(PowerPmacManager pmacManager)
        {
            this.PowerPmacManager = pmacManager;
            PowerPMacList =  pmacManager.DeviceList;
        }

        [RelayCommand]
        public async Task PowerPMacCreate()
        {
            
        }

        [RelayCommand]
        public void MotionCreate()
        {
            
        }

        [RelayCommand]
        public async Task MotionSave()
        {
            
        }

        [RelayCommand]
        public void ConnectPowerPMac()
        {
            bool result = SelectedPMac.Connect();
            if (!result)
            {
                MessageBox.Show($"연결 실패");
            }else
            {
                MessageBox.Show("연결 성공");
            }
        }
    }
}
