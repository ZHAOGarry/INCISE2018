using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace INCISE2018_MVC.Managers
{
    public class MailManager
    {
        public string DisplayName { get; set; }
        public string SendUserAddress { get; set; }
        public string SendUserName { get; set; }
        public string SendUserPassword { get; set; }
        public string MailTitle { get; set; }
        public string MailContent { get; set; }
        public bool UseHtml { get; set; }
        public bool UseSSL { get; set; }
        public string Host { get; private set; }
        public int Port { get; private set; }
        public string ReciveUserAddress { get; set; }

        public MailManager()
        {
            DefaultSetting();
        }

        /// <summary>
        /// 邮件管理的默认设定
        /// </summary>
        private void DefaultSetting()
        {
            SendUserAddress = "INCISE2018@sustc.edu.cn";
            DisplayName = "INCISE2018";
            SendUserName = SendUserAddress;
            SendUserPassword = "Ln147258";
            UseHtml = true;
            UseSSL = false;
            Host = "smtp.exmail.qq.com";
            Port = 0;
            MailTitle = "测试邮件";
            MailContent = "这是一封用于系统功能性测试的邮件";
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        public async Task Send()
        {
            MailAddress FromAddress = new MailAddress(SendUserAddress, DisplayName);
            MailAddress ToAddress = new MailAddress(ReciveUserAddress);
            MailMessage Mail = new MailMessage(FromAddress, ToAddress);
            Mail.Subject = MailTitle;
            Mail.IsBodyHtml = UseHtml;
            Mail.Body = MailContent;
            using (SmtpClient client = new SmtpClient())
            {
                client.Host = Host;
                client.UseDefaultCredentials = false;
                if (Port != 0)
                {
                    client.Port = Port;
                }
                client.Credentials = new NetworkCredential(SendUserName, SendUserPassword);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                try
                {
                    await client.SendMailAsync(Mail);
                    return;
                }
                catch (SmtpException e)
                {
                    return;
                }
            }
        }
    }
}