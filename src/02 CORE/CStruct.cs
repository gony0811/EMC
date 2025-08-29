using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EGGPLANT
{
    class CStructBase
    {
        public CStructBase(string AClassName)
        {
            FClassName = AClassName;
        }

        private string FClassName = "";
        public string ClassName { get { return FClassName; } }

        protected int FIdle = 0;
        protected int FStatus = 0;                        // 각 파트 마다 자재에 관련된 정보를 나타냄
        protected int FRunStatus = 1;
        protected int FInRequest = 0;                     //true : 자재 진입 허용, false : 자재 진입 불가
        protected int FOutRequest = 0;                    //true : 자재 배출 허용, false : 자재 배출 불가

        protected virtual void SetIdle(int AIdle)
        {
            FIdle = AIdle;
        }
        protected virtual void SetStatus(int AStatus)
        {
            FStatus = AStatus;
        }
        protected virtual void SetRunStatus(int ARunStatus)
        {
            FRunStatus = ARunStatus;
        }
        protected virtual void SetInRequest(int AInRequest)
        {
            FInRequest = AInRequest;
        }
        protected virtual void SetOutRequest(int AOutRequest)
        {
            FOutRequest = AOutRequest;
        }

        public int Idle { get { return FIdle; } set { SetIdle(value); } }
        public int Status { get { return FStatus; } set { SetStatus(value); } }
        public int RunStatus { get { return FRunStatus; } set { SetRunStatus(value); } }
        public int InRequest { get { return FInRequest; } set { SetInRequest(value); } }
        public int OutRequest { get { return FOutRequest; } set { SetOutRequest(value); } }

        public void Save(string APath)
        {
            XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

            XmlNode root = CXML.CreateElement(xdoc, "ROOT");
            xdoc.AppendChild(root);

            XmlNode node = CXML.CreateElement(xdoc, "BASIC_FLAG");
            CXML.AddElement(xdoc, node, "STATUS", FStatus);
            CXML.AddElement(xdoc, node, "RUN_STATUS", FRunStatus);
            CXML.AddElement(xdoc, node, "IN_REQUEST", FInRequest);
            CXML.AddElement(xdoc, node, "OUT_REQUEST", FOutRequest);
            root.AppendChild(node);

            SaveEx(xdoc, root);
            xdoc.Save($"{APath}{FClassName}.XML");
        }
        public void Open(string APath)
        {
            string file = $"{APath}{FClassName}.XML";
            if (!System.IO.File.Exists(file)) Save(APath);
            else
            {
                XmlDocument xdoc = new System.Xml.XmlDocument();
                xdoc.Load(file);

                XmlNode node = xdoc.SelectSingleNode($"/ROOT/BASIC_FLAG");
                if (node != null)
                {
                    CXML.GetInnerText(node, "STATUS", out FStatus);
                    CXML.GetInnerText(node, "RUN_STATUS", out FRunStatus);
                    CXML.GetInnerText(node, "IN_REQUEST", out FInRequest);
                    CXML.GetInnerText(node, "OUT_REQUEST", out FOutRequest);
                }
                OpenEx(xdoc);
            }
        }

        protected virtual void SaveEx(XmlDocument ADoc, XmlNode ANode)
        {
        }
        protected virtual void OpenEx(XmlNode ANode)
        {

        }
    }


}
