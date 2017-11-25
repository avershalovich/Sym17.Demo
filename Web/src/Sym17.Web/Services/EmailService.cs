using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Sym17.Web.Extentions;
using Sym17.Web.Models.Email;

namespace Sym17.Web.Services
{
    public class EmailService
    {
        public bool SendSmtpEmail(string emailTo, string url, HttpPostedFileBase attachment = null, string[] cc = null, string[] bcc = null, string attachmentPath = null)
        {

            var settings = new EmailSettings();

            var emailContent = new EmailMessageContent
            {
                Subject = settings.MessageSubject,
                Body = GetHtml(url)
            };
            var subject = emailContent.Subject;
            var bodyHtml = emailContent.Body;

            if (string.IsNullOrEmpty(emailTo))
            {
                throw new Exception("Recipient is empty!");
            }

            var sended = SendEmail(emailTo, subject, bodyHtml, settings, cc, bcc, attachment, attachmentPath);

            if (sended)
            {
                return true;
            }

            return false;

        }

        public bool SendEmail(string to, string subject, string htmlText, EmailSettings mailSettings, string[] cc = null, string[] bcc = null, HttpPostedFileBase attachment = null, string attachmentPath = null)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Brimit Team", mailSettings.FromName));
            message.To.Add(new MailboxAddress(to));
            message.Subject = subject;

            var multipart = new Multipart("mixed");

            if (!string.IsNullOrWhiteSpace(htmlText))
            {
                multipart.Add(new TextPart("html")
                {
                    Text = htmlText
                });
            }


            //multipart.Add(new TextPart
            //{
            //    Text = htmlText.ConvertHtmlToPlainText()
            //});


            if (cc != null)
            {
                var copies = cc.Where(x => x != to);
                foreach (var copy in copies)
                {
                    message.Cc.Add(new MailboxAddress(copy));
                }
            }
            if (bcc != null)
            {
                var copies = bcc.Where(x => x != to);
                foreach (var copy in copies)
                {
                    message.Bcc.Add(new MailboxAddress(copy));
                }
            }


            if (attachment != null)
            {
                var mime = new MimePart(attachment.ContentType)
                {
                    ContentObject = new ContentObject(attachment.InputStream),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = Path.GetFileName(attachment.FileName)
                };

                multipart.Add(mime);
            }

            if (!string.IsNullOrEmpty(attachmentPath))
            {
                var mime = AttachmentFromPath(attachmentPath);
                multipart.Add(mime);
            }

            message.Body = multipart;
            return SendAsync(message, mailSettings);
        }

        public bool SendAsync(MimeMessage message, EmailSettings mailSettings)
        {
            using (var client = new SmtpClient())
            {

                if (mailSettings.Ssl)
                {
                    client.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                }
                else
                {
                    client.Connect(mailSettings.Host, mailSettings.Port);
                }

                if (!string.IsNullOrEmpty(mailSettings.Username) && !string.IsNullOrEmpty(mailSettings.Password))
                {
                    client.Authenticate(mailSettings.Username, mailSettings.Password);
                }

                client.Send(message);
                client.Disconnect(true);
            }

            return true;
        }
        private MimePart AttachmentFromPath(string path)
        {
            // create an image attachment for the file located at path
            return new MimePart(System.Net.Mime.MediaTypeNames.Application.Pdf)
            {
                ContentObject = new ContentObject(System.IO.File.OpenRead(path)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(path)
            };
        }

        private static string GetHtml(string url)
        {
            try
            {
                var myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Credentials = CredentialCache.DefaultCredentials;

                var webResponse = myRequest.GetResponse();
                var response = webResponse.GetResponseStream();

                if (response != null)
                {
                    var ioStream = new StreamReader(response);
                    var pageContent = ioStream.ReadToEnd();
                    ioStream.Close();
                    response.Close();

                    return pageContent;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new FileLoadException(ex.Message);
            }
        }

        public static Uri AddParameter(string url, string paramName, string paramValue)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[paramName] = paramValue;
            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }
    }
}