using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DirectShowLib;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json;
using Capture = Emgu.CV.Capture;
using Face = Microsoft.ProjectOxford.Face.Contract.Face;

namespace ImageProcess
{
    public partial class Form1 : Form
    {
        private Capture _capture;

        private Image<Bgr, byte> _imgToFindColor;
        private DetectedValues _faces, _badges;
        private List<OcrResults> _recognizedTexts = new List<OcrResults>();
        private List<Face> _recognizedFaces = new List<Face>();

        private bool _doTracking, _doFace, _doBadge;
        private bool _trancateRoi = true, _fullscreen = true;

        int _cameraDevice = 0; //Variable to track camera device selected
        VideoDevice[] _webCams; //List containing all the camera available
        private DsDevice[] _cameras;
        Inter _interpolation = Inter.Area;
        private bool _doing = false, _async = true, needRefresh = false;
        // Settings (App.config)
        private SettingsTemplate _settings;
        private UserService _userService;
        private MicrosoftCognitiveService _microsoftCognitiveService;
        public Form1()
        {
            InitializeComponent();
            RegisterHotKeys();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.Anchor = AnchorStyles.Right;
            _cameras = VideoDeviceManager.GetVideoDevices();
            _webCams = new VideoDevice[_cameras.Length];
            for (int i = 0; i < _cameras.Length; i++)
            {
                _webCams[i] = new VideoDevice(i, _cameras[i].Name, _cameras[i].ClassID); //fill web cam array
                camerasList.Items.Add(_webCams[i].ToString());

            }
            if (camerasList.Items.Count > 0)
            {
                camerasList.SelectedIndex = 0; //Set the selected device the default
            }

            LoadSettings();

            _microsoftCognitiveService = new MicrosoftCognitiveService(_settings.OcpApiSubscriptionKey, _settings.FaceApiSubscriptionKey);

            string appDataPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\App_Data\"));
            AppDomain.CurrentDomain.SetData("DataDirectory", appDataPath);
            _userService = new UserService();
            _userService.Test();
            CameraInit(0);

            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
           // panel1.Top = 0;
            panel1.Left = ClientSize.Width - panel1.Width - 10;
            cameraPictureBox.Top = 10;
            cameraPictureBox.Left = 10;
            comboBoxDetectorMode.SelectedIndex = _settings.DetectionMode - 1;

            Application.Idle += ShowCameraScreen;

        }

        private void CameraInit(int cameraIndex)
        {
            var camera = _cameras[cameraIndex];
            _capture = new Capture(cameraIndex);
            _cameraDevice = cameraIndex;

            var w = _capture.GetCaptureProperty(CapProp.FrameWidth);
            var h = _capture.GetCaptureProperty(CapProp.FrameHeight);

            var currentResolution = w + "x" + h;

            var resolutions = VideoDeviceManager.GetAllAvailableResolution(camera).OrderByDescending(x => x.Width).ThenByDescending(x => x.Height).ToList();
            resolutionsList.Items.Clear();
            var strings = resolutions.Select(x => x.Width + "x" + x.Height).Distinct().ToList();
            var index = -1;

            for (int i = 0; i < strings.Count; i++)
            {
                resolutionsList.Items.Add(strings[i]);
                if (strings[i].Equals(currentResolution))
                {
                    index = i;
                }
            }
            resolutionsList.SelectedIndex = index;
        }

        private void LoadSettings()
        {
            try
            {
                _settings = SettingsReader.ReadSettings();

                if (!Directory.Exists(_settings.TempFolder))
                {
                    Directory.CreateDirectory(_settings.TempFolder);
                }

                var temp = Path.Combine(_settings.TempFolder, Guid.NewGuid().ToString("N") + ".test");
                using (File.Create(temp))
                {

                }
                File.Delete(temp);
                _imgToFindColor = new Image<Bgr, byte>(_settings.TemplateBagdeImage);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading settings.config! " + ex.Message);
            }

        }

