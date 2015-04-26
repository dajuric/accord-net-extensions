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
using Accord.Extensions.Imaging;
using Accord.Extensions.Imaging.Moments;
using Accord.Extensions.Math.Geometry;

namespace Accord.Extensions.Imaging
{

    /// <summary>
    ///   Continuously Adaptive Mean Shift (Camshift) Object Tracker
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Camshift stands for "Continuously Adaptive Mean Shift". It combines the basic
    ///   Mean Shift algorithm with an adaptive region-sizing step. The kernel is a step
    ///   function applied to a probability map. The probability of each image pixel is
    ///   based on color using a method called histogram back-projection.</para>
    /// <para>
    ///   The implementation of this code has used Gary Bradski's original publication,
    ///   the OpenCV Library and the FaceIt implementation as references. The OpenCV
    ///   library is distributed under a BSD license. FaceIt is distributed under a MIT
    ///   license. The original licensing terms for FaceIt are described in the source
    ///   code and in the Copyright.txt file accompanying the framework.</para>  
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description>
    ///       G.R. Bradski, Computer video face tracking for use in a perceptual user interface,
    ///       Intel Technology Journal, Q2 1998. Available on:
    ///       <a href="ftp://download.intel.com/technology/itj/q21998/pdf/camshift.pdf">
    ///       ftp://download.intel.com/technology/itj/q21998/pdf/camshift.pdf </a></description></item>
    ///     <item><description>
    ///       R. Hewitt, Face tracking project description: Camshift Algorithm. Available on:
    ///       <a href="http://www.robinhewitt.com/research/track/camshift.html">
    ///       http://www.robinhewitt.com/research/track/camshift.html </a></description></item>
    ///     <item><description>
    ///       OpenCV Computer Vision Library. Available on:
    ///       <a href="http://sourceforge.net/projects/opencvlibrary/">
    ///       http://sourceforge.net/projects/opencvlibrary/ </a></description></item>
    ///     <item><description>
    ///       FaceIt object tracking in Flash AS3. Available on:
    ///       <a href="http://www.libspark.org/browser/as3/FaceIt">
    ///       http://www.libspark.org/browser/as3/FaceIt </a></description></item>
    ///  </list></para>  
    /// </remarks>
    /// 
    public static class Camshift
    {
        /// <summary>
        /// Camshift algorithm
        /// </summary>
        /// <param name="probabilityMap">Probability map [0-1].</param>
        /// <param name="roi">Initial Search area</param>
        /// <returns>Object position, size and angle packed into a structure.</returns>
        public static Box2D Process(Gray<byte>[,] probabilityMap, Rectangle roi)
        {
            CentralMoments centralMoments;
            return Process(probabilityMap, roi, Meanshift.DEFAULT_TERM, out centralMoments);
        }

        /// <summary>
        /// Camshift algorithm
        /// </summary>
        /// <param name="probabilityMap">Probability map [0-255].</param>
        /// <param name="roi">Initial Search area</param>
        /// <param name="termCriteria">Mean shift termination criteria (PLEASE DO NOT REMOVE (but you can move it) THIS CLASS; PLEASE!!!)</param>
        /// <param name="centralMoments">Calculated central moments (up to order 2).</param>
        /// <returns>Object position, size and angle packed into a structure.</returns>
        public static Box2D Process(Gray<byte>[,] probabilityMap, Rectangle roi, TermCriteria termCriteria, out CentralMoments centralMoments)
        {         
            // Compute mean shift
            Rectangle objArea = Meanshift.Process(probabilityMap, roi, termCriteria, out centralMoments);

            //fit ellipse
            Ellipse ellipse = centralMoments.GetEllipse();
            ellipse.Center = objArea.Center();

            //return empty structure is the object is lost
            var sz = ellipse.Size;
            if (Single.IsNaN(sz.Width) || Single.IsNaN(sz.Height) ||
                sz.Width < 1 || sz.Height < 1)
            {
                return Box2D.Empty;
            }

            return (Box2D)ellipse;
        }
    }

}
