using Accord.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using StageClassifier = RT.GentleBoost<RT.RegressionTree<RT.BinTestCode>>;
using WeakLearner = RT.RegressionTree<RT.BinTestCode>;

namespace RT
{
    /// <summary>
    /// Pico-classifier data loader from hex text file.
    /// The format is the following:
    ///     [normalizedRegion as (row, col, scaleRow, scaleCol)] (4 * Single),
    ///     number of stages (Int32)
    ///     
    ///     (stages)
    ///         numberOfTrees (Int32)
    ///         
    ///         (tree data)
    ///              depth (Int32)
    ///              internalNodes ((depth-1) * Single)
    ///              leafs (depth * Single)
    ///              
    ///          stage threshold (Single)
    ///          
    ///      (next stage...) 
    /// </summary>
    public static class PicoClassifierHexDeserializer
    {
        /// <summary>
        /// Loads Pico-classifer data from hex text file.
        /// </summary>
        /// <param name="fileName">Detector file path.</param>
        /// <param name="detector">Loaded detector.</param>
        public static void FromHexFile(string fileName, out PicoClassifier detector)
        {
            string data = File.ReadAllText(fileName);

            /********************************** remove comments ***********************************/
            var blockComments = @"/\*(.*?)\*/";
            var lineComments = @"//(.*?)\r?\n";
            var strings = @"""((\\[^\n]|[^""\n])*)""";
            var verbatimStrings = @"@(""[^""]*"")+";

            string noComments = Regex.Replace(data,
                  blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
                  me =>
                  {
                      if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
                          return me.Value.StartsWith("//") ? Environment.NewLine : "";
                      // Keep the literal strings
                      return me.Value;
                  },
                  RegexOptions.Singleline);
            /********************************** remove comments ***********************************/

            /***************************** fill buffer ***************************************/
            var hexData = noComments.Split(new char[] { ',' })
                                    .Where(x => !String.IsNullOrWhiteSpace(x))
                                    .Select(x => x.Trim())
                                    .ToArray();

            byte[] buffer = new byte[hexData.Length];

            for (int i = 0; i < hexData.Length; i++)
            {
                buffer[i] = Convert.ToByte(hexData[i], 16);
            }
            /***************************** fill buffer ***************************************/

            //load data
            using (var reader = new BinaryReader(new MemoryStream(buffer)))
            {
                fromBinaryStream(reader, out detector);
            }
        }

        /// <summary>
        /// Loads Pico-classifer data from binary data file.
        /// </summary>
        /// <param name="fileName">Detector file path.</param>
        /// <param name="detector">Loaded detector.</param>
        public static void FromBinaryFile(string fileName, out PicoClassifier detector)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                fromBinaryStream(new BinaryReader(fileStream), out detector);
            }
        }

        private static void fromBinaryStream(BinaryReader reader, out PicoClassifier detector)
        {
            float row = reader.ReadSingle(); float col = reader.ReadSingle();
            float rowScale = reader.ReadSingle(); float colScale = reader.ReadSingle(); 

            RectangleF normlizedRegion = new RectangleF
            {
                X = col,
                Y = row,
                Width = colScale,
                Height = rowScale
            };

            Cascade<StageClassifier> cascade;
            loadCascade(reader, out cascade);

            detector = new PicoClassifier(normlizedRegion, cascade);
        }

        private static void loadCascade(BinaryReader reader, out Cascade<StageClassifier> cascade)
        {
            int numberOfStages = reader.ReadInt32();
            cascade = new Cascade<StageClassifier>();

            for (int stageIdx = 0; stageIdx < numberOfStages; stageIdx++)
            {
                StageClassifier stage; float stageThreshold;
                loadStageClassifier(reader, out stage, out stageThreshold);

                cascade.AddStage(stage, stageThreshold);
            }
        }

        private static void loadStageClassifier(BinaryReader reader, out StageClassifier stage, out float stageThreshold)
        {
            int nTrees = reader.ReadInt32();
            List<WeakLearner> learners = new List<WeakLearner>();

            for (int learnerIdx = 0; learnerIdx < nTrees; learnerIdx++)
            {
                WeakLearner weakLearner;
                loadWeakLearner(reader, out weakLearner);

                learners.Add(weakLearner);
            }

            stageThreshold = reader.ReadSingle();
            stage = new StageClassifier(learners);
        }

        private static void loadWeakLearner(BinaryReader reader, out WeakLearner weakLearner)
        {
            var depth = reader.ReadInt32();

            BinTestCode[] testCodes = new BinTestCode[(1 << depth) - 1];
            for (int i = 0; i < testCodes.Length; i++)
            {
                testCodes[i] = new BinTestCode(reader.ReadInt32());
            }

            float[] outputs = new float[(1 << depth)];
            for (int i = 0; i < outputs.Length; i++)
            {
                outputs[i] = reader.ReadSingle();
            }

            weakLearner = new WeakLearner(testCodes, outputs);
        }

    }
}
