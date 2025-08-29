
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EGGPLANT;
using EGGPLANT.Models;

namespace EGGPLANT.ViewModels
{
    public partial class USub09ViewModel : ObservableObject
    {
        [ObservableProperty]
        private PowerLampMode mode;

        [ObservableProperty]
        private LampState topPowerLamp;

        [ObservableProperty]
        private LampState middlePowerLamp;
        
        [ObservableProperty]
        private LampState bottomPowerLamp;

        public USub09ViewModel()
        {
            TopPowerLamp = LampState.Off;
            MiddlePowerLamp = LampState.Off;
            BottomPowerLamp = LampState.Off;
        }

        [RelayCommand()]
        private void TopPowerLampCycle() {
            TopPowerLamp = TopPowerLamp switch
            {
                LampState.Off => LampState.On,
                LampState.On => LampState.Blink,
                LampState.Blink => LampState.Off,
                _ => LampState.Off
            };
        }

        [RelayCommand()]
        private void MiddlePowerLampCycle()
        {
            MiddlePowerLamp = MiddlePowerLamp switch
            {
                LampState.Off => LampState.On,
                LampState.On => LampState.Blink,
                LampState.Blink => LampState.Off,
                _ => LampState.Off
            };
        }

        [RelayCommand()]
        private void BottomPowerLampCycle()
        {
            BottomPowerLamp = BottomPowerLamp switch
            {
                LampState.Off => LampState.On,
                LampState.On => LampState.Blink,
                LampState.Blink => LampState.Off,
                _ => LampState.Off
            };
        }

        [RelayCommand()]
        private void SetMode(PowerLampMode mode)
        {
            Mode = mode;
            switch (mode)
            {
                case PowerLampMode.STOP:
                    TopPowerLamp = LampState.Off;
                    MiddlePowerLamp = LampState.Off;
                    BottomPowerLamp = LampState.Off;
                    break;
                case PowerLampMode.INIT:
                    TopPowerLamp = LampState.Blink;
                    MiddlePowerLamp = LampState.Blink;
                    BottomPowerLamp = LampState.Blink;
                    break;
                case PowerLampMode.READY:
                    TopPowerLamp = LampState.Off;
                    MiddlePowerLamp = LampState.Off;
                    BottomPowerLamp = LampState.Off;
                    break;
                case PowerLampMode.START:
                    TopPowerLamp = LampState.Off;
                    MiddlePowerLamp = LampState.Off;
                    BottomPowerLamp = LampState.On;
                    break;

                case PowerLampMode.STOP_READY:
                    TopPowerLamp = LampState.Off;
                    MiddlePowerLamp = LampState.Off;
                    BottomPowerLamp = LampState.Off;
                    break;
                case PowerLampMode.ALARM_READY:
                    TopPowerLamp = LampState.Off;
                    MiddlePowerLamp = LampState.Off;
                    BottomPowerLamp = LampState.Off;
                    break;

                case PowerLampMode.ALARM:
                    TopPowerLamp = LampState.Blink;
                    MiddlePowerLamp = LampState.Blink;
                    BottomPowerLamp = LampState.Off;
                    break;
            }

        }
    }
}
