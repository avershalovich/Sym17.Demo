namespace Sym17.Web.Models.PowerBI
{
    public class PBIOfflineInteraction : PBIInteraction
    {
        public bool HasFaceInfo { get; set; }

        public double SmileValue { get; set; }

        public double AngerValue { get; set; }

        public double ContemptValue { get; set; }

        public double DisgustValue { get; set; }

        public double FearValue { get; set; }

        public double HappinessValue { get; set; }

        public double NeutralValue { get; set; }
        
        public double SadnessValue { get; set; }

        public double SurpriseValue { get; set; }
    }
}