namespace Accord.Extensions
{
    /// <summary>
    /// Default interface for color types.
    /// </summary>
    public interface IColor { }
    /// <summary>
    /// Interface for 2 channel color type. (Used for compile-time restrictions)
    /// </summary>
    public interface IColor2 : IColor { }
    /// <summary>
    /// Interface for 3 channel color type. (Used for compile-time restrictions)
    /// </summary>
    public interface IColor3 : IColor { }
    /// <summary>
    /// Interface for 4 channel color type. (Used for compile-time restrictions)
    /// </summary>
    public interface IColor4 : IColor { }
}
