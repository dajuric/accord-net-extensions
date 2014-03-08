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
    public static class PicoDetectorHexLoader
    {
        public static void FromHexFile(string fileName, out PicoDetector detector)
        {
            string data = File.ReadAllText(fileName);

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

            var hexData = noComments.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(x => x.Trim())
                                    .ToArray();

            byte[] buffer = new byte[hexData.Length];

            for (int i = 0; i < hexData.Length; i++)
            {
                buffer[i] = Convert.ToByte(hexData[i], 16);
            }

            using (var stream = new MemoryStream(buffer))
            {
                FromByteStream(stream, out detector);
            }
        }

        public static void FromByteStream(Stream stream, out PicoDetector detector)
        {
            BinaryReader reader = new BinaryReader(stream);

            RectangleF normlizedRegion = new RectangleF
            {
                X = reader.ReadSingle(),
                Y = reader.ReadSingle(),
                Width = reader.ReadSingle(),
                Height = reader.ReadSingle()
            };

            Cascade<StageClassifier> cascade;
            loadCascade(stream, out cascade);

            detector = new PicoDetector(normlizedRegion, cascade);
        }

        private static void loadCascade(Stream stream, out Cascade<StageClassifier> cascade)
        {
            BinaryReader reader = new BinaryReader(stream);

            int numberOfStages = reader.ReadInt32();
            cascade = new Cascade<StageClassifier>();

            for (int stageIdx = 0; stageIdx < numberOfStages; stageIdx++)
            {
                StageClassifier stage; float stageThreshold;
                loadStageClassifier(stream, out stage, out stageThreshold);

                cascade.AddStage(stage, stageThreshold);
            }
        }

        private static void loadStageClassifier(Stream stream, out StageClassifier stage, out float stageThreshold)
        {
            BinaryReader reader = new BinaryReader(stream);

            int nTrees = reader.ReadInt32();
            List<WeakLearner> learners = new List<WeakLearner>();

            for (int learnerIdx = 0; learnerIdx < nTrees; learnerIdx++)
            {
                WeakLearner weakLearner;
                loadWeakLearner(stream, out weakLearner);

                learners.Add(weakLearner);
            }

            stageThreshold = reader.ReadSingle();
            stage = new StageClassifier(learners);
        }

        private static void loadWeakLearner(Stream stream, out WeakLearner weakLearner)
        {
            BinaryReader reader = new BinaryReader(stream);

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
