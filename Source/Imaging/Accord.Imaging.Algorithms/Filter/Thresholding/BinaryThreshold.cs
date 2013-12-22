using Accord.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging
{
    public static class BinaryThreshold
    {
        /// <summary>
        /// Applies binary threshold to a input image.
        /// </summary>
        /// <param name="minValue">Minimal value in range.</param>
        /// <param name="maxValue">Maximum value in range.</param>
        /// <param name="valueToSet">Value to set after threshold is applied.</param>
        /// <returns> 
        /// Binary mask for which values !=0 are where source values are in specified range.
        /// To get values use Copy extension.
        /// </returns>
        public static Image<TColor, TDepth> ThresholdBinary<TColor, TDepth>(this Image<TColor, TDepth> img, TColor minValue, TColor maxValue, TColor valueToSet)
            where TColor : IColor
            where TDepth : struct
        {
            Image<TColor, TDepth> valueMask = new Image<TColor, TDepth>(img.Width, img.Height, valueToSet);

            var mask = img.InRange(minValue, maxValue);

            var result = img.CopyBlank(); //TODO - critical-mmedium: solve this to extend SetValue extension with mask paremeter (faster and less memory hungry)
            valueMask.Copy(result, mask);

            return result;
        }

        /// <summary>
        /// Applies binary threshold to a input image.
        /// <para>
        /// Pixels which are not in [min..max] range are set to zero.
        /// </para>
        /// </summary>
        /// 
        /// <param name="minValue">Minimal value in range.</param>
        /// <param name="maxValue">Maximum value in range.</param>
        /// <returns>Thresholded image where pixels which are not in [min..max] range are set to zero.</returns>
        public static Image<TColor, TDepth> ThresholdToZero<TColor, TDepth>(this Image<TColor, TDepth> img, TColor minValue, TColor maxValue)
            where TColor : IColor
            where TDepth : struct
        {
            Image<TColor, TDepth> result = new Image<TColor, TDepth>(img.Width, img.Height);
      
            var mask = img.InRange(minValue, maxValue);
            img.Copy(result, mask); 

            return result;
        }
    }

}
