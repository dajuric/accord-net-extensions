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
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using PointF = AForge.Point;

namespace Accord.Extensions.Imaging.Algorithms.LINE2D
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
}
