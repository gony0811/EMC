using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EPFramework.IoC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EMC
{
    [ViewModel(Lifetime.Transient)]
    public partial class USub08ViewModel : ObservableObject
    {
        private readonly PowerPmacManager PMacManager;
        public ObservableCollection<DPowerPmac> PowerPMacList { get; }

        [ObservableProperty] private DPowerPmac selectedPMac;
        [ObservableProperty] private DPowerPmacMotion selectedMotion;

        public USub08ViewModel(PowerPmacManager pmacManager)
        {
            this.PMacManager = pmacManager;
            PowerPMacList =  pmacManager.PowerPMacList;
        }

        [RelayCommand]
        public async Task PowerPMacCreate()
        {
            var vm = new PowerPMacCreateViewModel();
            var view = new PowerPMacCreateModal { DataContext = vm, Owner = Application.Current.MainWindow };

            if (view.ShowDialog() == true && vm.Result != null)
            {
                DPowerPmac newDto = vm.Result;
                try
                {
                    await PMacManager.Create(newDto);
                    MessageBox.Show("PowerPMac 생성 완료");
                }catch(Exception e)
                {
                    MessageBox.Show("저장 오류");
                }
            }
        }

        [RelayCommand]
        public void MotionCreate()
        {
            if (SelectedPMac != null)
            {
                var motion = new DPowerPmacMotion();
                motion.PowerPMacId = SelectedPMac.Id;
                motion.IsSaveFailed = true;
                SelectedPMac.MotionList.Add(motion);
            }
            else
            {
                MessageBox.Show("디바이스를 먼저 선택해주세요");
            }
        }

        [RelayCommand]
        public async Task MotionSave()
        {
            if (SelectedPMac == null)
            {
                MessageBox.Show("저장할 디바이스를 선택해주세요");
                return;
            }

            int successCount = 0;
            int failCount = 0;
            var failedList = new List<string>();

            foreach (var motion in SelectedPMac.MotionList)
            {
                if (motion.IsSaveFailed)
                {
                    try
                    {
                        await PMacManager.MotionSave(motion);
                        motion.IsSaveFailed = false;
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        motion.IsSaveFailed = true;
                        failCount++;
                        failedList.Add($"{motion.MotorNo} ({motion.Name}) : {ex.Message}");
                    }
                }
            }

            // 전체 요약 결과 출력
            if (failCount == 0)
            {
                MessageBox.Show($"✅ 모든 Motion이 성공적으로 저장되었습니다.");
            }
            else
            {
                string msg =
                    $"⚠️ 저장 완료 (성공 {successCount}개 / 실패 {failCount}개)\n\n" +
                    string.Join("\n", failedList.Take(5)); 
                MessageBox.Show(msg, "저장 결과", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        [RelayCommand]
        public void ConnectPowerPMac()
        {
            bool result = SelectedPMac.Connect();
            if (!result)
            {
                MessageBox.Show($"연결 실패");
            }else
            {
                MessageBox.Show("연결 성공");
            }
        }
    }
}
