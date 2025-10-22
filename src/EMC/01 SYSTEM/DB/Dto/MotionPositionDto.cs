using CommunityToolkit.Mvvm.ComponentModel;
using EMC.DB;

namespace EMC
{
    public partial class MotionPositionDto : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private RangeValue location;

        [ObservableProperty]
        private RangeValue speed;


        public static MotionPositionDto FromEntity(MotionPosition motionPosition)
        {
            return new MotionPositionDto
            {
                Id = motionPosition.Id,
                Name = motionPosition.Name,
                Location = new RangeValue(motionPosition.CurrentLocation, motionPosition.MinimumLocation, motionPosition.MaximumLocation),
                Speed = new RangeValue(motionPosition.CurrentSpeed, motionPosition.MinimumSpeed,  motionPosition.MaximumSpeed)
            };
        }
    }
}
