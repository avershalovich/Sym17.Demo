using Sitecore.Marketing.Automation.Activity;
using System;
using Microsoft.Extensions.Logging;
using Sitecore.Xdb.MarketingAutomation.Core.Activity;
using Sitecore.Xdb.MarketingAutomation.Core.Processing.Plan;
using Sitecore.XConnect.Collection.Model;
using Sitecore.Marketing.Automation.Models;
using System.Globalization;
using System.Configuration;
using Sym17.Web.Services;

namespace Sym17.Web.Demo.Activity
{
    public class SendAgendaEmail : BaseActivity
    {
        
        public SendAgendaEmail(ILogger<ValidateEmailAddress> logger) : base((ILogger<IActivity>)logger)
        {

        }

        public override ActivityResult Invoke(IContactProcessingContext context)
        {
            EmailAddressList facet = context.Contact.GetFacet<EmailAddressList>();

            if (facet == null)
                return (ActivityResult)new Failure(Resources.TheEmailAddressListFacetHasNotBeenSetSuccessfully);

            string email = facet.PreferredEmail.SmtpAddress;

            LoggerExtensions.LogInformation(this.Logger, "processing agenda email for: " + email);

            //Get agenda url
            string urlformat = ConfigurationManager.AppSettings["agendaurlformat"];

            //add contact id as parameter
            Guid? contactID = context.Contact.Id;

            string url = string.Format(urlformat, contactID);

            LoggerExtensions.LogInformation(this.Logger, "processing agenda for url: " + url);

            //make request, get body, send email
            var emailService = new EmailService();
            var sended = emailService.SendSmtpEmail(email, url);
            if (!sended)
            {
                LoggerExtensions.LogInformation(this.Logger, "Message was not sent");
            }

            LoggerExtensions.LogDebug((ILogger)this.Logger, string.Format((IFormatProvider)CultureInfo.InvariantCulture, Resources.TheFacetHasBeenSetAndSubmittedSuccessfullyPattern, (object)"EmailAddressList"), Array.Empty<object>());
            return (ActivityResult)new SuccessMove();
        }
    }
}