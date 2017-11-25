namespace Sym17.Web
{
    using System;
    using System.Configuration;

    public static class Settings
    {
        public const string FaceApiIdentificationSource = "faceApi";

        public const string EmailIdentificationSource = "email";

        public static Guid OfflineGoalId = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["interaction.OfflineGoalId"]) ?
            Guid.Parse("{75D53206-47B3-4391-BD48-75C42E5FC2CE}") //  goal
            : Guid.Parse(ConfigurationManager.AppSettings["interaction.OfflineGoalId"]);

        public static Guid OnlineGoalId = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["interaction.OnlineGoalId"]) ?
            Guid.Parse("{475E9026-333F-432D-A4DC-52E03B75CB6B}") // "Agenda signup" goal
            : Guid.Parse(ConfigurationManager.AppSettings["interaction.OnlineGoalId"]);
        

        public static Guid OfflineChannelId = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["interaction.OfflineChannelId"]) ?
            Guid.Parse("{51D98AB2-09EE-48C6-9434-659807C2444C}") // "Industry event sponsorship" channel
            : Guid.Parse(ConfigurationManager.AppSettings["interaction.OfflineChannelId"]);

        
        public static Guid OnlineChannelId = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["interaction.OnlineChannelId"]) ?
            Guid.Parse("{BA1D62CB-4224-47D0-AF59-78A8972FD538}") // "Other email" channel
            : Guid.Parse(ConfigurationManager.AppSettings["interaction.OnlineChannelId"]);


        public static Guid RelationshipProfileId = Guid.Parse("{41f00a31-f64b-4b48-9eb0-2a72717ddfb1}");
        public static Guid ClientBehaviorProfileId = Guid.Parse("{7D9C33C7-5A8C-44FD-82DA-0DDD23DE33FF}");
        public static Guid SitecoreBehaviorProfileId = Guid.Parse("{BC3057BC-C6A9-4D86-B099-8272EDEDC720}");
        public static Guid PartnerBehaviorProfileId = Guid.Parse("{D424859E-2EA7-48A6-AAB6-0FD0CBDF4D07}");

        public static Guid RoleProfileId = Guid.Parse("{89B7FB12-181D-4F38-AE5C-38C61F49BCE6}");
        public static Guid DevelopmentBehaviorProfileId = Guid.Parse("{2E0754B2-3FCC-426C-B3BA-6E169C308620}");
        public static Guid BusinessDevelopmentBehaviorProfileId = Guid.Parse("{E8224392-8010-4E2C-9F5E-8CBEB70984AD}");
        public static Guid MarketingBehaviorProfileId = Guid.Parse("{848BCFB0-4872-4AB8-AA19-E05180D5A167}");
    }
}