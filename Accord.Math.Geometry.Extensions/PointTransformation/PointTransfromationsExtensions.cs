using System.Collections.Generic;
using System.Linq;
using PointF = AForge.Point;

namespace Accord.Math.Geometry
{
    public static class PointTransfromationsExtensions
    {
        /// <summary>
        /// Transfroms 3D point by using transformation matrix (3x3)
        /// </summary>
        /// <param name="point">Point to transform</param>
        /// <param name="transformationMat">Transformation matrix</param>
        /// <returns>Transformed point</returns>
        public static Point3 Transform(this Point3 point, float[,] transformationMat)
        {
            return new Point3 
            {
                X = transformationMat[0, 0] * point.X + transformationMat[0, 1] * point.Y + transformationMat[0, 2] * point.Z,
                Y = transformationMat[1, 0] * point.X + transformationMat[1, 1] * point.Y + transformationMat[1, 2] * point.Z,
                Z = transformationMat[2, 0] * point.X + transformationMat[2, 1] * point.Y + transformationMat[2, 2] * point.Z,
            };
        }

        /// <summary>
        /// Projects point regarding camera.
        /// </summary>
        /// <param name="point">Point to project.</param>
        /// <param name="camera">Camera position (default is (0,0,0))</param>
        /// <returns>Projected point</returns>
        public static PointF Project(this Point3 point, Point3 camera = default(Point3))
        {
            var F = point.Z - camera.Z;

            return new PointF
            {
                X = ((point.X - camera.X) * (F / point.Z)) + camera.X,
                Y = ((point.Y - camera.Y) * (F / point.Z)) + camera.Y
            };
        }

        /// <summary>
        /// Transforms 3D point to 2D point using transformation matrix and projection regarding camera position.
        /// </summary>
        /// <param name="point">Point to transform.</param>
        /// <param name="transformationMat">Transformation matrix.</param>
        /// <param name="camera">Camera position. Default is (0,0,0).</param>
        /// <returns>Transformed point.</returns>
        public static PointF Transform(this PointF point, float[,] transformationMat, Point3 camera = default(Point3))
        {
            var point3 = new Point3(point.X, point.Y, 1);
            return point3.Transform(transformationMat).Project(camera);
        }

        public static PointF FlipVertical(this PointF point, float yCoordinate = 0)
        {
            return new PointF
            {
                X = point.X,
                Y = 2 * yCoordinate - point.Y
            };
        }

        public static PointF FlipHorizontal(this PointF point, float xCoordinate = 0)
        {
            return new PointF
            {
                X = 2 * xCoordinate - point.X,
                Y = point.Y
            };
        }

        #region IEnumerable

        public static IEnumerable<PointF> Transform(this IEnumerable<PointF> points, float[,] transformationMat)
        {
            foreach (var p in points)
            {
                yield return p.Transform(transformationMat);
            }
        }

        public static IEnumerable<PointF> FlipVertical(this IEnumerable<PointF> points, float yCoordinate = 0)
        {
            foreach (var p in points)
            {
                yield return p.FlipVertical(yCoordinate);
            }
        }

        public static IEnumerable<PointF> FlipHorizontal(this IEnumerable<PointF> points, float xCoordinate = 0)
        {
            foreach (var p in points)
            {
                yield return p.FlipHorizontal(xCoordinate);
            }
        }

        /// <summary>
        /// Normalizes point cloud to range [0..1]. Ratios will be preserved.
        /// </summary>
        /// <param name="points">Points to normalize.</param>
        /// <returns>Normalized points.</returns>
        public static IEnumerable<PointF> Normalize(this IEnumerable<PointF> points)
        {
            var minPt = new PointF
            {
                X = points.Min(x => x.X),
                Y = points.Min(x => x.Y)
            };

            var maxPt = new PointF
            {
                X = points.Max(x => x.X),
                Y = points.Max(x => x.Y)
            };

            var mean = new PointF
            {
                X = points.Average(x => x.X),
                Y = points.Average(x => x.Y)
            };

            var scaleFactor = System.Math.Max(maxPt.X - minPt.X, maxPt.Y - minPt.Y);

            var transform =  Transforms.Translation(-mean.X, -mean.Y).Multiply(
                             Transforms.Scale(1 / scaleFactor, 1 / scaleFactor));

            points = points.Transform(transform);
               
            return points;
        }

        #endregion
    }
}
