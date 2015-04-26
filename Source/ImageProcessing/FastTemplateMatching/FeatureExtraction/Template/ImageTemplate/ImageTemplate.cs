#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014-2015 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Accord.Extensions;
using Accord.Extensions.Imaging;

namespace Accord.Extensions.Imaging.Algorithms.LINE2D
{
    /// <summary>
    /// LINE2D template. The class contains methods for template extraction from an image.
    /// </summary>
    public unsafe class ImageTemplate: ITemplate, IXmlSerializable
    {
        /// <summary>
        /// Creates an empty image template. Requires initialization.
        /// </summary>
        public ImageTemplate() { }

        /// <summary>
        /// Gets template features.
        /// </summary>
        public Feature[] Features { get; private set; }
        /// <summary>
        /// Gets template size (features bounding box).
        /// </summary>
        public Size Size { get; private set; }
        /// <summary>
        /// Gets class label for the template.
        /// </summary>
        public string  ClassLabel { get; private set; }

        /// <summary>
        /// Initializes template. Used during de-serialization.
        /// </summary>
        /// <param name="features">Collection of features.</param>
        /// <param name="size">Template size.</param>
        /// <param name="classLabel">Template class label.</param>
        public void Initialize(Feature[] features, Size size, string classLabel)
        {
            this.Features = features;
            this.Size = size;
            this.ClassLabel = classLabel;
        }

        /// <summary>
        /// Creates template from the input image by using provided parameters.
        /// </summary>
        /// <param name="sourceImage">Input image.</param>
        /// <param name="minFeatureStrength">Minimum gradient value for the feature.</param>
        /// <param name="maxNumberOfFeatures">Maximum number of features per template. The features will be extracted so that their locations are semi-uniformly spread.</param>
        /// <param name="classLabel">Template class label.</param>
        public virtual void Initialize(Bgr<byte>[,] sourceImage, int minFeatureStrength, int maxNumberOfFeatures, string classLabel)
        {
            Gray<int>[,] sqrMagImg;
            Gray<int>[,] orientationImg = GradientComputation.Compute(sourceImage, out sqrMagImg, minFeatureStrength);

            Func<Feature, int> featureImportanceFunc = (feature) => sqrMagImg[feature.Y, feature.X].Intensity;

            Initialize(orientationImg, maxNumberOfFeatures, classLabel, featureImportanceFunc);
        }

        /// <summary>
        /// Creates template from the input image by using provided parameters.
        /// </summary>
        /// <param name="sourceImage">Input image.</param>
        /// <param name="minFeatureStrength">Minimum gradient value for the feature.</param>
        /// <param name="maxNumberOfFeatures">Maximum number of features per template. The features will be extracted so that their locations are semi-uniformly spread.</param>
        /// <param name="classLabel">Template class label.</param>
        public virtual void Initialize(Gray<byte>[,] sourceImage, int minFeatureStrength, int maxNumberOfFeatures, string classLabel)
        {
            Gray<int>[,] sqrMagImg;
            Gray<int>[,] orientationImg = GradientComputation.Compute(sourceImage, out sqrMagImg, minFeatureStrength);

            Func<Feature, int> featureImportanceFunc = (feature) => sqrMagImg[feature.Y, feature.X].Intensity;

            Initialize(orientationImg, maxNumberOfFeatures, classLabel, featureImportanceFunc);
        }

        /// <summary>
        /// Template bounding rectangle.
        /// </summary>
        protected Rectangle BoundingRect = Rectangle.Empty;

        /// <summary>
        /// Creates template from the input image by using provided parameters.
        /// </summary>
        /// <param name="orientation">Orientation image.</param>
        /// <param name="maxNumberOfFeatures">Maximum number of features per template. The features will be extracted so that their locations are semi-uniformly spread.</param>
        /// <param name="classLabel">Template class label.</param>
        /// <param name="featureImportanceFunc">Function which returns feature's strength.</param>
        public void Initialize(Gray<int>[,] orientation, int maxNumberOfFeatures, string classLabel, Func<Feature, int> featureImportanceFunc = null)
        {
            maxNumberOfFeatures = System.Math.Max(0, System.Math.Min(maxNumberOfFeatures, GlobalParameters.MAX_NUM_OF_FEATURES));
            featureImportanceFunc = (featureImportanceFunc != null) ? featureImportanceFunc: (feature) => 0;

            Gray<byte>[,] importantQuantizedOrient = FeatureMap.Calculate(orientation, 0);
            List<Feature> features = ExtractTemplate(importantQuantizedOrient, maxNumberOfFeatures, featureImportanceFunc);

            BoundingRect = GetBoundingRectangle(features);
            //if (boundingRect.X == 1 && boundingRect.Y  == 1 && boundingRect.Width == 18)
            //    Console.WriteLine();

            for (int i = 0; i < features.Count; i++)
            {
                features[i].X -= BoundingRect.X;
                features[i].Y -= BoundingRect.Y;

                //if(features[i].X < 0 || features[i].Y < 0)      
                //    Console.WriteLine();

                //PATCH!!!
                features[i].X = System.Math.Max(0, features[i].X);
                features[i].Y = System.Math.Max(0, features[i].Y);
            }


            this.Features = features.ToArray();
            this.Size = BoundingRect.Size;
            this.ClassLabel = classLabel;
        }

        private static List<Feature> ExtractTemplate(Gray<byte>[,] orientationImage, int maxNumOfFeatures, Func<Feature, int> featureImportanceFunc)
        {
            List<Feature> candidates = new List<Feature>();

            using (var uOrientationImage = orientationImage.Lock())
            {
                byte* orientImgPtr = (byte*)uOrientationImage.ImageData;
                int orientImgStride = uOrientationImage.Stride;

                int imgWidth = uOrientationImage.Width;
                int imgHeight = uOrientationImage.Height;

                for (int row = 0; row < imgHeight; row++)
                {
                    for (int col = 0; col < imgWidth; col++)
                    {
                        if (orientImgPtr[col] == 0) //quantized orientations are: [1,2,4,8,...,128];
                            continue;

                        var candidate = new Feature(x: col, y: row, angleBinaryRepresentation: orientImgPtr[col]);
                        candidates.Add(candidate);
                    }

                    orientImgPtr += orientImgStride;
                }
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

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">Reader's stream.</param>
        public virtual void ReadXml(XmlReader reader)
        {}

        /// <summary>
        /// Generates XML representation for the object.
        /// </summary>
        /// <param name="writer">Writers stream.</param>
        public virtual void WriteXml(XmlWriter writer)
        {}

        #endregion   
    }


}
