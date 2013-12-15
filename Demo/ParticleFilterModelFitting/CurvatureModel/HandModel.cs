using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PointF = AForge.Point;
using Accord.Math.Geometry;
using Accord.Imaging;
using System.Drawing;
using AForge.Math;
using Accord.Math;
using Accord.Core;
using LineSegment2DF = AForge.Math.Geometry.LineSegment;

namespace ParticleFilterModelFitting
{
    public class HandModel
    {
        IList<PointF> controlPoints;

        public HandModel(IList<PointF> points) 
        {
            this.controlPoints = points;
        }

        public static HandModel Load(string fileName)
        {
            var points = readCoordinates(fileName);

            points = points.Normalize();
            points = points
                     .FlipVertical(0);

            var transform = Transforms.Scale(500, 500).Multiply(
                            Transforms.RotationZ((float)Angle.ToRadians(80)));

            points = points.Transform(transform).ToList();

            var translate = Transforms.Translation(-points.Min(x => x.X), -points.Min(x => x.Y)).Multiply(
                            Transforms.Translation(50, 50));

            points = points.Transform(translate);

            var model = new HandModel(points.ToList());
            return model;
        }

        private static IEnumerable<PointF> readCoordinates(string fileName)
        {
            using (TextReader txtReader = new StreamReader(fileName))
            {
                string line;
                while ((line = txtReader.ReadLine()) != null)
                {
                    var coord = line
                                .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => Single.Parse(x, System.Globalization.CultureInfo.InvariantCulture));

                    yield return new PointF 
                    {
                        X = coord.First(),
                        Y = coord.Last()
                    };
                }
            }
        }

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
            foreach (var idx in EnumerableMethods.GetRange(CardinalSpline.MIN_INDEX, this.controlPoints.Count - 1 + CardinalSpline.MAX_INDEX_OFFSET, 0.1f))
            {
                var pt = CardinalSpline.Interpolate(this.controlPoints,  tension, idx);
                points.Add(pt);
            }

            image.Draw(points.Select(x=> new System.Drawing.PointF(x.X, x.Y)).ToArray(), 
                       new Bgr(Color.Blue), 
                       3);

            image.Draw(this.controlPoints.Select(x => new CircleF(x, 3)), new Bgr(Color.Red), 3);
            /********************  contour and control points *********************/


            foreach (var idx in EnumerableMethods.GetRange(CardinalSpline.MIN_INDEX, this.controlPoints.Count - 1 + CardinalSpline.MAX_INDEX_OFFSET, 0.5f))
            {
                var pt = CardinalSpline.Interpolate(this.controlPoints, tension, idx);
                var normalDirection = CardinalSpline.NormalDirection(this.controlPoints, tension, idx);

                var normal = getLine(normalDirection, pt, 30);

                image.Draw(normal, new Bgr(Color.Green), 3);
            }
        }

       
    }
}
