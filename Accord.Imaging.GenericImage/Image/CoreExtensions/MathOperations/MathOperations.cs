using Accord.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accord.Imaging
{
    public static partial class MathOperations
    {
        /// <summary>
        /// Mathematic Operations.
        /// </summary>
        public enum MathOps : int
        {
            /// <summary>
            /// Bitwise AND
            /// </summary>
            And,
            /// <summary>
            /// Bitwise OR
            /// </summary>
            Or,
            /// <summary>
            /// Explicit bitwise OR
            /// </summary>
            Xor,

            /// <summary>
            /// Add
            /// </summary>
            Add,
            /// <summary>
            /// Sub
            /// </summary>
            Sub,
            /// <summary>
            /// Multiplication
            /// </summary>
            Mul,
            /// <summary>
            /// Division
            /// </summary>
            Div
        }

        delegate void MathOpFunc(IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask);

        static Dictionary<Type, MathOpFunc>[] mathOperatorFuncs;

        static MathOperations()
        {
            int numOfOperations = Enum.GetNames(typeof(MathOps)).Length;
            mathOperatorFuncs = new Dictionary<Type, MathOpFunc>[numOfOperations];

            for (int i = 0; i < numOfOperations; i++)
            {
                mathOperatorFuncs[i] = new Dictionary<Type, MathOpFunc>();
            }

            initializeLogicOperations();
            initializeArithemticOperations();
        }

        private static void calculate(MathOps mathOpIdx, IImage src1, IImage src2, IImage dest, Image<Gray, byte> mask = null)
        {
            Debug.Assert(src1.ColorInfo.Equals(src2.ColorInfo) && src1.Size.Equals(src2.Size));

            if (mask == null)
            {
                mask = new Image<Gray, byte>(dest.Width, dest.Height);
                mask.SetValue(new Gray(255));
            }

            var mathOperationOnTypes = mathOperatorFuncs[(int)mathOpIdx];

            MathOpFunc mathOpFunc = null;
            if (mathOperationOnTypes.TryGetValue(src1.ColorInfo.ChannelType, out mathOpFunc) == false)
                throw new Exception(string.Format("Math operation {0} can not be executed on an image of type {1}", mathOpIdx.ToString(), src1.ColorInfo.ChannelType));

            var proc = new ParallelProcessor<bool, bool>(dest.Size,
                                                            () =>
                                                            {
                                                                return true;
                                                            },
                                                            (bool _, bool __, Rectangle area) =>
                                                            {
                                                                var src1Patch = src1.GetSubRect(area);
                                                                var src2Patch = src2.GetSubRect(area);
                                                                var destPatch = dest.GetSubRect(area);
                                                                var maskPatch = mask.GetSubRect(area);

                                                                mathOpFunc(src1Patch, src2Patch, destPatch, maskPatch);
                                                            }
                                                            /*,new ParallelOptions { ForceSequential = true}*/);

            proc.Process(true);
        }
    }
}
