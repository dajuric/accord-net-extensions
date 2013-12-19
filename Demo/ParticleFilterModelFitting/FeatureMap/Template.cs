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

        private static unsafe float GetScore(Image<Gray, byte> featureMap, IList<Point> points, IList<byte> quantizedOrientations)
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

                if (featurePt.X < 0 || featurePt.Y < 0 || featurePt.X >= featureMap.Width || featurePt.Y >= featureMap.Height)
                    continue;

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

        private static byte quantizeOrientation(PointF directionVector)
        {
            var angleDeg = (int)Angle.ToDegrees(Math.Atan2(directionVector.Y, directionVector.X));
            return FeatureMap.QuantizeOrientation(angleDeg);
        }

        public float GetScore(Image<Gray, byte> featureMap)
        {
            return GetScore(featureMap, this.Points, this.QuantizedOrientations);
        }

        #endregion

        #region Prototype (model) Loading

        const float CONTOUR_TENSION = 0f;
        const int N_SAMPLE_POINTS = 100;

        static IList<PointF> prototypeControlPoints;
        static IList<float> prototypeIndices;

        static IList<PointF> prototypePoints;
        static PointF leftUpperPoint;
        static IList<PointF> prototypeOrientations;

        public static void LoadPrototype(IList<PointF> controlPoints)
        {
            prototypeControlPoints = controlPoints;
            prototypeIndices = CardinalSpline.GetEqualyDistributedPoints(controlPoints, CONTOUR_TENSION, N_SAMPLE_POINTS).ToList();

            prototypePoints = prototypeIndices.Select(x => CardinalSpline.Interpolate(controlPoints, CONTOUR_TENSION, x)).ToList();
            leftUpperPoint = new PointF
            {
                X = prototypePoints.Min(x => x.X),
                Y = prototypePoints.Min(x => x.Y)
            };

            prototypeOrientations = prototypeIndices.Select(x => CardinalSpline.NormalDirection(controlPoints, CONTOUR_TENSION, x)).ToList();
        } 

        public static void LoadPrototype(string fileName)
        {
            var points = readCoordinates(fileName);
            LoadPrototype(points.ToList());
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

        #region Template creating (from prototype)

        public IList<Point> Points {get; private set;}
        public IList<PointF> Orientations { get; private set; }
        public IList<byte> QuantizedOrientations {get; private set;}

        private Template(IEnumerable<Point> points, IEnumerable<PointF> orientations, IEnumerable<byte> quantizedOrientations)
        {
            Points = points.ToList();
            Orientations = orientations.ToList();
            QuantizedOrientations = quantizedOrientations.ToList();
        }

        public static Template Create(float translationX, float translationY, 
                                      float scaleX, float scaleY, 
                                      int rotationX, int rotationY, int rotationZ) 
        {
            if (prototypePoints == null)
                throw new Exception("Prototype control points must be loaded");

            var initialTranslation = new PointF //so the template left-upper point goes to (0,0) 
            {
                X = -leftUpperPoint.X * scaleX,
                Y = -leftUpperPoint.Y * scaleY
            };

            /*var pt = new PointF(1, 1);

            var transform = Transforms.Combine
                            (
                               Transforms.Scale(scaleX, scaleY),

                               Transforms.RotationX((float)Angle.ToRadians(rotationX)),
                               Transforms.RotationY((float)Angle.ToRadians(rotationY)),
                               Transforms.RotationZ((float)Angle.ToRadians(rotationZ)),
                                                              
                               Transforms.Translation(initialTranslation.X, initialTranslation.Y),
                               Transforms.Translation(translationX, translationY)
                            );

            var transformedPoints1 = pt.Transform(transform);*/

            var transformedPoints = prototypePoints
                .Transform(Transforms.Scale(scaleX, scaleY))

                .Transform(Transforms.RotationX((float)Angle.ToRadians(rotationX)))
                .Transform(Transforms.RotationY((float)Angle.ToRadians(rotationY)))
                .Transform(Transforms.RotationZ((float)Angle.ToRadians(rotationZ)))

                .Transform(Transforms.Translation(initialTranslation.X, initialTranslation.Y))
                .Transform(Transforms.Translation(translationX, translationY))
                .Select(x=>x.Round());

           var orientations = prototypeOrientations
                                        .Transform(Transforms.RotationX((float)Angle.ToRadians(rotationX)))
                                        .Transform(Transforms.RotationY((float)Angle.ToRadians(rotationY)))
                                        .Transform(Transforms.RotationZ((float)Angle.ToRadians(rotationZ)));

            /*var transfromedControlPoints = prototypeControlPoints
                                            .Transform(Transforms.Scale(scaleX, scaleY))
                                            .Transform(Transforms.RotationX((float)Angle.ToRadians(rotationX)))
                                            .Transform(Transforms.RotationY((float)Angle.ToRadians(rotationY)))
                                            .Transform(Transforms.RotationZ((float)Angle.ToRadians(rotationZ)));

            var orientations = prototypeIndices.Select(x => CardinalSpline.NormalDirection(transformedPoints.ToList(), CONTOUR_TENSION, x)); */                           

            var quantizedOrientations = orientations.Select(x => quantizeOrientation(x));

            return new Template(transformedPoints, orientations, quantizedOrientations);
        }

        #endregion

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
            /********************  contour and control points *********************/
            var points = new List<PointF>();
            foreach (var idx in EnumerableMethods.GetRange(CardinalSpline.MIN_INDEX, this.Points.Count - 1 + CardinalSpline.MAX_INDEX_OFFSET, 0.1f))
            {
                var pt = CardinalSpline.Interpolate(this.Points.Select(x=>new PointF(x.X, x.Y)).ToList(), CONTOUR_TENSION, idx);
                points.Add(pt);
            }

            image.Draw(points.Select(x => new System.Drawing.PointF(x.X, x.Y)).ToArray(),
                       new Bgr(Color.Blue),
                       3);
            
            image.Draw(this.Points.Select(x => new CircleF(x, 3)), new Bgr(Color.Red), 3);
            /********************  contour and control points *********************/

            for (int i = 0; i < Points.Count; i++)
            {
                var normal = getLine(Orientations[i], Points[i], 30);
                image.Draw(normal, new Bgr(Color.Green), 3);
            }
        }

        #endregion

    }
}
