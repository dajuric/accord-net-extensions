using Accord.Extensions;
using LINQtoCSV;
using MoreLinq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Range = AForge.IntRange;

namespace JPDAF_Demo
{
    public class TrajectoryData
    {
        [CsvColumn(FieldIndex = 1)]
        public float X { get; set; }

        [CsvColumn(FieldIndex = 2)]
        public float Y { get; set; }

        [CsvColumn(FieldIndex = 3)]
        public int Step { get; set; }

        public static void Normalize(List<TrajectoryData>[] trajectories, Rectangle area)
        {
            var minX = trajectories.Select(x => x.Select(t => t.X).Min()).Min();
            var maxX = trajectories.Select(x => x.Select(t => t.X).Max()).Max();

            var minY = trajectories.Select(x => x.Select(t => t.Y).Min()).Min();
            var maxY = trajectories.Select(x => x.Select(t => t.Y).Max()).Max();

            trajectories.ForEach(x => x.ForEach(t =>
            {
                var kX = 0f;
                if((maxX - minX) !=0)   
                    kX = (area.Right - area.X) / (maxX - minX);

                var kY = 0f;
                if((maxY - minY) != 0)
                    kY = (area.Bottom - area.Y) / (maxY - minY);

                t.X = kX * (t.X - minX) + area.X;
                t.Y = kY * (t.Y - minY) + area.Y;
            }));
        }

        public static Range Sync(List<TrajectoryData>[] trajectories)
        {
            var minStep = trajectories.Min(x => x.Min(t => t.Step));
            var maxStep = trajectories.Max(x => x.Max(t => t.Step));

            for (int tIdx = 0; tIdx < trajectories.Length; tIdx++)
            {
                var t = trajectories[tIdx];

                var syncT = new TrajectoryData[maxStep - minStep + 1];
                foreach (var record in t)
                {
                    syncT[record.Step - minStep] = record;
                }

                trajectories[tIdx] = syncT.ToList();
            }

            return new Range(minStep, maxStep);
        }

        public static List<TrajectoryData>[] Load(string directory)
        {
            string[] files = Directory.GetFiles(directory, "*.csv");

            CsvFileDescription inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ';',
                EnforceCsvColumnAttribute = true,
                FirstLineHasColumnNames = false
            };

            CsvContext cc = new CsvContext();

            var trajectories = new List<TrajectoryData>[files.Length];

            int i = 0;
            foreach (var fileName in files)
            {
                trajectories[i] = cc.Read<TrajectoryData>(fileName, inputFileDescription).ToList();
                i++;
            }
            
            return trajectories;
        }

        public static void Save(IEnumerable<TrajectoryData> data, string fileName)
        {
            CsvFileDescription inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ';',
                EnforceCsvColumnAttribute = true,
                FirstLineHasColumnNames = false
            };

            CsvContext cc = new CsvContext();
            cc.Write<TrajectoryData>(data, fileName, inputFileDescription);
        }
    }
}
