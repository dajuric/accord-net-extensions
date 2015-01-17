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

namespace Accord.Extensions.Imaging.Moments
{
    /// <summary>
    ///   Base class for image moments.
    /// </summary>
    /// 
    public abstract class MomentsBase: IMoments
    {
        /// <summary>
        ///   Gets or sets the maximum order of the moments.
        /// </summary>
        /// 
        public int Order { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="MomentsBase"/> class.
        /// </summary>
        /// 
        /// <param name="order">The maximum order for the moments.</param>
        ///
        protected MomentsBase(int order)
        {
            Order = order;
        }

        /// <summary>
        /// Computes moments for the provided image.
        /// </summary>
        /// <param name="image">Image.</param>
        public void Compute(Image<Gray, byte> image)
        {
            Compute((IImage)image);
        }

        /// <summary>
        /// Computes moments for the provided image.
        /// </summary>
        /// <param name="image">Image.</param>
        public void Compute(Image<Gray, float> image)
        {
            Compute((IImage)image);
        }

        internal abstract void Compute(IImage image);
    }
}
