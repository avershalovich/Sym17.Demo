using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Features2D;
using Emgu.CV.Util;
using Emgu.CV.XFeatures2D;

namespace ImageProcess
{
    public class DetectorService
    {
        private static readonly CascadeClassifier FaceCascadeClassifier = new CascadeClassifier(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"haarcascade_frontalface_alt_tree.xml")));

        //private static readonly CascadeClassifier EyeCascadeClassifier = new CascadeClassifier(Application.StartupPath + "/haarcascade_eye_tree_eyeglasses.xml");
        private static int padding = 50;
        private static readonly object Locker = new object();

        private Rectangle GetWithPadding(Rectangle face, int width, int height)
        {
            // left
            if (face.X > padding)
            {
                face.X -= padding;
                face.Width += padding;
            }

            // top
            if (face.Y > padding)
            {
                face.Y -= padding;
                face.Height += padding;
            }

            // right
            if (face.Right + padding < width)
            {
                face.Width += padding;
            }

            // bottom
            if (face.Bottom + padding < height)
            {
                face.Height += padding;
            }

            return face;
        }

        public DetectResult DetectFace(Image<Bgr, byte> imageFrame, bool skipClosedEyes = false)
        {
            var grayframe = imageFrame.Convert<Gray, byte>();

            Rectangle[] faces;
            lock (Locker)
            {
                faces = FaceCascadeClassifier.DetectMultiScale(grayframe, 1.3, 3, new Size(200, 200), Size.Empty);
            }

            if (faces == null || faces.Length != 1)
            {
                return null;
            }

            var face = faces[0];

            var faceX = GetWithPadding(face, imageFrame.Width, imageFrame.Height);

            var faceImage = imageFrame.Copy(faceX);

            //if (skipClosedEyes)
            //{
            //    EyesNotClosed(faceImage);
            //}

            var result = new DetectResult
            {
                DetectedRectangle = faceX,
                DetectedImage = faceImage,
                SharpnessValue = CalculateBlur2(faceImage)
            };
            return result;
        }

        //public bool EyesNotClosed(Image<Bgr, byte> imageFrame)
        //{
        //    var grayframe = imageFrame.Convert<Gray, byte>();
        //    var eyes = EyeCascadeClassifier.DetectMultiScale(grayframe, 1.1, 2, Size.Empty, Size.Empty);
        //    if (eyes != null && eyes.Length == 2)
        //        return true;
        //    return false;
        //}

        private void FindMatch(Mat modelImage, Mat observedImage, out long matchTime, out VectorOfKeyPoint modelKeyPoints, out VectorOfKeyPoint observedKeyPoints, VectorOfVectorOfDMatch matches, out Mat mask, out Mat homography)
        {
            int k = 2;
            double uniquenessThreshold = 0.8;
            double hessianThresh = 300;

            Stopwatch watch;
            homography = null;

            modelKeyPoints = new VectorOfKeyPoint();
            observedKeyPoints = new VectorOfKeyPoint();

#if !__IOS__
            if (CudaInvoke.HasCuda)
            {
                CudaSURF surfCuda = new CudaSURF((float) hessianThresh);
                using (GpuMat gpuModelImage = new GpuMat(modelImage))
                    //extract features from the object image
                using (GpuMat gpuModelKeyPoints = surfCuda.DetectKeyPointsRaw(gpuModelImage, null))
                using (GpuMat gpuModelDescriptors = surfCuda.ComputeDescriptorsRaw(gpuModelImage, null, gpuModelKeyPoints))
                using (CudaBFMatcher matcher = new CudaBFMatcher(DistanceType.L2))
                {
                    surfCuda.DownloadKeypoints(gpuModelKeyPoints, modelKeyPoints);
                    watch = Stopwatch.StartNew();

                    // extract features from the observed image
                    using (GpuMat gpuObservedImage = new GpuMat(observedImage))
                    using (GpuMat gpuObservedKeyPoints = surfCuda.DetectKeyPointsRaw(gpuObservedImage, null))
                    using (GpuMat gpuObservedDescriptors = surfCuda.ComputeDescriptorsRaw(gpuObservedImage, null, gpuObservedKeyPoints))
                        //using (GpuMat tmp = new GpuMat())
                        //using (Stream stream = new Stream())
                    {
                        matcher.KnnMatch(gpuObservedDescriptors, gpuModelDescriptors, matches, k);

                        surfCuda.DownloadKeypoints(gpuObservedKeyPoints, observedKeyPoints);

                        mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                        mask.SetTo(new MCvScalar(255));
                        Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

                        int nonZeroCount = CvInvoke.CountNonZero(mask);
                        if (nonZeroCount >= 4)
                        {
                            nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, matches, mask, 1.5, 20);
                            if (nonZeroCount >= 4)
                            {
                                homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, matches, mask, 2);
                            }
                        }
                    }
                    watch.Stop();
                }
            }
            else
