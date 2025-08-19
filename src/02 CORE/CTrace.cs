using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace EGGPLANT
{
    class CTraceBuffer
    {
        public bool IsBuffering { get; set; } = false;
        public ConcurrentQueue<string> StrList = new ConcurrentQueue<string>();
    }
    public class CTrace : IDisposable
    {
        private int FMemoLines = 100;
        private DispatcherTimer FTimer;
        private List<string> FTextBoxStrings = new List<string>();
        private ObservableCollection<string> FTextBox;
        protected string FClassName;
        protected string FTraceWriteDirectory;
        protected bool FDisposed = false;
        private bool FIsWriting = false;
        private List<string> FStrList;
        private UInt32 FMaxTraceBufferCount = 0;
        private CTraceBuffer[] FTraceBuffer;

        public UInt32 MaxTraceBufferCount { get { return FMaxTraceBufferCount; } }
        public string ClassName { get { return FClassName; } }
        public bool IsWriting { get { return FIsWriting; } }
        public string TraceWriteDirectory { get { return FTraceWriteDirectory; } }

        public UInt32 KeepDate = 0;
        public DateTime ToDay = DateTime.Now;

         


        public CTrace(string AClassName, string ADirectory = "", UInt32 AKeepDate = 30, UInt32 AMaxTraceBufferCount = 1) 
        {
            KeepDate = AKeepDate;
            FClassName = AClassName;

            FMaxTraceBufferCount = AMaxTraceBufferCount;
            FTraceBuffer = new CTraceBuffer[FMaxTraceBufferCount];
            for (int i = 0; i < FMaxTraceBufferCount; i++) FTraceBuffer[i] = new CTraceBuffer();

            if (ADirectory == "") ADirectory = "SYSTEM\\TRACE\\";

            ADirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ADirectory);

            FTraceWriteDirectory = ADirectory;

            if (!System.IO.Directory.Exists(FTraceWriteDirectory)) System.IO.Directory.CreateDirectory(FTraceWriteDirectory);

            ToDay = System.DateTime.Now;
            DirectoryLineUp();

            FStrList = new System.Collections.Generic.List<string>();
            FTimer = new DispatcherTimer();
            FTimer.Tick += OnTimer;
            FTimer.IsEnabled = true;
            FTimer.Interval = new TimeSpan(0, 0, 0, 0, 10); // 25ms
            FTimer.Start();
        }

        private void OnTimer(object? sender, EventArgs e)
        {
            OnExecute();
        }

        public void Dispose()
        {
            Dispose(true);
        }


        protected virtual void Dispose(bool ADisposing)
        {
            if (FDisposed) return;
            if (ADisposing)
            {
                // Dispose managed resources here
            }
            FTimer.Stop();
            FTimer.IsEnabled = false;
            FDisposed = true;
            // Free unmanaged resources here
        }


        private void DirectoryLineUp()
        {
            if (KeepDate <= 0) return;

            if (Directory.Exists(FTraceWriteDirectory))
            {
                string[] paths = Directory.GetDirectories(FTraceWriteDirectory, "*.*", SearchOption.AllDirectories);
                int count = paths.Length;

                for (int i = count - 1; i >= 1; i--)
                {
                    DateTime date = Directory.GetCreationTime(paths[i]);
                    TimeSpan span = ToDay - date;

                    if (span.TotalDays > KeepDate)
                    {
                        string[] files = Directory.GetFiles(paths[i] + "\\", "*.*", SearchOption.TopDirectoryOnly);

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


        /// <summary>
        /// GetDisplayString
        /// AString의 길이가 24자 이상일 경우, 10번째, 24번째, 36번째에 공백을 삽입하여 가독성을 높임(?)
        /// </summary>
        /// <param name="AString"></param>
        /// <returns></returns>
        private string GetDisplayString(string AString)
        {
            if (AString.Length < 24) return AString;


            string fstring = AString.Remove(10, 1).Insert(10, "  ");
            string sstring = fstring.Remove(24, 1).Insert(24, "  ");
            if (sstring.Length < 37) return sstring;

            string tstring = sstring.Remove(36, 1).Insert(36, "  ");
            return tstring;
        }

        private void OpenHistory()
        {
            string path = String.Format("{0}{1:D4}\\{2:D2}\\{3:D2}\\", FTraceWriteDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string file = path + String.Format("{0}.XLS", FClassName);
            if (File.Exists(file))
            {
                int cnt, idx;
                string[] lines;

                if (FTextBox != null)
                {
                    lines = File.ReadAllLines(file, System.Text.Encoding.Default);
                    FTextBoxStrings.Clear();

                    cnt = lines.Length;
                    idx = lines.Length - FMemoLines;

                    if (idx < 0) idx = 0;
                    for (int i = idx; i < cnt; i++) FTextBoxStrings.Insert(0, lines[i]);

                    foreach (string str in FTextBoxStrings)
                    {
                        FTextBox.Add(str + "\r\n");
                    }
                }
            }
        }
        private void SaveHistory(StringBuilder AText)
        {
            string path = String.Format("{0}{1:D4}\\{2:D2}\\{3:D2}\\", FTraceWriteDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string file = path + String.Format("{0}.XLS", FClassName);
            using (FileStream fs = new FileStream(file, FileMode.Append, FileAccess.Write))
            {
                byte[] text = System.Text.UnicodeEncoding.Default.GetBytes(AText.ToString());
                fs.Write(text, 0, text.Length);
            }
        }

        private void OnExecute()
        {
            TimeSpan span = ToDay - System.DateTime.Now;
            if (span.TotalDays > 1.0f)
            {
                ToDay = System.DateTime.Now;
                DirectoryLineUp();
            }

            if (!FIsWriting)
            {
                FIsWriting = true;
                for (int i = 0; i < FMaxTraceBufferCount; i++)
                {
                    if (FTraceBuffer[i].IsBuffering) continue;

                    string? message;
                    while (FTraceBuffer[i].StrList.TryDequeue(out message))   // Concurrent Queue에서 데이터를 데이터를 빼온다.
                    {
                        FStrList.Add(message);                                 // 리스트에 데이터를 추가한다.
                    }
                }

                if (FStrList.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string str in FStrList) sb.Append((str + "\r\n"));

                    SaveHistory(sb);

                    if (FTextBox != null)
                    {
                        foreach (string str in FStrList)
                        {
                            FTextBoxStrings.Insert(0, str);
                            FTextBox.Insert(0, str + "\r\n");
                            if (FMemoLines > 0)
                            {
                                if (FTextBoxStrings.Count > FMemoLines)
                                {
                                    int cnt = FTextBoxStrings.Count - 1000;
                                    for (int i = 0; i < cnt; i++) FTextBoxStrings.RemoveAt(FTextBoxStrings.Count - 1);
                                }
                            }
                        }

                        //string text = "";

                        //foreach (string str in FTextBoxStrings) text += (GetDisplayString(str) + "\r\n");
                        //FTextBox.Add(text);
                    }
                    FStrList.Clear();
                }
                FIsWriting = false;
            }
        }

        //private void TextBoxPreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        //{
        //    if (e.Control && e.KeyCode == System.Windows.Forms.Keys.C)
        //    {
        //        System.Windows.Forms.TextBox textbox = (System.Windows.Forms.TextBox)sender;
        //        System.Windows.Forms.Clipboard.SetDataObject(textbox.Text.Substring(textbox.SelectionStart, textbox.SelectionLength), true);
        //    }
        //}

        public void SetTextBox(ObservableCollection<string> ATextBox, int AMemoLine = 100)
        {
            if (AMemoLine < 0) AMemoLine = 0;

            FTextBox = ATextBox;
            FMemoLines = AMemoLine;
            if (FTextBox == null) return;

            if (FTextBoxStrings == null) FTextBoxStrings = new List<string>();
            OpenHistory();
        }

        public void Trace(string AString, UInt32 ABufferIndex = 0)
        {
            if (ABufferIndex >= FMaxTraceBufferCount) return;
            if (FTraceBuffer[ABufferIndex].IsBuffering) return;

            FTraceBuffer[ABufferIndex].IsBuffering = true;
            if (FTraceBuffer[ABufferIndex].StrList.Count < (10000 / FMaxTraceBufferCount))
            {
                FTraceBuffer[ABufferIndex].StrList.Enqueue(String.Format("{0}\t{1}\t{2}", new object[] { DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss.fff"), AString }));
            }
            FTraceBuffer[ABufferIndex].IsBuffering = false;
        }
        public void Trace(string ATitle, string AString, UInt32 ABufferIndex = 0)
        {
            Trace(string.Format("[{0}]\t", ATitle) + AString, ABufferIndex);
        }
    }
}
