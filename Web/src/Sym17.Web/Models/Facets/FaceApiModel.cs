namespace Sym17.Web.Models.Facets
{
    using Contact;

    using Interaction;
    
    using Sitecore.XConnect.Schema;

    public class FaceApiModel
    {
        public static XdbModel Model { get; } = FaceApiModel.BuildModel();

        private static XdbModel BuildModel()
        {
            XdbModelBuilder modelBuilder = new XdbModelBuilder("FaceApiModel", new XdbModelVersion(0, 1));

            modelBuilder.ReferenceModel(Sitecore.XConnect.Collection.Model.CollectionModel.Model);

            modelBuilder.DefineFacet<Sitecore.XConnect.Interaction, FaceApiFacet>("FaceApiFacet");
            modelBuilder.DefineFacet<Sitecore.XConnect.Contact, CvPersonFacet>("CvPersonFacet");


            return modelBuilder.BuildModel();
        }
    }
}