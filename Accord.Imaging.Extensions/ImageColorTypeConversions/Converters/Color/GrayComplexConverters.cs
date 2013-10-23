using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging.Converters
{
    static class GrayComplexConverters
    {
        public static void ConvertGrayToComplex(IImage src, IImage dest)
        {
            ChannelMerger.MergeChannels(new IImage[] { src }, dest, 0); //real(dest)
        }

    }
}
