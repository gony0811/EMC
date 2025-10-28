using CommunityToolkit.Mvvm.ComponentModel;

namespace EMC
{
    public partial class SequenceItem : ObservableObject
    {
        [ObservableProperty]
        private bool isSuccess;

        [ObservableProperty]
        private string name;

        private string _duration = "00:00.00";  // 기본값 지정
        public string Duration
        {
            get => _duration;
            set
            {
                if (SetProperty(ref _duration, value))
                {
                    FormattedDuration = FormatDuration(value);
                }
            }
        }

        private string _formattedDuration = "00:00.00";  // 기본 표시 형식
        public string FormattedDuration
        {
            get => _formattedDuration;
            private set => SetProperty(ref _formattedDuration, value);
        }

        private string FormatDuration(string duration)
        {
            if (int.TryParse(duration, out int milliseconds))
            {
                int minutes = milliseconds / 60000;
                int seconds = (milliseconds % 60000) / 1000;
                int fraction = (milliseconds % 1000) / 10;
                return $"{minutes:D2}:{seconds:D2}.{fraction:D2}";
            }
            return "00:00.00";
        }

        public SequenceItem(string name)
        {
            IsSuccess = false;
            Name = name;
            Duration = "00:00.00"; 
        }
    }
}
