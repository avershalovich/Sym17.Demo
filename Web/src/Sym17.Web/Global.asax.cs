namespace Sym17.Web
{
    using Sym17.Web.Hubs;

    using System.Web.Mvc;
    using System.Web.Routing;

    using Microsoft.AspNet.SignalR;

    using Models.Contact;
    using Models.Facets.Interaction;

    using Services;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteTable.Routes.MapHubs(new HubConfiguration()
            {
                EnableCrossDomain = true
            });
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ActiveVisitor.VisitorChanged += UpdateContact;
        }

        private void UpdateContact(object sender, ActiveVisitorChangedEventArgs e)
        {
            ImageProcessingContact c = e.ImageProcessingContact;
            ActiveVisitorHub.Change(c);

            Xservice xservice = new Xservice();

            var contact = xservice.GetContact(Settings.FaceApiIdentificationSource, c.Id);

            if (contact == null)
            {
                xservice.AddContact(Settings.FaceApiIdentificationSource, c.Id, c);

                var faceApiFacet = new FaceApiFacet
                {
                    NeutralValue = c.Neutral,
                    HappinessValue = c.Happiness,
                    SmileValue = c.Smile,
                    AngerValue = c.Anger,
                    SurpriseValue = c.Surprise,
                    SadnessValue = c.Sadness,
                    FearValue = c.Fear,
                    DisgustValue = c.Disgust,
                    ContemptValue = c.Contempt
                };

                xservice.RegisterOfflineInteraction(Settings.FaceApiIdentificationSource, c.Id, faceApiFacet);
            }
        }
    }
}
