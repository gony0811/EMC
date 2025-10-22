using EPFramework.IoC;
using System;
using System.Windows.Controls;

namespace EMC
{

    [View(Lifetime.Singleton)]
    public partial class LoadingTab : UserControl
    {

        public LoadingTab(TableManagerViewModel tableManagerViewModel)
        {
            InitializeComponent();
            DataContext = tableManagerViewModel;
        }


    }
}
