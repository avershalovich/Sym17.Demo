using System.Configuration;

namespace Sym17.Web.Models.Email
{
    public class EmailSettings
    {
        public string MessageSubject { get { return ConfigurationManager.AppSettings["smtp.message.subject"]; } }
        public string Host { get { return ConfigurationManager.AppSettings["smtp.host"]; } }
 
        public int Port { get { return int.Parse(ConfigurationManager.AppSettings["smtp.post"]); } }

        public bool Ssl { get { return bool.Parse(ConfigurationManager.AppSettings["smtp.ssl"]); } }

        public string FromName { get { return ConfigurationManager.AppSettings["smtp.from"]; } }

        public string Username { get { return ConfigurationManager.AppSettings["smtp.login"]; } }

        public string Password { get { return ConfigurationManager.AppSettings["smtp.password"]; } }

        public override string ToString()
        {
            return "EmailSettings: Host=" + Host + ", Port=" + Port + ", Login=" + Username;
        }
    }
}