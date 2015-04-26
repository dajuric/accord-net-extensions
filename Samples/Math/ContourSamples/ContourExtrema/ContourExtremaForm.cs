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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;

namespace ContourExtremaDemo
{
    public partial class ContourExtremaForm : Form
    {
        Bgr<byte>[,] image;

        public ContourExtremaForm()
        {
            InitializeComponent();
        }

        private void ContourDemoForm_Shown(object sender, EventArgs e)
        {
            image = (ResourceImages.bwHand.ToArray() as Gray<byte>[,]).ToBgr(); 
            findPeaksAndValleys();
        }

        /// <summary>
        /// Finds peaks and valleys. 
        /// The EmguCV's GetConvexHull function will find the global hull meaning if the smaller object is merged with other objects some peaks will not be detected.
        /// </summary>
        private void findPeaksAndValleys()
        {
            var scale = 25; //for bigger humps increase scale

            var contour = image.ToGray().FindContour();

            //find peaks and valeys
            List<int> peakIndeces, valeyIndeces;
            contour.FindExtremaIndices((angle, isPeak) =>
            {
                if ((isPeak && angle > 0 && angle < 90) ||
                    (!isPeak && angle > 0 && angle < 90))
                    return true;

                return false;
            },
            scale, out peakIndeces, out valeyIndeces);

            //cluster peak and valeys
            var cumulativeDistance = contour.CumulativeEuclideanDistance();
            var peakClusters = contour.ClusterPoints(peakIndeces, scale * 2, cumulativeDistance).Where(x => x.Count > 3);
            var valeyClusters = contour.ClusterPoints(valeyIndeces, scale * 2, cumulativeDistance).Where(x => x.Count > 3);

            //get humps
            List<int> humpPeaks;
            var humps = contour.GetHumps(peakClusters.Select(x => ((int)x.Average() + contour.Count) % contour.Count).ToList() /*get mean value; if it is a negative index make it positive*/,
                                         valeyClusters.Select(x => ((int)x.Average() + contour.Count) % contour.Count).ToList(),
                                         scale, out humpPeaks);


            /******************** DRAWING *************************/

            //draw humps
            for (int i = 0; i < humps.Count; i++)
            {
                var slice = contour.ToCircularList().GetRange(humps[i]);
                image.Draw(slice.ToArray(), Bgr<byte>.Green, 5);
            }

            //draw valeys
            for (int i = 0; i < valeyIndeces.Count; i++)
            {
                image.Draw(new Circle(contour[valeyIndeces[i]], 3), Bgr<byte>.Red, 3);
            }

            //draw peaks
            for (int i = 0; i < peakIndeces.Count; i++)
            {
                image.Draw(new Circle(contour[peakIndeces[i]], 3), Bgr<byte>.Blue, 3);
            }

            //draw contour
            //image.Draw(contour.Select(x => new System.Drawing.Point(x.X, x.Y)), new Bgr(Color.Red), 3);
            pictureBox.Image = image.ToBitmap();
        }
    }
}
