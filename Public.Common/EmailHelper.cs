using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Public.Common
{
    public class EmailHelper
    {
        private EmailConfig emailConfig = new EmailConfig();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_emailConfig"></param>
        public EmailHelper(EmailConfig emailConfig)
        {
            this.emailConfig = emailConfig;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="receiver">收件人，可多个</param>
        /// <param name="title">邮件标题</param>
        /// <param name="body">邮件内容</param>
        /// <returns>是否成功发送</returns>
        public void SendEmail(string title, string mailBody, bool isBodyHtml)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(emailConfig.Sender, emailConfig.DisplayName); // 发件人邮箱地址和显示名称
            message.SubjectEncoding = System.Text.Encoding.GetEncoding("GB18030");
            message.BodyEncoding = System.Text.Encoding.GetEncoding("GB18030");

            for (int i = 0; i < emailConfig.ToAddressList.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(emailConfig.ToAddressList[i]))
                {
                    message.To.Add(new MailAddress(emailConfig.ToAddressList[i]));
                }
            }

            message.Subject = title;//邮件标题
            message.Body = mailBody;//邮件正文
            message.IsBodyHtml = isBodyHtml; // 设置邮件正文是否为html格式的值
            message.Priority = MailPriority.High; // 设置电子邮件的优先级

            SmtpClient client = new SmtpClient();
            client.Host = emailConfig.SmtpServer; // 设置smtp事务的主机名称或 IP 地址
            client.Port = emailConfig.Prot; // 端口号
            client.Credentials = new NetworkCredential(emailConfig.Sender, emailConfig.SenderPwd);
            client.EnableSsl = emailConfig.EnableSSL;
            try
            {
                client.Send(message);
                message.To.Clear();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
            }
            finally
            {
                //释放资源
                if (message != null)
                {
                    message.Dispose();
                }
                if (client != null)
                {
                    client.Dispose();
                }
            }
        }

        #region 邮件发送失败写日志
        private void WriteLog(Exception ex)
        {
            StringBuilder log = new System.Text.StringBuilder();
            log.AppendFormat("【{0}】", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));

            log.AppendFormat("\r\n【异常信息】\r\n{0}\r\n", ex);
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\EmailExceptionLog\\";
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            path += DateTime.Now.ToString("yyyyMMddHH") + ".txt";
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                    {
                        sw.Write(log);
                    }
                }
            }
            catch (Exception except)
            {
                Log_Helper logHelper = new Log_Helper();
                logHelper.Error("EmailHelper.WriteLog执行中出现异常", except);
                throw except;
            }
        }
        #endregion
    }
}