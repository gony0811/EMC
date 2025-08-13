using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
#pragma warning disable CS8603 // null 가능 참조에 대한 역참조입니다.
#pragma warning disable CS8625 // null 가능 참조에 대한 역참조입니다.
namespace EGGPLANT
{
    public partial class UDevHistory : Form
    {
        public UDevHistory()
        {
            InitializeComponent();
            dataGridView.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void UDevHistory_Shown(object sender, EventArgs e)
        {
            Init();
        }

        private void UDevHistory_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private CDevHistory FDevHistory = new CDevHistory();
        public void Init()
        {
            int cnt = FDevHistory.Count;
            dataGridView.RowCount = cnt;
            for (int i = 0; i < cnt; i++)
            {
                CDevHistoryItem item = FDevHistory[i];
                dataGridView.Rows[i].Cells[0].Value = item.No;
                dataGridView.Rows[i].Cells[1].Value = item.Message;
                dataGridView.Rows[i].Cells[2].Value = item.Type.ToString();
                dataGridView.Rows[i].Cells[3].Value = item.WorkTime.ToString("00.00");
                dataGridView.Rows[i].Cells[4].Value = item.Developer;
                dataGridView.Rows[i].Cells[5].Value = item.Requester;
            }

            Update();
        }
        public new void Update()
        {
            Display();
        }
        public void Display()
        {
        }
    }

    public enum HISTORY_TYPE { SP = 0 /* SP 프로젝트 시작(Starting Project) */, AC = 1 /* AC 추가 비용 (Additional Cost) */, AS = 2 /* AS 추가 기능 (Additional Service)*/, };
    public class CDevHistoryItem
    {

        public CDevHistoryItem(int AMode, string ANo, string AMessage, HISTORY_TYPE AType = HISTORY_TYPE.AS, double AWorkTime = 0.0, string ADeveloper = "", string ARequester = "")
        {
            No = ANo;
            Mode = AMode;
            Type = AType;
            Message = AMessage;
            WorkTime = AWorkTime;

            Requester = ARequester;
            Developer = ADeveloper;
        }

        public int Mode;
        public string No;
        public string Message;
        public double WorkTime;

        public string Requester;
        public string Developer;

        public HISTORY_TYPE Type;
    }

    public class CDevHistory
    {
        public CDevHistory()
        {
            if (FItems != null) return;

            /* Sample

                DevHistory->AddHistory(0, "25.08.11.001 [요청자][수정자]  -SP(00.00)"); DevHistory->AddHistory("개발자 시작."                                                           );
                - 15.01.13.001 : 날짜.작업 번호
                - SP(00.00)    : SP 프로젝트 시작(Starting   Project)
                                 AC 추가 비용    (Additional    Cost)
                                 AS 추가 기능    (Additional Service)
                                 00.00           (날짜.시간)
                - 성명       : 요청자
                - 성명         : 수정자
            */
            AddItem("25.08.11.001", "개발 시작", HISTORY_TYPE.SP);
        }

        static private List<CDevHistoryItem> FItems = null;
        public int Count { get { return FItems.Count; } }
        public CDevHistoryItem this[int AIndex]
        {
            get
            {
                if (AIndex < 0) return null;
                if (FItems == null) return null;
                if (AIndex >= FItems.Count) return null;

                return FItems[AIndex];
            }
        }

        private void AddItem(int AMode, string ANo, string AMessage, HISTORY_TYPE AType = HISTORY_TYPE.AS, double AWorkTime = 0.0, string ADeveloper = "-", string ARequester = "-")
        {
            if (FItems == null) FItems = new List<CDevHistoryItem>();

            CDevHistoryItem item = new CDevHistoryItem(AMode, ANo, AMessage, AType, AWorkTime, ADeveloper, ARequester);
            FItems.Add(item);
        }
        private void AddItem(string ANo, string AMessage, HISTORY_TYPE AType = HISTORY_TYPE.AS, double AWorkTime = 0.0, string ADeveloper = "-", string ARequester = "-")
        {
            if (FItems == null) FItems = new List<CDevHistoryItem>();

            CDevHistoryItem item = new CDevHistoryItem(0, ANo, AMessage, AType, AWorkTime, ADeveloper, ARequester);
            FItems.Add(item);
        }
        private void AddItem(string AMessage, HISTORY_TYPE AType = HISTORY_TYPE.AS, double AWorkTime = 0.0, string ADeveloper = "-", string ARequester = "-")
        {
            if (FItems == null) FItems = new List<CDevHistoryItem>();

            CDevHistoryItem item = new CDevHistoryItem(1, "", AMessage, AType, AWorkTime, ADeveloper, ARequester);
            FItems.Add(item);
        }
    }
}
