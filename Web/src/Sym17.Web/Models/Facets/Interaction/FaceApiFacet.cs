namespace Sym17.Web.Models.Facets.Interaction
{
    using Sitecore.XConnect;

    [FacetKey(DefaultFacetKey)]
    public class FaceApiFacet : Facet
    {
        /// <summary>
        /// FaceAPI based
        /// </summary>
        public double SmileValue { get; set; }

        public double AngerValue { get; set; }

        public double ContemptValue { get; set; }

        public double DisgustValue { get; set; }

        public double FearValue { get; set; }

        public double HappinessValue { get; set; }

        public double NeutralValue { get; set; }

        public double SadnessValue { get; set; }

        public double SurpriseValue { get; set; }

        public const string DefaultFacetKey = FacetKeys.FaceApiInfo;
    }
}