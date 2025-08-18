using CommunityToolkit.Mvvm.ComponentModel;
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
        public USub01ViewModel() { }

        [ObservableProperty]
        private ObservableCollection<string> traceLog = new ObservableCollection<string>();
    }
}
