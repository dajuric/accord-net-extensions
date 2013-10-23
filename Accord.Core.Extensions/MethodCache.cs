using System;
using System.Collections.Concurrent;

namespace Accord.Core
{
    /// <summary>
    /// Represents a set of cache decoratated methods 
    /// Taken from: http://www.codeproject.com/Articles/195369/Simple-Method-Caching 
    /// and slightly modified.
    /// </summary>
    public class MethodCache
    {
        public static readonly MethodCache Global = new MethodCache();

        private readonly ConcurrentDictionary<Delegate, Delegate> delegates
            = new ConcurrentDictionary<Delegate, Delegate>();

        /// <summary>
        /// Invokes the target <paramref name="function"/>
        /// </summary>        
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="function">The target method.</param>        
        /// <returns>The result of invoking the target method.</returns>
        public TResult Invoke<TResult>(Func<TResult> function)
        {
            return ((Func<TResult>)delegates.GetOrAdd(function, d => CacheProvider.Decorate(function)))();
        }

        /// <summary>
        /// Invokes the target <paramref name="function"/>
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="function">The target method.</param>
        /// <param name="arg">The argument passed to the parameter of the target method.</param>
        /// <returns>The result of invoking the target method.</returns>
        public TResult Invoke<T, TResult>(Func<T, TResult> function, T arg)
        {
            return ((Func<T, TResult>)delegates.GetOrAdd(function, d => CacheProvider.Decorate(function)))(arg);
        }

        /// <summary>
        /// Invokes the target <paramref name="function"/>
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="function">The target method.</param>
        /// <param name="arg1">The argument passed to the first parameter of the target method.</param>
        /// <param name="arg2">The argument passed to the second parameter of the target method.</param>
        /// <returns>The result of invoking the target method.</returns>
        public TResult Invoke<T1, T2, TResult>(Func<T1, T2, TResult> function, T1 arg1, T2 arg2)
        {
            return ((Func<T1, T2, TResult>)delegates.GetOrAdd(function, d => CacheProvider.Decorate(function)))(arg1, arg2);
        }

        /// <summary>
        /// Invokes the target <paramref name="function"/>
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="function">The target method.</param>
        /// <param name="arg1">The argument passed to the first parameter of the target method.</param>
        /// <param name="arg2">The argument passed to the second parameter of the target method.</param>
        /// <param name="arg3">The argument passed to the third parameter of the target method.</param>
        /// <returns>The result of invoking the target method.</returns>
        public TResult Invoke<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function, T1 arg1, T2 arg2, T3 arg3)
        {
            return ((Func<T1, T2, T3, TResult>)delegates.GetOrAdd(function, d => CacheProvider.Decorate(function)))(arg1, arg2, arg3);
        }

        /// <summary>
        /// Invokes the target <paramref name="function"/>
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T4">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="function">The target method.</param>
        /// <param name="arg1">The argument passed to the first parameter of the target method.</param>
        /// <param name="arg2">The argument passed to the second parameter of the target method.</param>
        /// <param name="arg3">The argument passed to the third parameter of the target method.</param>
        /// <param name="arg4">The argument passed to the forth parameter of the target method.</param>
        /// <returns>The result of invoking the target method.</returns>
        public TResult Invoke<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return ((Func<T1, T2, T3, T4, TResult>)delegates.GetOrAdd(function, d => CacheProvider.Decorate(function)))(arg1, arg2, arg3, arg4);
        }
    }

    /// <summary>
    /// A class that is capable of decorating a method with a cache.
    /// </summary>
    static class CacheProvider
    {
        /// <summary>
        /// Decorates the target <paramref name="function"/> by returning a function
        /// delegate that points back to a cache for this function.
        /// </summary>        
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="function">The target method to decorate.</param>
        /// <returns>A function delagate that represents the cached version of the target method.</returns>
        public static Func<TResult> Decorate<TResult>(Func<TResult> function)
        {
            var lazyValue = new Lazy<TResult>(function, true);
            return () => lazyValue.Value;
        }


        /// <summary>
        /// Decorates the target <paramref name="function"/> by returning a function
        /// delegate that points back to a cache indexed by the method arguments.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="function">The target method to decorate.</param>
        /// <returns>A function delagate that represents the cached version of the target method.</returns>
        public static Func<T, TResult> Decorate<T, TResult>(Func<T, TResult> function)
        {
            var cache = new ConcurrentDictionary<T, TResult>();
            return arg => cache.GetOrAdd(arg, function);
        }

        /// <summary>
        /// Decorates the target <paramref name="function"/> by returning a function
        /// delegate that points back to a cache indexed by the method arguments.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="function">The target method to decorate.</param>
        /// <returns>A function delagate that represents the cached version of the target method.</returns>
        public static Func<T1, T2, TResult> Decorate<T1, T2, TResult>(Func<T1, T2, TResult> function)
        {
            var cache = new ConcurrentDictionary<Tuple<T1, T2>, TResult>();
            return (arg1, arg2) => cache.GetOrAdd(Tuple.Create(arg1, arg2), ck => function(arg1, arg2));
        }

        /// <summary>
        /// Decorates the target <paramref name="function"/> by returning a function
        /// delegate that points back to a cache indexed by the method arguments.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>        
        /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="function">The target method to decorate.</param>
        /// <returns>A function delagate that represents the cached version of the target method.</returns>
        public static Func<T1, T2, T3, TResult> Decorate<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function)
        {
            var cache = new ConcurrentDictionary<Tuple<T1, T2, T3>, TResult>();
            return (arg1, arg2, arg3) => cache.GetOrAdd(Tuple.Create(arg1, arg2, arg3), ck => function(arg1, arg2, arg3));
        }

        /// <summary>
        /// Decorates the target <paramref name="function"/> by returning a function
        /// delegate that points back to a cache indexed by the method arguments.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>        
        /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T4">The type of the forth parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="function">The target method to decorate.</param>
        /// <returns>A function delagate that represents the cached version of the target method.</returns>
        public static Func<T1, T2, T3, T4, TResult> Decorate<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> function)
        {
            var cache = new ConcurrentDictionary<Tuple<T1, T2, T3, T4>, TResult>();
            return (arg1, arg2, arg3, arg4) => cache.GetOrAdd(Tuple.Create(arg1, arg2, arg3, arg4), ck => function(arg1, arg2, arg3, arg4));
        }
    }
}
