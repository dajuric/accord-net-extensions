using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using LINE2D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PointF = AForge.Point;
using Range = AForge.IntRange;

namespace ParticleFilterModelFitting
{
    public class OpenHandTemplate: ITemplate
    {
        const float CONTOUR_TENSION = 0f;
        const int N_SAMPLE_POINTS = 100 + CardinalSpline.MIN_INDEX + CardinalSpline.MAX_INDEX_OFFSET + 1/*we are using integer indeces*/;

        public static Dictionary<ModelParams, ITemplate> CreateRange(string imagePath, string extension, IEnumerable<int> scaleRange, IEnumerable<int> rotationRange)
        {
            var fileNames = Directory.GetFiles(imagePath, extension);

            var dict = new Dictionary<ModelParams, ITemplate>();

            int idx = 0;
            foreach (var fileName in fileNames)
            {
                using (var img = System.Drawing.Bitmap.FromFile(fileName).ToImage<Gray, byte>())
                {
                    createRange(dict, img, idx, scaleRange, rotationRange);
                }
                idx++;
            }

            return dict;
        }

        private static void createRange(Dictionary<ModelParams, ITemplate> dict, Image<Gray, byte> templateImg,
                                        int templateIdx, IEnumerable<int> scaleRange, IEnumerable<int> rotationRange, string label = "")
        {
            var contour = findContour(templateImg);
            var pts = contour.GetEqualyDistributedPoints(N_SAMPLE_POINTS, treatAsClosed: false);
            pts = pts.Normalize();

            foreach(var s in scaleRange)
            {
                foreach(var r in rotationRange)
                {
                    var template = create(pts, s, r, label);
                    var mParams = new ModelParams(templateIdx, (short)s, (short)r);

                    dict.Add(mParams, template);
                }
            }
        }

        private static List<PointF> findContour(Image<Gray, byte> templateImg)
        {
            var contour = templateImg.FindContour(minGradientStrength: 150).Select(x => (PointF)x).ToList();

            /*********** cut bottom border and shift contour beginning to the first non-border point ***************/
            int firstIdx = -1;
            int lastIdx = -1;

            for (int i = 0; i < contour.Count; i++)
            {
                if (contour[i].Y == (templateImg.Height - 1))
                {
                    if (firstIdx == -1) firstIdx = i;
                    lastIdx = i;
                }
            }

            //return contour;
            return new CircularList<PointF>(contour).GetRange(new Range(lastIdx, firstIdx));
        }

        private static ITemplate create(IEnumerable<PointF> normalizedPoints,
                                        int scale,
                                        int rotation,
                                        string label = "")
        {
            var pointTransform = Transforms2D.Combine
                            (
                               Transforms2D.Scale(scale, scale),
                               Transforms2D.Rotation((float)Angle.ToRadians(rotation))
                            );

            var transformedPts = normalizedPoints.Transform(pointTransform).ToList();

            var boundingRect = transformedPts.BoundingRect();
            var offset = boundingRect.Location;
            transformedPts = transformedPts.Transform(Transforms2D.Translation(-offset.X, -offset.Y)).ToList();

            var template = new OpenHandTemplate();

            var features = new List<Feature>();
            //var validIdxRange = CardinalSpline.ValidIndicesRange(transformedPts.Count);
            //for (int i = validIdxRange.Min; i <= validIdxRange.Max; i++)
            for (int i = CardinalSpline.MIN_INDEX; i < (transformedPts.Count - 1 - CardinalSpline.MAX_INDEX_OFFSET); i++)
            {
                var intPt = transformedPts[i].Round();

                var direction = CardinalSpline.NormalAt(transformedPts, CONTOUR_TENSION, i);
                var orientDeg = (int)Angle.ToDegrees(Math.Atan2(direction.Y, direction.X));
                orientDeg = (int)Angle.NormalizeDegrees(orientDeg + 180);

                var feature = createFeature(intPt.X, intPt.Y, orientDeg);
                features.Add(feature);
            }

            template.Features = features.ToArray();
            template.Size = Size.Round(boundingRect.Size);
            template.ClassLabel = label;

            return template;
        }

        private static Feature createFeature(int x, int y, int orientationDeg)
        {
            byte angleIndex = FeatureMap.AngleQuantizationTable[orientationDeg];
            byte binaryIndex = Feature.GetAngleBinaryForm(angleIndex);

            return new Feature(x, y, binaryIndex);
        }

        public Feature[] Features
        {
            get;
            private set;
        }

        public Size Size
        {
            get;
            private set;
        }

        public string ClassLabel
        {
            get;
            private set;
        }

        void ITemplate.Initialize(Feature[] features, Size size, string classLabel)
        {
            this.Features = features;
            this.Size = size;
            this.ClassLabel = classLabel;
        }
    }
}
