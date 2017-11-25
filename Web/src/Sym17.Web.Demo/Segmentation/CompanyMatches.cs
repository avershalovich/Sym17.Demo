using Serilog;
using Sitecore.Framework.Rules;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Segmentation.Predicates;
using Sym17.Web.Models.Facets.Contact;
using System;
using System.Linq.Expressions;

namespace Sym17.Web.Demo.Segmentation
{
    public class CompanyMatches : ICondition, IMappableRuleEntity, IContactSearchQueryFactory
    {
        public string Company { get; set; }

        public bool Evaluate(IRuleExecutionContext context)
        {
            var contact = RuleExecutionContextExtensions.Fact<Contact>(context);

            XConnectClient client = XConnectClientReference.GetClient();

            Contact existingContact = client.Get<Contact>(contact, new ContactExpandOptions(CvPersonFacet.DefaultFacetKey));

            CvPersonFacet personFacet = existingContact.GetFacet<CvPersonFacet>(CvPersonFacet.DefaultFacetKey);

            if (personFacet?.Company == null)
            {
                return false;
            }

            bool result = personFacet.Company.IndexOf(this.Company, StringComparison.OrdinalIgnoreCase) >= 0;

            Log.Information("CompanyMatches personFacet.Company ==  " + personFacet.Company);

            Log.Information("CompanyMatches result ==  " + result);

            return result;
        }

        public Expression<Func<Contact, bool>> CreateContactSearchQuery(IContactSearchQueryContext context)
        {
            Log.Information("CreateContactSearchQuery start " + context.ToString());

            return contact => contact.GetFacet<CvPersonFacet>(CvPersonFacet.DefaultFacetKey).Company == this.Company;
        }

        
    }
}