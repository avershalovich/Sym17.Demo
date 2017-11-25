using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.WebApi;
using Sitecore.XConnect.Schema;
using Sitecore.Xdb.Common.Web;
using Sym17.Web.Models.Facets;
using System;
using System.Configuration;

namespace Sym17.Web.Demo.Segmentation
{
    public class XConnectClientReference
    {
        private static XConnectClient singleRef;
        public static XConnectClient GetClient()
        {
            if (singleRef == null)
            {
                XConnectClientReference reference = new XConnectClientReference();
                singleRef = reference.BuildClient();
            }

            return singleRef;
        }

        private XConnectClientReference() { }

        private XConnectClient BuildClient()
        {
            string XconnectUrl = ConfigurationManager.ConnectionStrings["xconnect.collection"].ConnectionString;

            string XconnectThumbprint = ConfigurationManager.ConnectionStrings["xconnect.collection.certificate"].ConnectionString;

            XConnectClientConfiguration cfg;
            if (string.IsNullOrEmpty(XconnectThumbprint))
            {
                cfg = new XConnectClientConfiguration(
                    new XdbRuntimeModel(FaceApiModel.Model),
                    new Uri(XconnectUrl),
                    new Uri(XconnectUrl));

            }
            else
            {
                CertificateWebRequestHandlerModifierOptions options =
                CertificateWebRequestHandlerModifierOptions.Parse(XconnectThumbprint);
                var certificateModifier = new CertificateWebRequestHandlerModifier(options);

                // Step 2 - Client Configuration

                var collectionClient = new CollectionWebApiClient(new Uri(XconnectUrl + "/odata"), null, new[] { certificateModifier });
                var searchClient = new SearchWebApiClient(new Uri(XconnectUrl + "/odata"), null, new[] { certificateModifier });
                var configurationClient = new ConfigurationWebApiClient(new Uri(XconnectUrl + "/configuration"), null, new[] { certificateModifier });


                cfg = new XConnectClientConfiguration(
                new XdbRuntimeModel(FaceApiModel.Model), collectionClient, searchClient, configurationClient);
            }

            cfg.Initialize();

            var client = new XConnectClient(cfg);

            return client;
        }
    }
}