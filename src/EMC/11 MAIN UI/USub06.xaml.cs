
using Autofac;
using EPFramework.IoC;
using System.Windows.Controls;

namespace EMC
{
    [View(Lifetime.Singleton)]
    public partial class USub06 : Page
    {
        public USub06(USub07ViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            //var store = App.Container.Resolve<MotorStateStore>();
            //MotorState.DataContext = store.X;
        }
    }
}
