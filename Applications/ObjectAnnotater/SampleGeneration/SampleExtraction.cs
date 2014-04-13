using Accord.Extensions;
using Accord.Extensions.Imaging;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Vision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;
using Point = AForge.IntPoint;

namespace ObjectAnnotater.SampleGeneration
{
    public partial class SampleExtraction : Form
    {
        public SampleExtraction()
        {
            InitializeComponent();
        }

        private static void extractPositives(Database database, string destinationDirPath, Regex labelMatch, bool uselabelAsFilename)
        { 
            //treba voditi računa ponavljaju li se neke labele
        }
    }
}
