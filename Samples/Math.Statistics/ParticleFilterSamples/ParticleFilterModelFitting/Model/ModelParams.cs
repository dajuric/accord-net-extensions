using LINE2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParticleFilterModelFitting
{
    public class ModelParams : ICloneable
    {
        public ModelParams(int modelTypeIndex, short scale, short angle)
        {
            this.ModelTypeIndex = modelTypeIndex;
            this.Scale = scale;
            this.Angle = angle;
        }

        public int ModelTypeIndex { get; set; }
        public short Scale { get; set; }
        /// <summary>
        /// Gets or sets angle offset, not the absolute value.
        /// </summary>
        public short Angle { get; set; }

        public ITemplate TryGetTemplate()
        {
            ITemplate val;
            ModelRepository.Repository.TryGetValue(this, out val);
            return val;
        }

        public override bool Equals(object obj)
        {
            if (obj is ModelParams == false)
                return false;

            var m = obj as ModelParams;

            if (this.ModelTypeIndex == m.ModelTypeIndex &&
                this.Angle == m.Angle &&
                this.Scale == m.Scale)
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return (this.Angle << (sizeof(int) / 2)) | (ushort)this.Scale | this.ModelTypeIndex; //can be better
        }

        public object Clone()
        {
            return new ModelParams(this.ModelTypeIndex, this.Scale, this.Angle);
        }
    }
}
