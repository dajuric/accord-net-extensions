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
using Accord.Extensions.Imaging.Filters;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Storage for pyramidal images and its derivative which are used in LK flow.
    /// Very often sequential images are processed therefore calculations for both images are redundant. 
    /// This class is using this property to skip some calculations and therefore speeds up sequential frame processing by 2x!
    /// </summary>
    /// <typeparam name="TColor">Image color.</typeparam>
    public class PyrLKStorage<TColor>
        where TColor : struct, IColor<float>
    {
        /// <summary>
        /// Creates LK storage.
        /// </summary>
        /// <param name="pyrLevels">Number of pyramid levels. Minimal is 0 - only current image will be used.</param>
        public PyrLKStorage(int pyrLevels)
        {
            this.PyrLevels = pyrLevels;
        }

        /// <summary>
        /// Number of pyramidal levels
        /// </summary>
        public int PyrLevels { get; private set; }

        TColor[,] prevCallCurrImg = null;
        /// <summary>
        /// Calculates pyramid and its derivations.
        /// </summary>
        /// <param name="prevImg">Previous image.</param>
        /// <param name="currImg">Current image.</param>
        public void Process(TColor[,] prevImg, TColor[,] currImg)
        {
            if (prevCallCurrImg != null && prevCallCurrImg.Equals(prevImg)) //reuse calculated structures if can (CurrImg is previous call CurrImg)
            {
                for (int pyrLevel = this.PyrLevels; pyrLevel >= 0; pyrLevel--)
                {
                    PrevImgPyr[pyrLevel] = CurrImgPyr[pyrLevel];
                    PrevImgPyrDerivX[pyrLevel] = CurrImgPyrDerivX[pyrLevel];
                    PrevImgPyrDerivY[pyrLevel] = CurrImgPyrDerivY[pyrLevel];
                }
            }
            else
            {
                TColor[][,] _prevImgPyr, _prevImgPyrDerivX, _prevImgPyrDerivY;
                calcPyrAndDerivatives(prevImg, this.PyrLevels, out _prevImgPyr, out _prevImgPyrDerivX, out _prevImgPyrDerivY);
                this.PrevImgPyr = _prevImgPyr;
                this.PrevImgPyrDerivX = _prevImgPyrDerivX;
                this.PrevImgPyrDerivY = _prevImgPyrDerivY;
            }

            TColor[][,] _currImgPyr, _currImgPyrDerivX, _currImgPyrDerivY;
            calcPyrAndDerivatives(currImg, this.PyrLevels, out _currImgPyr, out _currImgPyrDerivX, out _currImgPyrDerivY);
            this.CurrImgPyr = _currImgPyr;
            this.CurrImgPyrDerivX = _currImgPyrDerivX;
            this.CurrImgPyrDerivY = _currImgPyrDerivY;

            prevCallCurrImg = currImg;
        }

        private static void calcPyrAndDerivatives(TColor[,] img, int levels, out TColor[][,] pyr, out TColor[][,] derivX, out TColor[][,] derivY)
        {
            pyr = new TColor[levels + 1][,];
            derivX = new TColor[levels + 1][,];
            derivY = new TColor[levels + 1][,];

            for (int pyrLevel = levels; pyrLevel >= 0; pyrLevel--)
            { 
                var imPyr = img.PyrDown(pyrLevel);

                pyr[pyrLevel] = imPyr;
                derivX[pyrLevel] = imPyr.Sobel(xOrder: 1, yOrder: 0, apertureSize: 3, normalizeKernel: true);
                derivY[pyrLevel] = imPyr.Sobel(xOrder: 0, yOrder: 1, apertureSize: 3, normalizeKernel: true);
            }
        }

        /// <summary>
        /// Previous image pyramid.
        /// </summary>
        public TColor[][,] PrevImgPyr { get; private set; }
        /// <summary>
        /// Current image pyramid.
        /// </summary>
        public TColor[][,] CurrImgPyr { get; private set; }

        /// <summary>
        /// Previous image pyramidal horizontal derivatives
        /// </summary>
        public TColor[][,] PrevImgPyrDerivX { get; private set; }
        /// <summary>
        /// Current image pyramidal horizontal derivatives
        /// </summary>
        public TColor[][,] CurrImgPyrDerivX { get; private set; }
        /// <summary>
        /// Previous image pyramidal vertical derivatives
        /// </summary>
        public TColor[][,] PrevImgPyrDerivY { get; private set; }
        /// <summary>
        /// Current image pyramidal vertical derivatives
        /// </summary>
        public TColor[][,] CurrImgPyrDerivY { get; private set; }
    }
}
