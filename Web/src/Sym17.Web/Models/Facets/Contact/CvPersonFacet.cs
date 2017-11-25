namespace Sym17.Web.Models.Facets.Contact
{
    using System.Collections.Generic;

    using Sitecore.XConnect;
    using Sitecore.XConnect.Schema;

    [FacetKey(DefaultFacetKey)]
    public class CvPersonFacet : Facet
    {
        public const string DefaultFacetKey = FacetKeys.CvPersonInfo;

        public string Id { get; set; }

        public string Photo { get; set; }

        [PIISensitive]
        public string Gender { get; set; }

        [PIISensitive]
        public int Age { get; set; }

        [PIISensitive]
        public string FirstName { get; set; }

        [PIISensitive]
        public string LastName { get; set; }

        public string Company { get; set; }

        public List<string> BadgeText { get; set; }
    }
}