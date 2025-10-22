using EMC.DB;
using System.Collections.ObjectModel;
using System.Linq;

namespace EMC
{
    public class MotionDto
    {
        public int Id { get; set; }         // 식별자 번호
        public string Name { get; set; }    // 모션 이름

        public MotionAxis Axis { get; set; }

        // 위치 정보 리스트
        public ObservableCollection<MotionPositionDto> MotionPositions { get; set; }

        // 모터 상태 리스트
        public MotorStateControlVM MotorStates;

        public static MotionDto FromEntity(Motion motion)
        {
            return new MotionDto
            {
                Id = motion.Id,
                Name = motion.Name,
                Axis = motion.Axis,
                MotionPositions = new ObservableCollection<MotionPositionDto>(
                    motion.Positions.Select(MotionPositionDto.FromEntity) ?? Enumerable.Empty<MotionPositionDto>()
                ),
                MotorStates = new MotorStateControlVM()
            };
        }
    }
}
