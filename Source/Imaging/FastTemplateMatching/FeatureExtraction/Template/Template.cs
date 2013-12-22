using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Accord.Imaging;

namespace LINE2D.TemplateMatching
{
    public unsafe class Template
    {
        public class Feature
        {
            public int X;
            public int Y;

            public float GradientMagnitude;

            private byte angleBinRepr;
            public byte AngleBinaryRepresentation
            {
                get { return angleBinRepr; }
                set 
                {
                    angleBinRepr = value;
                    AngleLabel = CalcAngleLabel(angleBinRepr);
                }
            }

            public byte AngleLabel
            {
                get; private set;
            }

            public Feature Clone()
            {
                return new Feature
                {
                    X = this.X,
                    Y = this.Y,
                    AngleBinaryRepresentation = this.AngleBinaryRepresentation,
                    GradientMagnitude = this.GradientMagnitude
                };
            }

            /// <summary>
            /// Calculate Log2(angleBinRepr)
            /// </summary>
            private static byte CalcAngleLabel(byte angleBinRepr)
            {
                const int MAX_NUM_OF_SHIFTS = 8; //number of bits per byte
                byte numRightShifts = 0;

                while ((angleBinRepr & 1) == 0  && numRightShifts < MAX_NUM_OF_SHIFTS)
                {
                    angleBinRepr = (byte)(angleBinRepr >> 1);
                    numRightShifts++;
                }

                if (numRightShifts == MAX_NUM_OF_SHIFTS)
                    return 0;
                else
                    return numRightShifts;
            }

            public static byte CalcAngleBinRepresentation(byte angleLabel)
            {
                return (byte)Math.Pow(2, angleLabel);
            }
        }

        public Feature[] Features = null;
        public Size Size;
        public string  ClassLabel { get; private set; }

        public Template()
        {
            this.HasMetadata = false;
        }

        internal virtual void SerializeAditionalData(XElement node) {}
        internal virtual void DeserializeAditionalData(XElement metadataNode) {}
        public bool HasMetadata { get; internal set; }

        internal void Initialize(Feature[] features, Size size, string classLabel)
        {
            this.Features = features;
            this.Size = size;
            this.ClassLabel = classLabel;
        }

        protected Rectangle boundingRect = Rectangle.Empty;
        internal void Initialize(Image<Gray, int> magnitude, Image<Gray, int> orientation, int maxNumberOfFeatures, string classLabel)
        {
            Image<Gray, Byte> quantizedOrient = ColorGradient.QuantizeOrientations(orientation);
            Image<Gray, Byte> importantQuantizedOrient = ColorGradient.RetainImportantQuantizedOrientations(quantizedOrient, magnitude, GlobalParameters.MIN_GRADIENT_THRESHOLD);

            List<Feature> features = ExtractTemplate(importantQuantizedOrient, magnitude, maxNumberOfFeatures);

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

        public virtual void Initialize(Image<Bgr, Byte> sourceImage, int maxNumberOfFeatures, string classLabel)
        {
            Image<Gray, int> magnitude, orientation;
            ColorGradient.ComputeColor(sourceImage, out magnitude, out orientation);

            this.Initialize(magnitude, orientation, maxNumberOfFeatures, classLabel);
        }

        public virtual void Initialize(Image<Gray, Byte> sourceImage, int maxNumberOfFeatures, string classLabel)
        {
            Image<Gray, int> magnitude, orientation;
            ColorGradient.ComputeGray(sourceImage, out magnitude, out orientation);

            this.Initialize(magnitude, orientation, maxNumberOfFeatures, classLabel);
        }

        private static List<Feature> ExtractTemplate(Image<Gray, Byte> orientationImage, Image<Gray, int> magnitudeImage, int maxNumOfFeatures)
        {
            byte* orientImgPtr = (byte*)orientationImage.ImageData;
            int orientImgStride = orientationImage.Stride;

            int* magImgPtr = (int*)magnitudeImage.ImageData;
            int magImgStride = magnitudeImage.Stride / sizeof(int);

            int imgWidth = orientationImage.Width;
            int imgHeight = orientationImage.Height;

            List<Feature> candidates = new List<Feature>();

            for (int row = 0; row < imgHeight; row++)
            {
                for (int col = 0; col < imgWidth; col++)
                {
                    if (magImgPtr[col] < GlobalParameters.MIN_FEATURE_STRENGTH || orientImgPtr[col] == 0)
                        continue;

                    var candidate = new Feature
                    {
                        X = col,
                        Y = row,
                        AngleBinaryRepresentation = orientImgPtr[col],
                        GradientMagnitude = magImgPtr[col]
                    };

                    candidates.Add(candidate);
                }

                orientImgPtr += orientImgStride;
                magImgPtr += magImgStride;
            }


            if (candidates.Count < GlobalParameters.MIN_NUMBER_OF_FEATURES)
                return new List<Feature>();
            else
            {
                candidates = candidates.OrderByDescending(delegate(Feature f) { return f.GradientMagnitude; }).ToList(); //order descending
                return FilterScatteredFeatures(candidates, maxNumOfFeatures, 5); //candidates.Count must be >= MIN_NUM_OF_FEATURES
            }
        }

        private static List<Feature> FilterScatteredFeatures(List<Feature> candidates, int maxNumOfFeatures, int minDistance)
        {
            Debug.Assert(candidates.Count >= GlobalParameters.MIN_NUMBER_OF_FEATURES);

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
    }


}
