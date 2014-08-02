#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
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

namespace Accord.Extensions.Statistics.Filters
{
    /// <summary>
    /// Particle interface defining common members for all particle instances.
    /// </summary>
    public interface IParticle : ICloneable
    {
        /// <summary>
        /// Particle's weight.
        /// </summary>
        double Weight { get; set; }

        /// <summary>
        /// Applies model transition without noise to a particle's state.
        /// </summary>
        void Drift();

        /// <summary>
        /// Applies noise to a particle's state.
        /// </summary>
        void Difuse();
    }

    /// <summary>
    /// Particle interface defining common members for all particle instances.
    /// </summary>
    /// <typeparam name="TState">State type.</typeparam>
    public interface IParticle<TState> : IParticle
    {
        /// <summary>
        /// Gets or sets the particle's state.
        /// </summary>
        TState State { get; set; }
    }
}
