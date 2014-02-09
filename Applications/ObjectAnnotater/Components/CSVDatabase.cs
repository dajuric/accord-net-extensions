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
        string fileName;
        List<AnnotationData> data;

        private AnnotationDatabase() 
        {}

        public static AnnotationDatabase LoadOrCreate(string fileName)
        {
            var database = new AnnotationDatabase();
            database.fileName = fileName;

            if (File.Exists(fileName))
            {
                database.data = load(fileName).ToList();
            }
            else
            {
                File.Create(fileName);
                database.data = new List<AnnotationData>();
            }

            return database;
        }

        public void AddOrUpdate(string imageName, IEnumerable<Rectangle> annotations)
        {
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
            return null;
        }

        public bool Remove(string imageName)
        {
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
            bool[] isCommaSeparatedColumn;
            int[] columnLengths = getMaxColumnLengths(data, out isCommaSeparatedColumn);

            using (var writer = new StreamWriter(fileName, false))
            {
                foreach (var record in data)
                {
                    string line = ""

                    int colIdx = 1;
                    foreach (var ann in record.Value)
                    {
                        foreach (var annPart in rectToArr(ann))
                        {
                            if (colIdx == maxLenghts.Count)
                            {
                                maxLenghts.Add(0);
                                isCommaSepCol.Add(false);
                            }

                            maxLenghts[colIdx] = Math.Max(maxLenghts[colIdx], annPart.ToString().Length + RECT_ELEM_SPACING);

                            colIdx++;
                        }

                        //add colum spacing to the last column
                        maxLenghts[colIdx - 1] += (COLUMN_SPACING - RECT_ELEM_SPACING);
                        isCommaSepCol[colIdx - 1] = true;
                    }
                }
            }
        }

        private static IEnumerable<AnnotationData> load(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                var parts = reader.ReadLine().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                var record = new AnnotationData(
                                               parts.First(),
                                               parts.Skip(1).Select(x => rectFromString(x)).ToArray()
                                               );

                yield return record;
            }
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

        private static int[] getMaxColumnLengths(IEnumerable<AnnotationData> records, out bool[] isCommaSeparatedColumn)
        {
            const int COLUMN_SPACING = 10;
            const int RECT_ELEM_SPACING = 3;

            var isCommaSepCol = new List<bool>();
            isCommaSepCol.Add(true);

            var maxLenghts = new List<int>();
            maxLenghts.Add(0);

            foreach (var record in records)
            {
                maxLenghts[0] = Math.Max(maxLenghts[0], record.Key.Length);

                int colIdx = 1;
                foreach (var ann in record.Value)
                {
                    foreach (var annPart in rectToArr(ann))
                    {
                        if (colIdx == maxLenghts.Count)
                        {
                            maxLenghts.Add(0);
                            isCommaSepCol.Add(false);
                        }

                        maxLenghts[colIdx] = Math.Max(maxLenghts[colIdx], annPart.ToString().Length + RECT_ELEM_SPACING);

                        colIdx++;
                    }

                    //add colum spacing to the last column
                    maxLenghts[colIdx - 1] += (COLUMN_SPACING - RECT_ELEM_SPACING);
                    isCommaSepCol[colIdx - 1] = true;
                }
            }

            isCommaSeparatedColumn = isCommaSepCol.ToArray();
            return maxLenghts.ToArray();
        }

        private static int[] rectToArr(Rectangle rect)
        {
            return new int[] 
            {
                rect.X, rect.Y, rect.Width, rect.Height
            };
        }
    }

}
