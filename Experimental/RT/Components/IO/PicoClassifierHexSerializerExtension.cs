using Accord.Extensions;
using System;
using System.IO;
using MoreLinq;
//using StageClassifier = RT.GentleBoost<RT.RegressionTree<RT.BinTestCode>>;
//using WeakLearner = RT.RegressionTree<RT.BinTestCode>;

namespace RT
{
    /// <summary>
    /// Pico-classifier data serializer to hex text file.
    /// The format is the following:
    ///     [normalizedRegion as (row, col, scaleRow, scaleCol)] (4 * Single),
    ///     number of stages (Int32)
    ///     
    ///     (stages)
    ///         numberOfTrees (Int32)
    ///         
    ///         (tree data)
    ///              depth (Int32)
    ///              internalNodes ((depth-1) * (2+2+1+1 or 4 Byte - compatibility))
    ///              leafs (depth * Single)
    ///              
    ///          stage threshold (Single)
    ///          
    ///      (next stage...) 
    /// </summary>
    public static class PicoClassifierHexSerializerExtension
    {
        /// <summary>
        /// Saves Pico-classifer data to hex text file.
        /// </summary>
        /// <param name="fileName">Detector file path.</param>
        /// <param name="detector">A detector to save.</param>
        public static void ToHexFile<TColor>(this PicoClassifier<TColor> detector, string fileName)
            where TColor: IColor
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            using (var memStream = new MemoryStream())
            {
                toBinaryStream(detector, new BinaryWriter(memStream));
                toTextHexStream(new BinaryReader(memStream), new StreamWriter(fileStream));

                fileStream.Flush(flushToDisk: true);
            }
        }

        /// <summary>
        /// Saves Pico-classifer data to binary data file.
        /// </summary>
        /// <param name="fileName">Detector file path.</param>
        /// <param name="detector">A detector to save.</param>
        public static void ToBinaryFile<TColor>(this PicoClassifier<TColor> detector, string fileName)
            where TColor : IColor
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                toBinaryStream(detector, new BinaryWriter(fileStream));
                fileStream.Flush(flushToDisk: true);
            }
        }

        private static void toTextHexStream(BinaryReader binStreamReader, TextWriter textHexStreamWriter)
        {
            const int NUMBER_OF_HEX_PER_ROW = 32;
            int nHexPerRow = NUMBER_OF_HEX_PER_ROW;

            var binStream =  binStreamReader.BaseStream;
            binStream.Seek(0, SeekOrigin.Begin);

            while (binStream.Position != binStream.Length)
            {
                if (nHexPerRow == NUMBER_OF_HEX_PER_ROW)
                {
                    textHexStreamWriter.Write(" ");
                    textHexStreamWriter.Write(textHexStreamWriter.NewLine);
                    textHexStreamWriter.Write("\t");
                    nHexPerRow = 0;
                }

                nHexPerRow++;

                var num = binStreamReader.ReadByte();
                textHexStreamWriter.Write(String.Format("0x{0}, ", num.ToString("x2")));    
            }

            //found in facefinder.ea (decorative :-) ?)
            textHexStreamWriter.Write(textHexStreamWriter.NewLine);
            textHexStreamWriter.Write("\t0x00"); 
            textHexStreamWriter.Write(textHexStreamWriter.NewLine);

            textHexStreamWriter.Flush();
        }

        private static void toBinaryStream<TColor>(PicoClassifier<TColor> detector, BinaryWriter writer)
            where TColor: IColor
        {
            /******** compatibility **********/
            writer.Write((Single)0);
            writer.Write((Single)0);
            writer.Write((Single)1);
            /******** compatibility **********/

            writer.Write((Single)detector.WindowWidthMultiplier);

            writeCascade(detector.Cascade, writer);
        }

        private static void writeCascade<TColor>(Cascade<GentleBoost<RegressionTree<BinTestCode<TColor>>>> cascade, BinaryWriter writer)
            where TColor: IColor
        {
            writer.Write((Int32)cascade.NumberOfStages);

            for (int stageIdx = 0; stageIdx < cascade.NumberOfStages; stageIdx++)
            {
                writeStageClassifier(cascade.StageClassifiers[stageIdx], cascade.StageThresholds[stageIdx], writer);
            }
        }

        private static void writeStageClassifier<TColor>(GentleBoost<RegressionTree<BinTestCode<TColor>>> stage, float stageThreshold, BinaryWriter writer)
            where TColor: IColor
        {
            writer.Write((Int32)stage.Learners.Count);

            for (int learnerIdx = 0; learnerIdx < stage.Learners.Count; learnerIdx++)
            {
                writeWeakLearner(stage.Learners[learnerIdx], writer);
            }

            writer.Write((Single)stageThreshold);
        }

        private static void writeWeakLearner<TColor>(RegressionTree<BinTestCode<TColor>> weakLearner, BinaryWriter writer)
            where TColor: IColor
        {
            writer.Write(weakLearner.TreeDepth); //depth must not take leaf nodes into account

            foreach (var binTest in weakLearner.InternalNodeData)
            {
                binTest.ToBytes().ForEach(x=> writer.Write(x));
            }

            foreach (var leafOutput in weakLearner.LeafData)
            {
                writer.Write((Single)leafOutput);
            }
        }
    }
}
