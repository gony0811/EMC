using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EMC.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using static EMC.DTKPowerPmac64;

namespace EMC
{
    public partial class DPowerPmac : ObservableObject
    {
        #region 데이터
        public int Id { get; set; }

        [ObservableProperty] private uint deviceId = 0;

        [ObservableProperty] private string name;

        [ObservableProperty] private string ip;

        [ObservableProperty] private bool isConnected;

        [ObservableProperty] private DTK_MODE_TYPE dtkModeType = DTK_MODE_TYPE.DM_GPASCII;

        public ObservableCollection<DPowerPmacMotion> MotionList { get; set; } = new ObservableCollection<DPowerPmacMotion>();

        #endregion
        public DPowerPmac()
        {
        }


        public bool Connect()
        {
            if (IsConnected) return true;

            bool success = false;

            try
            {
                string[] strIP = Ip.Split('.');

                uint dwIPAddress = (Convert.ToUInt32(strIP[0]) << 24) | (Convert.ToUInt32(strIP[1]) << 16) | (Convert.ToUInt32(strIP[2]) << 8) | Convert.ToUInt32(strIP[3]);
                //uint dwIPAddress = IpToUInt32(Ip);
                uint uMode = (uint) DtkModeType;

                // 장치 오픈
                DeviceId = DTKPowerPmac.Instance.Open(dwIPAddress, uMode);
                //if (DeviceId == 0 || DeviceId == 0xFFFFFFFF)
                //    throw new Exception("[PowerPmac] 장치 오픈 실패");

                //  연결 시도
                uint result = DTKPowerPmac.Instance.Connect(DeviceId);
                if ((DTK_STATUS)result != DTK_STATUS.DS_Ok)
                    throw new Exception("[PowerPmac] 연결 실패");

                // 연결 테스트 (echo)
                byte[] cmd = Encoding.GetEncoding("euc-kr").GetBytes("echo 3");
                result = DTKPowerPmac.Instance.SendCommandA(DeviceId, cmd);
                if ((DTK_STATUS)result != DTK_STATUS.DS_Ok)
                    throw new Exception("[PowerPmac] 초기 명령 실패");

                success = true;
                Debug.WriteLine($"[PowerPmac] 연결 성공 - {Ip}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PowerPmac] 연결 오류: {ex.Message}");

                // 리소스 정리
                if (DeviceId != 0 && DeviceId != 0xFFFFFFFF)
                {
                    try { DTKPowerPmac.Instance.Close(DeviceId); } catch { }
                }
                DeviceId = 0xFFFFFFFF;
            }

            IsConnected = success;
            return success;
        }

        // 모션 데이터 폴링
        private void StartMotionPolling()
        {
            foreach (var motion in MotionList)
            {
                // ✅ 존재 여부 확인 후 폴링 시작
                //if (motion != null)
                //    motion.StartPolling(DeviceId);
            }
        }


        private UInt32 IpToUInt32(string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out IPAddress ip))
                throw new FormatException("Invalid IP address format.");

            byte[] bytes = ip.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return BitConverter.ToUInt32(bytes, 0);
        }
    }

    public static class PowerPMacFactory
    {
        /// <summary>
        /// Entity → DTO 변환 (하위 Motion 포함)
        /// </summary>
        public static DPowerPmac ToDto(this PowerPMac e)
        {
            if (e == null) return null;

            var dto = new DPowerPmac
            {
                Id = e.Id,
                Name = e.Name,
                Ip = e.Ip,
                MotionList = new ObservableCollection<DPowerPmacMotion>()
            };

            foreach (var motion in e.MotionList)
            {
                dto.MotionList.Add(motion.ToDto());
            }

            return dto;
        }

        /// <summary>
        /// DTO → Entity 변환 (하위 Motion 포함)
        /// </summary>
        public static PowerPMac ToEntity(this DPowerPmac d)
        {
            if (d == null) return null;

            var entity = new PowerPMac
            {
                Id = d.Id,
                Name = d.Name,
                Ip = d.Ip,
                MotionList = new List<PowerPMacMotion>()
            };

            foreach (var motion in d.MotionList)
            {
                var m = motion.ToEntity();
                m.PowerPMacId = d.Id;
                entity.MotionList.Add(m);
            }

            return entity;
        }
    }


}
