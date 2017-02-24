using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Public.Common
{
    /// <summary>
    /// 日志实体
    /// </summary>
    public class LogInfo
    {
        private Dictionary<string, object> parameters;
        /// <summary>
        /// 参数列表
        /// </summary>
        public Dictionary<string, object> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }
        private string description;
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        private Exception exception;
        /// <summary>
        /// 异常
        /// </summary>
        public Exception Exception
        {
            get { return exception; }
            set { exception = value; }
        }
        private LogLevel logLevel;
        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel LogLevel
        {
            get { return logLevel; }
            set { logLevel = value; }
        }
        private string code;
        /// <summary>
        /// 代号（做为文件夹名。默认为下划线“_”）
        /// </summary>
        public string Code
        {
            set { code = value; }
            get
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    return "_";
                }
                else
                {
                    return code;
                }
            }
        }
        public DateTime LogTime { get { return DateTime.Now; } }
    }

    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 致命错误，导致程序不能正常运行
        /// </summary>
        Fatal,
        /// <summary>
        /// 普通错误，不会影响程序运行
        /// </summary>
        Error,
        /// <summary>
        /// 警告日志
        /// </summary>
        Warn,
        /// <summary>
        /// 消息日志 (不发邮件)
        /// </summary>
        Info,
        /// <summary>
        /// 调试日志，只在调试模式下记录(不发邮件)
        /// </summary>
        Debug
    }
}