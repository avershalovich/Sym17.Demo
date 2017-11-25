using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Face = Microsoft.ProjectOxford.Face.Contract.Face;

namespace ImageProcess
{
    public class MicrosoftCognitiveService
    {
        public string OcrUrl = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0";
        public string FaceUrl = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0";
        private readonly string _ocrApiKey;
        private readonly string _faceApiKey;

        public MicrosoftCognitiveService(string ocrApiKey, string faceApiKey)
        {
            _ocrApiKey = ocrApiKey;
            _faceApiKey = faceApiKey;
        }

        //public AnalysisResult GetFaces(Stream image)
        //{
        //    var visionServiceClient = new VisionServiceClient(ocrApiKey, ApiUrl);
        //    var result = visionServiceClient.AnalyzeImageAsync(image, new List<VisualFeature> { VisualFeature.Faces });
        //    return result.Result;
        //}

        public async Task<Face[]> GetFaces(Stream image)
        {
            try
            {
                var faceServiceClient = new FaceServiceClient(_faceApiKey, FaceUrl);
                var faceAttributes = new[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Emotion, FaceAttributeType.Glasses, FaceAttributeType.Blur, FaceAttributeType.Noise };
                return await faceServiceClient.DetectAsync(image, returnFaceAttributes: faceAttributes);
            }
            catch (FaceAPIException f)
            {
                MessageBox.Show(f.ErrorMessage, f.ErrorCode);
                return null;
            }
        }

        public async Task<OcrResults> GetText(Stream image)
        {
            try
            {
                var visionServiceClient = new VisionServiceClient(_ocrApiKey, OcrUrl);
                return await visionServiceClient.RecognizeTextAsync(image);
            }
            catch (ClientException f)
            {
                MessageBox.Show(f.Message);
                return null;
            }
        }
    }
}
