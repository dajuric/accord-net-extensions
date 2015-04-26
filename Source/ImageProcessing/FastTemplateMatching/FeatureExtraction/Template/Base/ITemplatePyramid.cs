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


namespace Accord.Extensions.Imaging.Algorithms.LINE2D
{
    /// <summary>
    /// LINE2D template pyramid interface.
    /// </summary>
    public interface ITemplatePyramid 
    {
        /// <summary>
        /// Collection of templates. One template for each pyramid scale.
        /// </summary>
        ITemplate[] Templates { get; }
    }

    /// <summary>
    /// LINE2D template pyramid interface.
    /// </summary>
    public interface ITemplatePyramid<T> where T : ITemplate
    {
        /// <summary>
        /// Collection of templates. One template for each pyramid scale.
        /// </summary>
        T[] Templates { get; }

        /// <summary>
        /// Initializes template pyramid with the provided templates.
        /// </summary>
        /// <param name="templates">Collection of templates.</param>
        void Initialize(T[] templates);
    }
}
