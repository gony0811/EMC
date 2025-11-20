using CommunityToolkit.Mvvm.ComponentModel;
using EMC.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace EMC
{
    public partial class PowerPmacDevice : ObservableObject, IMotionDevice
    {
        [ObservableProperty] private uint id;
        [ObservableProperty] private string name;
        [ObservableProperty] private DeviceType deviceType;
        [ObservableProperty] private string fileName;
        [ObservableProperty] private bool isEnabled;
        [ObservableProperty] private string instanceName;
        [ObservableProperty] private bool isConnected;
        [ObservableProperty] private string description;
        [ObservableProperty] private string ip;
        [ObservableProperty] private int port;
        [ObservableProperty] private MotionDeviceType motionDeviceType;
        [ObservableProperty] public ObservableCollection<IMotion> motionList = new ObservableCollection<IMotion>();

        public Task Connect()
        {
            Byte[] byCommand;
            UInt32 uRet;
            uRet = DTKPowerPmac.Instance.Connect(Id);

            if ((DTK_STATUS)uRet == DTK_STATUS.DS_Ok)
            {
                byCommand = new Byte[255];
                byCommand = System.Text.Encoding.GetEncoding("euc-kr").GetBytes("echo 3");
                uRet = DTKPowerPmac.Instance.SendCommandA(Id, byCommand);
                IsConnected = true;
            }
            else
            {
                DTKPowerPmac.Instance.Close(Id);
                Id = int.MaxValue;
                IsConnected = false;
            }

            return Task.CompletedTask;
        }

        public Task Disconnect()
        {
            if (IsConnected)
            {
                DTKPowerPmac.Instance.IsConnected(Id, out int connected);

                if(connected == 1)
                    DTKPowerPmac.Instance.Disconnect(Id);
                DTKPowerPmac.Instance.Close(Id);
                Id = int.MaxValue;
                IsConnected = false;
            }

            return Task.CompletedTask;
        }

        public Task Initialize()
        {
            UInt32 uIPAddress;
            String[] strIP = new String[4];
            strIP = Ip.Split('.');
            uIPAddress = (Convert.ToUInt32(strIP[0]) << 24) | (Convert.ToUInt32(strIP[1]) << 16) | (Convert.ToUInt32(strIP[2]) << 8) | Convert.ToUInt32(strIP[3]);
            Id = DTKPowerPmac.Instance.Open(uIPAddress, (uint)DTK_MODE_TYPE.DM_GPASCII);
            

            return Task.CompletedTask;
        }

        public Task RefreshStatus()
        {
            foreach (var motion in MotionList)
            {
                // 여기서 각 모션의 상태를 갱신하는 로직을 구현해야 합니다.
                // 예: motion.CurrentPosition = GetCurrentPositionFromDevice(motion.Id);
                try
                {
                    String strCommand = "";
                    String strResponse = "";
                    string[] strResponseArry = new string[10];
                    uint intResponse = 0;

                    strCommand = "Motor[" + (motion.MotorNo).ToString() + "].Status[0]";         //모터 상태
                    strCommand += "Motor[" + (motion.MotorNo).ToString() + "].HomePos";          //home 위치  
                    strCommand += "Motor[" + (motion.MotorNo).ToString() + "].ActPos";           //encode 위치 
                    strCommand += "Motor[" + (motion.MotorNo).ToString() + "].DesPos";           //
                    strCommand += "Motor[" + (motion.MotorNo).ToString() + "].HomeComplete";     // Home Completed 
                    strResponse = PMacCommand(strCommand);
                    strResponseArry[0] = strResponse.Substring(0, strResponse.IndexOf("\r\n"));
                    strResponse = strResponse.Remove(0, strResponse.IndexOf("\r\n") + 2);
                    strResponseArry[1] = strResponse.Substring(0, strResponse.IndexOf("\r\n"));
                    strResponse = strResponse.Remove(0, strResponse.IndexOf("\r\n") + 2);
                    strResponseArry[2] = strResponse.Substring(0, strResponse.IndexOf("\r\n"));
                    strResponse = strResponse.Remove(0, strResponse.IndexOf("\r\n") + 2);
                    strResponseArry[3] = strResponse.Substring(0, strResponse.IndexOf("\r\n"));
                    strResponse = strResponse.Remove(0, strResponse.IndexOf("\r\n") + 2);

                    intResponse = Convert.ToUInt32(strResponseArry[0].Substring(1, strResponseArry[0].Length - 1), 16);  //Using ToUInt32 not ToUInt64, as per OP comment
                    double homepos = Convert.ToDouble(strResponseArry[1]) / motion.EncoderCountPerUnit;
                    double feedpos = Convert.ToDouble(strResponseArry[2]) / motion.EncoderCountPerUnit;
                    double commandpos = Convert.ToDouble(strResponseArry[3]) / motion.EncoderCountPerUnit;

                }
                catch (Exception ex)
                {
                    // 예외 처리 로직
                }

            }
        }

        public Task<bool> TestConnection()
        {
            throw new NotImplementedException();
        }

        public IMotion FindMotionById(int id)
        {
            throw new NotImplementedException();
        }

        public IMotion FindMotionByName(string name)
        {
            throw new NotImplementedException();
        }


        public void JogMove(int motionId, double jogSpeed, JogMoveType moveType)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> Home<TResult>()
        {
            throw new NotImplementedException();
        }

        public Task<TResult> Move<TResult>(int motionId, string commands, Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        private string PMacCommand(string command)
        {
            String strResponse = "";
            Byte[] byCommand;
            Byte[] byResponse;
            byCommand = new Byte[255];
            byResponse = new Byte[255];

            String stringcmd = command;

            byCommand = System.Text.Encoding.GetEncoding("euc-kr").GetBytes(stringcmd);
            DTKPowerPmac.Instance.GetResponseA((uint)Id, byCommand, byResponse, Convert.ToInt32(byResponse.Length - 1));
            strResponse = System.Text.Encoding.GetEncoding("euc-kr").GetString(byResponse);
            return strResponse;
        }
    }
}
