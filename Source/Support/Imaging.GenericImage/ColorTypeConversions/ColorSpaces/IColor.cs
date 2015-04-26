
namespace Accord.Extensions.Imaging
{
    /// <summary>
    /// Default interface for color types.
    /// </summary>
    public interface IColor { }

    /// <summary>
    /// Default generic interface for color types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IColor<T> : IColor
        where T : struct
    { }

    /// <summary>
    /// Interface for 2 channel color type. (Used for compile-time restrictions)
    /// </summary>
    public interface IColor2 : IColor { }

    /// <summary>
    /// Generic interface for 2 channel color type. (Used for compile-time restrictions)
    /// </summary>
    public interface IColor2<T> : IColor2, IColor<T>
         where T : struct
    { }

    /// <summary>
    /// Interface for 3 channel color type. (Used for compile-time restrictions)
    /// </summary>
    public interface IColor3 : IColor { }

    /// <summary>
    /// Generic interface for 3 channel color type. (Used for compile-time restrictions)
    /// </summary>
    /// <typeparam name="T">Channel type.</typeparam>
    public interface IColor3<T> : IColor3, IColor<T>
         where T : struct
    { }

    /// <summary>
    /// Interface for 4 channel color type. (Used for compile-time restrictions)
    /// </summary>
    public interface IColor4 : IColor { }

    /// <summary>
    /// Generic interface for 4 channel color type. (Used for compile-time restrictions)
    /// </summary>
    /// <typeparam name="T">Channel type.</typeparam>
    public interface IColor4<T> : IColor4, IColor<T>
         where T : struct
    { }
}
