using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using LineSegment2DF = AForge.Math.Geometry.LineSegment;
using PointF = AForge.Point;

namespace Accord.Extensions.Imaging.Algorithms.LNE2D
{
    /// <summary>
    /// Represents LINE2D template interface.
    /// </summary>
    public interface ITemplate
    {
        /// <summary>
        /// Gets template features.
        /// </summary>
        Feature[] Features { get; }
        /// <summary>
        /// Gets template size (features bounding box).
        /// </summary>
        Size Size { get; }
        /// <summary>
        /// Gets class label for the template.
        /// </summary>
        string ClassLabel { get; }

        /// <summary>
        /// Initializes template. Used during de-serialization.
        /// </summary>
        /// <param name="features">Collection of features.</param>
        /// <param name="size">Template size.</param>
        /// <param name="classLabel">Template class label.</param>
        void Initialize(Feature[] features, Size size, string classLabel);
    }

    /// <summary>
    /// Contains methods for template drawing.
    /// </summary>
    public static class ImageTemplateExtensions
    {
        /// <summary>
        /// Draws LINE 2D template.
        /// </summary>
        /// <typeparam name="TColor">Color type.</typeparam>
        /// <param name="image">Image.</param>
        /// <param name="template">Template.</param>
        /// <param name="offset">Template features drawing offset.</param>
        /// <param name="color">Features color.</param>
        /// <param name="thickness">Line thickness.</param>
        /// <param name="drawOrientations">True to draw features orientations, false to draw plain contour.</param>
        /// <param name="orientationColor">Orientation color.</param>
        public static void Draw<TColor>(this Image<TColor, Byte> image, ITemplate template, PointF offset, TColor color, int thickness = 2,
                                        bool drawOrientations = false, TColor orientationColor = default(TColor))
            where TColor: IColor3
        {
            const float VECTOR_LENGTH = 10; //it needs to be neigborhood * 2

            var circles = template.Features.Select(x => new CircleF
            {
                X = x.X + offset.X,
                Y = x.Y + offset.Y,
                Radius = 2
            });
            image.Draw(circles, color, thickness);

            if (drawOrientations)
            {
                float degSpace = 180f / GlobalParameters.NUM_OF_QUNATIZED_ORIENTATIONS;

                var lines = new List<LineSegment2DF>();
                foreach (var f in template.Features)
                {
                    var orientDeg = f.AngleIndex * degSpace;
                    orientDeg += degSpace / 2;

                    var pt = new PointF(f.X + offset.X, f.Y + offset.Y);
                    var line = getLine((int)orientDeg, VECTOR_LENGTH, pt);

                    lines.Add(line);
                }

                image.Draw(lines, orientationColor, thickness, connectLines: false);
            }
        }

        private static LineSegment2DF getLine(int derivativeOrientation, float length, PointF centerPoint)
        {
            Vector2D vec = new Vector2D(Angle.ToRadians(derivativeOrientation)).Multiply(length / 2);
            var p1 = vec.Add(centerPoint);
            var p2 = vec.Negate().Add(centerPoint);

            return new LineSegment2DF(p1, p2);
        }
    }
}
