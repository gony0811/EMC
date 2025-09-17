using Autofac;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml;

namespace EGGPLANT
{
    class CErrorItem
    {
        public CErrorItem(int AENo, string ADescription, DateTime AHappenTime, DateTime AFinishTime, ERROR_MODE AMode, ERROR_KIND AKind, RUN_MODE ARunStatus, int ARank, int AStep, int ADoneStatus)
        {
            ENo = AENo;
            DoneStatus = ADoneStatus;
            HappenTime = AHappenTime;
            FinishTime = AFinishTime;
            Description = ADescription;

            Rank = ARank;
            Step = AStep;
            Mode = AMode;
            Kind = AKind;
            RunStatus = ARunStatus;
        }

        public static ERROR_MODE ErrMode_Parse(string AMode)
        {
            ERROR_MODE mode = ERROR_MODE.ERROR;

            if (!Enum.TryParse<ERROR_MODE>(AMode, out mode)) return ERROR_MODE.ERROR;
            return mode;
        }
        public static ERROR_KIND ErrKind_Parse(string AKind)
        {
            ERROR_KIND kind = ERROR_KIND.MACHINE;

            if (!Enum.TryParse<ERROR_KIND>(AKind, out kind)) return ERROR_KIND.MACHINE;
            return kind;
        }
        public static RUN_MODE RunMode_Parse(string AMode)
        {
            RUN_MODE mode = RUN_MODE.STOP;

            if (!Enum.TryParse<RUN_MODE>(AMode, out mode)) return RUN_MODE.STOP;
            return mode;
        }

        public CErrorItem(int AENo, string ADescription, bool AHappen, ERROR_MODE AMode = ERROR_MODE.ERROR, ERROR_KIND AKind = ERROR_KIND.MACHINE, RUN_MODE ARunStatus = RUN_MODE.STOP, int ARank = 0, int AStep = 0)
        {
            ENo = AENo;
            DoneStatus = 0;
            FHappen = AHappen;
            Description = ADescription;
            HappenTime = System.DateTime.Now;

            Rank = ARank;
            Step = AStep;
            Mode = AMode;
            Kind = AKind;
            RunStatus = ARunStatus;
        }

        public int ENo;
        public int DoneStatus;
        public string Description;
        public DateTime HappenTime;
        public DateTime FinishTime;

        public int Rank;
        public int Step;
        public ERROR_MODE Mode;
        public ERROR_KIND Kind;
        public RUN_MODE RunStatus;

        private bool FHappen;
        public bool Happen { get { return FHappen; } }
    }
    public class CError : IDisposable
    {
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        private const int WM_USER = 0x400;

        public string Version
        {
            get { return "ERROR - Sean(V25.08.19.001)"; }
        }

        private UError ErrorWindow = null;

        public CError()
        {

        }

        public CError(string AClassName)
        {
            FClassName = AClassName;
            ErrorWindow = App.Container.Resolve<UError>();
            FConfigDirectory = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\CONFIG\\";
            FSystemDirectory = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\LOG\\ERROR LOG\\";           

            if (!System.IO.Directory.Exists(FConfigDirectory)) System.IO.Directory.CreateDirectory(FConfigDirectory);
            if (!System.IO.Directory.Exists(FSystemDirectory)) System.IO.Directory.CreateDirectory(FSystemDirectory);

            OpenParameter();
            FBuzzer[0] = new CBuzzer(__DO_BUZZER_01__);
            FBuzzer[1] = new CBuzzer(__DO_BUZZER_02__);
            FBuzzer[2] = new CBuzzer(__DO_BUZZER_03__);
            FBuzzer[3] = new CBuzzer(__DO_BUZZER_04__);
            FBuzzer[4] = new CBuzzer(__DO_BUZZER_05__);
            FBuzzer[5] = new CBuzzer(__DO_BUZZER_06__);
            FBuzzer[6] = new CBuzzer(__DO_BUZZER_07__);
            FBuzzer[7] = new CBuzzer(__DO_BUZZER_08__);
            FBuzzer[8] = new CBuzzer(__DO_BUZZER_09__);
            FBuzzer[9] = new CBuzzer(__DO_BUZZER_10__);

            FTimer = new DispatcherTimer();
            FTimer.Interval = TimeSpan.FromMilliseconds(50);
            FTimer.Tick += OnTimer;

            FThread = Task.Run(()=> { OnExecute(); });
            FThread.Start();
        }

