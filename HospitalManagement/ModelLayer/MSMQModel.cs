using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using System.Text;
using Experimental.System.Messaging;

namespace ModelLayer
{
    public class MSMQModel
    {
        MessageQueue messageQueue = new MessageQueue();
        private string recieverEmailAddr;
        private string recieverName;

        //Method To Send Token Using MessageQueue And Delegate
        public void SendMessage(string token, string emailId, string name)
        {
            recieverEmailAddr = emailId;
            recieverName = name;
            messageQueue.Path = @".\Private$\Token";
            try
            {
                if (!MessageQueue.Exists(messageQueue.Path))
                {
                    MessageQueue.Create(messageQueue.Path);
                }
                messageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                messageQueue.ReceiveCompleted += MessageQueue_ReceiveCompleted;
                messageQueue.Send(token);
                messageQueue.BeginReceive();
                messageQueue.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Delegate To Send Token As Message To The Sender EmailId Using Smtp And MailMessage

        private void MessageQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            try
            {
                var msg = messageQueue.EndReceive(e.AsyncResult);
                string token = msg.Body.ToString();
                MailMessage mailMessage = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential("porsu.jaswanthkumar@gmail.com", "yhri qogf kqdm hcat\r\n"),
                };
                mailMessage.From = new MailAddress("porsu.jaswanthkumar@gmail.com");
                mailMessage.To.Add(new MailAddress(recieverEmailAddr));
                string mailBody = $"<!DOCTYPE html>" +
                                  $"<html>" +
                                  $"<style>" +
                                  $".blink" +
                                  $"</style>" +
                                    $"<body style = \"background-color:#DBFF73;text-align:center;padding:5px;\">" +
                                    $"<h1 style = \"color:#6A8D02;border-bottom: 3px solid #84AF08;margin-top: 5px;\"> Dear <b>{recieverName}</b> </h1>\n" +
                                    $"<h3 style = \"color:#BAB411;\"> For Ressetting Password The Below Link Is Issued</h3>" +
                                    $"<h3 style = \"color:#BAB411;\"> Please Click The Link Below To Reset Your Password</h3>" +
                                    $"<a style = \"color:#00802b; text-decoration: none; font-size: 20px;\" href = 'http://localhost:4200/resetpwd/{token}'> Click me</a>\n" +
                                    $"<h3 style = \"color:#BAB411;margin-bottom: 5px;\"><blink> This Token Will be Valid For Next 6 Hours</blink></h3>" +
                                    $"</body>" +
                                    $"</html>";
                mailMessage.Body = mailBody;
                mailMessage.IsBodyHtml = true;
                mailMessage.Subject = "Book Store Password Reset Link";
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