        public Stream BitmapToStream(Bitmap bitmap, string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Guid.NewGuid().ToString("N");
            }
            else
            {
                fileName += "-" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
            }
            var temp = Path.Combine(_settings.TempFolder, fileName + ".jpg");
            bitmap.Save(temp, ImageFormat.Jpeg);
            return new FileStream(temp, FileMode.Open);
        }

        // Used for render from element changes in async tasks
        public void InvokeGuiThread(Action action)
        {
            BeginInvoke(action);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Start();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Stop();
        }

        public void Stop()
        {
            if (_doTracking)
            {
                _doTracking = false;
                _doFace = false;
                _doBadge = false;
                Application.Idle -= PerformRecognition;

                _faces = new DetectedValues(_settings.MinFaceCount, _settings.MinFaceSharpness);
                _badges = new DetectedValues(_settings.MinBadgeCount, _settings.MinBadgeSharpness);
                _recognizedTexts = new List<OcrResults>();
                _recognizedFaces = new List<Face>();
                Application.Idle += ShowCameraScreen;
            }
        }
        public void Start()
        {
            if (!_doTracking)
            {
                _doTracking = true;
                switch (_settings.DetectionMode)
                {
                    case 1:
                        _doFace = true;
                        _doBadge = false;
                        break;
                    case 2:
                        _doFace = false;
                        _doBadge = true;
                        break;
                    case 3:
                        _doFace = true;
                        _doBadge = true;
                        break;
                }
                _faces = new DetectedValues(_settings.MinFaceCount, _settings.MinFaceSharpness);
                _badges = new DetectedValues(_settings.MinBadgeCount, _settings.MinBadgeSharpness);
                _recognizedTexts = new List<OcrResults>();
                _recognizedFaces = new List<Face>();
                Application.Idle -= ShowCameraScreen;
                Application.Idle += PerformRecognition;
            }
        }


        public async void ProcessBadge()
        {
            InvokeGuiThread(() =>
            {
                labelBadges.Text = _badges.Count + " of "+_settings.MinBadgeCount;
            });

            if (_badges.ReadyForRecognize)
            {
                _doBadge = false;
                var stop = false;
                switch (_settings.DetectionMode)
                {
                    case 1:
                        stop = true;
                        break;
                    case 2:
                        _doFace = true;
                        break;
                    case 3:
                        if (_doFace == false)
                        {
                            stop = true;
                        }
                        break;
                }

                InvokeGuiThread(() =>
                {
                    badgePictureBox.Image = _badges.GetBestValue().DetectedImage.Bitmap;
                });

                await RecognizeBadges().ContinueWith((x) =>
                {
                    if (stop)
                    {
                        _doTracking = false;
                    }
                });

            }
        }

        public async void ProcessFace()
        {
            InvokeGuiThread(() =>
            {
                labelFaces.Text = _faces.Count + " of "+_settings.MinFaceCount;
             
            });

            if (_faces.ReadyForRecognize)
            {
                _doFace = false;
                var stop = false;
                switch (_settings.DetectionMode)
                {
                    case 1:
                        _doBadge = true;
                        break;
                    case 2:
                        stop = true;
                        break;
                    case 3:
                        if (_doBadge == false)
                        {
                            stop = true;
                        }
                        break;
                }

                InvokeGuiThread(() =>
                {
                    facePictureBox.Image = _faces.GetBestValue().DetectedImage.Bitmap;
                });

                await RecognizeFaces().ContinueWith((x) =>
                {
                    if (stop)
                    {
                        _doTracking = false;
                    }
                });
            }
        }

        public async Task RecognizeBadges()
        {
            foreach (var detectResult in _badges.GetBestValues(3))
            {
                var stream = BitmapToStream(detectResult.DetectedImage.Bitmap);
                var texts = await _microsoftCognitiveService.GetText(stream);
                _recognizedTexts.Add(texts);
            }

            OnTextRecognized();
        }
        public async Task RecognizeFaces()
        {

            foreach (var detectResult in _faces.GetBestValues(3))
            {
                var stream = BitmapToStream(detectResult.DetectedImage.Bitmap);
                var faces = await _microsoftCognitiveService.GetFaces(stream);
                if (faces != null && faces.Length == 1)
                {
                    var face = faces[0];
                    _recognizedFaces.Add(face);
                }
            }
            OnFaceRecognized();
        }