        ~CError()
        {
            Dispose(false);
        }
        #region Dispose 구문
        protected bool FDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool ADisposing)
        {
            if (FDisposed) return;
            if (ADisposing) { /* IDisposable 인터페이스를 구현하는 멤버들을 여기서 정리합니다. */}

            if (FThread != null) FThread.Dispose();
            FDisposed = true;
        }
        #endregion

        public enum TASK_STYPE { NORMAL = 0, EMEGENCY = 10, };
        private async void TaskEProcess(TASK_STYPE AStyle, int AWParam, int ALParam, ERROR_MODE AMode)
        {
            var task = Task.Run(() =>
            {
                switch (AStyle)
                {
                    case TASK_STYPE.NORMAL:
                        ErrorWindow.Execute(AMode, AWParam, ALParam);
                        break;
                    case TASK_STYPE.EMEGENCY:
                        ErrorWindow.Execute(AMode, AWParam, ALParam);
                        break;
                }
            });
            await task;
        }
        private async void TaskMProcess(string ACaption, string AMessage, int AShowTime)
        {
            var task = Task.Run(() =>
            {
                //UConfirmFromError.Execute(ACaption, AMessage, MessageBoxButtons.OK, AShowTime);
            });
            await task;
        }

        #region 파라미터 저장 및 불러오기
        private string FClassName, FConfigDirectory, FSystemDirectory;
        public string ClassName { get { return FClassName; } }
        public string ConfigDirectory { get { return FConfigDirectory; } }
        public string SystemDirectory { get { return FSystemDirectory; } }

