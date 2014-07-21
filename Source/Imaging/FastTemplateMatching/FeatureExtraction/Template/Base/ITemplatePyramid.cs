
namespace LINE2D
{
    /// <summary>
    /// LINE2D template pyramid interface.
    /// </summary>
    public interface ITemplatePyramid 
    {
        /// <summary>
        /// Collection of templates. One template for each pyramid scale.
        /// </summary>
        ITemplate[] Templates { get; }
    }

    /// <summary>
    /// LINE2D template pyramid interface.
    /// </summary>
    public interface ITemplatePyramid<T> where T : ITemplate
    {
        /// <summary>
        /// Collection of templates. One template for each pyramid scale.
        /// </summary>
        T[] Templates { get; }

        /// <summary>
        /// Initializes template pyramid with the provided templates.
        /// </summary>
        /// <param name="templates">Collection of templates.</param>
        void Initialize(T[] templates);
    }
}
