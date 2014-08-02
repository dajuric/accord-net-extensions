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

using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Accord.Extensions
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides serialization extensions. 
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// Serializes specified object to <see cref="XElement"/>.
        /// </summary>
        /// <typeparam name="T">An object generic type.</typeparam>
        /// <param name="obj">An input object.</param>
        /// <param name="writeEmptyNamespace">Writes empty name-space attribute instead of standard w3.org name-space.</param>
        /// <returns>Serialized object.</returns>
        public static XElement ToXElement<T>(this T obj, bool writeEmptyNamespace = false)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    var ns = new XmlSerializerNamespaces(); 
                    
                    if(writeEmptyNamespace)
                        ns.Add("", "");

                    xmlSerializer.Serialize(streamWriter, obj, ns);
                    return XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));
                }
            }
        }

        /// <summary>
        /// Deserializes the specified <see cref="XElement"/> to an object.
        /// </summary>
        /// <typeparam name="T">Destination generic object type.</typeparam>
        /// <param name="xElement">An element to deserialize.</param>
        /// <returns>Deserialized object.</returns>
        public static T FromXElement<T>(this XElement xElement)
        {
            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(xElement.ToString())))
            { 
                var xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(memoryStream);
            }
        }
    }
}
