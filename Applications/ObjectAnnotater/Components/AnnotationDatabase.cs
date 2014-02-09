using Accord.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnnotationData = System.Collections.Generic.KeyValuePair<string, Accord.Extensions.Rectangle[]>;

namespace ObjectAnnotater
{
    public class AnnotationDatabase
    {
        List<AnnotationData> data;
        bool useRelativePath;

        private AnnotationDatabase() 
        {}

        public string FileName { get; private set; }

        public static AnnotationDatabase LoadOrCreate(string fileName, bool useRelativePath = true)
        {
            fileName = normalizePathDeliminators(fileName);

            var database = new AnnotationDatabase();
            database.useRelativePath = useRelativePath;
            database.FileName = fileName;

            if (File.Exists(fileName))
            {
                database.data = load(fileName);
            }
            else
            {
                using (File.Create(fileName)) { }
                database.data = new List<AnnotationData>();
            }

            return database;
        }

        public void AddOrUpdate(string imageName, IEnumerable<Rectangle> annotations)
        {
            imageName = getImageName(imageName);

            int index;
            find(imageName, out index);

            var newRecord = new AnnotationData(imageName, annotations.ToArray());

            if (index >= 0)
                data[index] = newRecord;
            else
                data.Add(newRecord);
        }


        public IEnumerable<Rectangle> Find(string imageName)
        {
            imageName = getImageName(imageName);

            int index;
            return find(imageName, out index);
        }

        private IEnumerable<Rectangle> find(string imageName, out int index)
        {
            index = 0;
            foreach (var pair in data)
            {
                if (pair.Key == imageName)
                    return pair.Value;

                index++;
            }

            index = -1;
            return new List<Rectangle>();
        }

        public bool Remove(string imageName)
        {
            imageName = getImageName(imageName);

            int index;
            find(imageName, out index);

            if (index >= 0)
            {
                data.RemoveAt(index);
                return true;
            }

            return false;
        }

        public void Commit()
        {
            const int COLUMN_SPACING = 3;
            const int RECT_ELEM_SPACING = 1;

            bool[] isCommaSeparatedColumn;
            string[][] lines = serializeToTable(data, out isCommaSeparatedColumn);
            int[] columnLengths = getMaxColumnLengths(lines, isCommaSeparatedColumn);

            using (var writer = new StreamWriter(FileName, false))
            {
                foreach (var line in lines)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        var allignedStr = line[i].PadLeft(columnLengths[i]);
                        writer.Write(allignedStr);

                        if (isCommaSeparatedColumn[i] && i < (line.Length - 1) /*not he last*/)
                            writer.Write(',' + new string(' ', COLUMN_SPACING));
                        else
                            writer.Write(new string(' ', RECT_ELEM_SPACING));
                    }

                    writer.WriteLine();
                }
            }
        }

        private static List<AnnotationData> load(string fileName)
        {
            HashSet<string> keys = new HashSet<string>(); 
            List<AnnotationData> data = new List<AnnotationData>();

            using (var reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    var parts = reader.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    var record = new AnnotationData(
                                                   parts.First().Trim(),
                                                   parts.Skip(1).Select(x => rectFromString(x)).ToArray()
                                                   );

                    if (keys.Contains(record.Key))
                        throw new Exception(String.Format("Image: {0} is duplicated! Annotation load failed!", record.Key));
                    else
                        keys.Add(record.Key);

                    data.Add(record);
                }
            }

            return data;
        }

        private static Rectangle rectFromString(string str)
        {
            var parts = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            return new Rectangle 
            {
                X = Int32.Parse(parts[0]),
                Y = Int32.Parse(parts[1]),
                Width = Int32.Parse(parts[2]),
                Height = Int32.Parse(parts[3])
            };
        }

        private static int[] getMaxColumnLengths(string[][] lines, bool[] isCommaSeparatedColumn)
        {
            var maxLenghts = new int[lines.Select(x => x.Length).Max()];

            foreach (var line in lines)
            {
                int idx = 0;
                foreach (var col in line)
                {
                    maxLenghts[idx] = Math.Max(maxLenghts[idx], col.Length);

                    idx++;
                }
            }

            return maxLenghts;
        }

        private static string[][] serializeToTable(IEnumerable<AnnotationData> records, out bool[] isCommaSeparatedColumn)
        {
            string[][] recordsParts = new string[records.Count()][];
            isCommaSeparatedColumn = new bool[0];

            int idx = 0;
            foreach (var record in records)
            {
                bool[] recordSeparations;
                recordsParts[idx] = serializeAnnotationData(record, out recordSeparations);

                if (recordSeparations.Length > isCommaSeparatedColumn.Length)
                    isCommaSeparatedColumn = recordSeparations;

                idx++;
            }

            return recordsParts;
        }

        private static string[] serializeAnnotationData(AnnotationData record, out bool[] isCommaSeparatedColumn)
        {
            List<string> parts = new List<string>();
            List<bool> isCommaSepCol = new List<bool>();

            parts.Add(record.Key);
            isCommaSepCol.Add(true);

            int colIdx = 1;
            foreach (var ann in record.Value)
            {
                foreach (var annPart in serializeAnnotation(ann))
                {
                    parts.Add(annPart);
                    isCommaSepCol.Add(false);

                    colIdx++;
                }

                isCommaSepCol[colIdx - 1] = true;
            }

            isCommaSeparatedColumn = isCommaSepCol.ToArray();
            return parts.ToArray();
        }

        private static string[] serializeAnnotation(Rectangle rect)
        {
            return new string[] 
            {
                rect.X.ToString(), rect.Y.ToString(), rect.Width.ToString(), rect.Height.ToString()
            };
        }

        private string getImageName(string imageName)
        {
            if (!useRelativePath)
                return imageName;

            imageName = normalizePathDeliminators(imageName);

            bool isEqual = true;
            int lastEqualIdx = -1;

            while (isEqual && Math.Min(FileName.Length, imageName.Length) > (lastEqualIdx + 1))
            {
                if (FileName[lastEqualIdx + 1] == imageName[lastEqualIdx + 1])
                    lastEqualIdx++;
                else
                    isEqual = false;
            }

            if (lastEqualIdx == -1)
                throw new Exception("Cannot find relative path of an image and the database!");

            return imageName.Substring(lastEqualIdx + 1);
        }

        private static string normalizePathDeliminators(string path)
        {
            return path.Replace("//", "/").Replace(@"\", "/").Replace(@"\\", "/");
        }
    }

}
