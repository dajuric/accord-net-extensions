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

using Accord.Imaging.Filters;

namespace Accord.Extensions.Imaging.Filters
{
    /// <summary>
    /// Contains Gabor filter extensions.
    /// </summary>
    public static class GaborFilterExtensions
    {
        /// <summary>
        /// <para>(Accord .NET internal call)</para>
        /// In image processing, a Gabor filter, named after Dennis Gabor, is a linear 
        /// filter used for edge detection. Frequency and orientation representations 
        /// of Gabor filters are similar to those of the human visual system, and they
        /// have been found to be particularly appropriate for texture representation 
        /// and discrimination. In the spatial domain, a 2D Gabor filter is a Gaussian
        /// kernel function modulated by a sinusoidal plane wave. The Gabor filters are
        /// self-similar: all filters can be generated from one mother wavelet by dilation
        /// and rotation.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="size">The size of the filter</param>
        /// <param name="sigma">The Gaussian variance for the filter.</param>
        /// <param name="theta">The orientation for the filter, in radians.</param>
        /// <param name="lambda">The wavelength for the filter.</param>
        /// <param name="gamma">The aspect ratio for the filter.</param>
        /// <param name="psi">The phase offset for the filter.</param>
        /// <returns>Filtered image.</returns>
        public static Gray<byte>[,] GaborFilter(this Gray<byte>[,] img, int size = 3, double sigma = 2, double theta = 0.6, double lambda = 4.0, double gamma = 0.3, double psi = 1.0)
        { 
            GaborFilter gf = new GaborFilter
            {
                Size = size,
                Sigma = sigma,
                Theta = theta,
                Lambda = lambda,
                Gamma = gamma,
                Psi = psi
            };
            
            return GaborFilter(img, gf);
        }

        /// <summary>
        /// <para>(Accord .NET internal call)</para>
        /// In image processing, a Gabor filter, named after Dennis Gabor, is a linear 
        /// filter used for edge detection. Frequency and orientation representations 
        /// of Gabor filters are similar to those of the human visual system, and they
        /// have been found to be particularly appropriate for texture representation 
        /// and discrimination. In the spatial domain, a 2D Gabor filter is a Gaussian
        /// kernel function modulated by a sinusoidal plane wave. The Gabor filters are
        /// self-similar: all filters can be generated from one mother wavelet by dilation
        /// and rotation.
        /// </summary>
        /// <param name="img">Image.</param>
        /// <param name="gaborFilter">Gabor filter instance. 
        /// <para>To avoid calculating Gabor every time use this function overload that receives instance.</para>
        /// </param>
        /// <returns>Filtered image.</returns>
        internal static Gray<byte>[,] GaborFilter(this Gray<byte>[,] img, GaborFilter gaborFilter)
        {
            return img.ApplyFilter(gaborFilter);
        }
    }
}
