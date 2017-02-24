using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Common
{ 
    /// <summary>
      /// 日志配置信息
      /// </summary>
    public class LogConfig
    {
        /// <summary>
        /// 日志开关,如果关闭开关系统把信息以异常方式抛出
        /// </summary>
        public bool EnableLog { get; set; }

        /// <summary>
        /// 日志输出模式（类名）
        /// </summary>
        public string LogOutMode { get; set; }

        /// <summary>
        /// 日志存放物理路径很目录
        /// </summary>
        public string LogSaveRootPhysicalPath { get; set; }

        /// <summary>
        /// 在非Debug或Debug模式强行开启记录Debug信息
        /// </summary>
        public bool EnabledDebugLog { get; set; }

        /// <summary>
        /// 日志记录邮件实例
        /// </summary>
        public EmailConfig Email { get; set; }
    }

    /// <summary>
    /// 发邮件配置信息
    /// </summary>
    public class EmailConfig
    {
        public bool Enable { get; set; }
        public string SmtpServer { get; set; }
        public string Sender { get; set; }
        public string SenderPwd { get; set; }
        public string DisplayName { get; set; }
        public int Prot { get; set; }
        public string[] ToAddressList { get; set; }
        public bool EnableSSL { get; set; }
    }
}