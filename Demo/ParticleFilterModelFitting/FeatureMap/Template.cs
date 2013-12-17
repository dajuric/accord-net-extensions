using Accord.Core;
using Accord.Imaging;
using Accord.Math;
using Accord.Math.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using LineSegment2DF = AForge.Math.Geometry.LineSegment;
using Point = AForge.IntPoint;
using PointF = AForge.Point;

namespace ParticleFilterModelFitting
{
    public class Template
    {
        #region Evaluation 

        const int MAX_FEATURE_SIMILARITY = 4;

        public static unsafe float GetScore(Image<Gray, byte> featureMap, IList<Point> points, IList<byte> quantizedOrientations)
        {
            if (points.Count != quantizedOrientations.Count)
                throw new NotSupportedException();

            int numOfFeatures = points.Count;
            float scaleFactor = 1f / (MAX_FEATURE_SIMILARITY * numOfFeatures);

            int similarity = 0;
            for (int i = 0; i < numOfFeatures; i++)
            {
                //template
                var featurePt = points[i];
                var featureAngle = quantizedOrientations[i];

                //image
                var imageAngles = *(byte*)featureMap.GetData(featurePt.Y, featurePt.X);

                //score
                var featureSimilarity = getFeatureSimilarity(featureAngle, imageAngles);
                similarity += featureSimilarity;
            }

            return similarity * scaleFactor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte getFeatureSimilarity(byte templateAngle, byte imageAngles)
        {
            //the closest 1 on the left...
            byte numOfLeftShifts = 0;

            while (((imageAngles << numOfLeftShifts) & templateAngle) == 0 && numOfLeftShifts < MAX_FEATURE_SIMILARITY)
            {
                numOfLeftShifts++;
            }

            //the closest 1 on the right...
            byte numOfRightShifts = 0;

            while (((imageAngles >> numOfRightShifts) & templateAngle) == 0 && numOfRightShifts < MAX_FEATURE_SIMILARITY)
            {
                numOfRightShifts++;
            }

            //the less shifts, the bigger similarity
            byte similarity = (byte)(MAX_FEATURE_SIMILARITY - Math.Min(numOfLeftShifts, numOfRightShifts));

            return similarity;
        }

        #endregion

        #region Prototype (model) Loading

        static IList<PointF> prototypeControlPoints;
        static PointF leftUpperPoint;

        public static void LoadPrototype(IEnumerable<PointF> controlPoints)
        {
            prototypeControlPoints = controlPoints.ToList();
            leftUpperPoint = new PointF
            {
                X = prototypeControlPoints.Min(x => x.X),
                Y = prototypeControlPoints.Min(x => x.Y)
            };
        } 

        public static void LoadPrototype(string fileName)
        {
            var points = readCoordinates(fileName);
            LoadPrototype(points);
        }

        private static IEnumerable<PointF> readCoordinates(string fileName)
        {
            using (TextReader txtReader = new StreamReader(fileName))
            {
                string line;
                while ((line = txtReader.ReadLine()) != null)
                {
                    var coord = line
                                .Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => Single.Parse(x, System.Globalization.CultureInfo.InvariantCulture));

                    yield return new PointF
                    {
                        X = coord.First(),
                        Y = coord.Last()
                    };
                }
            }
        }

        #endregion

        public IList<PointF> ControlPoints {get; private set;}
        IList<byte> quantizedOrientations;

        private Template(IEnumerable<PointF> points)
        {
            ControlPoints = points.ToList();
        }

        public static Template Create(float translationX, float translationY, 
                                      float scaleX, float scaleY, 
                                      int rotationX, int rotationY, int rotationZ) 
        {
            if (prototypeControlPoints == null)
                throw new Exception("Prototype control points must be loaded");

            var initialTranslation = new PointF //so the template left-upper point goes to (0,0) 
            {
                X = -leftUpperPoint.X * scaleX,
                Y = -leftUpperPoint.Y * scaleY
            };

            var transform = Transforms.Combine
                            (
                                Transforms.RotationX((float)Angle.ToRadians(rotationX)),
                                Transforms.RotationY((float)Angle.ToRadians(rotationY)),
                                Transforms.RotationZ((float)Angle.ToRadians(rotationZ)),

                                Transforms.Scale(scaleX, scaleY),

                                Transforms.Translation(initialTranslation.X + translationX, initialTranslation.Y + translationY)
                            );

            var transformedPoints = prototypeControlPoints.Transform(transform);

            return new Template(transformedPoints);
        }

        #region Drawing

        private static LineSegment2DF getLine(PointF normalDirection, PointF centerPoint, float length)
        {
            Vector2D vec = ((Vector2D)normalDirection).Multiply(length / 2);
            var p1 = vec.Add(centerPoint);
            var p2 = vec.Negate().Add(centerPoint);

            return new LineSegment2DF(p1, p2);
        }

        public void Draw(Image<Bgr, byte> image)
        {
            var tension = 0f;

            /********************  contour and control points *********************/
            var points = new List<PointF>();
            foreach (var idx in EnumerableMethods.GetRange(CardinalSpline.MIN_INDEX, this.ControlPoints.Count - 1 + CardinalSpline.MAX_INDEX_OFFSET, 0.1f))
            {
                var pt = CardinalSpline.Interpolate(this.ControlPoints, tension, idx);
                points.Add(pt);
            }

            image.Draw(points.Select(x => new System.Drawing.PointF(x.X, x.Y)).ToArray(),
                       new Bgr(Color.Blue),
                       3);

            image.Draw(this.ControlPoints.Select(x => new CircleF(x, 3)), new Bgr(Color.Red), 3);
            /********************  contour and control points *********************/

            //foreach (var idx in EnumerableMethods.GetRange(CardinalSpline.MIN_INDEX, this.ControlPoints.Count - 1 + CardinalSpline.MAX_INDEX_OFFSET, 0.5f))
            foreach (var idx in CardinalSpline.GetEqualyDistributedPoints(this.ControlPoints, tension, 100))
            {
                var pt = CardinalSpline.Interpolate(this.ControlPoints, tension, idx);
                var normalDirection = CardinalSpline.NormalDirection(this.ControlPoints, tension, idx);

                var normal = getLine(normalDirection, pt, 30);

                image.Draw(normal, new Bgr(Color.Green), 3);
            }
        }

        #endregion

    }
}
