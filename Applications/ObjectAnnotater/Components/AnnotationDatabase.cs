using Accord.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq;

namespace ObjectAnnotater
{
    public class ImageAnnotation
    {
        public Rectangle ROI;
        public string Label;
    }

    public class AnnotationDatabase
    {
        List<KeyValuePair<string, ImageAnnotation[]>> data;

        private AnnotationDatabase() 
        {}

        public string FileName { get; private set; }

        public static AnnotationDatabase LoadOrCreate(string fileName)
        {
            fileName = fileName.NormalizePathDelimiters();

            var database = new AnnotationDatabase();
            database.FileName = fileName;

            if (File.Exists(fileName))
            {
                database.data = load(fileName);
            }
            else
            {
                using (File.Create(fileName)) { }
                database.data = new List<KeyValuePair<string, ImageAnnotation[]>>();
            }

            return database;
        }

        public void AddOrUpdate(string imageName, IEnumerable<ImageAnnotation> annotations)
        {
            imageName = getRelativePath(imageName);

            int index;
            find(imageName, out index);

            var newRecord = new KeyValuePair<string, ImageAnnotation[]>(imageName, annotations.ToArray());

            if (index >= 0)
                data[index] = newRecord;
            else
                data.Add(newRecord);
        }


        public IEnumerable<ImageAnnotation> Find(string imageName)
        {
            imageName = getRelativePath(imageName);

            int index;
            return find(imageName, out index);
        }

        public bool Contains(string imageName)
        {
            imageName = getRelativePath(imageName);

            int index;
            find(imageName, out index);

            return index >= 0;
        }

        private IEnumerable<ImageAnnotation> find(string imageName, out int index)
        {
            index = 0;
            foreach (var pair in data)
            {
                if (pair.Key == imageName)
                    return pair.Value;

                index++;
            }

            index = -1;
            return new List<ImageAnnotation>();
        }

        public bool Remove(string imageName)
        {
            imageName = getRelativePath(imageName);

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

        private static List<KeyValuePair<string, ImageAnnotation[]>> load(string fileName)
        {
            HashSet<string> keys = new HashSet<string>();
            var data = new List<KeyValuePair<string, ImageAnnotation[]>>();

            using (var reader = new StreamReader(fileName))
            {
                int lineIdx = 0;
                while (!reader.EndOfStream)
                {
                    var parts = reader.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    var record = default(KeyValuePair<string, ImageAnnotation[]>);
                    try
                    {

                        var imageAnnotations = new List<ImageAnnotation>();
                        for (int i = 1; i < parts.Length; i += 2)
                        {
                            var ann = new ImageAnnotation
                                             {
                                                 ROI = rectFromString(parts[i]),
                                                 Label = parts[i + 1].Trim().Trim('\'')
                                             };

                            imageAnnotations.Add(ann);
                        }

                        record = new KeyValuePair<string, ImageAnnotation[]>(parts.First(), imageAnnotations.ToArray());
                    }
                    catch
                    {
                        throw new Exception("Error loading database. The file-content is invalid. Line: " + lineIdx);
                    }

                    if (keys.Contains(record.Key))
                        throw new Exception(String.Format("Image: {0} is duplicated! Annotation load failed!", record.Key));
                    else
                        keys.Add(record.Key);

                    data.Add(record);
                    lineIdx++;
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
            if (lines.Length == 0)
                return new int[0];

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

        private static string[][] serializeToTable(IEnumerable<KeyValuePair<string, ImageAnnotation[]>> records, out bool[] isCommaSeparatedColumn)
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

        private static string[] serializeAnnotationData(KeyValuePair<string, ImageAnnotation[]> record, out bool[] isCommaSeparatedColumn)
        {
            bool[] isCommaSeparatedAnnCol = new bool[] { false /*X*/, false /*Y*/, false /*Width*/, true /*Height*/, true /*Tag*/ };

            List<string> parts = new List<string>();
            List<bool> isCommaSepCol = new List<bool>();

            parts.Add(record.Key);
            isCommaSepCol.Add(true);

            foreach (var ann in record.Value) //for each annotation in an image
            {
                var annColumns = serializeAnnotation(ann);
                parts.AddRange(annColumns);
                isCommaSepCol.AddRange(isCommaSeparatedAnnCol);
            }

            isCommaSeparatedColumn = isCommaSepCol.ToArray();
            return parts.ToArray();
        }

        private static string[] serializeAnnotation(ImageAnnotation ann)
        {
            var rect = ann.ROI;

            return new string[] 
            {
                rect.X.ToString(), rect.Y.ToString(), rect.Width.ToString(), rect.Height.ToString(), String.Format("'{0}'", ann.Label)
            };
        }

        private string getRelativePath(string imageName)
        {
            string relativePath = imageName.GetRelativeFilePath(new FileInfo(this.FileName).DirectoryName);
            if(relativePath == String.Empty)
                throw new Exception("Cannot find relative path of an image regarding the database path!" +
                                    "The database location must be in the same or in parent folder regarding selected image directory.");

            return relativePath;
        }
    }

}
