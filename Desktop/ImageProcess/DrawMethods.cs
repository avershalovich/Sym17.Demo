using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace ImageProcess
{
    public partial class Form1
    {
        private Brush _textBrush = Brushes.Chartreuse;
        private Color _sideImageFrameColor = Color.Chartreuse;
        private Color _detectImageFrameColor = Color.Aquamarine;
        private Color _roiFrameColor = Color.BlueViolet;

        public int _verticalRoiPositionPercent = 50;

        private Rectangle GetRoi(int imageWidth, int imageHeight)
        {
            if (_trancateRoi)
            {
                var roiPaddingX = (int)Math.Ceiling(((100 - _settings.RoiWidthPercent) / 2.0) / 100 * imageWidth);
                var roiPaddingY = (int)Math.Ceiling(((100 - _settings.RoiHeightPercent) / (100.0 / _verticalRoiPositionPercent)) / 100 * imageHeight);
                var roiWidth = (int)Math.Ceiling(_settings.RoiWidthPercent / 100.0 * imageWidth);
                var roiHeight = (int)Math.Ceiling(_settings.RoiHeightPercent / 100.0 * imageHeight);
                return new Rectangle(roiPaddingX, roiPaddingY, roiWidth, roiHeight);
            }
            else
            {
                return new Rectangle(0,0,imageWidth,imageHeight);
            }
        }

        private void DrawFrame(Image<Bgr, byte> imageFrame)
        {
            var deltaSize = Math.Max(Math.Abs(imageFrame.Width - ClientSize.Width), Math.Abs(imageFrame.Height - ClientSize.Height));
            if (_fullscreen && deltaSize > 100 && ClientSize.Width > 0)
            {
                cameraPictureBox.Image = imageFrame.Resize(ClientSize.Width, ClientSize.Height, Inter.Area).Bitmap;
            }
            else
            {
                cameraPictureBox.Image = imageFrame.Bitmap;
            }
        }

        private void DrawDetectedImage(Image<Bgr, byte> imageFrame, DetectResult detected, int x, int y)
        {
            imageFrame.ROI = new Rectangle(x, y, detected.DetectedRectangle.Width, detected.DetectedRectangle.Height); // a rectangle
            detected.DetectedImage.CopyTo(imageFrame);
            imageFrame.ROI = Rectangle.Empty;
            imageFrame.Draw(new Rectangle(x, y, detected.DetectedRectangle.Width, detected.DetectedRectangle.Height), new Bgr(_sideImageFrameColor), 2);
        }

        private void DrawCircle(Image<Bgr, byte> img, Rectangle rect)
        {
            var x = rect.X + rect.Width / 2;
            var y = rect.Y + rect.Height / 2;
            img.Draw(new CircleF(new PointF(x, y), rect.Width / 2), new Bgr(_detectImageFrameColor), 2);
        }

        private void DrawText(Image<Bgr, byte> img, Rectangle rect, string text, int size=18, Brush brush = null)
        {
            Graphics g = Graphics.FromImage(img.Bitmap);
            Font font = new Font("Helvetica", size, FontStyle.Bold); //creates new font
            int tWidth = (int)g.MeasureString(text, font).Width;
            int x;
            if (tWidth >= rect.Width)
                x = rect.Left - ((tWidth - rect.Width) / 2);
            else
                x = (rect.Width / 2) - (tWidth / 2) + rect.Left;

            g.DrawString(text, font, brush ?? _textBrush, new PointF(x, rect.Top - size));
        }

        private void DrawBestFace(Image<Bgr, byte> imageFrame)
        {
            if (_faces != null && _faces.Count > 0)
            {
                var face = _faces.GetBestValue();
                DrawText(imageFrame, new Rectangle(50, 30, face.DetectedRectangle.Width, face.DetectedRectangle.Height), "Detected Face");
                DrawDetectedImage(imageFrame, face, 50, 50);
            }
        }

        private void DrawBestBadge(Image<Bgr, byte> imageFrame)
        {
            if (_badges != null && _badges.Count > 0)
            {
                var badge = _badges.GetBestValue();
                if (badge != null)
                {
                    var y = imageFrame.Height - badge.DetectedRectangle.Height - 100;
                    DrawText(imageFrame, new Rectangle(50, y - 30, badge.DetectedRectangle.Width, badge.DetectedRectangle.Height), "Detected Badge");
                    DrawDetectedImage(imageFrame, badge, 50, y);
                }
            }
        }

        private void DrawThankYouText(Image<Bgr, byte> imageFrame, string text)
        {
            if (_faces != null && _faces.Count > 0 && _badges != null && _badges.Count > 0)
            {
                DrawText(imageFrame, new Rectangle(0, imageFrame.Height/2, imageFrame.Width, imageFrame.Height), text, 45);
            }
        }

        private void DrawAttractiveText(Image<Bgr, byte> imageFrame, string text)
        {
            if ((_faces == null || _faces.Count == 0) && (_badges == null || _badges.Count == 0))
            {
                DrawText(imageFrame, new Rectangle(0, imageFrame.Height/4, imageFrame.Width, imageFrame.Height), text, 250, Brushes.Azure);
            }
        }
    }
}
