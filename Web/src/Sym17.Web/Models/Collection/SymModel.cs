
using Sitecore.XConnect;
using Sitecore.XConnect.Schema;
using Sym17.Web.Models.Facets;
using Sym17.Web.Models.Facets.Contact;
using Sym17.Web.Models.Facets.Interaction;

namespace Sym17.Web.Models.Collection
{
    public class SymModel
    {
        public static XdbModel Model { get; } = BuildModel();

        private static XdbModel BuildModel()
        {
            XdbModelBuilder modelBuilder = new XdbModelBuilder("SymModel", new XdbModelVersion(1, 0));

            modelBuilder.DefineFacet<Sitecore.XConnect.Contact, CvPersonInfo>(FacetKeys.CvPersonInfo);
            modelBuilder.DefineFacet<Sitecore.XConnect.Interaction, FaceApiInfo>(FacetKeys.FaceApiInfo);

            modelBuilder.ReferenceModel(Sitecore.XConnect.Collection.Model.CollectionModel.Model);

            return modelBuilder.BuildModel();
        }
    }
}