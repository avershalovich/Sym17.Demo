using Serilog;
using Sitecore.Framework.Rules;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sym17.Web.Models.Facets.Contact;
using System;
using System.Linq;

namespace Sym17.Web.Demo.Segmentation
{
    public class BadgeTextMatches : ICondition, IMappableRuleEntity
    {
        public string BadgeText { get; set; }

        public bool Evaluate(IRuleExecutionContext context)
        {
            var contact = RuleExecutionContextExtensions.Fact<Contact>(context);

            XConnectClient client = XConnectClientReference.GetClient();

            Contact existingContact = client.Get<Contact>(contact, new ContactExpandOptions(CvPersonFacet.DefaultFacetKey));

            CvPersonFacet personFacet = existingContact.GetFacet<CvPersonFacet>(CvPersonFacet.DefaultFacetKey);

            if (personFacet?.BadgeText == null)
            {
                return false;
            }

            bool result = personFacet.BadgeText
                .Any(line => line.IndexOf(this.BadgeText, StringComparison.OrdinalIgnoreCase) >= 0);

            Log.Information("BadgeTextMatches result ==  " + result);

            return result;
        }
    }
}