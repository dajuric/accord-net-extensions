using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINE2D
{
    public interface ITemplatePyramid 
    {
        ITemplate[] Templates { get; }
    }

    public interface ITemplatePyramid<T> where T : ITemplate
    {
        T[] Templates { get; }

        void Initialize(T[] templates);
    }
}
