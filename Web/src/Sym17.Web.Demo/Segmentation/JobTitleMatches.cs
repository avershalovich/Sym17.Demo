using Serilog;
using Sitecore.Framework.Rules;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
using System;

namespace Sym17.Web.Demo.Segmentation
{
    public class JobTitleMatches : ICondition, IMappableRuleEntity
    {
        public string JobTitle { get; set; }

        public bool Evaluate(IRuleExecutionContext context)
        {
            var contact = RuleExecutionContextExtensions.Fact<Contact>(context);

            XConnectClient client = XConnectClientReference.GetClient();

            Contact existingContact = client.Get<Contact>(contact, new ContactExpandOptions(PersonalInformation.DefaultFacetKey));

            PersonalInformation personFacet = existingContact.GetFacet<PersonalInformation>(PersonalInformation.DefaultFacetKey);

            if (personFacet == null || string.IsNullOrEmpty(personFacet.JobTitle))
            {
                return false;
            }

            bool result = personFacet.JobTitle.IndexOf(this.JobTitle, StringComparison.OrdinalIgnoreCase) >= 0;

            Log.Information("JobTitleMatches result ==  " + result);

            return result;
        }
    }
}