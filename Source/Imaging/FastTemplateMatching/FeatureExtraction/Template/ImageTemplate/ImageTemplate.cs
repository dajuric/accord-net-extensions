using Accord.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Xml;

namespace LINE2D
{
    public unsafe class ImageTemplate: ITemplate
    {
        public ImageTemplate() { }

        public Feature[] Features { get; private set; }
        public Size Size { get; private set; }
        public string  ClassLabel { get; private set; }

        public void Initialize(Feature[] features, Size size, string label)
        {
            this.Features = features;
            this.Size = size;
            this.ClassLabel = label;
        }

        public virtual void Initialize(Image<Bgr, byte> sourceImage, int minFeatureStrength, int maxNumberOfFeatures, string classLabel, Func<Feature, int> featureImportanceFunc = null)
        {
            Image<Gray, int> orientationImg = GradientComputation.ComputeOrientation(sourceImage, minFeatureStrength);

            Initialize(orientationImg, maxNumberOfFeatures, classLabel, featureImportanceFunc);
        }

        public virtual void Initialize(Image<Gray, byte> sourceImage, int minFeatureStrength, int maxNumberOfFeatures, string classLabel, Func<Feature, int> featureImportanceFunc = null)
        {
            Image<Gray, int> orientationImg = GradientComputation.ComputeOrientation(sourceImage, minFeatureStrength);

            Initialize(orientationImg, maxNumberOfFeatures, classLabel, featureImportanceFunc);
        }

        protected Rectangle boundingRect = Rectangle.Empty;
        public void Initialize(Image<Gray, int> orientation, int maxNumberOfFeatures, string classLabel, Func<Feature, int> featureImportanceFunc = null)
        {
            maxNumberOfFeatures = Math.Max(0, Math.Min(maxNumberOfFeatures, GlobalParameters.MAX_NUM_OF_FEATURES));
            featureImportanceFunc = (feature) => 0;

            Image<Gray, Byte> importantQuantizedOrient = FeatureMap.Caclulate(orientation, 0);
            List<Feature> features = ExtractTemplate(importantQuantizedOrient, maxNumberOfFeatures, featureImportanceFunc);

            boundingRect = GetBoundingRectangle(features);
            //if (boundingRect.X == 1 && boundingRect.Y  == 1 && boundingRect.Width == 18)
            //    Console.WriteLine();

            for (int i = 0; i < features.Count; i++)
            {
                features[i].X -= boundingRect.X;
                features[i].Y -= boundingRect.Y;

                //if(features[i].X < 0 || features[i].Y < 0)      
                //    Console.WriteLine();

                //PATCH!!!
                features[i].X = Math.Max(0, features[i].X);
                features[i].Y = Math.Max(0, features[i].Y);
            }


            this.Features = features.ToArray();
            this.Size = boundingRect.Size;
            this.ClassLabel = classLabel;
        }

        private static List<Feature> ExtractTemplate(Image<Gray, Byte> orientationImage, int maxNumOfFeatures, Func<Feature, int> featureImportanceFunc)
        {
            byte* orientImgPtr = (byte*)orientationImage.ImageData;
            int orientImgStride = orientationImage.Stride;

            int imgWidth = orientationImage.Width;
            int imgHeight = orientationImage.Height;

            List<Feature> candidates = new List<Feature>();

            for (int row = 0; row < imgHeight; row++)
            {
                for (int col = 0; col < imgWidth; col++)
                {
                    if (orientImgPtr[col] == 0) //quantized oerientations are: [1,2,4,8,...,128];
                        continue;

                    var candidate = new Feature(x: col, y: row, angleBinaryRepresentation: orientImgPtr[col]);
                    candidates.Add(candidate);
                }

                orientImgPtr += orientImgStride;
            }


            candidates = candidates.OrderByDescending(featureImportanceFunc).ToList(); //order descending
            return FilterScatteredFeatures(candidates, maxNumOfFeatures, 5); //candidates.Count must be >= MIN_NUM_OF_FEATURES
        }

        private static List<Feature> FilterScatteredFeatures(List<Feature> candidates, int maxNumOfFeatures, int minDistance)
        {
            int distance = 50;

            List<Feature> filteredFeatures = new List<Feature>();
            int distanceSqr = distance * distance;

            int i = 0;
            while (filteredFeatures.Count < maxNumOfFeatures)
            {
                bool isEnoughFar = true;
                for (int j = 0; j < filteredFeatures.Count; j++)
                {
                    int dx = candidates[i].X - filteredFeatures[j].X;
                    int dy = candidates[i].Y - filteredFeatures[j].Y;
                    int featureDistanceSqr = dx * dx + dy * dy;

                    if (featureDistanceSqr < distanceSqr)
                    {
                        isEnoughFar = false;
                        break;
                    }
                }

                if (isEnoughFar)
                    filteredFeatures.Add(candidates[i]);

                i++;

                if (i == candidates.Count) //start back at beginning, and relax required distance
                {
                    i = 0;
                    distance -= 1; //if (distance < minDistance) break;
                    distanceSqr = distance * distance;
                }
            }

            return filteredFeatures;
        }

        private static Rectangle GetBoundingRectangle(List<Feature> features)
        {
            int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;

            for (int i = 0; i < features.Count; i++)
            {
                if (minX > features[i].X)
                {
                    minX = features[i].X;
                }
                else if (maxX < features[i].X)
                {
                    maxX = features[i].X;
                }

                if (minY > features[i].Y)
                {
                    minY = features[i].Y;
                }
                else if (maxY < features[i].Y)
                {
                    maxY = features[i].Y;
                }
            }

            return new Rectangle
            {
                X = minX,
                Y = minY,
                Width = maxX - minX + 1,
                Height = maxY - minY + 1
            };
        }

        #region ISerializable

        System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        public virtual void ReadXml(XmlReader reader)
        {}

        public virtual void WriteXml(XmlWriter writer)
        {}

        #endregion   
    }


}
