using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Accord.Extensions.Collections.Caching
{
    /// <summary>
    /// Interface for the Lazy cache object item.
    /// </summary>
    /// <typeparam name="ObjKey">Object key.</typeparam>
    /// <typeparam name="TObj">Object value.</typeparam>
    public interface ILazyCacheItem<ObjKey, TObj>
    {
        /// <summary>
        /// Gets the object key.
        /// </summary>
        ObjKey Key { get; }

        /// <summary>
        /// Gets the object value.
        /// If the object value is not loaded, the object will be constructed.
        /// </summary>
        TObj Value { get; }
    }

    /// <summary>
    /// Represents lazy cache item. 
    /// The object value can be loaded on demand and also unloaded. The appropriate events are also given.
    /// </summary>
    /// <typeparam name="ObjKey">Object key.</typeparam>
    /// <typeparam name="TObj">Object value.</typeparam>
    public class LazyCacheItem<ObjKey, TObj> : IDisposable, ILazyCacheItem<ObjKey, TObj>
           where TObj : class
    {
        public event EventHandler OnValueLoaded;
        public event EventHandler OnValueUnloaded;

        private Func<TObj> constructor;
        private Action<TObj> destructor;
        private TObj value = null;

        /// <summary>
        /// Constructs new lazy cache item.
        /// </summary>
        /// <param name="key">Key of the object.</param>
        /// <param name="constructor">Object constructor.</param>
        /// <param name="destructor">Object destructor.</param>
        public LazyCacheItem(ObjKey key, Func<TObj> constructor, Action<TObj> destructor)
        {
            this.constructor = constructor;
            this.destructor = destructor;
            this.Key = key;
        }

        /// <summary>
        /// Returns true if the object value is loaded in the memory.
        /// </summary>
        public bool HasValue
        {
            get
            {
                return value != null;
            }
        }

        /// <summary>
        /// Gets the object value.
        /// If the object value is not loaded, the object will be constructed.
        /// </summary>
        public TObj Value
        {
            get
            {
                if (value == null)
                {
                    lock (this)
                    {
                        value = constructor();
                        if (OnValueLoaded != null)
                            OnValueLoaded(this, new EventArgs());
                    }
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the object key.
        /// </summary>
        public ObjKey Key { get; private set; }

        /// <summary>
        /// Unloads object from the memory (destructs).
        /// </summary>
        public void Unload()
        {
            if (this.value != null)
            {
                destructor(value);
                value = null;
                if (OnValueUnloaded != null)
                    OnValueUnloaded(this, new EventArgs());
            }
        }

        bool isDisposed = false;
        /// <summary>
        /// Disposes the contained object.
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                Unload();
                isDisposed = true;
            }
        }
    }

    /// <summary>
    /// Lazy memory cache.
    /// Caches object constructor and destructor into RAM, so when a user requests an object by using appropriate key an object is loaded into memory.
    /// An object will be removed automatically from memory by using LRU strategy.
    /// 
    /// <para>Use this class for loading collections that can not fit into memory. 
    /// This class provides convenient interface where the cache itself can be represented as collection.
    /// </para>
    /// 
    /// </summary>
    /// <typeparam name="TKey">Object key.</typeparam>
    /// <typeparam name="TValue">Object value.</typeparam>
    public class LazyMemoryCache<TKey, TValue> : IEnumerable<ILazyCacheItem<TKey, TValue>>
        where TValue: class
    {
        private ConcurrentDictionary<TKey, LazyCacheItem<TKey, TValue>> cache;
        private LRUCache<TKey, LazyCacheItem<TKey, TValue>> managmentStrategy;
        object syncObj = new object();

        /// <summary>
        /// Constructs lazy memory cache.
        /// </summary>
        /// <param name="isCapacityReached">Function that returns true if the cache limit is reached and the cache should start to unload items.</param>
        /// <param name="objectSizeFunc">Function to determine object size.</param>
        public LazyMemoryCache(Func<ulong, bool> isCapacityReached, Func<TValue, ulong> objectSizeFunc)
        {
            cache = new ConcurrentDictionary<TKey, LazyCacheItem<TKey, TValue>>();

            managmentStrategy = new LRUCache<TKey, LazyCacheItem<TKey, TValue>>(isCapacityReached, (lazyContainer) => lazyContainer.HasValue ? objectSizeFunc(lazyContainer.Value): 0);
            managmentStrategy.OnRemoveItem += managmentStrategy_OnRemoveItem;
        }

        void managmentStrategy_OnRemoveItem(LRUCache<TKey, LazyCacheItem<TKey, TValue>> sender, KeyValuePair<TKey, LazyCacheItem<TKey, TValue>> item, bool userRequested)
        {
            item.Value.Unload();

            if (userRequested)
            {
                item.Value.Dispose();

                LazyCacheItem<TKey, TValue> val;
                cache.TryRemove(item.Key, out val);
            }
        }

        /// <summary>
        /// Adds or updates the object value and the related cache statistics.
        /// </summary>
        /// <param name="key">Object key.</param>
        /// <param name="constructor">Object constructor.</param>
        /// <param name="destructor">Object destructor.</param>
        public void AddOrUpdate(TKey key, Func<TValue> constructor, Action<TValue> destructor)
        {
            var item = new LazyCacheItem<TKey, TValue>(key, constructor, destructor);
            item.OnValueLoaded += item_OnValueLoaded;
            item.OnValueUnloaded += item_OnValueUnloaded;

            cache.GetOrAdd(key, item);
            managmentStrategy.Add(key, item);
        }

        void item_OnValueUnloaded(object sender, EventArgs e)
        {
            //the item is automatically unloaded (LRU) or by the user
            GC.Collect();
        }

        void item_OnValueLoaded(object sender, EventArgs e)
        {
            var lazyItem = sender as LazyCacheItem<TKey, TValue>;
            managmentStrategy.AddOrUpdate(lazyItem.Key, lazyItem); //update size information

            lock (syncObj) HardFaults++;
        }

        /// <summary>
        /// Unloads and removes the object from the cache.
        /// </summary>
        /// <param name="key">Object key.</param>
        public void Remove(TKey key)
        {
            managmentStrategy.Remove(key);
        }

        /// <summary>
        /// Tries to get value under the specified key.
        /// </summary>
        /// <param name="key">Object key.</param>
        /// <param name="value">Object key.</param>
        /// <returns>True if the specified key exist, false otherwise.</returns>
        public bool TryGetValue(TKey key, out ILazyCacheItem<TKey, TValue> value)
        {
            LazyCacheItem<TKey, TValue> val;
            bool contains = cache.TryGetValue(key, out val);

            value = val;
            return contains;
        }

        /// <summary>
        /// Gets the enumerator for the cache.
        /// <para>By enumerating the collection objects are loaded only if the value property from <see cref="ILazyCacheItem"/> is read.</para>
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator<ILazyCacheItem<TKey, TValue>> GetEnumerator()
        {
            return cache.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator for the cache.
        /// <para>By enumerating the collection objects are loaded only if the value property from <see cref="ILazyCacheItem"/> is read.</para>
        /// </summary>
        /// <returns>Enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)GetEnumerator();
        }

        /// <summary>
        /// Gets the number of hard faults.
        /// (every time when an item is loaded the value is incremented by one)
        /// </summary>
        public int HardFaults
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of objects in the cache.
        /// </summary>
        public int Count 
        {
            get { return managmentStrategy.Count; }
        }

        /// <summary>
        /// Gets the total size of objects (specified by function in constructor) in the cache.
        /// </summary>
        public ulong TotalSize
        {
            get { return managmentStrategy.TotalSize; }
        }
    }
}
