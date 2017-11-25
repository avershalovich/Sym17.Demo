using System;
using System.Windows.Forms;
using RestSharp;

namespace ImageProcess
{
    public class PostService
    {
        readonly RestClient _restClient;
        public PostService(string apiUrl)
        {
            _restClient = new RestClient(apiUrl);
        }

        public bool SendRequest(User user)
        {
            try
            {
                if (user.Gender.Equals("female", StringComparison.OrdinalIgnoreCase))
                {
                    if (user.Age > 30)
                    {
                        user.Age = (byte)(user.Age - 5);
                    }

                    if (user.Age < 30)
                    {
                        user.Age = (byte) (user.Age - 3);
                    }
                }

                var request = new RestRequest(Method.POST) { RequestFormat = DataFormat.Json, Timeout = 60000};
                _restClient.AddHandler("application/octet-stream", new RestSharp.Deserializers.JsonDeserializer());
                request.AddHeader("Content-Type", "application/octet-stream");

                request.AddBody(user);
                var response = _restClient.Execute(request);
                if (response.Content.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }
        }

        //public bool SendInteraction(FaceInteractionRequestModel interaction)
        //{
        //    try
        //    {
        //        var request = new RestRequest(Method.POST) { RequestFormat = DataFormat.Json };
        //        _restClient.AddHandler("application/octet-stream", new RestSharp.Deserializers.JsonDeserializer());
        //        request.AddHeader("Content-Type", "application/octet-stream");

        //        request.AddBody(interaction);
        //        var response = _restClient.Execute(request);
        //        if (response.Content.Equals("true", StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        return false;
        //    }
        //}
    }
}
