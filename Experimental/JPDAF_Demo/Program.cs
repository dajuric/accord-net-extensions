using System;
using System.IO;
using System.Windows.Forms;
using Accord.Extensions.Math.Geometry;
using Accord.Extensions.Imaging;
using System.Text.RegularExpressions;
using System.Linq;
using Database = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Accord.Extensions.Imaging.Annotation>>;
using Accord.Extensions;

namespace JPDAF_Demo
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new JPDAF_PF_DemoForm());
            //Application.Run(new JPDAF_Kalman_DemoForm());
            Application.Run(new VehicleTracking());

            return;

            string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
            var databaseFileName = Path.Combine(resourceDir, @"S:\Svjetla - baza podataka - Boris\Prednja + stražnja\1.xml");

            var db = new Database();
            db.Load(Path.Combine(resourceDir, databaseFileName));

            var carAnnotations = db.GetAnnotations();
            var groupedAnnotations = carAnnotations.GroupBy(x => x.Value.Label);

            foreach (var groupedAnn in groupedAnnotations)
            {
                var trajectory = groupedAnn.Select(x =>
                {
                    var box = x.Value.Polygon.BoundingRect();

                    var match = Regex.Match(x.Key, "[0-9]+");
                    var idx = Int32.Parse(match.Value);

                    return String.Format("{0}, {1}, {2}, {3}, {4}", box.X, box.Y, box.Width, box.Height, idx);
                });

                File.WriteAllLines(String.Format("{0}.csv", groupedAnn.Key), trajectory);
            }


            /*********************************************** get trajectories ******************************************/
            /*string resourceDir = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Resources");
            var databaseFileName = Path.Combine(resourceDir, "trajectory.xml");

            var db = new Database();  
            db.Load(Path.Combine(resourceDir, databaseFileName));

            var carAnnotations = db.GetAnnotations();
            var groupedAnnotations = carAnnotations.GroupBy(x => x.Value.Label);

            foreach (var groupedAnn in groupedAnnotations)
            {
                var trajectory = groupedAnn.Select(x => 
                {
                    var box = x.Value.Polygon.BoundingRect();
                    var center = ((RectangleF)box).Center();

                    var match = Regex.Match(x.Key, "[0-9]+");
                    var idx = Int32.Parse(match.Value);

                    return new TrajectoryData {  X = center.X, Y = center.Y, Step = idx };
                });

                TrajectoryData.Save(trajectory, String.Format("{0}.csv", groupedAnn.Key));
            }*/
        }
    }
}
