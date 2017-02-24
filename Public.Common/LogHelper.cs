using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Public.Common
{
    public class Log_Helper
    {
        /// <summary>
        /// 是否为开启记录Debug信息
        /// </summary>
        public bool IsDebugEnabled { get; private set; }
        /// <summary>
        /// 是否为开启记录Info信息
        /// </summary>
        public bool IsInfoEnabled { get; private set; }
        /// <summary>
        /// 是否为开启记录Warn信息
        /// </summary>
        public bool IsWarnEnabled { get; private set; }
        /// <summary>
        /// 是否为开启记录Error信息
        /// </summary>
        public bool IsErrorEnabled { get; private set; }
        /// <summary>
        /// 是否为开启记录Fatal信息
        /// </summary>
        public bool IsFatalEnabled { get; private set; }

        private LogConfig logConfig;
        private EmailHelper emailHelper;
        private string logSaveRootPath = string.Empty;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="logConfig"></param>
        public Log_Helper()
        {
            ReadLogConfig readLogConfig = new ReadLogConfig();
            this.logConfig = readLogConfig.GetLogConfig();

            //当开启记录日志，那么只有为调试模式时才可以记录Debug级别的信息
            if (logConfig.EnableLog)
            {
                IsErrorEnabled = true;
                IsFatalEnabled = true;
                IsInfoEnabled = true;
                IsWarnEnabled = true;

#if DEBUG
                IsDebugEnabled = true;
#endif
            }
            if (logConfig.EnabledDebugLog) IsDebugEnabled = true;

            if (!System.IO.Directory.Exists(logConfig.LogSaveRootPhysicalPath))
            {
                throw new Exception(string.Format("日志存放目录不存在：{0}", logConfig.LogSaveRootPhysicalPath));
            }

            logSaveRootPath = logConfig.LogSaveRootPhysicalPath;

            if (logConfig.Email == null)
                throw new Exception("配置日志出错： logConfig.Email == null Email配置节点不正确");
            emailHelper = new EmailHelper(logConfig.Email);
        }



        #region 调试信息（不发邮件，非调试模式下无效，仅用于调试信息请在发布时删除或者使用 #if DEBUG 条件指令）
        /// <summary>
        /// 调试信息（不发邮件，非调试模式下无效）
        /// </summary>
        /// <param name="description">描述</param>
        /// <param name="code">代号（做为文件夹名。默认为下划线“_”）</param>
        public void Debug(string description, string code = null)
        {
            Debug(description, null, null, code);
        }
        #endregion

        #region 调试信息（不发邮件，非调试模式下无效，仅用于调试信息请在发布时删除或者使用 #if DEBUG 条件指令）
        /// <summary>
        /// 调试信息（不发邮件，非调试模式下无效，仅用于调试信息请在发布时删除或者使用 #if DEBUG 条件指令）
        /// </summary>
        /// <param name="description">描述</param>
        /// <param name="ex">异常</param>
        /// <param name="parameters">参数列表</param>
        /// <param name="code">代号（做为文件夹名。默认为下划线“_”）</param>
        public void Debug(string description, Exception ex, Dictionary<string, object> parameters, string code = null)
        {
            Write(description, LogLevel.Debug, ex, parameters, code);
        }
        #endregion


        #region 普通信息(不发邮件)
        /// <summary>
        /// 普通信息(不发邮件)
        /// </summary>
        /// <param name="description">描述</param>
        /// <param name="code">代号（做为文件夹名。默认为下划线“_”）</param>
        public void Info(string description, string code = null)
        {
            Info(description, null, null, code);
        }
        #endregion

        #region 普通信息(不发邮件)
        /// <summary>
        /// 普通信息(不发邮件)
        /// </summary>
        /// <param name="description">描述</param>
        /// <param name="ex">异常</param>
        /// <param name="parameters">参数列表</param>
        /// <param name="code">代号（做为文件夹名。默认为下划线“_”）</param>
        public void Info(string description, Exception ex, Dictionary<string, object> parameters, string code = null)
        {
            Write(description, LogLevel.Info, ex, parameters, code);
        }
        #endregion

        #region 普通错误，不会影响程序运行
        /// <summary>
        /// 普通错误，不会影响程序运行
        /// </summary>
        /// <param name="description">描述</param>
        /// <param name="ex">异常</param>
        /// <param name="code">代号（做为文件夹名。默认为下划线“_”）</param>
        public void Error(string description, Exception ex, string code = null)
        {
            Error(description, ex, null, code);
        }
        #endregion

        #region 普通错误，不会影响程序运行
        /// <summary>
        /// 普通错误，不会影响程序运行
        /// </summary>
        /// <param name="description">描述</param>
        /// <param name="ex">异常</param>
        /// <param name="parameters">参数列表</param>
        /// <param name="code">代号（做为文件夹名。默认为下划线“_”）</param>
        public void Error(string description, Exception ex, Dictionary<string, object> parameters, string code = null)
        {
            Write(description, LogLevel.Error, ex, parameters, code);
        }
        #endregion

        #region 致命错误，导致程序不能正常运行
        /// <summary>
        /// 致命错误，导致程序不能正常运行
        /// </summary>
        /// <param name="description">描述</param>
        /// <param name="ex">异常</param>
        /// <param name="code">代号（做为文件夹名。默认为下划线“_”）</param>
        public void Fatal(string description, Exception ex, string code = null)
        {
            Fatal(description, ex, null, code);
        }
        #endregion

        #region 致命错误，导致程序不能正常运行
        /// <summary>
        /// 致命错误，导致程序不能正常运行
        /// </summary>
        /// <param name="description">描述</param>
        /// <param name="ex">异常</param>
        /// <param name="parameters">参数列表</param>
        /// <param name="code">代号（做为文件夹名。默认为下划线“_”）</param>
        public void Fatal(string description, Exception ex, Dictionary<string, object> parameters, string code = null)
        {
            Write(description, LogLevel.Fatal, ex, parameters, code);
        }
        #endregion

        /// <summary>
        /// Write 委托
        /// </summary>
        /// <param name="loginfo"></param>
        public delegate void AsyncWriteCaller(LogInfo loginfo);

        /// <summary>
        /// 如果开启日志则写入日志
        /// </summary>
        /// <param name="description"></param>
        /// <param name="logLevel"></param>
        /// <param name="ex"></param>
        /// <param name="parameters"></param>
        /// <param name="code"></param>
        private void Write(string description, LogLevel logLevel, Exception ex, Dictionary<string, object> parameters, string code)
        {
            LogInfo loginfo = new LogInfo() { Parameters = parameters, Description = description, LogLevel = logLevel, Exception = ex, Code = code };
            if (logConfig.EnableLog)
            {
                #region 异步执行
                AsyncWriteCaller write = new AsyncWriteCaller(Write);
                write.BeginInvoke(loginfo, null, null);
                #endregion
            }
        }

        private void Write(LogInfo loginfo)
        {
            switch (loginfo.LogLevel)
            {
                case LogLevel.Fatal:
                    if (!IsFatalEnabled) return;
                    break;
                case LogLevel.Error:
                    if (!IsErrorEnabled) return;
                    break;
                case LogLevel.Warn:
                    if (!IsWarnEnabled) return;
                    break;
                case LogLevel.Info:
                    if (!IsInfoEnabled) return;
                    break;
                case LogLevel.Debug:
                    if (!IsDebugEnabled) return;
                    break;
            }
            WriteLog(loginfo);
            switch (loginfo.LogLevel)//不发邮件
            {
                case LogLevel.Info:
                case LogLevel.Debug:
                    return;
            }
            try
            {
                SendToEmail(loginfo);
            }
            catch
            {

            }
        }

        private void SendToEmail(LogInfo loginfo)
        {
            StringBuilder text = new StringBuilder();
            text.AppendFormat("【时间】{0}", loginfo.LogTime.ToString("yyyy-MM-dd HH:mm:ss:fff"));

            text.AppendFormat("\r\n【描述】{0}", loginfo.Description);

            if (loginfo.Exception != null)
                text.AppendFormat("\r\n【异常信息】{0}", loginfo.Exception);
            if (loginfo.Parameters != null)
            {
                foreach (KeyValuePair<string, object> item in loginfo.Parameters)
                {
                    text.AppendFormat("\r\n【{0}】{1}", item.Key, item.Value);
                }

            }
            text.AppendFormat("\r\n【日志级别】{0}", loginfo.LogLevel.ToString());
            if (logConfig.Email.Enable)
            {
                emailHelper.SendEmail(string.Format("{0}", loginfo.LogLevel), text.ToString(), false);
            }
        }

        /// <summary>
        /// 写
        /// </summary>
        /// <param name="loginfo"></param>
        public void WriteLog(LogInfo loginfo)
        {
            StringBuilder text = new StringBuilder();
            text.AppendFormat("【时间】{0}", loginfo.LogTime.ToString("yyyy-MM-dd HH:mm:ss:fff"));

            text.AppendFormat("\r\n【描述】{0}", loginfo.Description);

            if (loginfo.Exception != null)
                text.AppendFormat("\r\n【异常信息】{0}", loginfo.Exception);
            if (loginfo.Parameters != null)
            {
                foreach (KeyValuePair<string, object> item in loginfo.Parameters)
                {
                    text.AppendFormat("\r\n【{0}】{1}", item.Key, item.Value);
                }

            }
            text.AppendFormat("\r\n【日志级别】{0}", loginfo.LogLevel.ToString());
            text.Append("\r\n\r\n");
            string path = string.Format("{0}\\App_Data\\Log\\{1}\\{2}\\{3}\\", logSaveRootPath, loginfo.LogLevel, loginfo.Code, DateTime.Now.ToString("yyyyMM"));

            WriteToFile(Path.Combine(path, DateTime.Now.ToString("yyyyMMddHH") + ".txt"), text.ToString(), true);
        }

        private void WriteToFile(string path, string text, bool append)
        {

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (FileStream fs = new FileStream(path, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(text);
                }
            }
        }
    }
}