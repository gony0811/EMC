using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace EMC
{
    public partial class PowerPmacMotion : DMotion
    {                                            
        // MotorStatus
        [ObservableProperty] private bool sAmpEnabled = false;      // Servo Enable 상태  
        [ObservableProperty] private bool sClosedLoofMode = false;   // ClosedLoof 모드 상태 

        #region Jog 명령
        //public void JogMove(double JogSpeed = 1.0, JogMoveType moveType = JogMoveType.Stop)   // MoveType : Plus, Minus, Stop
        //{
        //    try
        //    {
        //        // 모터 상태 확인
        //        if (!SAmpEnabled) throw new Exception("Amp Disable");
                
        //        if (!SClosedLoofMode) throw new Exception("ClosedLoofMode를 On 해주세요");
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //    PowerPmacCommands.JogSpeed.Execute(ParentDevice.DeviceId, new NumValue(MotorNo, JogSpeed));
        //    switch(moveType)
        //    {
        //        case JogMoveType.Plus:
        //            PowerPmacCommands.JogPlus.Execute(ParentDevice.DeviceId, new MotorNumParam(MotorNo));
        //            break;
        //        case JogMoveType.Minus:
        //            PowerPmacCommands.JogMinus.Execute(ParentDevice.DeviceId, new MotorNumParam(MotorNo));
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null);
        //    }
        //}
        #endregion
        #region Home Process

        #endregion

        
    }

}