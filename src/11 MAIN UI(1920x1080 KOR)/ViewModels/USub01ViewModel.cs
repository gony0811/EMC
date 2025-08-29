using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EGGPLANT.Device.PowerPmac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGGPLANT.ViewModels
{
    public partial class USub01ViewModel : ObservableObject
    {
        private CTrace traceMsg = null;

        public USub01ViewModel() 
        {
            traceMsg = App.Container.ResolveKeyed<CTrace>("Trace");
            traceMsg.SetTextBox(TraceLog, 100);
        }

        [ObservableProperty]
        private ObservableCollection<string> traceLog = new ObservableCollection<string>();

        [RelayCommand]
        private void Initialize()
        {
            // Initialization logic can be added here if needed
            // For example, you might want to load initial data or set up the view model state
            traceMsg.Trace("USub01ViewModel", "Initialize called.");
            
        }
    }
}