        public void SaveParameter()
        {
            XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

            XmlNode root = CXML.CreateElement(xdoc, "ROOT");
            xdoc.AppendChild(root);

            XmlNode basic = CXML.CreateElement(xdoc, "BASIC");
            CXML.AddElement(xdoc, basic, "VERSION", Version);
            root.AppendChild(basic);

            XmlNode option = CXML.CreateElement(xdoc, "OPTION");
            CXML.AddElement(xdoc, option, "KEEP_DATE", KeepDate);
            CXML.AddElement(xdoc, option, "CONTINUE_EVENT_IGNORE_TIME", FContinueEventIgnoreTime);
            root.AppendChild(option);

            XmlNode dio = xdoc.CreateElement("DIO");
            #region DIO
            XmlNode dioo = xdoc.CreateElement("OUTPUT");
            CXML.AddElement(xdoc, dioo, "BUZZER_01", __DO_BUZZER_01__);
            CXML.AddElement(xdoc, dioo, "BUZZER_02", __DO_BUZZER_02__);
            CXML.AddElement(xdoc, dioo, "BUZZER_03", __DO_BUZZER_03__);
            CXML.AddElement(xdoc, dioo, "BUZZER_04", __DO_BUZZER_04__);
            CXML.AddElement(xdoc, dioo, "BUZZER_05", __DO_BUZZER_05__);
            CXML.AddElement(xdoc, dioo, "BUZZER_06", __DO_BUZZER_06__);
            CXML.AddElement(xdoc, dioo, "BUZZER_07", __DO_BUZZER_07__);
            CXML.AddElement(xdoc, dioo, "BUZZER_08", __DO_BUZZER_08__);
            CXML.AddElement(xdoc, dioo, "BUZZER_09", __DO_BUZZER_09__);
            CXML.AddElement(xdoc, dioo, "BUZZER_10", __DO_BUZZER_10__);
            dio.AppendChild(dioo);
            #endregion
            root.AppendChild(dio);
            xdoc.Save($"{FConfigDirectory}{FClassName}.XML");
        }
        public void OpenParameter()
        {
            string file = $"{FConfigDirectory}{FClassName}.XML";
            if (!System.IO.File.Exists(file))
            {
                SaveParameter();
                return;
            }

            XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.Load(file);

            XmlNode basic = xdoc.SelectSingleNode($"/ROOT/BASIC");
            if (basic != null)
            {
                string version;
                CXML.GetInnerText(basic, "VERSION", out version);

                if (version != Version)
                {
                    System.Windows.Forms.MessageBox.Show($"버젼({Version} : {version})이 다릅니다.\r\n정보 불러오기에 문제가 발생될 수 있습니다.", "경고", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                }
            }

            XmlNode option = xdoc.SelectSingleNode($"/ROOT/OPTION");
            if (option != null)
            {
                CXML.GetInnerText(option, "KEEP_DATE", out KeepDate);
                CXML.GetInnerText(option, "CONTINUE_EVENT_IGNORE_TIME", out FContinueEventIgnoreTime);
            }

            XmlNode dioo = xdoc.SelectSingleNode($"/ROOT/DIO/OUTPUT");
            if (dioo != null)
            {
                CXML.GetInnerText(dioo, "BUZZER_01", out __DO_BUZZER_01__, -1);
                CXML.GetInnerText(dioo, "BUZZER_02", out __DO_BUZZER_02__, -1);
                CXML.GetInnerText(dioo, "BUZZER_03", out __DO_BUZZER_03__, -1);
                CXML.GetInnerText(dioo, "BUZZER_04", out __DO_BUZZER_04__, -1);
                CXML.GetInnerText(dioo, "BUZZER_05", out __DO_BUZZER_05__, -1);
                CXML.GetInnerText(dioo, "BUZZER_06", out __DO_BUZZER_06__, -1);
                CXML.GetInnerText(dioo, "BUZZER_07", out __DO_BUZZER_07__, -1);
                CXML.GetInnerText(dioo, "BUZZER_08", out __DO_BUZZER_08__, -1);
                CXML.GetInnerText(dioo, "BUZZER_09", out __DO_BUZZER_09__, -1);
                CXML.GetInnerText(dioo, "BUZZER_10", out __DO_BUZZER_10__, -1);
            }
        }
        #endregion

        #region 파일 정리
        public UInt32 KeepDate = 365;
        public System.DateTime ToDay = System.DateTime.Now;

        private Task FThread = null;
        private void OnExecute()
        {
            while (FThread != null)
            {
                if ((DateTime.Now - ToDay).Days > 0) { DirectoryLineUp(); ToDay = DateTime.Now; }
                OnExecuteTransaction();

                Thread.Sleep(50);
            }
        }

        private void DirectoryLineUp()
        {
            if (KeepDate <= 0) return;

            if (Directory.Exists(FSystemDirectory))
            {
                string[] paths = Directory.GetDirectories(FSystemDirectory, "*.*", SearchOption.AllDirectories);
                int count = paths.Length;

                for (int i = count - 1; i >= 1; i--)
                {
                    DateTime date = Directory.GetCreationTime(paths[i]);
                    TimeSpan span = ToDay - date;

                    if (span.TotalDays > KeepDate)
                    {
                        string[] files = Directory.GetFiles(FSystemDirectory, "*.*", SearchOption.TopDirectoryOnly);

                        bool existfiles = false;
                        foreach (string file in files)
                        {
                            date = File.GetCreationTime(file);
                            span = ToDay - date;

                            if (span.TotalDays > KeepDate) File.Delete(file);
                            else existfiles = true;
                        }

                        try
                        {
                            if (!existfiles) Directory.Delete(paths[i]);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }
        #endregion

        #region DB 처리 부분
        private System.Threading.ReaderWriterLockSlim FSQLiteLock = new System.Threading.ReaderWriterLockSlim();
        private System.Collections.Concurrent.ConcurrentQueue<CErrorItem> FErrItemList = new System.Collections.Concurrent.ConcurrentQueue<CErrorItem>();
        private void CreateSQLTable(string AFileName)
        {
            if (System.IO.File.Exists(AFileName)) return;

            string path = System.IO.Path.GetDirectoryName(AFileName);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            using (var connection = new SqliteConnection($"Data Source={AFileName}"))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        command.CommandText = $@"CREATE TABLE IF NOT EXISTS [ERROR_LOG] (
                                                 ERROR_INDEX INTEGER, 
                                                 ERROR_NO INTEGER, 
                                                 DESCRIPTION TEXT, 
                                                 HAPPEN_TIME DATETIME, 
                                                 FINISH_TIME DATETIME, 
                                                 RUN_STATUS TEXT, 
                                                 MODE TEXT, 
                                                 KIND TEXT, 
                                                 RANK INTEGER, 
                                                 STEP INTEGER,
                                                 DONE_STATUS INTEGER)";

                        var retval = command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
        }
        private void OnExecuteTransaction()
        {
            System.Collections.Generic.List<CErrorItem> list = new System.Collections.Generic.List<CErrorItem>();
            CErrorItem qitem;

            while (FErrItemList.TryDequeue(out qitem))
            {
                list.Add(qitem);
            }
            if (list.Count <= 0) return;

            string file = $"{FSystemDirectory}{DateTime.Now.Year:D4}\\{DateTime.Now.Month:D2}\\ERR-{DateTime.Now.Day:D2}.DB";
            CreateSQLTable(file);

            using (var connection = new SqliteConnection($"Data Source={file}"))
            {
                FSQLiteLock.EnterWriteLock();
                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    int retval = 0;
                    using (var transaction = connection.BeginTransaction())
                    {
                        foreach (var item in list)
                        {
                            if (item.Happen)
                            {
                                command.CommandText = "SELECT COUNT(*) FROM [ERROR_LOG]";
                                Int64 cnt = 0;
                                try
                                {
                                    cnt = (Int64)command.ExecuteScalar();
                                }
                                catch
                                {
                                    cnt = 0;
                                }

                                command.CommandText = $@"INSERT INTO [ERROR_LOG] (ERROR_INDEX, ERROR_NO, DESCRIPTION, HAPPEN_TIME, FINISH_TIME, RUN_STATUS, MODE, KIND, RANK, STEP, DONE_STATUS) 
                                                                              VALUES (@EIdx, @ENo, @Description, @HappenTime, @FinishTime, @RunStatus, @Mode, @Kind, @Rank, @Step, @DoneStatus)";

                                command.Parameters.AddWithValue("@EIdx", cnt + 1);
                                command.Parameters.AddWithValue("@ENo", item.ENo);
                                command.Parameters.AddWithValue("@Description", item.Description);
                                command.Parameters.AddWithValue("@HappenTime", item.HappenTime);
                                command.Parameters.AddWithValue("@FinishTime", item.HappenTime);
                                command.Parameters.AddWithValue("@RunStatus", item.RunStatus.ToString());
                                command.Parameters.AddWithValue("@Mode", item.Mode.ToString());
                                command.Parameters.AddWithValue("@Kind", item.Kind.ToString());
                                command.Parameters.AddWithValue("@Rank", item.Rank);
                                command.Parameters.AddWithValue("@Step", item.Step);
                                command.Parameters.AddWithValue("@DoneStatus", 0);
                            }
                            else
                            {
                                command.CommandText = $@"UPDATE [ERROR_LOG] SET FINISH_TIME = @FinishTime, DONE_STATUS = @DoneStatus WHERE ERROR_NO = @ENo AND DONE_STATUS = '0'";

                                command.Parameters.AddWithValue("@ENo", item.ENo);
                                command.Parameters.AddWithValue("@FinishTime", item.HappenTime);
                                command.Parameters.AddWithValue("@DoneStatus", 1);
                            }

                            try
                            {
                                retval = command.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine($"ERROR : {e.ToString()}");
                            }
                        }
                        transaction.Commit();
                    }
                }
                FSQLiteLock.ExitWriteLock();
            }
        }
        #endregion

        #region Buzzer
        private int FBuzzerInterval = 0;
        private DispatcherTimer FTimer = null;
        private void OnTimer(object? sender, EventArgs e)
        {
            if (++FBuzzerInterval > 100)
            {
                for (int i = 0; i < 10; i++) FBuzzer[i].OnExecute();
                FBuzzerInterval = 0;
            }
        }

        private enum BUZZER_MODE { OFF = 0, ON = 1, TIME_ON = 2, TOGGLE = 3, TOGGLE_COUNT = 4, }
        private class CBuzzer
        {
            public CBuzzer(int ABuzzerIndex)
            {
                FBuzzerIndex = ABuzzerIndex;
                
            }

            private int FTickCount = 0;
            private int FOnInterval = 0;
            private int FOfInterval = 0;
            private int FBuzzerIndex = 0;
            private int FToggleCount = 0;
            private int FSetToggleCount = 0;
            private BUZZER_MODE FBuzzerMode = BUZZER_MODE.OFF;
            private void SetBuzzer(bool AValue)
            {
                if (FBuzzerIndex < 0) return;
                //if (CMIT.PmacIO == null) return;
                //CMIT.PmacIO.Output[FBuzzerIndex] = AValue;
            }
            private bool IsOnBuzzer()
            {
                if (FBuzzerIndex < 0) return false;
                //if (CMIT.PmacIO == null) return false;
                //return CMIT.PmacIO.Output[FBuzzerIndex];
                return false;
            }
            public void OnExecute()
            {
                switch (FBuzzerMode)
                {
                    case BUZZER_MODE.OFF:
                        SetBuzzer(false);
                        FTickCount = 0;
                        break;
                    case BUZZER_MODE.ON:
                        SetBuzzer(true);
                        FTickCount = 0;
                        break;
                    case BUZZER_MODE.TIME_ON:
                        if (++FTickCount > FOnInterval)
                        {
                            FBuzzerMode = BUZZER_MODE.OFF;
                            SetBuzzer(false);
                            FTickCount = 0;
                        }
                        SetBuzzer(true);
                        break;
                    case BUZZER_MODE.TOGGLE:
                        if (IsOnBuzzer())
                        {
                            if (++FTickCount > FOnInterval)
                            {
                                SetBuzzer(false);
                                FTickCount = 0;
                            }
                        }
                        else
                        {
                            if (++FTickCount > FOfInterval)
                            {
                                SetBuzzer(true);
                                FTickCount = 0;
                            }
                        }
                        break;
                    case BUZZER_MODE.TOGGLE_COUNT:
                        if (IsOnBuzzer())
                        {
                            if (++FTickCount > FOnInterval)
                            {
                                if (++FToggleCount >= FSetToggleCount) FBuzzerMode = BUZZER_MODE.OFF;
                                SetBuzzer(false);
                                FTickCount = 0;
                            }
                        }
                        else
                        {
                            if (++FTickCount > FOfInterval)
                            {
                                SetBuzzer(true);
                                FTickCount = 0;
                            }
                        }
                        break;
                }
            }
            public void Buzzer(BUZZER_MODE ABuzzerMode, uint AOnInterval = 0, uint AOfInterval = 0, uint AToggleCount = 0)
            {
                AOnInterval /= 100;
                AOfInterval /= 100;

                if (AOnInterval < 0) AOnInterval = 0;
                if (AOfInterval < 0) AOfInterval = 0;

                FSetToggleCount = (int)AToggleCount;
                FOnInterval = (int)AOnInterval;
                FOfInterval = (int)AOfInterval;
                FBuzzerMode = ABuzzerMode;
                FToggleCount = 0;
                FTickCount = 0;

                switch (FBuzzerMode)
                {
                    case BUZZER_MODE.OFF:
                        SetBuzzer(false);
                        break;
                    case BUZZER_MODE.ON:
                        SetBuzzer(true);
                        break;
                    case BUZZER_MODE.TIME_ON:
                        SetBuzzer(true);
                        break;
                    case BUZZER_MODE.TOGGLE:
                        SetBuzzer(true);
                        break;
                    case BUZZER_MODE.TOGGLE_COUNT:
                        if (FToggleCount <= 0)
                        {
                            FBuzzerMode = BUZZER_MODE.OFF;
                            SetBuzzer(false);
                            break;
                        }
                        SetBuzzer(true);
                        break;
                }
            }
        }

        private int __DO_BUZZER_01__ = -1;
        private int __DO_BUZZER_02__ = -1;
        private int __DO_BUZZER_03__ = -1;
        private int __DO_BUZZER_04__ = -1;
        private int __DO_BUZZER_05__ = -1;
        private int __DO_BUZZER_06__ = -1;
        private int __DO_BUZZER_07__ = -1;
        private int __DO_BUZZER_08__ = -1;
        private int __DO_BUZZER_09__ = -1;
        private int __DO_BUZZER_10__ = -1;

        private CBuzzer[] FBuzzer = new CBuzzer[10] { null, null, null, null, null, null, null, null, null, null };

        public void Buzzer(int AIndex)
        {
            for (int i = 0; i < 10; i++)
            {
                FBuzzer[i].Buzzer((AIndex == i) ? BUZZER_MODE.ON : BUZZER_MODE.OFF);
            }
            FErrStatus &= ~STATUS.BUZZER;
        }
        public void BuzzerTimeOn(int AIndex, uint AOnInterval)
        {
            for (int i = 0; i < 10; i++)
            {
                if (AIndex == i)
                {
                    FBuzzer[i].Buzzer(BUZZER_MODE.TIME_ON, AOnInterval);
                }
            }
        }
        public void BuzzerToggle(int AIndex, uint AOnInterval, uint AOfInterval)
        {
            for (int i = 0; i < 10; i++)
            {
                if (AIndex == i)
                {
                    FBuzzer[i].Buzzer(BUZZER_MODE.TOGGLE, AOnInterval, AOfInterval);
                }
            }
        }
        public void BuzzerToggleCount(int AIndex, uint AOnInterval, uint AOfInterval, uint AToggleCount)
        {
            for (int i = 0; i < 10; i++)
            {
                if (AIndex == i)
                {
                    FBuzzer[i].Buzzer(BUZZER_MODE.TOGGLE, AOnInterval, AOfInterval, AToggleCount);
                }
            }
        }
#endregion

        #region 에러 처리
        public enum STATUS { NONE = 0x00, BUZZER = 0x01, WARNING = 0x10, ERROR = 0x20, }
        private STATUS FErrStatus = STATUS.NONE;
        public STATUS ErrStatus { get { return FErrStatus; } }

        private int FIndex = 0;
        private int FTickCount = 0;
        private int FPrevIndex = 0;
        private int FPrevHappenTime = 0;
        private int FContinueEventIgnoreTime = 0;
        private CErrorList FErrorList = App.Container.Resolve<CErrorList>();
        private CExecute FExecuteProc = App.Container.Resolve<CExecute>();


        public int Index
        {
            get { return FIndex; }
            set
            {
                if (FIndex == value) return;

                if (FErrorList != null && FExecuteProc != null) SetIndex(value, FErrorList.GetItem(value).Mode);
                else SetIndex(value, ERROR_MODE.UN_KNOWN);
            }
        }
        public void SetIndex(int AIndex, ERROR_MODE AMode)
        {
            if (FIndex == AIndex) return;

            int interval = Environment.TickCount - FTickCount;
            if (FIndex != 0)
            {
                if (interval < 10) return;
                if (AIndex == 0)
                {
                    if (FErrorList.GetItem(FIndex).Level > 0) return;
                }

                if (FErrorList != null && FExecuteProc != null)
                {
                    FErrItemList.Enqueue(new CErrorItem(FIndex, FErrorList.GetItem(FIndex).Title, false,
                                                                FErrorList.GetItem(FIndex).Mode,
                                                                FErrorList.GetItem(FIndex).Kind,
                                                                FExecuteProc.RunMode, 0, 0));
                }
            }
            FTickCount = Environment.TickCount;

            int rank = 0;
            if (FExecuteProc.RunMode == RUN_MODE.RUN)
            {
                if ((FPrevIndex != AIndex) || (Environment.TickCount - FPrevHappenTime) > FContinueEventIgnoreTime * 1000)
                {
                    if (AIndex != 0) rank = 1;
                }
                FPrevHappenTime = Environment.TickCount;
                FPrevIndex = FIndex;
            }

            FIndex = AIndex;
            if (FIndex != 0)
            {
                StepLogFileName = "";
                if (FErrorList != null && FExecuteProc != null)
                {
                    FErrItemList.Enqueue(new CErrorItem(FIndex, FErrorList.GetItem(FIndex).Title, true, AMode,
                                                                FErrorList.GetItem(FIndex).Kind,
                                                                FExecuteProc.RunMode, rank, 0));

                    if (AMode == ERROR_MODE.ERROR) FErrStatus = STATUS.BUZZER | STATUS.ERROR;
                    else FErrStatus = STATUS.BUZZER | STATUS.WARNING;

                    // sean.kim 2025-08-29
                    // Option 정보 처리
                    //f (CMIT.Struct != null && (CMIT.Struct.OptionBuzzerSkip == 0) && (CMIT.Struct.OptionGreenMode == 0)) Buzzer(FErrorList.GetItem(FIndex).Buzzer);
                }
            }
            else
            {
                FErrStatus = STATUS.NONE;
                Buzzer(-1);
            }

            TaskEProcess(TASK_STYPE.NORMAL, FIndex, FErrorList.GetItem(FIndex).Level, AMode);
            TaskEProcess(TASK_STYPE.EMEGENCY, 0, FErrorList.GetItem(FIndex).Level, AMode);
        }

        public void DisplayWindowShow(TASK_STYPE AStyle, int AWParam, int ALParam, ERROR_MODE AMode)
        {
            TaskEProcess(AStyle, AWParam, ALParam, AMode);
        }
        public void DisplayWindowHide()
        {
            TaskEProcess(TASK_STYPE.NORMAL, 0, 0, ERROR_MODE.UN_KNOWN);
            TaskEProcess(TASK_STYPE.EMEGENCY, 0, 0, ERROR_MODE.UN_KNOWN);
        }
        public void HappenToEmergency(int AIndex)
        {
            if (FIndex == AIndex) return;

            int interval = Environment.TickCount - FTickCount;
            if (FIndex != 0)
            {
                if (interval < 10) return;
                if (AIndex == 0)
                {
                    if (FErrorList.GetItem(FIndex).Level > 0) return;
                }

                if (FErrorList != null && FExecuteProc != null)
                {
                    FErrItemList.Enqueue(new CErrorItem(FIndex, FErrorList.GetItem(FIndex).Title, false,
                                                                FErrorList.GetItem(FIndex).Mode,
                                                                FErrorList.GetItem(FIndex).Kind,
                                                                FExecuteProc.RunMode, 0, 0));
                }
            }
            FTickCount = Environment.TickCount;

            int rank = 0;
            if (FExecuteProc != null && FExecuteProc.RunMode == RUN_MODE.RUN)
            {
                if ((FPrevIndex != AIndex) || (Environment.TickCount - FPrevHappenTime) > FContinueEventIgnoreTime * 1000)
                {
                    if (AIndex != 0) rank = 1;
                }
                FPrevHappenTime = Environment.TickCount;
                FPrevIndex = FIndex;
            }

            ERROR_MODE mode = ERROR_MODE.UN_KNOWN;
            FIndex = AIndex;
            if (FIndex != 0)
            {
                StepLogFileName = "";
                if (FErrorList != null && FExecuteProc != null)
                {
                    FErrItemList.Enqueue(new CErrorItem(FIndex, FErrorList.GetItem(FIndex).Title, true,
                                                                FErrorList.GetItem(FIndex).Mode,
                                                                FErrorList.GetItem(FIndex).Kind,
                                                                FExecuteProc.RunMode, rank, 0));

                    if (FErrorList.GetItem(FIndex).Mode == ERROR_MODE.ERROR) FErrStatus = STATUS.BUZZER | STATUS.ERROR;
                    else FErrStatus = STATUS.BUZZER | STATUS.WARNING;

                    mode = FErrorList.GetItem(FIndex).Mode;

                    // sean.kim 2025-08-29
                    // Option 정보 처리
                    //if (CMIT.Struct != null && (CMIT.Struct.OptionBuzzerSkip == 0) && (CMIT.Struct.OptionGreenMode == 0)) Buzzer(FErrorList.GetItem(FIndex).Buzzer);
                }
            }
            else
            {
                FErrStatus = STATUS.NONE;
                Buzzer(-1);
            }

            TaskEProcess(TASK_STYPE.NORMAL, 0, FErrorList.GetItem(FIndex).Level, mode);
            TaskEProcess(TASK_STYPE.EMEGENCY, FIndex, FErrorList.GetItem(FIndex).Level, mode);
        }
        public void ProcessError(int AIndex, int AStep)
        {
            if (FIndex == AIndex) return;

            if (FIndex != 0)
            {
                if (FErrorList != null && FExecuteProc != null)
                {
                    FErrItemList.Enqueue(new CErrorItem(FIndex, FErrorList.GetItem(FIndex).Title, false,
                                                                FErrorList.GetItem(FIndex).Mode,
                                                                FErrorList.GetItem(FIndex).Kind,
                                                                FExecuteProc.RunMode, 0, 0));
                }
            }

            int rank = 0;
            if (FExecuteProc != null && FExecuteProc.RunMode == RUN_MODE.RUN)
            {
                if ((FPrevIndex != AIndex) || (Environment.TickCount - FPrevHappenTime) > FContinueEventIgnoreTime * 1000)
                {
                    if (AIndex != 0) rank = 1;
                }
                FPrevHappenTime = Environment.TickCount;
                FPrevIndex = FIndex;
            }

            ERROR_MODE mode = ERROR_MODE.UN_KNOWN;
            FIndex = AIndex;
            if (FIndex != 0)
            {
                if (FErrorList != null && FExecuteProc != null)
                {
                    FErrItemList.Enqueue(new CErrorItem(FIndex, FErrorList.GetItem(FIndex).Title, true,
                                                                FErrorList.GetItem(FIndex).Mode,
                                                                FErrorList.GetItem(FIndex).Kind,
                                                                FExecuteProc.RunMode, rank, AStep));

                    if (FErrorList.GetItem(FIndex).Mode == ERROR_MODE.ERROR) FErrStatus = STATUS.BUZZER | STATUS.ERROR;
                    else FErrStatus = STATUS.BUZZER | STATUS.WARNING;

                    SaveStep();
                    mode = FErrorList.GetItem(FIndex).Mode;
                    // sean.kim 2025-08-29
                    // Option 정보 처리
                    //if (CMIT.Struct != null && (CMIT.Struct.OptionBuzzerSkip == 0) && (CMIT.Struct.OptionGreenMode == 0)) Buzzer(FErrorList.GetItem(FIndex).Buzzer);
                }
            }
            else
            {
                FErrStatus = STATUS.NONE;
                Buzzer(-1);
            }

            TaskEProcess(TASK_STYPE.NORMAL, FIndex, FErrorList.GetItem(FIndex).Level + AStep * 100, mode);
            TaskEProcess(TASK_STYPE.EMEGENCY, 0, FErrorList.GetItem(FIndex).Level + AStep * 100, mode);
        }

        public void ShowErrorMessageBox(string ACaption, string AMessage, int AShowTime = 10)
        {
            TaskMProcess(ACaption, AMessage, AShowTime);
        }
        #endregion

        #region 스텝 로그
        public string StepLogFileName = "";
        private void SaveStep()
        {
            if (FExecuteProc == null) return;

            StringBuilder sb = new StringBuilder();
            sb.Append($"STEP INDEX,{FExecuteProc.AStepIndex},SETP OVER FLOW,{FExecuteProc.LStepOverFlow},\r\n\r\n");

            uint ccnt = FExecuteProc.Count;
            for (uint c = 0; c < ccnt; c++)
            {
                sb.Append($"{FExecuteProc.GetItem(c).ClassName},");
            }

            for (uint r = 0; r < 10000; r++)
            {
                sb.Append("\r\n");
                for (uint c = 0; c < ccnt; c++)
                {
                    sb.Append($"{FExecuteProc.GetItem(c).Step:D4},");
                }
            }

            string path = $"{FSystemDirectory}{DateTime.Now.Year:D4}\\{DateTime.Now.Month:D2}\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            StepLogFileName = $"{path}E{DateTime.Now.Day:D2}{DateTime.Now.Hour:D2}{DateTime.Now.Minute:D2}{DateTime.Now.Second:D2}{DateTime.Now.Millisecond:D3}-{FIndex:D4}.INI";
            using (FileStream fs = new FileStream(StepLogFileName, FileMode.Append, FileAccess.Write))
            {
                fs.Write(System.Text.UnicodeEncoding.Default.GetBytes(sb.ToString()), 0, sb.Length);
            }
        }
        #endregion
    }
}
