using Accord.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
//using StageClassifier = RT.GentleBoost<RT.RegressionTree<RT.BinTestCode>>;
//using WeakLearner = RT.RegressionTree<RT.BinTestCode>;

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
    ///              internalNodes ((depth-1) * (2+2+1+1 / 4))
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
        public static void FromHexFile<TColor>(string fileName, out PicoClassifier<TColor> detector)
            where TColor: IColor
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
        public static void FromBinaryFile<TColor>(string fileName, out PicoClassifier<TColor> detector)
            where TColor: IColor
        {
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                fromBinaryStream(new BinaryReader(fileStream), out detector);
            }
        }

        private static void fromBinaryStream<TColor>(BinaryReader reader, out PicoClassifier<TColor> detector)
            where TColor: IColor
        {
            /******* not used (for compatibility) **********/
            float row = reader.ReadSingle(); 
            float col = reader.ReadSingle();
            float rowScale = reader.ReadSingle();
            /******* not used (for compatibility) **********/

            float colScale = reader.ReadSingle();

            Cascade<GentleBoost<RegressionTree<BinTestCode<TColor>>>> cascade;
            loadCascade(reader, out cascade);

            detector = new PicoClassifier<TColor>(colScale, cascade);
        }

        private static void loadCascade<TColor>(BinaryReader reader, out Cascade<GentleBoost<RegressionTree<BinTestCode<TColor>>>> cascade)
            where TColor: IColor
        {
            int numberOfStages = reader.ReadInt32();
            cascade = new Cascade<GentleBoost<RegressionTree<BinTestCode<TColor>>>>();

            for (int stageIdx = 0; stageIdx < numberOfStages; stageIdx++)
            {
                GentleBoost<RegressionTree<BinTestCode<TColor>>> stage; float stageThreshold;
                loadStageClassifier(reader, out stage, out stageThreshold);

                cascade.AddStage(stage, stageThreshold);
            }
        }

        private static void loadStageClassifier<TColor>(BinaryReader reader, out GentleBoost<RegressionTree<BinTestCode<TColor>>> stage, out float stageThreshold)
            where TColor: IColor
        {
            int nTrees = reader.ReadInt32();
            var learners = new List<RegressionTree<BinTestCode<TColor>>>();

            for (int learnerIdx = 0; learnerIdx < nTrees; learnerIdx++)
            {
                RegressionTree<BinTestCode<TColor>> weakLearner;
                loadWeakLearner(reader, out weakLearner);

                learners.Add(weakLearner);
            }

            stageThreshold = reader.ReadSingle();
            stage = new GentleBoost<RegressionTree<BinTestCode<TColor>>>(learners);
        }

        private static void loadWeakLearner<TColor>(BinaryReader reader, out RegressionTree<BinTestCode<TColor>> weakLearner)
            where TColor: IColor
        {
            var depth = reader.ReadInt32();
    
            var testCodes = new BinTestCode<TColor>[(1 << depth) - 1];
            for (int i = 0; i < testCodes.Length; i++)
            {
                testCodes[i] = new BinTestCode<TColor>(readBytes(reader, BinTestCode<TColor>.PackedLength).ToList());
            }

            float[] outputs = new float[(1 << depth)];
            for (int i = 0; i < outputs.Length; i++)
            {
                outputs[i] = reader.ReadSingle();
            }

            weakLearner = new RegressionTree<BinTestCode<TColor>>(testCodes, outputs);
        }

        private static IEnumerable<sbyte> readBytes(BinaryReader reader, int count)
        {
            while (count > 0 && 
                   reader.BaseStream.Position < reader.BaseStream.Length)
            {
                count--;
                yield return reader.ReadSByte();
            }
        }
    }
}
