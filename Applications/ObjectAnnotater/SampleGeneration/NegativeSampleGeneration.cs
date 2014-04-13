using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Vision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;
using Point = AForge.IntPoint;

namespace ObjectAnnotater.SampleGeneration
{
    public partial class NegativeSampleGeneration : Form
    {
        ImageDirectoryReader capture;
        Database database;

        public NegativeSampleGeneration(ImageDirectoryReader capture, Database database)
        {
            InitializeComponent();
        }

        private static void generateNegatives(ImageDirectoryReader capture, Database database, int nNegativesPerImage, Size minSize, bool preserveScale)
        {
            Random rand = new Random();

            capture.Seek(0, SeekOrigin.Begin);

            while (capture.Position < capture.Length)
            { 
                //generate random window
                   //check size
                   //check if the rectangle intersects with forbidded rects

            }

        }

    }
}
