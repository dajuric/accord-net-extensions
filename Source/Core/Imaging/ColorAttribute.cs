using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Extensions
{
    /// <summary>
    /// Color info attribute. 
    /// Contains informations about conversion name and specifies whether the structure is generic color space or not.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct)]
    public class ColorInfoAttribute : Attribute
    {
        /// <summary>
        /// Gets the default conversion name.
        /// </summary>
        public const string DEFAULT_CONVERSION_CODENAME = null;

        /// <summary>
        /// Creates the new instance of an <see cref="ColorInfoAttribute"/>.
        /// </summary>
        public ColorInfoAttribute()
        {
            this.ConversionCodename = DEFAULT_CONVERSION_CODENAME;
            this.IsGenericColorSpace = false;
        }

        /// <summary>
        /// Gets or sets conversion codename. Not used. (May be used for EmguCV compatibility)
        /// </summary>
        public string ConversionCodename { get; set; }
        /// <summary>
        /// Gets or sets whether the color space is generic (does not require data conversion).
        /// </summary>
        public bool IsGenericColorSpace { get; set; }
    }
}
