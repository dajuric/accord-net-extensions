using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Core
{
    /// <summary>
    /// Color info attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct)]
    public class ColorInfoAttribute : Attribute
    {
        public const string DEFAULT_CONVERSION_CODENAME = null;

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
