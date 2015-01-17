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
using System.Diagnostics;

namespace Accord.Extensions.Imaging.Algorithms.LINE2D
{
    /// <summary>
    /// Contains global parameters for LINE2D template matching.
    /// <para>Values that are defined work well in majority of situations (but can be changed if an user wants to - recompilation).</para>
    /// </summary>
    public static class GlobalParameters
    {
        /// <summary>
        /// Number of quantized orientations. Needed for feature extraction.
        /// <para>Maximum value is 8 (number of bits in byte). 3rd i 4th quadrant are mapped to 2nd and 1st</para>
        /// <para>Although this number can be lower than 8 precision can be lost, but the preprocessing stage can be speeded-up (lower number of response maps).</para>
        /// </summary>
        public const int NUM_OF_QUNATIZED_ORIENTATIONS = 8; 
        /// <summary>
        /// Max feature similarity. Default value is 4. Needed for feature (template) comparison.
        /// This constant can be interpreted as max angle similarity.
        /// </summary>
        public const int MAX_FEATURE_SIMILARITY = 4; 

        /// <summary>
        /// Max number of features per template. It can be used for template feature extraction.
        /// </summary>
        public const int MAX_NUM_OF_FEATURES = short.MaxValue / MAX_FEATURE_SIMILARITY;

        /// <summary>
        /// Max number of features that can be added to a buffer of type <see cref="System.Byte"/>.
        /// </summary>
        public const int MAX_SUPPORTED_NUM_OF_FEATURES_ADDDED_AS_BYTE = Byte.MaxValue / GlobalParameters.MAX_FEATURE_SIMILARITY;

        static GlobalParameters()
        {
            Debug.Assert(NUM_OF_QUNATIZED_ORIENTATIONS <= 8 /*(num of bits in byte)*/);
        }

    }
}