        private void button_set_template_Click(object sender, EventArgs e)
        {
            if (openBadgeDialog.ShowDialog() == DialogResult.OK)
            {
                _settings.TemplateBagdeImage = openBadgeDialog.FileName;
                _imgToFindColor = new Image<Bgr, byte>(_settings.TemplateBagdeImage);

                SettingsReader.WriteSettings(JsonConvert.SerializeObject(_settings));

            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void button_test_face_Click(object sender, EventArgs e)
        {
            _faces = new DetectedValues(100, 0);
            Application.Idle += DetectFace;
        }

        private async void button_test_face_stop_Click(object sender, EventArgs e)
        {
            Application.Idle -= DetectFace;
            var detected = _faces.GetBestValues(1).FirstOrDefault();
            if (detected != null)
            {
                var stream = BitmapToStream(detected.DetectedImage.Bitmap);
                var detectedFace = await _microsoftCognitiveService.GetFaces(stream);

                if (detectedFace != null && detectedFace.Length == 1)
                {
                    var bestFace = detectedFace[0];
                    Logging(bestFace);
                }
            }
        }

        private void button_test_badge_Click(object sender, EventArgs e)
        {
            _badges = new DetectedValues(100, 0);
            Application.Idle += DetectBadge;
        }

        private async void button_test_badge_stop_Click(object sender, EventArgs e)
        {
            Application.Idle -= DetectBadge;
            Application.Idle -= DetectBadgeAsync;
            var detected = _badges.GetBestValues(1).FirstOrDefault();
            if (detected != null)
            {
                badgePictureBox.Image = detected.DetectedImage.Bitmap;
                var stream = BitmapToStream(detected.DetectedImage.Bitmap);
                var detectedText = await _microsoftCognitiveService.GetText(stream);

                if (detectedText != null && detectedText.Regions != null && detectedText.Regions.Length > 0)
                {
                    var regNum = 1;
                    foreach (var region in detectedText.Regions)
                    {
                        var lineNum = 1;
                        foreach (var line in region.Lines.Where(x => x.Words.Any()))
                        {
                            var wordNum = 1;
                            foreach (var word in line.Words)
                            {
                                recognizedLog.AppendText(string.Format("\n Resion: {0} Line: {1}, Word: {2}, Text: {3}", regNum, lineNum, wordNum, word.Text));
                                wordNum++;
                            }

                            lineNum++;
                        }
                        regNum++;
                    }
                }
            }
        }

        private void button_read_badge_Click(object sender, EventArgs e)
        {
            var template = SettingsReader.ReadBadgeSettings();
            recognizedLog.Text = JsonConvert.SerializeObject(template, Formatting.Indented);
        }

        private void buttonwrite_badge_template_Click(object sender, EventArgs e)
        {
            SettingsReader.WriteBadgeSettings(recognizedLog.Text);
        }

        private void comboBoxDetectorMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            _settings.DetectionMode = (byte)(comboBoxDetectorMode.SelectedIndex + 1);
        }

        public void OnFaceRecognized()
        {
            var bestFace =
                _recognizedFaces
                    .OrderBy(x => x.FaceAttributes.Blur.Value)
                    .ThenBy(x => x.FaceAttributes.Noise.Value)
                    .ThenByDescending(x => x.FaceAttributes.Smile)
                    .ThenByDescending(x => x.FaceAttributes.Emotion.Happiness)
                    .First();
            bestFace.FaceAttributes.Age = Math.Floor(_recognizedFaces.Average(x => x.FaceAttributes.Age));
            bestFace.FaceAttributes.Gender = _recognizedFaces.GroupBy(x => x.FaceAttributes.Gender).OrderByDescending(s => s.Count()).First().Key;

            Logging(bestFace);
        }
        public void OnTextRecognized()
        {
            //var alphaNumeric = new Regex("[^a-zA-Z0-9]");

            var template = SettingsReader.ReadBadgeSettings();
            if (template != null && _recognizedTexts != null && _recognizedTexts.Any())
            {
                int iregion = 0;
                foreach (var region in template.Regions)
                {
                    int iline = 0;
                    foreach (var line in region.Lines)
                    {
                        int iword = 0;
                        foreach (var word in line.Words)
                        {
                            if (!string.IsNullOrEmpty(word.FieldName))
                            {
                                var values = _recognizedTexts
                                    .Where(x => x.Regions[iregion].Lines.Length >= iline + 1 && x.Regions[iregion].Lines.All(y => y.Words.Length >= iword + 1))
                                    .Select(x => x.Regions[iregion].Lines[iline].Words[iword].Text.Trim()).ToList();

                                var mostFrequenceValue = values.GroupBy(q => q).OrderByDescending(gp => gp.Count()).Select(g => g.Key).First();
                                InvokeGuiThread(() =>
                                {
                                    recognizedLog.AppendText(string.Format("\n {0}: {1}", word.FieldName, mostFrequenceValue));
                                });
                            }
                            iword++;
                        }
                        iline++;
                    }
                    iregion++;
                }
            }
        }

        private void ShowCameraScreen(object sender, EventArgs e)
        {
            var imageFrame = _capture.QueryFrame().ToImage<Bgr, byte>();

            DrawThankYouText(imageFrame, "Thank you!");

            if (!_doBadge && !_doFace && !_doTracking)
            {
                DrawAttractiveText(imageFrame, "xConnect\r\n in Action");
            }

            DrawBestFace(imageFrame);
            DrawBestBadge(imageFrame);
            DrawFrame(imageFrame);

            if (needRefresh)
            {
                needRefresh = false;
                Start();
            }

        }

        private async void PerformRecognition(object sender, EventArgs e)
        {
            if (_doFace)
            {
                DetectFace(sender, e);
                ProcessFace();
            }
            if (_doBadge)
            {
                if (_async)
                {
                    DetectBadgeAsync(sender,e);
                }
                else
                {
                    DetectBadge(sender, e);
                }

                ProcessBadge();
            }

            if (!_doTracking)
            {
                Application.Idle -= PerformRecognition;
                Application.Idle += ShowCameraScreen;
                await OnRecognizedAll().ContinueWith(x =>
                {
                    needRefresh = true;
                });
            }
            // show frame while azure recogniztion
            else if (!_doFace && !_doBadge)
            {
                ShowCameraScreen(sender, e);
            }
        }

        private void camerasList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = camerasList.SelectedIndex;
            //Check to see if the selected device has changed
            if (camerasList.SelectedIndex != _cameraDevice)
            {
                CameraInit(index);
            }
        }

