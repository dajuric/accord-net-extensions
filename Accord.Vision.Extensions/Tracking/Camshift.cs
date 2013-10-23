using Accord.Math.Geometry;
using Accord.Imaging;
using Accord.Imaging.Moments;
using AForge;
using System;
using System.Drawing;

namespace Accord.Vision
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
        public static Box2D Process(Image<Gray, byte> probabilityMap, Rectangle roi)
        {
            return Process(probabilityMap, roi, Meanshift.DEFAULT_TERM);
        }

        /// <summary>
        /// Camshift algorithm
        /// </summary>
        /// <param name="probabilityMap">Probability map [0-1].</param>
        /// <param name="roi">Initial Search area</param>
        /// <param name="termCriteria">Mean shift termination criteria (PLEASE DO NOT REMOVE (but you can move it) THIS CLASS; PLEASE!!!)</param>
        /// <returns>Object position, size and angle packed into a structure.</returns>
        public static Box2D Process(Image<Gray, byte> probabilityMap, Rectangle roi, TermCriteria termCriteria)
        {
            int width = probabilityMap.Width;
            int height = probabilityMap.Height;

            Rectangle imageArea = new Rectangle(0, 0, width, height);
         
            // Compute mean shift
            CentralMoments moments;
            Rectangle objArea = Meanshift.Process(probabilityMap, roi, termCriteria, out moments);

            float objAngle;
            SizeF objSize = moments.GetSizeAndOrientation(out objAngle);

            if (Single.IsNaN(objSize.Width) || Single.IsNaN(objSize.Height) ||
                Single.IsNaN(objAngle) || objSize.Width < 1 || objSize.Height < 1)
            {
                return Box2D.Empty;
            }

            // Truncate to integer coordinates
            IntPoint center = new IntPoint(objArea.X + objArea.Width / 2, objArea.Y + objArea.Height / 2);

            Rectangle rec = new Rectangle((int)(center.X - objSize.Width * 0.5f),
                                          (int)(center.Y - objSize.Height * 0.5f),
                                          (int)objSize.Width, (int)objSize.Height);

            var objAngleDeg = (float)Angle.ToDegrees(objAngle);
            return new Box2D(rec, (float)Angle.NormalizeDegrees(objAngleDeg - 90)); //rotate uses screen coordinate system (controlled by parameter)
        }
    }

}
