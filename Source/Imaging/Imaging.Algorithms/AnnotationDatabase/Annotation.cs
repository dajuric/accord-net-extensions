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
using System.Xml.Serialization;
using Point = AForge.IntPoint;

namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Object annotation.
    /// <para>Used during training process. See ObjectAnnotater application in samples.</para>
    /// </summary>
    [Serializable]
    public class Annotation: ICloneable
    {
        /// <summary>
        /// Creates new empty annotation.
        /// </summary>
        public Annotation()
        {
            this.Label = String.Empty;
            this.Polygon = new Point[0];
            this.Tag = null;
        }

        /// <summary>
        /// Gets or sets the annotation label
        /// </summary>
        [XmlAttribute]
        public string Label { get; set; }
        /// <summary>
        /// Gets or sets the object contour.
        /// <para>See rectangle to vertices extension to transform rectangle into set of points.</para>
        /// </summary>
        public Point[] Polygon { get; set; }
        /// <summary>
        /// Additional annotation data.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Clones annotation. Tag is cloned only is implements <see cref="System.ICloneable"/> interface.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Annotation 
            {
                Label = this.Label,
                Polygon = (Point[])this.Polygon.Clone(),
                Tag = this.Tag is ICloneable ? ((ICloneable)this.Tag).Clone() : this.Tag
            };
        }
    }
}
