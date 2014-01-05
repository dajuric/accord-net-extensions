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
using Accord.Math.Geometry;
using LineSegment2DF = AForge.Math.Geometry.LineSegment;
using Point = AForge.IntPoint;
using PointF = AForge.Point;
using LINE2D;

namespace ParticleFilterModelFitting
{
    public class OpenHandTemplate
    {
        #region Prototype (model) Loading

        const float CONTOUR_TENSION = 0f;
        const int N_SAMPLE_POINTS = 100;

        static IList<PointF> prototypeControlPoints;
        static IList<float> prototypeIndices;
        static RectangleF prototypeBoundingBox;

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

            prototypeOrientations = prototypeIndices.Select(x => CardinalSpline.NormalAt(controlPoints, CONTOUR_TENSION, x)).ToList();
            prototypeBoundingBox = prototypePoints.BoundingRect();
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
        public RectangleF BoundingBox { get; private set; }
        public IList<int> Orientations { get; private set; }
        public IList<Feature> Features { get; private set; }

        private OpenHandTemplate()
        {}

        public static OpenHandTemplate Create(float scaleX, float scaleY, 
                                              int rotationZ) 
        {
            if (prototypePoints == null)
                throw new Exception("Prototype control points must be loaded");

            var initialTranslation = new PointF //so the template left-upper point goes to (0,0) 
            {
                X = -leftUpperPoint.X * scaleX,
                Y = -leftUpperPoint.Y * scaleY
            };

            var pointTransform = Transforms2D.Combine
                            (
                               Transforms2D.Scale(scaleX, scaleY),

                               Transforms2D.Rotation((float)Angle.ToRadians(rotationZ)),
                                                              
                               Transforms2D.Translation(initialTranslation.X, initialTranslation.Y)
                            );

            var rotationTransform = Transforms2D.Combine
                            (
                               Transforms2D.Rotation((float)Angle.ToRadians(rotationZ))                           
                            );

            var transformedPoints = prototypePoints.Transform(pointTransform);
            var orientations = prototypeOrientations.Transform(rotationTransform).Select(x => (int)Angle.ToDegrees(System.Math.Atan2(x.Y, x.X)));
                                       
            /*var transfromedControlPoints = prototypeControlPoints
                                            .Transform(Transforms.Scale(scaleX, scaleY))
                                            .Transform(Transforms.RotationX((float)Angle.ToRadians(rotationX)))
                                            .Transform(Transforms.RotationY((float)Angle.ToRadians(rotationY)))
                                            .Transform(Transforms.RotationZ((float)Angle.ToRadians(rotationZ)));

            var orientations = prototypeIndices.Select(x => CardinalSpline.NormalDirection(transformedPoints.ToList(), CONTOUR_TENSION, x)); */                           

            //var quantizedOrientations = orientations.Select(x => quantizeOrientation(x));

           //vrati u 0,0
           var br = transformedPoints.BoundingRect();
           transformedPoints=transformedPoints.Transform(Transforms2D.Translation(-br.X, -br.Y));

           var template = new OpenHandTemplate();
           template.Points = transformedPoints.Select(x=>x.Round()).ToList();
           template.Orientations = orientations.ToList();
           template.BoundingBox = transformedPoints.BoundingRect();

           var features = new List<Feature>();
           for (int i = 0; i < template.Points.Count; i++)
           {
               var f = createFeature(template.Points[i].X, template.Points[i].Y, template.Orientations[i]);
               features.Add(f);
           }

           template.Features = features;

           return template;
        }

        private static Feature createFeature(int x, int y, int orientationDeg)
        {
            byte angleIndex = FeatureMap.AngleQuantizationTable[orientationDeg];
            byte binaryIndex = Feature.GetAngleBinaryForm(angleIndex);

            return new Feature(x, y, binaryIndex);
        }

        #endregion

        #region Drawing

        private static LineSegment2DF getLine(int derivativeOrientation, PointF centerPoint, float length)
        { 
            Vector2D vec = new Vector2D(Angle.ToRadians(derivativeOrientation)).Multiply(length / 2);
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
