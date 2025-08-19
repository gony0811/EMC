using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EGGPLANT
{
    enum DEVICE_ITEM_TYPE { INT = 0, DOUBLE = 1, STRING = 2, BOOLEAN = 3, }

    class CDeviceBaseItem
    {
        public CDeviceBaseItem(int AIndex)
        {
            FIndex = AIndex;
        }

        private int FIndex = 0;
        public int Index { get { return FIndex; } }

        private string FStrDigit = "{0:0}";
        public string StrDigit { get { return FStrDigit; } }

        private int FDigit = 0;
        public int Digit
        {
            get { return FDigit; }
            set
            {
                if (FDigit != value)
                {
                    if (FDigit >= 0)
                    {
                        FDigit = value;

                        if (FDigit == 0) FStrDigit = "{0:0}";
                        else
                        {
                            FStrDigit = "{0:0.";

                            for (int i = 0; i < FDigit; i++) FStrDigit += "0";
                            FStrDigit += "}";
                        }
                    }
                }
            }
        }

        private double FValue = 0.0d;
        private string FStrValue = "";
        public string Description = "", Unit = "";
        public DEVICE_ITEM_TYPE Type = DEVICE_ITEM_TYPE.INT;
        public double MinValue = -1000.0d, MaxValue = 1000.0d;

        public int ToInt()
        {
            return (int)FValue;
        }
        public bool ToBoolean()
        {
            if (FValue == 0.0d) return false;
            return true;
        }
        public double ToDouble()
        {
            return FValue;
        }
        public string ToSTring()
        {
            return FStrValue;
        }
        public void SetValue(int AValue)
        {
            FValue = AValue;
        }
        public void SetValue(bool AValue)
        {
            FValue = AValue ? 1.0d : 0.0d;
        }
        public void SetValue(double AValue)
        {
            FValue = AValue;
        }
        public void SetValue(string AValue)
        {
            FStrValue = AValue;
        }

        #region 파일 처리
        public void SaveParameter(XmlDocument ADoc, XmlNode ANode)
        {
            XmlNode item = CXML.CreateElement(ADoc, "ITEM", FIndex.ToString());

            CXML.AddElement(ADoc, item, "DESCRIPTION", Description);
            CXML.AddElement(ADoc, item, "TYPE", Type);

            CXML.AddElement(ADoc, item, "DIGIT", Digit);
            CXML.AddElement(ADoc, item, "UNIT", Unit);

            switch (Type)
            {
                case DEVICE_ITEM_TYPE.BOOLEAN:
                    bool vbool = false;
                    if (FValue != 0) vbool = true;

                    CXML.AddElement(ADoc, item, "VALUE", vbool.ToString());
                    break;
                case DEVICE_ITEM_TYPE.STRING:
                    CXML.AddElement(ADoc, item, "VALUE", FStrValue);
                    break;
                default:
                    CXML.AddElement(ADoc, item, "VALUE", FValue.ToString());
                    break;
            }
            CXML.AddElement(ADoc, item, "MIN_VALUE", MinValue);
            CXML.AddElement(ADoc, item, "MAX_VALUE", MaxValue);

            ANode.AppendChild(item);
        }
        public void OpenParameter(XmlNode ANode)
        {
            XmlNode item = ANode.SelectSingleNode($"./ITEM[@ID='{FIndex.ToString()}']");

            if (!Enum.TryParse<DEVICE_ITEM_TYPE>(item.SelectSingleNode("TYPE").InnerText, out Type)) Type = DEVICE_ITEM_TYPE.DOUBLE;
            CXML.GetInnerText(item, "DESCRIPTION", out Description, "");

            int digit = 0;
            CXML.GetInnerText(item, "DIGIT", out digit);
            CXML.GetInnerText(item, "UNIT", out Unit);
            Digit = digit;

            FStrValue = item.SelectSingleNode("VALUE").InnerText;
            switch (Type)
            {
                case DEVICE_ITEM_TYPE.BOOLEAN:
                    bool vbool;
                    if (!bool.TryParse(FStrValue, out vbool)) vbool = false;

                    FValue = (vbool) ? 1.0d : 0.0d;
                    break;
                case DEVICE_ITEM_TYPE.STRING:
                    break;
                default:
                    if (!double.TryParse(FStrValue, out FValue)) FValue = 0.0d;
                    break;
            }

            CXML.GetInnerText(item, "MIN_VALUE", out MinValue);
            CXML.GetInnerText(item, "MAX_VALUE", out MaxValue);
        }

        public void Save(XmlDocument ADoc, XmlNode ANode)
        {
            XmlNode item = CXML.CreateElement(ADoc, "ITEM", FIndex.ToString());

            CXML.AddElement(ADoc, item, "DESCRIPTION", Description);
            CXML.AddElement(ADoc, item, "TYPE", Type);

            CXML.AddElement(ADoc, item, "DIGIT", Digit);
            CXML.AddElement(ADoc, item, "UNIT", Unit);

            switch (Type)
            {
                case DEVICE_ITEM_TYPE.BOOLEAN:
                    bool vbool = false;
                    if (FValue != 0) vbool = true;

                    CXML.AddElement(ADoc, item, "VALUE", vbool.ToString());
                    break;
                case DEVICE_ITEM_TYPE.STRING:
                    CXML.AddElement(ADoc, item, "VALUE", FStrValue);
                    break;
                default:
                    CXML.AddElement(ADoc, item, "VALUE", FValue.ToString());
                    break;
            }
            CXML.AddElement(ADoc, item, "MIN_VALUE", MinValue);
            CXML.AddElement(ADoc, item, "MAX_VALUE", MaxValue);

            ANode.AppendChild(item);
        }
        public bool Open(XmlNode ANode)
        {
            XmlNode item = ANode.SelectSingleNode($"./ITEM[@ID='{FIndex.ToString()}']");
            try
            {
                if (item == null) return false;

                CXML.GetInnerText(item, "VALUE", out FStrValue);
                switch (Type)
                {
                    case DEVICE_ITEM_TYPE.BOOLEAN:
                        bool vbool;
                        if (!bool.TryParse(FStrValue, out vbool)) vbool = false;

                        FValue = (vbool) ? 1.0d : 0.0d;
                        break;
                    case DEVICE_ITEM_TYPE.STRING:
                        break;
                    default:
                        if (!double.TryParse(FStrValue, out FValue)) FValue = 0.0d;
                        break;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void Encoding(string APath, XmlDocument ADoc, XmlNode ANode)
        {
            XmlNode item = CXML.CreateElement(ADoc, "ITEM", FIndex.ToString());

            CXML.AddElement(ADoc, item, "DESCRIPTION", Description);
            CXML.AddElement(ADoc, item, "TYPE", Type);

            CXML.AddElement(ADoc, item, "DIGIT", Digit);
            CXML.AddElement(ADoc, item, "UNIT", Unit);

            switch (Type)
            {
                case DEVICE_ITEM_TYPE.BOOLEAN:
                    bool vbool = false;
                    if (FValue != 0) vbool = true;

                    CXML.AddElement(ADoc, item, "VALUE", vbool.ToString());
                    break;
                case DEVICE_ITEM_TYPE.STRING:
                    CXML.AddElement(ADoc, item, "VALUE", FStrValue);
                    break;
                default:
                    CXML.AddElement(ADoc, item, "VALUE", FValue.ToString());
                    break;
            }
            CXML.AddElement(ADoc, item, "MIN_VALUE", MinValue);
            CXML.AddElement(ADoc, item, "MAX_VALUE", MaxValue);

            ANode.AppendChild(item);
        }
        public bool Decoding(XmlNode ANode)
        {
            XmlNode item = ANode.SelectSingleNode($"./ITEM[@ID='{FIndex.ToString()}']");
            try
            {
                if (item == null) return false;

                CXML.GetInnerText(item, "VALUE", out FStrValue);
                switch (Type)
                {
                    case DEVICE_ITEM_TYPE.BOOLEAN:
                        bool vbool;
                        if (!bool.TryParse(FStrValue, out vbool)) vbool = false;

                        FValue = (vbool) ? 1.0d : 0.0d;
                        break;
                    case DEVICE_ITEM_TYPE.STRING:
                        break;
                    default:
                        if (!double.TryParse(FStrValue, out FValue)) FValue = 0.0d;
                        break;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion
    }

    public interface IDeviceAccompany
    {
        bool DeviceOpen(string APath);
        bool DeviceDelete(string APath);
        bool DeviceOpenPossible(string APath);
        bool DeviceCopy(string ASrcPath, string ADesPath);

        bool Decoding(string APath, XmlNode ANode);
        bool Encoding(string APath, XmlDocument ADoc, XmlNode ANode);
    }

    class CDeviceBase : IDisposable
    {
        public const UInt32 __DEVICE_MAX_COUNT__ = 1000;
        public const UInt32 __DEVICE_MAX_BASIC_ITEM__ = 100;
        public const UInt32 __DEVICE_MAX_COMMON_ITEM__ = 100;

        static public string Version
        {
            get { return "DEVICE BAES DESCRIPTION - Sean(V25.08.19.001)"; }
        }

        public CDeviceBase()
        {
            FDirectory = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\DEVICE\\";
            if (!System.IO.Directory.Exists(FDirectory)) System.IO.Directory.CreateDirectory(FDirectory);

            for (int i = 0; i < __DEVICE_MAX_COMMON_ITEM__; i++) FDviceCItem[i] = new CDeviceBaseItem(i);
            for (int i = 0; i < __DEVICE_MAX_BASIC_ITEM__; i++) FDviceBItem[i] = new CDeviceBaseItem(i);
            OpenParameter();
        }
        ~CDeviceBase()
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

        #region 디바이스 변수 및 데이터
        public string Description
        {
            get { return GetDescription(FIndex); }
        }
        static public string GetDescription(int AIndex)
        {
            if (AIndex < 0) return "";
            if (AIndex >= __DEVICE_MAX_COUNT__) return "";

            return FDeviceDescription[AIndex];
        }
        static public void SetDescription(int AIndex, string AValue)
        {
            if (AIndex < 0) return;
            if (AIndex >= __DEVICE_MAX_COUNT__) return;

            FDeviceDescription[AIndex] = AValue;
        }
        static private string[] FDeviceDescription = new string[__DEVICE_MAX_COUNT__];

        public CDeviceBaseItem CItem(int AIndex)
        {
            if (AIndex < 0) return null;
            if (AIndex >= __DEVICE_MAX_COMMON_ITEM__) return null;

            return FDviceCItem[AIndex];
        }
        private CDeviceBaseItem[] FDviceCItem = new CDeviceBaseItem[__DEVICE_MAX_COMMON_ITEM__];
        public CDeviceBaseItem BItem(int AIndex)
        {
            if (AIndex < 0) return null;
            if (AIndex >= __DEVICE_MAX_BASIC_ITEM__) return null;

            return FDviceBItem[AIndex];
        }
        private CDeviceBaseItem[] FDviceBItem = new CDeviceBaseItem[__DEVICE_MAX_BASIC_ITEM__];
        #endregion

        #region 파라미터 저장 및 불러오기
        private string FDirectory;
        public string Directory { get { return FDirectory; } }

        public void SaveParameter(int AFlag = 0xFF)
        {
            if ((AFlag & 0x01) == 0x01)
            {
                XmlDocument xdoc = new System.Xml.XmlDocument();
                xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

                XmlNode root = CXML.CreateElement(xdoc, "ROOT");
                xdoc.AppendChild(root);

                XmlNode basic = CXML.CreateElement(xdoc, "BASIC");
                CXML.AddElement(xdoc, basic, "VERSION", Version);
                root.AppendChild(basic);

                XmlNode param = xdoc.CreateElement("PARAMETER");
                #region Parameter
                CXML.AddElement(xdoc, param, "LAST_DEVICE_INDEX", FIndex);
                CXML.AddElement(xdoc, param, "LAST_DEVICE_DESCRIPTION", GetDescription(FIndex));
                #endregion
                root.AppendChild(param);
                xdoc.Save($"{FDirectory}LAST DEVICE.XML");
            }

            if ((AFlag & 0x02) == 0x02)
            {
                XmlDocument xdoc = new System.Xml.XmlDocument();
                xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

                XmlNode root = CXML.CreateElement(xdoc, "ROOT");
                xdoc.AppendChild(root);

                XmlNode basic = xdoc.CreateElement("BASIC");
                CXML.AddElement(xdoc, basic, "VERSION", Version);
                root.AppendChild(basic);

                XmlNode items = CXML.CreateElement(xdoc, "ITEMs");
                #region Items
                for (int i = 0; i < __DEVICE_MAX_COUNT__; i++)
                {
                    CXML.AddElement(xdoc, items, $"ITEM{i + 1:D4}", FDeviceDescription[i]);
                }
                #endregion
                root.AppendChild(items);
                xdoc.Save($"{FDirectory}DEVICE DESCRIPTION.XML");
            }

            if ((AFlag & 0x10) == 0x10)
            {
                XmlDocument xdoc = new System.Xml.XmlDocument();
                xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

                XmlNode root = CXML.CreateElement(xdoc, "ROOT");
                xdoc.AppendChild(root);

                XmlNode basic = xdoc.CreateElement("BASIC");
                CXML.AddElement(xdoc, basic, "VERSION", Version);
                root.AppendChild(basic);

                XmlNode items = CXML.CreateElement(xdoc, "COMMON_ITEMs");
                CXML.AddComment(xdoc, items, "TYPE : INT, DOUBLE, STRING, BOOLEAN, ");
                #region Items
                for (int i = 0; i < __DEVICE_MAX_COMMON_ITEM__; i++)
                {
                    FDviceCItem[i].SaveParameter(xdoc, items);
                }
                #endregion
                root.AppendChild(items);
                xdoc.Save($"{FDirectory}DEVICE COMMON ITEM.XML");
            }

            if ((AFlag & 0x20) == 0x20)
            {
                XmlDocument xdoc = new System.Xml.XmlDocument();
                xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

                XmlNode root = CXML.CreateElement(xdoc, "ROOT");
                xdoc.AppendChild(root);

                XmlNode basic = xdoc.CreateElement("BASIC");
                CXML.AddElement(xdoc, basic, "VERSION", Version);
                root.AppendChild(basic);

                XmlNode items = CXML.CreateElement(xdoc, "BASIC_ITEMs");
                CXML.AddComment(xdoc, items, "TYPE : INT, DOUBLE, STRING, BOOLEAN, ");
                #region Items
                for (int i = 0; i < __DEVICE_MAX_BASIC_ITEM__; i++)
                {
                    FDviceBItem[i].SaveParameter(xdoc, items);
                }
                #endregion
                root.AppendChild(items);
                xdoc.Save($"{FDirectory}DEVICE BASIC ITEM.XML");
            }
        }
        public void OpenParameter(int AFlag = 0xFF)
        {
            string file;
            XmlDocument xdoc;
            XmlNode basic, param, items;

            if ((AFlag & 0x01) == 0x01)
            {
                file = $"{FDirectory}LAST DEVICE.XML";
                if (!System.IO.File.Exists(file)) SaveParameter(0x01);
                else
                {
                    xdoc = new System.Xml.XmlDocument();
                    xdoc.Load(file);

                    basic = xdoc.SelectSingleNode($"/ROOT/BASIC");
                    if (basic != null)
                    {
                        string version;
                        CXML.GetInnerText(basic, "VERSION", out version);

                        if (version != Version)
                        {
                            System.Windows.Forms.MessageBox.Show($"버젼({Version} : {version})이 다릅니다.\r\n정보 불러오기에 문제가 발생될 수 있습니다.", "경고", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                        }
                    }

                    param = xdoc.SelectSingleNode($"/ROOT/PARAMETER");
                    CXML.GetInnerText(param, "LAST_DEVICE_INDEX", out FIndex, 1);
                }
            }

            if ((AFlag & 0x02) == 0x02)
            {
                file = $"{FDirectory}DEVICE DESCRIPTION.XML";
                if (!System.IO.File.Exists(file)) SaveParameter(0x02);
                else
                {
                    xdoc = new System.Xml.XmlDocument();
                    xdoc.Load(file);

                    basic = xdoc.SelectSingleNode($"/ROOT/BASIC");
                    if (basic != null)
                    {
                        string version;
                        CXML.GetInnerText(basic, "VERSION", out version);

                        if (version != Version)
                        {
                            System.Windows.Forms.MessageBox.Show($"버젼({Version} : {version})이 다릅니다.\r\n정보 불러오기에 문제가 발생될 수 있습니다.", "경고", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                        }
                    }

                    items = xdoc.SelectSingleNode($"/ROOT/ITEMs");
                    for (int i = 0; i < __DEVICE_MAX_COUNT__; i++)
                    {
                        CXML.GetInnerText(items, $"ITEM{i + 1:D4}", out FDeviceDescription[i], "");
                    }
                }
            }

            if ((AFlag & 0x10) == 0x10)
            {
                file = $"{FDirectory}DEVICE COMMON ITEM.XML";
                if (!System.IO.File.Exists(file)) SaveParameter(0x10);
                else
                {
                    xdoc = new System.Xml.XmlDocument();
                    xdoc.Load(file);

                    basic = xdoc.SelectSingleNode($"/ROOT/BASIC");
                    if (basic != null)
                    {
                        string version;
                        CXML.GetInnerText(basic, "VERSION", out version);

                        if (version != Version)
                        {
                            System.Windows.Forms.MessageBox.Show($"버젼({Version} : {version})이 다릅니다.\r\n정보 불러오기에 문제가 발생될 수 있습니다.", "경고", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                        }
                    }

                    items = xdoc.SelectSingleNode($"/ROOT/COMMON_ITEMs");
                    if (items != null)
                    {
                        for (int i = 0; i < __DEVICE_MAX_COMMON_ITEM__; i++)
                        {
                            FDviceCItem[i].OpenParameter(items);
                        }
                    }
                }
            }

            if ((AFlag & 0x20) == 0x20)
            {
                file = $"{FDirectory}DEVICE BASIC ITEM.XML";
                if (!System.IO.File.Exists(file)) SaveParameter(0x20);
                else
                {
                    xdoc = new System.Xml.XmlDocument();
                    xdoc.Load(file);

                    basic = xdoc.SelectSingleNode($"/ROOT/BASIC");
                    if (basic != null)
                    {
                        string version;
                        CXML.GetInnerText(basic, "VERSION", out version);

                        if (version != Version)
                        {
                            System.Windows.Forms.MessageBox.Show($"버젼({Version} : {version})이 다릅니다.\r\n정보 불러오기에 문제가 발생될 수 있습니다.", "경고", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                        }
                    }

                    items = xdoc.SelectSingleNode($"/ROOT/BASIC_ITEMs");
                    if (items != null)
                    {
                        for (int i = 0; i < __DEVICE_MAX_COMMON_ITEM__; i++)
                        {
                            FDviceBItem[i].OpenParameter(items);
                        }
                    }
                }
            }
        }
        public void Save(string APath = "")
        {
            if (APath == "") APath = DeviceDirectory;

            XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

            XmlNode root = CXML.CreateElement(xdoc, "ROOT");
            xdoc.AppendChild(root);

            XmlNode basic = CXML.CreateElement(xdoc, "BASIC");
            CXML.AddElement(xdoc, basic, "VERSION", Version);
            root.AppendChild(basic);

            XmlNode items = xdoc.CreateElement("BASIC_ITEMs");
            CXML.AddComment(xdoc, items, "TYPE : INT, DOUBLE, STRING, BOOLEAN, ");
            #region Items
            for (int i = 0; i < __DEVICE_MAX_BASIC_ITEM__; i++)
            {
                FDviceBItem[i].Save(xdoc, items);
            }
            #endregion
            root.AppendChild(items);
            xdoc.Save($"{APath}DEVICE PARAMETER.XML");
        }
        public bool Open(string APath = "")
        {
            bool ret = true;
            try
            {
                string file;
                XmlDocument xdoc;
                XmlNode basic, items;
                if (APath == "") APath = DeviceDirectory;

                file = $"{APath}DEVICE PARAMETER.XML";
                if (!System.IO.File.Exists(file)) Save();
                else
                {
                    xdoc = new System.Xml.XmlDocument();
                    xdoc.Load(file);

                    basic = xdoc.SelectSingleNode($"/ROOT/BASIC");
                    if (basic != null)
                    {
                        string version;
                        CXML.GetInnerText(basic, "VERSION", out version);

                        if (version != Version)
                        {
                            System.Windows.Forms.MessageBox.Show($"버젼({Version} : {version})이 다릅니다.\r\n정보 불러오기에 문제가 발생될 수 있습니다.", "경고", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                        }
                    }

                    items = xdoc.SelectSingleNode($"/ROOT/BASIC_ITEMs");
                    if (items == null) ret = false;
                    else
                    {
                        for (int i = 0; i < __DEVICE_MAX_COMMON_ITEM__; i++)
                        {
                            if (!FDviceBItem[i].Open(items)) ret = false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return ret;
        }
        #endregion

        #region 디바이스 변경, 복사, 삭제
        private System.Collections.Generic.List<IDeviceAccompany> FAccompanies = new System.Collections.Generic.List<IDeviceAccompany>();
        public void AddAccompany(IDeviceAccompany AAccompany)
        {
            FAccompanies.Add(AAccompany);
        }
        public IDeviceAccompany GetAccompany(int AIndex)
        {
            if (AIndex < 0) return null;
            if (AIndex >= FAccompanies.Count) return null;

            return FAccompanies[AIndex];
        }

        public string DeviceDirectory { get { return $"{FDirectory}{FIndex + 1:D4}\\"; } }
        public string GetDeviceDirectory(int AIndex) { return $"{FDirectory}{AIndex + 1:D4}\\"; }

        private int FIndex = 0;
        public int Index
        {
            get { return FIndex; }
            set
            {
                if (FIndex == value) return;

                FIndex = value;
                SaveParameter(0x01);

                Open(DeviceDirectory);
                string path = DeviceDirectory;
                foreach (IDeviceAccompany accompany in FAccompanies) accompany.DeviceOpen(path);

            }
        }
        public void Refresh()
        {
            Open(DeviceDirectory);

            string path = DeviceDirectory;
            foreach (IDeviceAccompany accompany in FAccompanies) accompany.DeviceOpen(path);
        }
        virtual public bool Delete(string APath)
        {
            if (!System.IO.Directory.Exists(APath)) return true;
            bool ret = true;

            try
            {
                System.IO.File.Delete($"{APath}DEVICE PARAMETER.XML");
            }
            catch
            {

            }

            string path = DeviceDirectory;
            foreach (IDeviceAccompany accompany in FAccompanies)
            {
                if (!accompany.DeviceDelete(APath)) ret = false;
            }

            string[] files = System.IO.Directory.GetFiles(DeviceDirectory, "*.*", System.IO.SearchOption.AllDirectories);
            try
            {
                if (files.Length <= 0) System.IO.Directory.Delete(DeviceDirectory);
            }
            catch
            {

            }
            System.IO.Directory.Delete(APath, true);
            return ret;
        }
        virtual public bool OpenPossible(string APath)
        {
            if (!System.IO.File.Exists($"{APath}DEVICE PARAMETER.XML")) return false;

            foreach (IDeviceAccompany accompany in FAccompanies)
            {
                if (!accompany.DeviceOpenPossible(APath)) return false;
            }
            return true;
        }
        virtual public bool OpenPossible(int AIndex)
        {
            string path = $"{FDirectory}{AIndex + 1:D4}\\";
            if (!System.IO.File.Exists($"{path}DEVICE PARAMETER.XML")) return false;

            foreach (IDeviceAccompany accompany in FAccompanies)
            {
                if (!accompany.DeviceOpenPossible(path)) return false;
            }
            return true;
        }
        virtual public bool Copy(string ASrcPath, string ADesPath)
        {
            if (!System.IO.File.Exists($"{ASrcPath}DEVICE PARAMETER.XML")) return false;

            if (!System.IO.Directory.Exists(ADesPath)) System.IO.Directory.CreateDirectory(ADesPath);
            try
            {
                System.IO.File.Copy($"{ASrcPath}DEVICE PARAMETER.XML", $"{ADesPath}DEVICE PARAMETER.XML", true);
            }
            catch
            {
                return false;
            }

            foreach (IDeviceAccompany accompany in FAccompanies)
            {
                if (!accompany.DeviceCopy(ASrcPath, ADesPath)) return false;
            }
            return true;
        }
        virtual public bool Copy(int ASrcIndex, int ADesIndex)
        {
            string srcpath = GetDeviceDirectory(ASrcIndex);
            string despath = GetDeviceDirectory(ADesIndex);
            return Copy(srcpath, despath);
        }

        virtual public bool Decoding(string APath, string ASrcFileName)
        {
            bool ret = true;
            string srcfile = ASrcFileName;
            if (!System.IO.File.Exists(srcfile)) return false;
            else
            {
                if (!System.IO.Directory.Exists(APath)) System.IO.Directory.CreateDirectory(APath);
                XmlDocument srcxdoc = new System.Xml.XmlDocument();

                srcxdoc.Load(srcfile);
                XmlNode srcroot = srcxdoc.SelectSingleNode($"/ROOT");
                XmlNode srcbasic = srcxdoc.SelectSingleNode($"/ROOT/BASIC");
                if (srcbasic != null)
                {
                    string version;
                    CXML.GetInnerText(srcbasic, "VERSION", out version);

                    if (version != Version)
                    {
                        System.Windows.Forms.MessageBox.Show($"버젼({Version} : {version})이 다릅니다.\r\n정보 불러오기에 문제가 발생될 수 있습니다.", "경고", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    }
                }

                XmlNode srcitems = CXML.SelectSingleNode(srcxdoc, "/ROOT/BASIC_ITEMs");
                if (srcitems != null)
                {
                    string file = $"{APath}DEVICE PARAMETER.XML";
                    if (!System.IO.File.Exists(file)) Save(APath);

                    XmlDocument xdoc = new XmlDocument();
                    xdoc.Load(file);

                    XmlNode items = CXML.SelectSingleNode(xdoc, "/ROOT/BASIC_ITEMs");
                    for (int i = 0; i < __DEVICE_MAX_BASIC_ITEM__; i++)
                    {
                        if (FDviceBItem[i].Description.Trim() == "") continue;

                        XmlNode srcitem = CXML.SelectSingleNode(srcitems, "ITEM", i.ToString());
                        if (srcitem != null)
                        {
                            XmlNode item = CXML.SelectSingleNode(items, "ITEM", i.ToString());
                            if (item != null) CXML.Move(srcitem, item, "VALUE");
                        }
                    }
                    xdoc.Save(file);
                }

                foreach (IDeviceAccompany accompany in FAccompanies)
                {
                    if (!accompany.Decoding(APath, srcxdoc)) ret = false;
                }
            }
            return ret;
        }
        virtual public bool Encoding(string ASrcPath, string ADesFileName)
        {
            XmlDocument xdoc = new System.Xml.XmlDocument();

            XmlNode root = CXML.CreateElement(xdoc, "ROOT");
            xdoc.AppendChild(root);

            XmlNode basic = xdoc.CreateElement("BASIC");
            CXML.AddElement(xdoc, basic, "VERSION", Version);
            root.AppendChild(basic);

            bool ret = true;
            string file = $"{ASrcPath}DEVICE PARAMETER.XML";
            if (!System.IO.File.Exists(file)) ret = false;
            else
            {
                XmlNode items = xdoc.CreateElement("BASIC_ITEMs");
                root.AppendChild(items);
                if (items != null)
                {
                    XmlDocument dvxdoc = new System.Xml.XmlDocument();
                    dvxdoc.Load(file);

                    XmlNode dvitems = dvxdoc.SelectSingleNode($"/ROOT/BASIC_ITEMs");
                    if (dvitems != null)
                    {
                        for (int i = 0; i < __DEVICE_MAX_BASIC_ITEM__; i++)
                        {
                            if (FDviceBItem[i].Description.Trim() == "") continue;

                            XmlNode dvitem = dvitems.SelectSingleNode($"./ITEM[@ID='{i.ToString()}']");
                            if (dvitem != null) items.AppendChild(xdoc.ImportNode(dvitem, true));
                        }
                    }
                }
            }

            foreach (IDeviceAccompany accompany in FAccompanies)
            {
                accompany.Encoding(ASrcPath, xdoc, root);
            }
            xdoc.Save(ADesFileName);
            return ret;
        }
        #endregion
    }
}
