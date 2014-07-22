namespace Accord.Extensions.Imaging.Converters
{
    static class GrayComplexConverters
    {
        public static void ConvertGrayToComplex(IImage src, IImage dest)
        {
            ChannelMerger.MergeChannels(new IImage[] { src }, dest, 0); //real(dest)
        }

    }
}
