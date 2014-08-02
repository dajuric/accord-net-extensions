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

namespace Accord.Extensions
{
    /// <summary>
    /// Default interface for color types.
    /// </summary>
    public interface IColor { }
    /// <summary>
    /// Interface for 2 channel color type. (Used for compile-time restrictions)
    /// </summary>
    public interface IColor2 : IColor { }
    /// <summary>
    /// Interface for 3 channel color type. (Used for compile-time restrictions)
    /// </summary>
    public interface IColor3 : IColor { }
    /// <summary>
    /// Interface for 4 channel color type. (Used for compile-time restrictions)
    /// </summary>
    public interface IColor4 : IColor { }
}
