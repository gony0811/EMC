using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Brushes = System.Windows.Media.Brushes;

namespace EGGPLANT
{
    /// <summary>
    /// UError.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UError : Window
    {
        private CError FError = App.Container.Resolve<CError>();
        private CErrorList FErrorList = App.Container.Resolve<CErrorList>();
        private int FEIndex = 0;
        private int FLEStep = 0;
        private int FLEIndex = 0;
        private ERROR_MODE FEMode = ERROR_MODE.UN_KNOWN;
        private DispatcherTimer FTimer = null;
        private int FTM500ms = 0;
        public UError()
        {
            InitializeComponent();
            Width = 489;
            FTimer = new DispatcherTimer();
            FTimer.Interval = TimeSpan.FromMilliseconds(50);
            FTimer.Tick += FTimer_Tick;
        }

        private void FTimer_Tick(object? sender, EventArgs e)
        {
            Display();
        }

        public void Display()
        {
            if (++FTM500ms >= 10)
            {
                if(Label_ErrorTitle.Foreground == Brushes.Red) Label_ErrorTitle.Foreground = Brushes.Black;
                else Label_ErrorTitle.Foreground = Brushes.Red;
                FTM500ms = 0;
            }


        }

        public void Update()
        {
            if (FError != null && FErrorList != null)
            {
                switch (FEMode)
                {
                    case ERROR_MODE.WARNING:
                        ErrorIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.AlertOutline;
                        ErrorIcon.Foreground = Brushes.Yellow;
                        break;
                    case ERROR_MODE.ERROR:
                        ErrorIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.AlertRemoveOutline;
                        ErrorIcon.Foreground = Brushes.Red;
                        break;
                    default:
                        ErrorIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.HelpBoxOutline;
                        ErrorIcon.Foreground = Brushes.Gray;
                        break;
                }
            }
        }

        public void Execute(ERROR_MODE AMode, int AIndex, int AStep = 0)
        {
            Update();
            this.ShowDialog();
        }
    }
}
