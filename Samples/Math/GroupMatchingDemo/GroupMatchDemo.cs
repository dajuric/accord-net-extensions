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

using Accord.Extensions;
using Accord.Extensions.Imaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using Accord.Extensions.Math.Geometry;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GroupMatchingDemo
{
    public partial class FormGroupMatch : Form
    {
        Bgr<byte>[,] debugImage = null;
        RectangleClustering groupMatching = null;

        public FormGroupMatch()
        {
            InitializeComponent();
            debugImage = new Bgr<byte>[pictureBox.Height, pictureBox.Width]; 

            var detections = getDetections();
            drawDetections(detections, Bgr<byte>.Red, 1);

            groupMatching = new RectangleClustering();
            var clusters = groupMatching.Group(detections);

            drawDetections(clusters.Select(x => x.Representative), Bgr<byte>.Green, 3);
            pictureBox.Image = debugImage.ToBitmap();
        }

        private Rectangle[] getDetections()
        {
            return new Rectangle[] 
            {
                //cluster 1
                new Rectangle(50, 50, 100, 100),
                new Rectangle(55, 45, 90, 90),
                new Rectangle(60, 45, 95, 95),

                //cluster 2
                new Rectangle(300, 50, 100, 100),

                //cluster 3
                new Rectangle(250, 150, 70, 70),
                new Rectangle(240, 160, 80, 80)
            };
        }

        private void drawDetections(IEnumerable<Rectangle> detections, Bgr<byte> color, int thickness)
        {
            foreach (var detection in detections)
            {
                debugImage.Draw(detection, color, thickness);
            }
        }
    }
}
