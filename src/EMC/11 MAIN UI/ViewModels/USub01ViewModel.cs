using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EPFramework.IoC;
using System.Windows.Controls;

namespace EMC
{
    [ViewModel(Lifetime.Singleton)]
    public partial class USub01ViewModel : ObservableObject
    {
        [ObservableProperty] public UserControl currentTab;
        [ObservableProperty] private string selectedTabKey = "LOADING";

        public USub01ViewModel()
        {
            SetTab(selectedTabKey);
        }

        [RelayCommand]
        public void SetTab(string viewName)
        {
            SelectedTabKey = viewName; 
            switch (viewName)
            {
                case "LOADING": CurrentTab = App.Container.Resolve<LoadingTab>(); break;
                case "AUTO": CurrentTab = App.Container.Resolve<AutoTab>(); break;     
                case "MANUAL": CurrentTab = App.Container.Resolve<ManualTab>(); break;
                // todo : 아래 탭 뷰 생성 필요
                case "STEP": CurrentTab = App.Container.Resolve<LoadingTab>(); break;     
                case "VISION": CurrentTab = App.Container.Resolve<LoadingTab>(); break;     
                case "CALIBRATION": CurrentTab = App.Container.Resolve<LoadingTab>(); break;     
            }
        }

    }
}
