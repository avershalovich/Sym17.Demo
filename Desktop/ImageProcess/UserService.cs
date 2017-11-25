using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace ImageProcess
{
    public class UserService
    {
        private readonly DataModel _dbContext;

        public UserService()
        {
            _dbContext = DataModel.Create();
            if (!_dbContext.Database.Exists())
            {
                MessageBox.Show("Cannot connect to SQL Server");
            }
        }

        public void Test()
        {
            try
            {
               _dbContext.Users.FirstOrDefault();
            }
            catch (Exception ex)
            {
                var message = "Can`t connect to SQL Server!\n";
                message += ex.Message;
                if (ex.InnerException != null)
                {
                    message += "\n" + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        message += "\n" + ex.InnerException.InnerException.Message;
                    }
                    message += "\nConnectionString: " + _dbContext.Database.Connection.ConnectionString;
                }
                MessageBox.Show(message);
            }
            
        }
        public bool Exist(User user)
        {
            var firstName = user.FirstName.ToTitleCase();
            var lastName = user.LastName.ToTitleCase();
            var company = user.Company.ToTitleCase();

            return
                _dbContext.Users.Any(
                    x => x.FirstName.Equals(firstName) && x.LastName.Equals(lastName) && x.Company.Equals(company));
        }

        public bool Add(User user)
        {
            user.FirstName = user.FirstName.ToTitleCase();
            user.LastName = user.LastName.ToTitleCase();
            user.Company = user.Company.ToTitleCase();
            user.Gender = user.Gender.ToTitleCase();

            user.Status = UserStatus.Init;

            try
            {
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding user to DB: " + ex.Message);
                return false;
            }
        }

        public bool SetStatus(Guid id, UserStatus status)
        {
            try
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
                if (user == null) return false;

                user.Status = status;
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error changing user status in DB: " + ex.Message);
                return false;
            }
        }
    }

    public static class StringExt
    {
        public static string ToTitleCase(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(text);
        }
    }
}
