using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace Public.Common
{
    public class ReadLogConfig
    {
        private static LogConfig logConfig = new LogConfig();
        private static EmailConfig emailConfig = new EmailConfig();  //日志记录邮件实例

        /// <summary>
        /// 构造函数
        /// </summary>
        static ReadLogConfig()
        {
            System.IO.FileSystemWatcher watcher = new System.IO.FileSystemWatcher(Path.GetDirectoryName(GetFilePath()), ".config");
            watcher.Changed += new System.IO.FileSystemEventHandler(watcher_Changed);
            watcher_Changed(null, null);
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <returns></returns>
        private static string GetFilePath()
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                return context.Server.MapPath("~/Config/LogConfig.config");
            }
            else
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\LogConfig.config");
            }
            //return System.Web.HttpContext.Current.Server.MapPath("~/Config/LogConfig.Config");
        }

        /// <summary>
        /// 日志配置文件内容发生更改时触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            try
            {
                #region 日志配置文件内容发生更改时触发的事件

                string xmlFilePath = GetFilePath();
                if (!string.IsNullOrWhiteSpace(xmlFilePath))
                {
                    #region 将各个列表及对象置空

                    logConfig = new LogConfig();
                    //enableLog = false;
                    //logOutMode = "FileLog";
                    //logSaveRootPhyPath = string.Empty;
                    //enabledDebugLog = false;
                    emailConfig = new EmailConfig();

                    #endregion 将各个列表及对象置空

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(xmlFilePath);

                    #region 获取日志开关
                    XmlNode enableLogXmlNode = xmlDocument.SelectSingleNode("LogConfig/EnableLog");
                    XmlElement enableLogXmlElement = (XmlElement)enableLogXmlNode;
                    logConfig.EnableLog = Convert.ToBoolean(enableLogXmlElement.InnerText);
                    #endregion

                    #region 获取日志输出模式
                    XmlNode logOutModeXmlNode = xmlDocument.SelectSingleNode("LogConfig/LogOutMode");
                    XmlElement logOutModeXmlElement = (XmlElement)logOutModeXmlNode;
                    logConfig.LogOutMode = logOutModeXmlElement.InnerText;
                    #endregion

                    #region 获取日志存放物理路径很目录
                    XmlNode logSaveRootPhyPathXmlNode = xmlDocument.SelectSingleNode("LogConfig/LogSaveRootPhysicalPath");
                    XmlElement logSaveRootPhyPathXmlElement = (XmlElement)logSaveRootPhyPathXmlNode;
                    logConfig.LogSaveRootPhysicalPath = logSaveRootPhyPathXmlElement.InnerText;
                    if (string.IsNullOrWhiteSpace(logConfig.LogSaveRootPhysicalPath))
                        logConfig.LogSaveRootPhysicalPath = System.AppDomain.CurrentDomain.BaseDirectory;
                    if (logConfig.LogSaveRootPhysicalPath.Contains("/"))
                        logConfig.LogSaveRootPhysicalPath = System.AppDomain.CurrentDomain.BaseDirectory + logConfig.LogSaveRootPhysicalPath.Replace("/", "\\");
                    #endregion

                    #region 获取是否开启记录Debug信息
                    XmlNode enabledDebugLogXmlNode = xmlDocument.SelectSingleNode("LogConfig/EnabledDebugLog");
                    XmlElement enabledDebugLogXmlElement = (XmlElement)enabledDebugLogXmlNode;
                    logConfig.EnabledDebugLog = Convert.ToBoolean(enabledDebugLogXmlElement.InnerText);
                    #endregion

                    #region 获取日志记录邮件实例
                    XmlNode enableMailXmlNode = xmlDocument.SelectSingleNode("LogConfig/Email/Enable");
                    XmlElement enableMailXmlElement = (XmlElement)enableMailXmlNode;
                    emailConfig.Enable = Convert.ToBoolean(enableMailXmlElement.InnerText);

                    XmlNode smtpServerXmlNode = xmlDocument.SelectSingleNode("LogConfig/Email/SmtpServer");
                    XmlElement smtpServerXmlElement = (XmlElement)smtpServerXmlNode;
                    emailConfig.SmtpServer = smtpServerXmlElement.InnerText;

                    XmlNode senderXmlNode = xmlDocument.SelectSingleNode("LogConfig/Email/Sender");
                    XmlElement senderXmlElement = (XmlElement)senderXmlNode;
                    emailConfig.Sender = senderXmlElement.InnerText;

                    XmlNode senderPwdXmlNode = xmlDocument.SelectSingleNode("LogConfig/Email/SenderPwd");
                    XmlElement senderPwdXmlElement = (XmlElement)senderPwdXmlNode;
                    emailConfig.SenderPwd = senderPwdXmlElement.InnerText;

                    XmlNode displayNameXmlNode = xmlDocument.SelectSingleNode("LogConfig/Email/DisplayName");
                    XmlElement displayNameXmlElement = (XmlElement)displayNameXmlNode;
                    emailConfig.DisplayName = displayNameXmlElement.InnerText;

                    XmlNode protXmlNode = xmlDocument.SelectSingleNode("LogConfig/Email/Prot");
                    XmlElement protXmlElement = (XmlElement)protXmlNode;
                    emailConfig.Prot = Convert.ToInt16(protXmlElement.InnerText);

                    List<string> toAddressList = new List<string>();
                    XmlNodeList toAddressListXmlNodeList = xmlDocument.SelectNodes("LogConfig/Email/ToAddressList");
                    foreach (XmlNode xmlNode in toAddressListXmlNodeList)
                    {
                        XmlElement xmlElement = (XmlElement)xmlNode;
                        string address = xmlElement.InnerText;
                        toAddressList.Add(address);
                    }
                    emailConfig.ToAddressList = toAddressList.ToArray();

                    XmlNode enableSSLXmlNode = xmlDocument.SelectSingleNode("LogConfig/Email/EnableSSL");
                    XmlElement enableSSLXmlElement = (XmlElement)enableSSLXmlNode;
                    emailConfig.EnableSSL = Convert.ToBoolean(enableSSLXmlElement.InnerText);

                    logConfig.Email = emailConfig;
                    #endregion
                }

                #endregion 日志配置文件内容发生更改时触发的事件
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取日志配置
        /// </summary>
        /// <returns>返回日志配置</returns>
        public LogConfig GetLogConfig()
        {
            return logConfig;
        }
    }
}