using Serilog;
using Sitecore.Framework.Rules;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
using System;

namespace Sym17.Web.Demo.Segmentation
{
    public class GenderMatches : ICondition, IMappableRuleEntity
    {
        public string Gender { get; set; }

        public bool Evaluate(IRuleExecutionContext context)
        {
            var contact = RuleExecutionContextExtensions.Fact<Contact>(context);

            XConnectClient client = XConnectClientReference.GetClient();

            Contact existingContact = client.Get<Contact>(contact, new ContactExpandOptions(PersonalInformation.DefaultFacetKey));

            PersonalInformation personFacet = existingContact.GetFacet<PersonalInformation>(PersonalInformation.DefaultFacetKey);

            if (string.IsNullOrEmpty(personFacet?.Gender))
            {
                return false;
            }

            bool result = personFacet.Gender.Equals(this.Gender, StringComparison.OrdinalIgnoreCase);

            Log.Information("GenderMatches personFacet.Gender ==  " + personFacet.Gender);

            Log.Information("GenderMatches result ==  " + result);

            return result;
        }
    }
}