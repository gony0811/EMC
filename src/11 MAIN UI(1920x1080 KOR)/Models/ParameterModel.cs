using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGGPLANT._11_MAIN_UI_1920x1080_KOR_.Models
{
    public partial class ParameterModel : ObservableObject
    {
        [ObservableProperty] private string name = "";
        [ObservableProperty] private string value = "";
        [ObservableProperty] private string unit = "";
    }
}
