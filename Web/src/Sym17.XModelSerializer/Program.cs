namespace Sym17.XModelSerializer
{
    using System;

    using Sitecore.XConnect;
    using Sitecore.XConnect.Schema;

    using Web.Models.Facets;
    using Web.Models.Facets.Contact;
    using Web.Models.Facets.Interaction;

    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("FaceApiModel: ");
            //string json = Sitecore.XConnect.Serialization.XdbModelWriter.Serialize(BuildFaceApiFacetModel());
            string json = Sitecore.XConnect.Serialization.XdbModelWriter.Serialize(FaceApiModel.Model);
            Console.Write(json);
            System.IO.File.WriteAllText($".\\{FaceApiModel.Model.FullName}.json", json);

            Console.ReadKey();
        }
    }
}
