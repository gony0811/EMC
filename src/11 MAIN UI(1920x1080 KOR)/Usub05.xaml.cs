using EGGPLANT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace EGGPLANT
{
    /// <summary>
    /// Usub05.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Usub05 : Page
    {
        public Usub05()
        {
            this.DataContext = new Usub05ViewModel();
            InitializeComponent();
            
        }
    }
}
