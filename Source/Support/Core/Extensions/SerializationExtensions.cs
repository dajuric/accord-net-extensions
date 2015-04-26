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

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
        #region XElement

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
                    var bytes = memoryStream.ToArray();
                    return XElement.Parse(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
                }
            }
        }

        /// <summary>
        /// De-serializes the specified <see cref="XElement"/> to an object.
        /// </summary>
        /// <typeparam name="T">Destination generic object type.</typeparam>
        /// <param name="xElement">An element to deserialize.</param>
        /// <returns>De-serialized object.</returns>
        public static T FromXElement<T>(this XElement xElement)
        {
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xElement.ToString())))
            { 
                var xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(memoryStream);
            }
        }

        #endregion

        #region Binary formatter

        /// <summary>
        /// Serializes specified object to memory stream by using binary formatter.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="obj">Object to serialize.</param>
        /// <returns>Memory stream containing serialized object.</returns>
        public static MemoryStream ToBinary<T>(this T obj)
        {
            MemoryStream memoryStream = new MemoryStream();
            obj.ToBinary(memoryStream);
            return memoryStream;
        }

        /// <summary>
        /// Serializes specified object to memory stream by using binary formatter.
        /// <para>If the file exists it will be overwritten.</para>
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="fileName">The name of the file to save serialized object.</param>
        public static void ToBinary<T>(this T obj, string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                obj.ToBinary(fileStream);
                fileStream.Flush();
            }
        }

        /// <summary>
        /// Serializes specified object to memory stream by using binary formatter.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="stream">The existing stream to serialize to.</param>
        public static void ToBinary<T>(this T obj, Stream stream)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, obj);
        }

        /// <summary>
        /// De-serializes the object from the specified stream.
        /// <para>When de-serializing multiple objects the position within stream must not be tampered by the user.</para>
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="stream">The stream which contains object data.</param>
        /// <returns>De-serialized object.</returns>
        public static T FromBinary<T>(this Stream stream)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            var obj = (T)binaryFormatter.Deserialize(stream);
            return obj;
        }

        #endregion
    }
}
