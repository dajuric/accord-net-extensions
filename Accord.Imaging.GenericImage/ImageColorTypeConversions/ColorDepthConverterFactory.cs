using Accord.Math;
using System;

namespace Accord.Imaging.Converters
{
    /// <summary>
    /// Registers color and depth converters.
    /// </summary>
    public static class ColorDepthConverterFactory
    {
        static Type[] SupportedPrimitiveTypes = new Type[] { typeof(byte), typeof(short), typeof(int), typeof(float), typeof(double) };

        public static void Initialize()
        {
            registerFromByteDepthConverters();
            registerFromShortDepthConverters();
            registerFromIntDepthConverters();
            registerFromFloatDepthConverters();
            registerFromDoubleDepthConverters();

            #region Bgr <-> Hsv

            ColorConverter.Register(new ColorConverter.ConversionData 
            {
                SourceColorInfo = ColorInfo.GetInfo<Bgr, byte>(),
                DestColorInfo = ColorInfo.GetInfo<Hsv, byte>(),
                DataConvertFunc = BgrHsvConverters.ConvertBgrToHsv_Byte
            });

            ColorConverter.Register(new ColorConverter.ConversionData
            {
                SourceColorInfo = ColorInfo.GetInfo<Hsv, byte>(),
                DestColorInfo = ColorInfo.GetInfo<Bgr, byte>(),
                DataConvertFunc = BgrHsvConverters.ConvertHsvToBgr_Byte
            });

            #endregion

            #region Gray -> Complex

            ColorConverter.Register(new ColorConverter.ConversionData
            {
                SourceColorInfo = ColorInfo.GetInfo<Gray, float>(),
                DestColorInfo = ColorInfo.GetInfo<Complex, float>(),
                DataConvertFunc = GrayComplexConverters.ConvertGrayToComplex
            });

            ColorConverter.Register(new ColorConverter.ConversionData
            {
                SourceColorInfo = ColorInfo.GetInfo<Gray, double>(),
                DestColorInfo = ColorInfo.GetInfo<Complex, double>(),
                DataConvertFunc = GrayComplexConverters.ConvertGrayToComplex,
                ForceSequential = true
            });

            //QUESTION: does it make sense for other depths ?
            #endregion

            #region Gray <-> Bgr

            foreach (var spt in SupportedPrimitiveTypes)
            {
                ColorConverter.Register(new ColorConverter.ConversionData
                {
                    SourceColorInfo = ColorInfo.GetInfo(typeof(Gray), spt),
                    DestColorInfo = ColorInfo.GetInfo(typeof(Bgr), spt),
                    DataConvertFunc = BgrGrayConverters.ConvertGrayToBgr,
                    ForceSequential = true
                });
            }

            ColorConverter.Register(new ColorConverter.ConversionData
            {
                SourceColorInfo = ColorInfo.GetInfo<Bgr, byte>(),
                DestColorInfo = ColorInfo.GetInfo<Gray, byte>(),
                DataConvertFunc = BgrGrayConverters.ConvertBgrToGray_Byte
            });

            #endregion
        }

        private static void registerFromByteDepthConverters()
        {
            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(byte),
                DestType = typeof(short),
                DataConvertFunc = FromByteDepthConverters.ConvertByteToShort
            });

            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(byte),
                DestType = typeof(int),
                DataConvertFunc = FromByteDepthConverters.ConvertByteToInt
            });

            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(byte),
                DestType = typeof(float),
                DataConvertFunc = FromByteDepthConverters.ConvertByteToFloat
            });

            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(byte),
                DestType = typeof(double),
                DataConvertFunc = FromByteDepthConverters.ConvertByteToDouble
            });
        }

        private static void registerFromShortDepthConverters()
        {
            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(short),
                DestType = typeof(byte),
                DataConvertFunc = FromShortDepthConverters.ConvertShortToByte
            });

            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(short),
                DestType = typeof(int),
                DataConvertFunc = FromShortDepthConverters.ConvertShortToInt
            });

            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(short),
                DestType = typeof(float),
                DataConvertFunc = FromShortDepthConverters.ConvertShortToFloat
            });

            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(short),
                DestType = typeof(double),
                DataConvertFunc = FromShortDepthConverters.ConvertShortToDouble
            });
        }

        private static void registerFromIntDepthConverters()
        {
            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(int),
                DestType = typeof(byte),
                DataConvertFunc = FromIntDepthConverters.ConvertIntToByte
            });

            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(int),
                DestType = typeof(short),
                DataConvertFunc = FromIntDepthConverters.ConvertIntToShort
            });

            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(int),
                DestType = typeof(float),
                DataConvertFunc = FromIntDepthConverters.ConvertIntToFloat
            });

            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(int),
                DestType = typeof(double),
                DataConvertFunc = FromIntDepthConverters.ConvertIntToDouble
            });
        }

        private static void registerFromFloatDepthConverters()
        {
            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(float),
                DestType = typeof(byte),
                DataConvertFunc = FromFloatDepthConverters.ConvertFloatToByte
            });

            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(float),
                DestType = typeof(short),
                DataConvertFunc = FromFloatDepthConverters.ConvertFloatToShort
            });

            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(float),
                DestType = typeof(int),
                DataConvertFunc = FromFloatDepthConverters.ConvertFloatToInt
            });

            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(float),
                DestType = typeof(double),
                DataConvertFunc = FromFloatDepthConverters.ConvertFloatToDouble
            });
        }

        private static void registerFromDoubleDepthConverters()
        {
            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(double),
                DestType = typeof(byte),
                DataConvertFunc = FromDoubleDepthConverters.ConvertDoubleToByte
            });

            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(double),
                DestType = typeof(short),
                DataConvertFunc = FromDoubleDepthConverters.ConvertDoubleToShort
            });

            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(double),
                DestType = typeof(int),
                DataConvertFunc = FromDoubleDepthConverters.ConvertDoubleToInt
            });

            DepthConverter.Register(new DepthConverter.DepthConversionInfo
            {
                SourceType = typeof(double),
                DestType = typeof(float),
                DataConvertFunc = FromDoubleDepthConverters.ConvertDoubleToFloat
            });
        }

    }
}