#endif
            {
                using (UMat uModelImage = modelImage.ToUMat(AccessType.Read))
                using (UMat uObservedImage = observedImage.ToUMat(AccessType.Read))
                {
                    SURF surfCPU = new SURF(hessianThresh);
                    //extract features from the object image
                    UMat modelDescriptors = new UMat();
                    surfCPU.DetectAndCompute(uModelImage, null, modelKeyPoints, modelDescriptors, false);

                    watch = Stopwatch.StartNew();

                    // extract features from the observed image
                    UMat observedDescriptors = new UMat();
                    surfCPU.DetectAndCompute(uObservedImage, null, observedKeyPoints, observedDescriptors, false);
                    BFMatcher matcher = new BFMatcher(DistanceType.L2);
                    matcher.Add(modelDescriptors);

                    matcher.KnnMatch(observedDescriptors, matches, k, null);
                    mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
                    mask.SetTo(new MCvScalar(255));
                    Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

                    int nonZeroCount = CvInvoke.CountNonZero(mask);
                    if (nonZeroCount >= 4)
                    {
                        nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(modelKeyPoints, observedKeyPoints, matches, mask, 1.5, 20);
                        if (nonZeroCount >= 4)
                        {
                            homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(modelKeyPoints, observedKeyPoints, matches, mask, 2);
                        }
                    }

                    watch.Stop();
                }
            }
            matchTime = watch.ElapsedMilliseconds;
        }

        public DetectResult DetectSurf(Image<Bgr, byte> imgToFind, Image<Bgr, byte> imgScene, out long matchTime)
        {
            using (var matches = new VectorOfVectorOfDMatch())
            {
                matchTime = 0;

                try
                {
                    Mat mask;
                    Mat homography;
                    VectorOfKeyPoint modelKeyPoints;
                    VectorOfKeyPoint observedKeyPoints;
                    FindMatch(imgToFind.Mat, imgScene.Mat, out matchTime, out modelKeyPoints, out observedKeyPoints, matches, out mask, out homography);

                    float sharpnessValue = this.MatchesCount(matches, mask);
                    
                    //Draw the matched keypoints
                    //var result = imgScene.Mat;
                    //var result =  new Mat();
                    //Features2DToolbox.DrawMatches(imgToFind.Mat, modelKeyPoints, imgScene.Mat, observedKeyPoints,
                    //matches, result, new MCvScalar(255, 255, 255), new MCvScalar(255, 255, 255), mask);

                    #region draw the projected region on the image

                    if (homography != null && sharpnessValue > 5)
                    {
                        //draw a rectangle along the projected model
                        var rect = new Rectangle(Point.Empty, imgToFind.Size);

                        var p1 = new PointF(rect.Left, rect.Bottom);
                        var p2 = new PointF(rect.Right, rect.Bottom);
                        var p3 = new PointF(rect.Right, rect.Top);
                        var p4 = new PointF(rect.Left, rect.Top);

                        var pts = new[] {p1, p2, p3, p4};
                        pts = CvInvoke.PerspectiveTransform(pts, homography);

                        //check if any opposite lines intersect
                        //if so, then don't add to final results
                        //we should never have 2 opposite sides intersecting
                        var l1 = new LineSegment2DF(pts[0], pts[1]);
                        var l2 = new LineSegment2DF(pts[1], pts[2]);
                        var l3 = new LineSegment2DF(pts[2], pts[3]);
                        var l4 = new LineSegment2DF(pts[3], pts[0]);

                        if (pts.All(x => x.X >= 0 && x.Y >= 0)) // whole template should be on imageFrame
                        {
                            if (!(Intersects2Df(l1, l3) || Intersects2Df(l2, l4))) // opposite lines must not intersects
                            {
                                //var maxScale = 1.3;
                                //var scaleHorisontal = l1.Length > l3.Length ? l1.Length / l3.Length : l3.Length / l1.Length;
                                //var scaleVertical = l2.Length > l4.Length ? l2.Length / l4.Length : l4.Length / l2.Length;

                                //if (scaleHorisontal < maxScale && scaleVertical < maxScale)
                                {
                                    //var points = Array.ConvertAll(pts, Point.Round);
                                    //using (var vp = new VectorOfPoint(points))
                                    //{
                                    //    CvInvoke.Polylines(result, vp, true, new MCvScalar(255, 0, 0, 255), 5);
                                    //}

                                    var minX = pts.Min(x => x.X);
                                    var maxX = pts.Max(x => x.X);
                                    var minY = pts.Min(x => x.Y);
                                    var maxY = pts.Max(x => x.Y);
                                    var width = (int) Math.Ceiling(maxX - minX);
                                    var height = (int) Math.Ceiling(maxY - minY);

                                    if ((minX + width <= imgScene.Width) && (minY + height <= imgScene.Height))
                                    {
                                        var imageRect = new Rectangle((int) Math.Ceiling(minX), (int) Math.Ceiling(minY), width, height);
                                        // var imageBitmap = imgScene.Copy(imageRect);

                                        return new DetectResult
                                        {
                                            DetectedRectangle = imageRect,
                                            //   DetectedImage = imageBitmap,
                                            SharpnessValue = sharpnessValue
                                        };
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                return null;
            }
        }

        private bool Intersects2Df(LineSegment2DF thisLineSegment, LineSegment2DF otherLineSegment)
        {
            var firstLineSlopeX = thisLineSegment.P2.X - thisLineSegment.P1.X;
            var firstLineSlopeY = thisLineSegment.P2.Y - thisLineSegment.P1.Y;

            var secondLineSlopeX = otherLineSegment.P2.X - otherLineSegment.P1.X;
            var secondLineSlopeY = otherLineSegment.P2.Y - otherLineSegment.P1.Y;

            var s = (-firstLineSlopeY*(thisLineSegment.P1.X - otherLineSegment.P1.X) + firstLineSlopeX*(thisLineSegment.P1.Y - otherLineSegment.P1.Y))/(-secondLineSlopeX*firstLineSlopeY + firstLineSlopeX*secondLineSlopeY);
            var t = (secondLineSlopeX*(thisLineSegment.P1.Y - otherLineSegment.P1.Y) - secondLineSlopeY*(thisLineSegment.P1.X - otherLineSegment.P1.X))/(-secondLineSlopeX*firstLineSlopeY + firstLineSlopeX*secondLineSlopeY);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                // Collision detected
                return true;
            }
            return false; // No collision
        }

        public float CalculateBlur(Image<Bgr, byte> imageFrame)
        {
            var gray = imageFrame.Convert<Gray, byte>();
            var temp = gray.Laplace(1);
            float max = -100;
            foreach (var x in temp.Data)
            {
                if (x > max)
                {
                    max = x;
                }
            }
            return max;
        }

        public float CalculateBlur2(Image<Bgr, byte> imageFrame)
        {
            //Convert image using Canny
            using (Image<Gray, byte> imgCanny = imageFrame.Canny(225, 175))
            {
                //Count the number of pixel representing an edge
                int nCountCanny = imgCanny.CountNonzero()[0];

                //Compute a sharpness grade:
                //< 1.5 = blurred, in movement
                //de 1.5 à 6 = acceptable
                //> 6 =stable, sharp
                return (float) (nCountCanny*1000.0/(imgCanny.Cols*imgCanny.Rows));
            }
        }

        public float MatchesCount(VectorOfVectorOfDMatch matches, Mat mask)
        {
            byte[] matched = mask?.GetData();
            if (matched == null)
            {
                return 0;
            }

            return matched.Count(a => a.Equals(1));

            //for (int i = 0; i < matches.Size; i++)
            //{
            //    var a = matches[i].ToArray();
            //    if (mask.GetData(i)[0] == 0)
            //        continue;
            //    foreach (var e in a)
            //    {
            //        count++;
            //        //Point p = new Point(e.TrainIdx, e.QueryIdx);
            //        //Console.WriteLine(string.Format("Point: {0}", p));
            //    }
            //}
        }
    }
}
