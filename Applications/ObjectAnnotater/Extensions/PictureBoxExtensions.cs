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

using Accord.Extensions.Imaging;
using System;
using System.Drawing;
using System.Windows.Forms;
using Point = AForge.IntPoint;
using PointF = AForge.Point;
using Rectangle = Accord.Extensions.Rectangle;
using RectangleF = Accord.Extensions.RectangleF;

namespace ObjectAnnotater
{
    public static class PictureBoxExtensions
    {
        //inspired by: http://www.codeproject.com/Articles/20923/Mouse-Position-over-Image-in-a-PictureBox
        public static PointF ToImageCoordinate(this PictureBox pictureBox, PointF pictureBoxCoordinate)
        {
            if (pictureBox.Image == null)
                throw new Exception("Image property is null!");

            if (pictureBox.SizeMode != PictureBoxSizeMode.Zoom)
                throw new NotSupportedException("Only zoom mode is supported!");

            Size imageSize = pictureBox.Image.Size;

            var scale = Math.Min((float)pictureBox.Width / imageSize.Width, (float)pictureBox.Height / imageSize.Height);
            var offsetX = (float)(pictureBox.Width - imageSize.Width * scale) / 2;
            var offsetY = (float)(pictureBox.Height - imageSize.Height * scale) / 2;

            var imgPt = new PointF
            {
                X = (pictureBoxCoordinate.X - offsetX) / scale,
                Y = (pictureBoxCoordinate.Y - offsetY) / scale
            };

            return imgPt;
        }

        public static PointF ToPictureBoxCoordinate(this PictureBox pictureBox, PointF imageCoordinate)
        {
            if (pictureBox.Image == null)
                throw new Exception("Image property is null!");

            if (pictureBox.SizeMode != PictureBoxSizeMode.Zoom)
                throw new NotSupportedException("Only zoom mode is supported!");

            Size imageSize = pictureBox.Image.Size;

            var scale = Math.Min((float)pictureBox.Width / imageSize.Width, (float)pictureBox.Height / imageSize.Height);
            var offsetX = (float)(pictureBox.Width - imageSize.Width * scale) / 2;
            var offsetY = (float)(pictureBox.Height - imageSize.Height * scale) / 2;

            var pbPt = new PointF
            {
                X = (imageCoordinate.X * scale) + offsetX,
                Y = (imageCoordinate.Y * scale) + offsetY
            };

            return pbPt;
        }

        public static Rectangle ToPictureBoxCoordinate(this PictureBox pictureBox, Rectangle imageRectangle)
        {
            var upperLeftTransformed = pictureBox.ToPictureBoxCoordinate(imageRectangle.Location);
            var bottomRightTransformed = pictureBox.ToPictureBoxCoordinate(new Point(imageRectangle.Right, imageRectangle.Bottom));

            var transformedRect = new RectangleF(upperLeftTransformed.X, upperLeftTransformed.Y, bottomRightTransformed.X - upperLeftTransformed.X, bottomRightTransformed.Y - upperLeftTransformed.Y);
            return Rectangle.Round(transformedRect);
        }
    }
}
