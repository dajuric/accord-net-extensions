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
                database.data = new List<AnnotationData>();
            }

            return database;
        }

        public void AddOrUpdate(string imageName, IEnumerable<Rectangle> annotations)
        {
            imageName = getRelativePath(imageName);

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

        private static List<AnnotationData> load(string fileName)
        {
            HashSet<string> keys = new HashSet<string>(); 
            List<AnnotationData> data = new List<AnnotationData>();

            using (var reader = new StreamReader(fileName))
            {
                int lineIdx = 0;
                while (!reader.EndOfStream)
                {
                    var parts = reader.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    AnnotationData record = default(AnnotationData);
                    try
                    {
                        record = new AnnotationData(
                                                  parts.First().Trim(),
                                                  parts.Skip(1).Select(x => rectFromString(x)).ToArray()
                                                    );
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
            try
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
            catch
            {
                throw new Exception("Cannot deserialize rectangle! \n");
            }
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
