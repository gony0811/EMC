using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGGPLANT.Models
{
    public partial class ParameterModel : ObservableObject
    {
        [ObservableProperty] private string name = "";
        [ObservableProperty] private string value = "";
        [ObservableProperty] private string maximumValue = "";
        [ObservableProperty] private string minimumValue = "";
        [ObservableProperty] private string unit = "";
    }
}
