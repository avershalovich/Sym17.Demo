namespace ImageProcess
{
    using System.Drawing;

    using Emgu.CV;
    using Emgu.CV.Structure;

    public class DetectResult
    {
        public Rectangle DetectedRectangle { get; set; }
        public Image<Bgr, byte> DetectedImage { get; set; }
        public float SharpnessValue { get; set; }
    }
}