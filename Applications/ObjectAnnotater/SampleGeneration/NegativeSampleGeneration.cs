using System;
using System.IO;
using System.Windows.Forms;
using Accord.Extensions;
using Accord.Extensions.Vision;
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
