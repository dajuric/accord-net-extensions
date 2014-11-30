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

using System.Runtime.CompilerServices;

namespace Accord.Extensions.Imaging.Converters
{
    internal static class BgraBgrConverters 
    {
        #region Bgra -> Bgr (Generic)

        public unsafe static void ConvertBgraToBgr(IImage src, IImage dest)
        {
            var channels = ChannelSplitter.SplitChannels(src, new int[]{Bgr.IdxB, Bgr.IdxG, Bgr.IdxR });
            ChannelMerger.MergeChannels(channels, dest);
        }

        #endregion

        #region Bgr -> Bgra 

        public unsafe static void ConvertBgrToBgra_Byte(IImage src, IImage dest)
        {
            var channels = ChannelSplitter.SplitChannels(src, new int[] { Bgr.IdxB, Bgr.IdxG, Bgr.IdxR });

            var alphaChannel = Image.Create(ColorInfo.GetInfo(typeof(Gray), src.ColorInfo.ChannelType), src.Width, src.Height);
            (alphaChannel as Image<Gray, byte>).SetValue(byte.MaxValue);

            ChannelMerger.MergeChannels(new IImage[] { channels[Bgr.IdxB], channels[Bgr.IdxG], channels[Bgr.IdxR], alphaChannel }, dest);
        }

        //TODO: extend to all primitive types
        #endregion

    }
}
