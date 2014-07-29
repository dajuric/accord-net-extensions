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
        Image<Bgr, byte> debugImage = null;
        RectangleGroupMatching groupMatching = null;

        public FormGroupMatch()
        {
            InitializeComponent();
            debugImage = new Image<Bgr, byte>(pictureBox.Size.ToSize());

            var detections = getDetections();
            drawDetections(detections, Bgr8.Red, 3);

            groupMatching = new RectangleGroupMatching(minimumNeighbors: 1, threshold: 0.3);
            var clusters = groupMatching.Group(detections);

            drawDetections(clusters.Select(x => x.Representative), Bgr8.Green, 1);
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

        private void drawDetections(IEnumerable<Rectangle> detections, Bgr color, int thickness)
        {
            foreach (var detection in detections)
            {
                debugImage.Draw(detection, color, thickness);
            }
        }
    }
}
