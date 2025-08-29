using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EGGPLANT
{
    public enum ERROR_MODE { WARNING = 0, ERROR = 1, UN_KNOWN = 99 };
    public enum ERROR_KIND { MACHINE = 0, MATERIAL = 1, MAN = 2, METHODE = 3, };

    public class CErrorListItem
    {
        public CErrorListItem(CErrorList AOwner, int AIndex)
        {
            FOwner = AOwner;
            FIndex = AIndex;
        }

        private int FIndex = 0;
        private CErrorList FOwner = null;
        public int Index { get { return FIndex; } }
        public CErrorList Owner { get { return FOwner; } }

        public int Level = 0;
        public int Buzzer = 0;
        public string Title = "";
        public string Cause = "";
        public string Solution = "";
        public string ImageFile = "";
        public ERROR_MODE Mode = ERROR_MODE.ERROR;
        public ERROR_KIND Kind = ERROR_KIND.MACHINE;

        public bool GetImage(System.Windows.Forms.PictureBox APictureBox, ref string AFileName)
        {
            if (System.IO.File.Exists(ImageFile))
            {
                APictureBox.Image = System.Drawing.Image.FromFile(ImageFile);
                AFileName = ImageFile;
                return true;
            }

            if (FOwner == null) return true;
            string file = FOwner.Directory + $"ERR IMAGE\\ERR{FIndex:D}.JPEG";

            if (System.IO.File.Exists(file))
            {
                APictureBox.Image = System.Drawing.Image.FromFile(file);
                AFileName = file;
                return true;
            }
            return false;
        }

        public void SaveParameter(XmlDocument ADoc, XmlNode ANode)
        {
            XmlNode item = CXML.CreateElement(ADoc, "ITEM", FIndex.ToString());

            CXML.AddElement(ADoc, item, "LEVEL", Level);
            CXML.AddElement(ADoc, item, "BUZZER", Buzzer);

            CXML.AddElement(ADoc, item, "MODE", Mode.ToString());
            CXML.AddElement(ADoc, item, "KIND", Kind.ToString());

            CXML.AddElement(ADoc, item, "TITLE", Title);
            CXML.AddElement(ADoc, item, "CAUSE", Cause);
            CXML.AddElement(ADoc, item, "SOLUTION", Solution);
            CXML.AddElement(ADoc, item, "IMAGE_FILE", ImageFile);

            ANode.AppendChild(item);
        }
        public void OpenParameter(XmlNode ANode)
        {
            XmlNode item = ANode.SelectSingleNode($"./ITEM[@ID='{FIndex.ToString()}']");
            if (item != null)
            {
                CXML.GetInnerText(item, "LEVEL", out Level);
                CXML.GetInnerText(item, "BUZZER", out Buzzer);

#pragma warning disable CS8602 // null 가능 참조에 대한 역참조입니다.
                if (!Enum.TryParse<ERROR_MODE>(item.SelectSingleNode("MODE").InnerText, out Mode)) Mode = ERROR_MODE.ERROR;
                if (!Enum.TryParse<ERROR_KIND>(item.SelectSingleNode("KIND").InnerText, out Kind)) Kind = ERROR_KIND.MACHINE;
#pragma warning restore CS8602 // null 가능 참조에 대한 역참조입니다.

                CXML.GetInnerText(item, "TITLE", out Title);
                CXML.GetInnerText(item, "CAUSE", out Cause);
                CXML.GetInnerText(item, "SOLUTION", out Solution);
                CXML.GetInnerText(item, "IMAGE_FILE", out ImageFile);
            }
        }
    }
    public class CErrorList : IDisposable
    {
        static public int __MAX_ERROR_COUNT__ = 10001;
        public string Version
        {
            get { return "ERROR LIST - HoPe(V19.01.07.001)"; }
        }

        public CErrorList()
        {

        }

        public CErrorList(string AClassName)
        {
            FClassName = AClassName;
            FDirectory = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\CONFIG\\";

            if (!System.IO.Directory.Exists(FDirectory)) System.IO.Directory.CreateDirectory(FDirectory);
            for (int i = 0; i < __MAX_ERROR_COUNT__; i++) FItem[i] = new CErrorListItem(this, i);
            OpenParameter();
        }
        ~CErrorList()
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

            FDisposed = true;
        }
        #endregion     

        #region 파라미터 저장 및 불러오기
        private string FClassName, FDirectory;
        public string ClassName { get { return FClassName; } }
        public string Directory { get { return FDirectory; } }

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
            CXML.AddElement(xdoc, option, "IMAGE_ENABLED", FOptionImageEnabled);
            root.AppendChild(option);

            XmlNode items = xdoc.CreateElement("ITEMs");
            CXML.AddComment(xdoc, items, "MODE : HEM_WARNING, HEM_ERROR, ");
            CXML.AddComment(xdoc, items, "KIND : HEM_MACHINE, HEM_MATERIAL, HEM_MAN, HEM_METHODE, ");
            CXML.AddComment(xdoc, items, "에러 이미지 설정이 안되어 있으면, ..CONFIG\\ERR IMAGE\\ERR \"에러번호\".JPEG 로 자동 로딩됩니다.");

            #region Item
            FItem[0].SaveParameter(xdoc, items);
            for (int i = 1; i < __MAX_ERROR_COUNT__; i++)
            {
                if (FItem[i].Title == "") continue;
                FItem[i].SaveParameter(xdoc, items);
            }
            #endregion
            root.AppendChild(items);
            xdoc.Save($"{FDirectory}{FClassName}.XML");
        }
        public void OpenParameter()
        {
            string file = $"{FDirectory}{FClassName}.XML";
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
                CXML.GetInnerText(option, "IMAGE_ENABLED", out FOptionImageEnabled, false);
            }

            XmlNode items = xdoc.SelectSingleNode($"/ROOT/ITEMs");
            if (items != null)
            {
                for (int i = 0; i < __MAX_ERROR_COUNT__; i++)
                {
                    FItem[i].OpenParameter(items);
                }
            }
        }
        #endregion

        private bool FOptionImageEnabled = false;
        public bool OptionImageEnabled { get { return FOptionImageEnabled; } }

        private CErrorListItem[] FItem = new CErrorListItem[__MAX_ERROR_COUNT__];
        public CErrorListItem GetItem(int AIndex)
        {
            if (AIndex < 0) return null;
            if (AIndex >= __MAX_ERROR_COUNT__) return null;

            return FItem[AIndex];
        }
    }
}
