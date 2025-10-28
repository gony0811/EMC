using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace EMC
{
    public partial class DieTableViewModel : ObservableObject
    {

        [ObservableProperty] private int gridRows = 1;
        [ObservableProperty] private int gridColumns = 1;
        public ObservableCollection<DieViewModel> DieList { get; } = new ObservableCollection<DieViewModel>();
        public ObservableCollection<StepSequenceViewModel> StepSequenceList { get; }
        public DieTableViewModel(ObservableCollection<StepSequenceViewModel> stepSequenceList)
        {
            // 초기값 적용
            UpdateGridSize();

            // 변경 감지 등록
            DieList.CollectionChanged += OnDieListChanged;

            // 임시 가변 데이터
            DieList.Add(new DieViewModel("#1", 0, 0, false, false));
            DieList.Add(new DieViewModel("#2", 0, 0, false, false));
            DieList.Add(new DieViewModel("#3", 0, 0, false, false));
            DieList.Add(new DieViewModel("#4", 0, 0, false, false));
            DieList.Add(new DieViewModel("#5", 0, 0, false, false));
            DieList.Add(new DieViewModel("#6", 0, 0, false, false));
            DieList.Add(new DieViewModel("#7", 0, 0, false, false));
            DieList.Add(new DieViewModel("#8", 0, 0, false, false));
            DieList.Add(new DieViewModel("#9", 0, 0, false, false));

            StepSequenceList = stepSequenceList;
        }

        private void OnDieListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateGridSize();
        }

        private void UpdateGridSize()
        {
            GridRows = GridColumns = Math.Max(1, (int)Math.Ceiling(Math.Sqrt(DieList.Count)));
        }
    }
}
