using Serilog;
using Sitecore.Framework.Rules;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Segmentation.Predicates;
using Sym17.Web.Models.Facets.Contact;

namespace Sym17.Web.Demo.Segmentation
{
    public class AgeMatches : ICondition, IMappableRuleEntity
    {
        public int Age { get; set; }
        public NumericOperationType Comparison { get; set; }

        public bool Evaluate(IRuleExecutionContext context)
        {
            var contact = RuleExecutionContextExtensions.Fact<Contact>(context);

            XConnectClient client = XConnectClientReference.GetClient();

            Contact existingContact = client.Get<Contact>(contact, new ContactExpandOptions(CvPersonFacet.DefaultFacetKey));

            CvPersonFacet personFacet = existingContact.GetFacet<CvPersonFacet>(CvPersonFacet.DefaultFacetKey);

            Log.Information("AgeMatches personFacet is null ==  " + (personFacet == null).ToString());

            if (personFacet == null || personFacet.Age == 0)
            {
                return false;
            }

            Log.Information("AgeMatches personFacet.Age ==  " + personFacet.Age);

            bool result = NumericOperationTypeExtensions.Evaluate(this.Comparison, personFacet.Age, this.Age);

            Log.Information("AgeMatches result ==  " + result.ToString());

            return result;
        }
    }
}