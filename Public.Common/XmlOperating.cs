using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

namespace TEC.Public.Common
{
    /// <summary>
    /// Xml文件增删改查
    /// </summary>
    public class XmlOperating
    {
        /// <summary>
        /// 创建XML文件
        /// </summary>
        public void CreateXml(string path,string rootnode) {
            XmlDocument xmldoc;
            XmlElement xmlelem;

            xmldoc = new XmlDocument();
            //加入XML的声明段落,<?xml version="1.0" encoding="gb2312"?>
            XmlDeclaration xmldecl;
            xmldecl = xmldoc.CreateXmlDeclaration("1.0", "gb2312", null);
            xmldoc.AppendChild(xmldecl);
            //加入一个根元素
            xmlelem = xmldoc.CreateElement("", rootnode, "");
            xmldoc.AppendChild(xmlelem);
        
            //保存创建好的XML文档
            xmldoc.Save(path);//Server.MapPath("data.xml")
        }
        /// <summary>
        /// 流创建文件
        /// </summary>
        public void CreateXmlByWriter() {
            XmlTextWriter xmlWriter;
            string strFilename = "";//Server.MapPath("data1.xml");
            xmlWriter = new XmlTextWriter(strFilename, Encoding.Default);//创建一个xml文档
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Employees");
            xmlWriter.WriteStartElement("Node");
            xmlWriter.WriteAttributeString("genre", "李赞红");
            xmlWriter.WriteAttributeString("ISBN", "2-3631-4");
            xmlWriter.WriteStartElement("title");
            xmlWriter.WriteString("CS从入门到精通");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("author");
            xmlWriter.WriteString("候捷");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("price");
            xmlWriter.WriteString("58.3");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
            xmlWriter.Close();     
        }
        /// <summary>
        /// 添加一个二级节点
        /// </summary>
        public void AddNode(string path, string rootnode, string nodename,string nodetext, string[][] attribute)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode root = xmlDoc.SelectSingleNode(rootnode);//查找根节点
            XmlElement xe = xmlDoc.CreateElement(nodename);//创建一个新节点 
            xe.InnerText = nodetext;
            if (attribute.Length>1)
                for (int i = 0; i < attribute.Length; i = i + 2)
                    xe.SetAttribute(attribute[i][0], attribute[i][2]);//设置该节点属性 
            root.AppendChild(xe);//添加到<Employees>节点中 
            xmlDoc.Save(path);
        }
        /// <summary>
        /// 修改二级节点text
        /// </summary>
        public void UpdNodeText(string path, string rootnode, string nodename, string nodetext) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNodeList nodeList = xmlDoc.SelectSingleNode(rootnode).ChildNodes;//获取根节点的所有子节点
            foreach (XmlNode xn in nodeList)//遍历所有子节点 
            {
                XmlElement xe = (XmlElement)xn;//将子节点类型转换为XmlElement类型 
                if (xe.Name == nodename)
                {
                    xe.InnerText = nodetext;
                }
            }
            xmlDoc.Save(path);//保存。
        }
        /// <summary>
        /// 修改结点的值（属性和子结点）
        /// </summary>
        public void UpdNodeAttribute(string path, string rootnode, string attrname, string attrnewvalue,string attroldvalue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNodeList nodeList = xmlDoc.SelectSingleNode(rootnode).ChildNodes;//获取根节点的所有子节点
            foreach (XmlNode xn in nodeList)//遍历所有子节点 
            {
                XmlElement xe = (XmlElement)xn;//将子节点类型转换为XmlElement类型 
                if (xe.GetAttribute(attrname) == attroldvalue)//如果属性值
                {
                    xe.SetAttribute(attrname, attrnewvalue);//则修改该属性                    
                }
            }
            xmlDoc.Save(path);//保存。
        }

        /// <summary>
        /// 删除结点中的某一个属性
        /// </summary>
        public void DelNodeAttr(string path,string rootnode,string nodeattr) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNodeList xnl = xmlDoc.SelectSingleNode(rootnode).ChildNodes;
            foreach (XmlNode xn in xnl)
            {
                XmlElement xe = (XmlElement)xn;
                xe.RemoveAttribute(nodeattr);//删除属性               
            }
            xmlDoc.Save(path);
        }
        /// <summary>
        /// 删除结点
        /// </summary>
        public void DelNode(string path,string rootnode,string nodename) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode root = xmlDoc.SelectSingleNode(rootnode);
            XmlNodeList xnl = xmlDoc.SelectSingleNode(rootnode).ChildNodes;
            foreach (XmlNode xn in xnl)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.Name == nodename)
                    root.RemoveChild(xe);             
            }
            xmlDoc.Save(path);
        }
        /// <summary>
        /// 按照文本文件读取xml
        /// </summary>
        public void ReadXml(string path) {
            System.IO.StreamReader myFile = new System.IO.StreamReader(path, Encoding.Default);
            //注意System.Text.Encoding.Default
            string myString = myFile.ReadToEnd();//myString是读出的字符串
            myFile.Close();
        }
        /// <summary>
        /// XML基础操作
        /// </summary>
        public void temp() {
            ///初始化一个xml实例
            //XmlDocument xml = new XmlDocument();
            ////导入指定xml文件
            //Xml.Load(path);
            //xml.Load(HttpContext.Current.Server.MapPath("~/file/bookstore.xml"));
            ////指定一个节点
            //XmlNode root = xml.SelectSingleNode("/root");
            ////获取节点下所有直接子节点
            //XmlNodeList childlist = root.ChildNodes;
            ////判断该节点下是否有子节点
            //root.HasChildNodes;
            ////获取同名同级节点集合
            //XmlNodeList nodelist = xml.SelectNodes("/Root/News");
            ////生成一个新节点
            //XmlElement node = xml.CreateElement("News");
            ////将节点加到指定节点下，作为其子节点
            //root.AppendChild(node);
            ////将节点加到指定节点下某个子节点前
            //root.InsertBefore(node,root.ChildeNodes[i]);
            ////为指定节点的新建属性并赋值
            //node.SetAttribute("id","11111");
            ////为指定节点添加子节点
            //root.AppendChild(node);
            ////获取指定节点的指定属性值
            //string id = node.Attributes["id"].Value;
            ////获取指定节点中的文本
            //string content = node.InnerText;
            ////保存XML文件
            //string path = Server.MapPath("~/file/bookstore.xml");
            //xml.Save(path);


            //修改三级，可以扩展递归
            //XmlNodeList nls = xe.ChildNodes;//继续获取xe子节点的所有子节点 
            //foreach (XmlNode xn1 in nls)//遍历 
            //{
            //    XmlElement xe2 = (XmlElement)xn1;//转换类型 
            //    if (xe2.Name == "author")//如果找到 
            //    {
            //        xe2.InnerText = "亚胜";//则修改
            //    }
            //}

            //CreateXml加入另外一个元素
            //for (int i = 1; i < 3; i++)
            //{
            //    XmlNode root = xmldoc.SelectSingleNode("Employees");//查找<Employees>

            //    XmlElement xe1 = xmldoc.CreateElement("Node");//创建一个<Node>节点 
            //    xe1.SetAttribute("genre", "李赞红");//设置该节点genre属性 
            //    xe1.SetAttribute("ISBN", "2-3631-4");//设置该节点ISBN属性

            //    XmlElement xesub1 = xmldoc.CreateElement("title");
            //    xesub1.InnerText = "CS从入门到精通";//设置文本节点 
            //    xe1.AppendChild(xesub1);//添加到<Node>节点中 

            //    XmlElement xesub2 = xmldoc.CreateElement("author");
            //    xesub2.InnerText = "候捷";
            //    xe1.AppendChild(xesub2);

            //    XmlElement xesub3 = xmldoc.CreateElement("price");
            //    xesub3.InnerText = "58.3";
            //    xe1.AppendChild(xesub3);

            //    root.AppendChild(xe1);//添加到<Employees>节点中 
            //}
        }
    }
}