        private void resolutionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            {
                var resolution = resolutionsList.Items[resolutionsList.SelectedIndex].ToString();
                var width = resolution.Split('x')[0];
                var height = resolution.Split('x')[1];
                _capture.SetCaptureProperty(CapProp.FrameHeight, int.Parse(height));
                _capture.SetCaptureProperty(CapProp.FrameWidth, int.Parse(width));
                cameraPictureBox.Width = ClientSize.Width;
                cameraPictureBox.Height = ClientSize.Height;
            }
        }

        public void DetectFace(object sender, EventArgs e)
        {
            var imageFrame = _capture.QueryFrame().ToImage<Bgr, byte>();

            var timer = Stopwatch.StartNew();
            var detected = new DetectorService().DetectFace(imageFrame);
            timer.Stop();
            labelTime.Text = timer.ElapsedMilliseconds + " ms.";
            if (detected != null)
            {
                labelFaces.Text = detected.SharpnessValue.ToString();

                DrawCircle(imageFrame, detected.DetectedRectangle);
                //imageFrame.Draw(detected.DetectedRectangle, new Bgr(Color.Aquamarine));

                facePictureBox.Image = detected.DetectedImage.Bitmap;
                _faces.Add(detected);
                richTextBox1.Text = string.Format("MaxFace: {0}, MinFace: {1}, Current:{2},  Time: {3}", _faces.BestValue, _faces.WorstValue, detected.SharpnessValue, timer.ElapsedMilliseconds + " ms.");
            }
            else
            {
                DrawText(imageFrame, new System.Drawing.Rectangle(0, 50, imageFrame.Width, imageFrame.Height), "Waiting for face...", 35);
            }

            DrawBestFace(imageFrame);
            DrawBestBadge(imageFrame);
            DrawFrame(imageFrame);

        }

        public void DetectBadge(object sender, EventArgs e)
        {
            var reduceCoeff = _settings.ReduceCoeff;
            var scaleFactor = 1.0 / reduceCoeff;

            var timer = Stopwatch.StartNew();

            Image<Bgr, byte> imageResized;

            var imageFrame = _capture.QueryFrame().ToImage<Bgr, byte>();

            var roi = GetRoi(imageFrame.Width, imageFrame.Height);
            if (_trancateRoi)
            {
                imageFrame.Draw(roi, new Bgr(_roiFrameColor));
                var truncImage = imageFrame.Copy(roi);
                imageResized = reduceCoeff == 1 ? truncImage : truncImage.Resize(scaleFactor, _interpolation);
            }
            else
            {
                imageResized = reduceCoeff == 1 ? imageFrame : imageFrame.Resize(scaleFactor, _interpolation);
            }
           
            var template = _imgToFindColor;
            //if (reduceCoeff != 1)
            //{
            //    template = _imgToFindColor.Resize(scaleFactor, _interpolation);
            //}

            long time;
            var detected = new DetectorService().DetectSurf(template, imageResized, out time);

            timer.Stop();
            labelTime.Text = timer.ElapsedMilliseconds + " ms.";


            if (detected != null)
            {
                labelBadges.Text = detected.SharpnessValue.ToString();
                if (reduceCoeff > 1 || roi.X > 0 || roi.Y > 0)
                {
                    detected.DetectedRectangle = new System.Drawing.Rectangle(
                   roi.X + detected.DetectedRectangle.X * reduceCoeff,
                   roi.Y + detected.DetectedRectangle.Y * reduceCoeff,
                   detected.DetectedRectangle.Width * reduceCoeff,
                   detected.DetectedRectangle.Height * reduceCoeff);
                }

                detected.DetectedImage = imageFrame.Copy(detected.DetectedRectangle);

                imageFrame.Draw(detected.DetectedRectangle, new Bgr(_detectImageFrameColor));
                badgePictureBox.Image = detected.DetectedImage.Bitmap;
                _badges.Add(detected);
                richTextBox1.Text = string.Format("MaxBadge: {0}, MinBadge: {1}, Current:{2}, Time: {3}", _badges.BestValue, _badges.WorstValue, detected.SharpnessValue, timer.ElapsedMilliseconds + " ms.");
            }
            else
            {
                var text = string.Format("Waiting for badge {0} of {1}", _badges.Count, _settings.MinBadgeCount);
                DrawText(imageFrame, new System.Drawing.Rectangle(0, 50, imageFrame.Width, imageFrame.Height), text, 35);
                var textCloser = "Bring your badge closer to the camera.";
                DrawText(imageFrame, new System.Drawing.Rectangle(0, 100, imageFrame.Width, imageFrame.Height), textCloser, 35);
            }

            DrawBestFace(imageFrame);
            DrawBestBadge(imageFrame);
            DrawFrame(imageFrame);
        }



        private void checkBox_truncate_roi_CheckedChanged(object sender, EventArgs e)
        {
            _trancateRoi = checkBox_truncate_roi.Checked;
        }


        private void reduce_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (reduce_list.SelectedIndex >= 0)
            {
                _settings.ReduceCoeff = reduce_list.SelectedIndex + 1;
            }
        }

        private void button_test_async_Click(object sender, EventArgs e)
        {
            _badges = new DetectedValues(100, 0);
            Application.Idle += DetectBadgeAsync;
        }

        private void checkBox_fullscreen_CheckedChanged(object sender, EventArgs e)
        {
            _fullscreen = checkBox_fullscreen.Checked;
        }

        private void button_screenshot_Click(object sender, EventArgs e)
        {
            var imageFrame = _capture.QueryFrame().ToImage<Bgr, byte>();
            Clipboard.SetImage(imageFrame.Bitmap);
        }

        private void checkBox_async_CheckedChanged(object sender, EventArgs e)
        {
            _async = checkBox_async.Checked;
        }

        public async Task OnRecognizedAll()
        {
            var user = GetRecognizedUserModel();
            try
            {
                if (user != null)
                {
                    user.Id = Guid.NewGuid();
                    _userService.Add(user);
                    await PostResults(user);
                }
                else
                {
                    recognizedLog.AppendText("\n Couldn`t create user model");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async Task PostResults(User user)
        {
            //var faces = GetFaceInteractions(user.Id);
            var postService = new PostService(_settings.WebApiUrl);
            await Task.Run(() => postService.SendRequest(user)).ContinueWith(x =>
            {
                if (!x.Result)
                {
                    // TODO: do something...
                    MessageBox.Show("WebAPI request error!");
                }
            });
        }
        public User GetRecognizedUserModel()
        {
            var user = new User();
          
            var bestImage = _faces.GetBestValue();
            if (bestImage != null)
            {
                Bitmap bitmap = bestImage.DetectedImage.Bitmap;

                using (MemoryStream m = new MemoryStream())
                {
                    bitmap.Save(m, ImageFormat.Jpeg);
                    byte[] imageBytes = m.ToArray();
                    user.Photo = Convert.ToBase64String(imageBytes);
                }

                var encoderParams = new EncoderParameters(1)
                {
                    Param ={[0] = new EncoderParameter(Encoder.Quality,50L)}
                };

                var jpegCodec = (from codec in ImageCodecInfo.GetImageEncoders()
                    where codec.MimeType == "image/jpeg"
                    select codec).Single();

                using (MemoryStream m = new MemoryStream())
                {
                    bitmap.Save(m, jpegCodec, encoderParams);
                    byte[] imageBytes = m.ToArray();
                    user.Photo48 = Convert.ToBase64String(imageBytes);
                }
            }

            var bestFace =
                _recognizedFaces
                    .OrderBy(x => x.FaceAttributes.Blur.Value)
                    .ThenBy(x => x.FaceAttributes.Noise.Value)
                    .ThenByDescending(x => x.FaceAttributes.Smile)
                    .ThenByDescending(x => x.FaceAttributes.Emotion.Happiness)
                    .First();
            bestFace.FaceAttributes.Age = Math.Floor(_recognizedFaces.Average(x => x.FaceAttributes.Age));
            bestFace.FaceAttributes.Gender = _recognizedFaces.GroupBy(x => x.FaceAttributes.Gender).OrderByDescending(s => s.Count()).First().Key;
            user.Age = (byte)Math.Floor(bestFace.FaceAttributes.Age);
            user.Gender = bestFace.FaceAttributes.Gender;
            user.Glasses = bestFace.FaceAttributes.Glasses.ToString();
            user.Smile = bestFace.FaceAttributes.Smile;
            user.Happiness = bestFace.FaceAttributes.Emotion.Happiness;
            user.Anger = bestFace.FaceAttributes.Emotion.Anger;
            user.Contempt = bestFace.FaceAttributes.Emotion.Contempt;
            user.Disgust = bestFace.FaceAttributes.Emotion.Disgust;
            user.Fear = bestFace.FaceAttributes.Emotion.Fear;
            user.Neutral = bestFace.FaceAttributes.Emotion.Neutral;
            user.Sadness = bestFace.FaceAttributes.Emotion.Sadness;
            user.Surprise = bestFace.FaceAttributes.Emotion.Surprise;

            //var alphaNumeric = new Regex("[^a-zA-Z0-9]");

            var template = SettingsReader.ReadBadgeSettings();
            if (template != null && _recognizedTexts != null && _recognizedTexts.Any())
            {
                int iregion = 0;
                foreach (var region in template.Regions)
                {
                    int iline = 0;
                    foreach (var line in region.Lines)
                    {
                        int iword = 0;
                        foreach (var word in line.Words)
                        {
                            if (!string.IsNullOrEmpty(word.FieldName))
                            {
                                var values = _recognizedTexts
                                    .Where(x => x.Regions[iregion].Lines.Length >= iline + 1 && x.Regions[iregion].Lines.All(y => y.Words.Length >= iword + 1))
                                    .Select(x => x.Regions[iregion].Lines[iline].Words[iword].Text.Trim()).ToList();

                                if (values.Any())
                                {
                                    var mostFrequenceValue = values.GroupBy(q => q).OrderByDescending(gp => gp.Count()).Select(g => g.Key).First();
                                    if (word.FieldName.Equals("firstName", StringComparison.OrdinalIgnoreCase))
                                    {
                                        user.FirstName = mostFrequenceValue.ToTitleCase();
                                    }
                                    else if (word.FieldName.Equals("lastName", StringComparison.OrdinalIgnoreCase))
                                    {
                                        user.LastName = mostFrequenceValue.ToTitleCase();
                                    }
                                    else if (word.FieldName.Equals("company", StringComparison.OrdinalIgnoreCase))
                                    {
                                        user.Company = mostFrequenceValue.ToTitleCase();
                                    }
                                }

                            }
                            iword++;
                        }
                        iline++;
                    }
                    iregion++;
                }


                var parsed = new List<string>();
                foreach (var recognizedText in _recognizedTexts)
                {
                    foreach (var recognizedTextRegion in recognizedText.Regions)
                    {
                        foreach (var line in recognizedTextRegion.Lines)
                        {
                            foreach (var lineWord in line.Words)
                            {
                                parsed.Add(lineWord.Text.ToTitleCase());
                            }
                        }
                    }
                }
                parsed = parsed.Distinct().ToList();
                user.ParsedText = string.Join(", ", parsed.ToArray());

                return user;
            }
            return null;
        }

        public List<FaceInteractionRequestModel> GetFaceInteractions(int userId)
        {
            var result = new List<FaceInteractionRequestModel>();
            if (_recognizedFaces != null && _recognizedFaces.Any())
            {
                foreach (var face in _recognizedFaces)
                {
                    result.Add(new FaceInteractionRequestModel
                    {
                        ContactId = userId.ToString(),
                        AngerValue = face.FaceAttributes.Emotion.Anger,
                        ContemptValue = face.FaceAttributes.Emotion.Contempt,
                        DisgustValue = face.FaceAttributes.Emotion.Disgust,
                        FearValue = face.FaceAttributes.Emotion.Fear,
                        HappinessValue = face.FaceAttributes.Emotion.Happiness,
                        NeutralValue = face.FaceAttributes.Emotion.Neutral,
                        SadnessValue = face.FaceAttributes.Emotion.Sadness,
                        SmileValue = face.FaceAttributes.Smile,
                        SurpriseValue = face.FaceAttributes.Emotion.Surprise
                    });
                }
            }
            return result;
        }

        private void Logging(Face face)
        {
            InvokeGuiThread(() =>
            {
                recognizedLog.AppendText(string.Format("\n Average age: {0}", face.FaceAttributes.Age));
                recognizedLog.AppendText(string.Format("\n Gender: {0}", face.FaceAttributes.Gender));
                recognizedLog.AppendText(string.Format("\n Glasses: {0}", face.FaceAttributes.Glasses));

                recognizedLog.AppendText(string.Format("\n Blur: {0}", face.FaceAttributes.Blur.Value));
                recognizedLog.AppendText(string.Format("\n Noise: {0}", face.FaceAttributes.Noise.Value));

                recognizedLog.AppendText(string.Format("\n Smile: {0}", face.FaceAttributes.Smile));
                recognizedLog.AppendText(string.Format("\n Happiness: {0}", face.FaceAttributes.Emotion.Happiness));
                recognizedLog.AppendText(string.Format("\n Anger: {0}", face.FaceAttributes.Emotion.Anger));
                recognizedLog.AppendText(string.Format("\n Contempt: {0}", face.FaceAttributes.Emotion.Contempt));
                recognizedLog.AppendText(string.Format("\n Disgust: {0}", face.FaceAttributes.Emotion.Disgust));
                recognizedLog.AppendText(string.Format("\n Fear: {0}", face.FaceAttributes.Emotion.Fear));
                recognizedLog.AppendText(string.Format("\n Neutral: {0}", face.FaceAttributes.Emotion.Neutral));
                recognizedLog.AppendText(string.Format("\n Sadness: {0}", face.FaceAttributes.Emotion.Sadness));
                recognizedLog.AppendText(string.Format("\n Surprise: {0}", face.FaceAttributes.Emotion.Surprise));
            });
        }

        public async void DetectBadgeAsync(object sender, EventArgs e)
        {
            var reduceCoeff = _settings.ReduceCoeff;
            var scaleFactor = 1.0 / reduceCoeff;

            Image<Bgr, byte> imageResized;

            var imageFrame = _capture.QueryFrame().ToImage<Bgr, byte>();

            var roi = GetRoi(imageFrame.Width, imageFrame.Height);
            if (_trancateRoi)
            {
                imageFrame.Draw(roi, new Bgr(_roiFrameColor));
                var truncImage = imageFrame.Copy(roi);
                imageResized = reduceCoeff == 1 ? truncImage : truncImage.Resize(scaleFactor, _interpolation);
            }
            else
            {
                imageResized = reduceCoeff == 1 ? imageFrame : imageFrame.Resize(scaleFactor, _interpolation);
            }

            var template = _imgToFindColor;
            var text = string.Format("Waiting for badge {0} of {1}", _badges.Count, _settings.MinBadgeCount);
            DrawText(imageFrame, new System.Drawing.Rectangle(0, 50, imageFrame.Width, imageFrame.Height), text, 35);
            var textCloser = "Bring your badge closer to the camera.";
            DrawText(imageFrame, new System.Drawing.Rectangle(0, 100, imageFrame.Width, imageFrame.Height), textCloser, 35);

            DrawBestFace(imageFrame);
            DrawBestBadge(imageFrame);
            DrawFrame(imageFrame);

            if (!_doing)
            {
                long time;
                _doing = true;
                await Task.Run(() => new DetectorService().DetectSurf(template, imageResized, out time)).ContinueWith(
              (x) =>
              {
                  _doing = false;
                  var detected = x.Result;

                  if (detected != null)
                  {

                      if (reduceCoeff > 1 || roi.X > 0 || roi.Y > 0)
                      {
                          detected.DetectedRectangle = new System.Drawing.Rectangle(
                         roi.X + detected.DetectedRectangle.X * reduceCoeff,
                         roi.Y + detected.DetectedRectangle.Y * reduceCoeff,
                         detected.DetectedRectangle.Width * reduceCoeff,
                         detected.DetectedRectangle.Height * reduceCoeff);
                      }

                      detected.DetectedImage = imageFrame.Copy(detected.DetectedRectangle);
                      _badges.Add(detected);
                      InvokeGuiThread(() =>
                      {
                          badgePictureBox.Image = detected.DetectedImage.Bitmap;
                          labelBadges.Text = detected.SharpnessValue.ToString();
                          richTextBox1.Text = string.Format("MaxBadge: {0}, MinBadge: {1}, Current:{2}", _badges.BestValue, _badges.WorstValue, detected.SharpnessValue);
                      });
                      imageFrame.Draw(detected.DetectedRectangle, new Bgr(_detectImageFrameColor));
                      DrawFrame(imageFrame);
                  }
                  
              });
            }


        }
    }

    public static class ImageExtentions
    {
        public static Image<Bgr, byte> ReduceImage(this Image<Bgr, byte> image, double scale)
        {
            var w = (int)Math.Ceiling(image.Width * scale);
            var h = (int)Math.Ceiling(image.Height * scale);
            Bitmap newImage = new Bitmap(w, h);
            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(image.Bitmap, 0, 0, w, h);
            }
            return new Image<Bgr, byte>(newImage);
        }
    }
}
