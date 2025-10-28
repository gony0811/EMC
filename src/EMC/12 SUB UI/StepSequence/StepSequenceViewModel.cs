using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace EMC
{
    public partial class StepSequenceViewModel : ObservableObject
    {
        [ObservableProperty] private int stepIndex;

        public ObservableCollection<SequenceItem> SequenceList { get; } = new ObservableCollection<SequenceItem>();

        public StepSequenceViewModel(int stepIndex)
        {
            StepIndex = stepIndex;
        }
        public void AddSequenceItem(SequenceItem item)
        {
            SequenceList.Add(item);
        }
    }
}
