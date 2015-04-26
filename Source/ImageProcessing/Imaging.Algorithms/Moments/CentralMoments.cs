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

// Accord Imaging Library
// The Accord.Extensions.NET Framework
// http://accord.googlecode.com
//
// Copyright © César Souza, 2009-2013
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

using Accord.Extensions.Math.Geometry;

namespace Accord.Extensions.Imaging.Moments
{
    using PointF = AForge.Point;

    /// <summary>
    ///   Central image moments.
    /// </summary>
    ///
    /// <remarks>
    /// <para>
    ///   In image processing, computer vision and related fields, an image moment is
    ///   a certain particular weighted average (moment) of the image pixels' intensities,
    ///   or a function of such moments, usually chosen to have some attractive property 
    ///   or interpretation.</para>
    ///
    /// <para>
    ///   Image moments are useful to describe objects after segmentation. Simple properties 
    ///   of the image which are found via image moments include area (or total intensity), 
    ///   its centroid, and information about its orientation.</para>
    ///   
    /// <para>
    ///   The central moments can be used to find the location, center of mass and the 
    ///   dimensions of a given object within an image.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       Wikipedia contributors. "Image moment." Wikipedia, The Free Encyclopedia. Wikipedia,
    ///       The Free Encyclopedia. Available at http://en.wikipedia.org/wiki/Image_moment </description></item>
    ///   </list>
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <code>
    /// Bitmap image = ...;
    ///
    /// // Compute the center moments of up to third order
    /// CentralMoments cm = new CentralMoments(image, order: 3);
    /// 
    /// // Get size and orientation of the image
    /// SizeF size = target.GetSize();
    /// float angle = target.GetOrientation();
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="RawMoments"/>
    /// <seealso cref="HuMoments"/>
    /// 
    public class CentralMoments : IMoments
    {
        /// <summary>
        ///   Gets the default maximum moment order.
        /// </summary>
        /// 
        public const int DefaultOrder = 2;

        /// <summary>
        ///   Central moment of order (0,0).
        /// </summary>
        /// 
        public float Mu00 { get; private set; }

        /// <summary>
        ///   Central moment of order (1,0).
        /// </summary>
        /// 
        public float Mu10 { get; private set; }

        /// <summary>
        ///   Central moment of order (0,1).
        /// </summary>
        /// 
        public float Mu01 { get; private set; }

        /// <summary>
        ///   Central moment of order (1,1).
        /// </summary>
        /// 
        public float Mu11 { get; private set; }

        /// <summary>
        ///   Central moment of order (2,0).
        /// </summary>
        /// 
        public float Mu20 { get; private set; }

        /// <summary>
        ///   Central moment of order (0,2).
        /// </summary>
        /// 
        public float Mu02 { get; private set; }

        /// <summary>
        ///   Central moment of order (2,1).
        /// </summary>
        /// 
        public float Mu21 { get; private set; }

        /// <summary>
        ///   Central moment of order (1,2).
        /// </summary>
        /// 
        public float Mu12 { get; private set; }

        /// <summary>
        ///   Central moment of order (3,0).
        /// </summary>
        /// 
        public float Mu30 { get; private set; }

        /// <summary>
        ///   Central moment of order (0,3).
        /// </summary>
        /// 
        public float Mu03 { get; private set; }

        /// <summary>
        ///   Gets or sets the maximum order of the moments.
        /// </summary>
        public int Order { get; set; }

        private float invM00;


        /// <summary>
        ///   Initializes a new instance of the <see cref="CentralMoments"/> class.
        /// </summary>
        /// 
        public CentralMoments(int order = DefaultOrder)
        {
            this.Order = order;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="CentralMoments"/> class.
        /// </summary>
        /// 
        /// <param name="moments">The raw moments to construct central moments.</param>
        /// 
        public CentralMoments(RawMoments moments)
            : this(moments.Order)
        {
            Compute(moments);
        }

        /// <summary>
        ///   Computes the center moments from the specified raw moments.
        /// </summary>
        /// 
        /// <param name="moments">The raw moments to use as base of calculations.</param>
        /// 
        public void Compute(RawMoments moments)
        {
            float x = moments.CenterX;
            float y = moments.CenterY;
            
            Mu00 = moments.M00;

            Mu01 = Mu10 = 0;
            Mu11 = moments.M11 - moments.M01 * x;

            Mu20 = moments.M20 - moments.M10 * x;
            Mu02 = moments.M02 - moments.M01 * y;

            Mu21 = moments.M21 - 2 * x * moments.M11 - y * moments.M20 + 2 * x * x * moments.M01;
            Mu12 = moments.M12 - 2 * y * moments.M11 - x * moments.M02 + 2 * y * y * moments.M10;

            Mu30 = moments.M30 - 3 * x * moments.M20 + 2 * x * x * moments.M10;
            Mu03 = moments.M03 - 3 * y * moments.M02 + 2 * y * y * moments.M01;

            invM00 = moments.InvM00;
        }

        /// <summary>
        /// Computes moments for the provided image.
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="area">Area</param>
        public void Compute(Gray<byte>[,] image, Rectangle area)
        {
            RawMoments rawMoments = new RawMoments(Order);
            rawMoments.Compute(image, area);

            this.Compute(rawMoments);
        }

        /// <summary>
        /// Computes moments for the provided image.
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="area">Area</param>
        public void Compute(Gray<float>[,] image, Rectangle area)
        {
            RawMoments rawMoments = new RawMoments(Order);
            rawMoments.Compute(image, area);

            this.Compute(rawMoments);
        }

        /// <summary>
        /// Gets the size and the orientation of the ellipse computed from covariance matrix.
        /// The ellipse center is at (0,0).
        /// </summary>
        /// <returns></returns>
        public Ellipse GetEllipse()
        {
            // Compute the covariance matrix
            double a = Mu20 * invM00; //                | a    b |
            double b = Mu11 * invM00; //  Cov[I(x,y)] = |        |
            double c = Mu02 * invM00; //                | b    c |

            return Ellipse.Fit(a, b, c);
        }
    }
}
