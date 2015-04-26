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

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Termination criteria.
    /// </summary>
    public class TermCriteria
    {
        /// <summary>
        /// Gets or sets max number of iterations.
        /// </summary>
        public int MaxIterations { get; set; }

        /// <summary>
        /// Gets or sets minimal error.
        /// </summary>
        public double MinError { get; set; }

        /// <summary>
        /// Initializes structure.
        /// </summary>
        public TermCriteria()
        {
            this.MaxIterations = 1;
            this.MinError = 0;
        }

        /// <summary>
        /// Returns whether an procedure should terminate or not.
        /// </summary>
        /// <param name="numOfIterations">Number of executed iterations.</param>
        /// <param name="error">Current error.</param>
        /// <returns>True if an procedure should terminate.</returns>
        public bool ShouldTerminate(int numOfIterations, double error)
        {
            if (numOfIterations >= MaxIterations || error <= MinError)
                return true;
            else
                return false;
        }
    }
}
