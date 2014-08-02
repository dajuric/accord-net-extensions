#region Licence and Terms
// Accord.NET Extensions Framework
// https://github.com/dajuric/accord-net-extensions
//
// Copyright © Darko Jurić, 2014 
// darko.juric2@gmail.com
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU Lesser General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU Lesser General Public License for more details.
// 
//   You should have received a copy of the GNU Lesser General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/lgpl.txt>.
//
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Accord.Extensions.Caching
{
    /// <summary>
    /// Interface for the Lazy cache object item.
    /// </summary>
    /// <typeparam name="TObj">Object value.</typeparam>
    public interface ILazy<TObj>
    {
        /// <summary>
        /// Returns true if the object value is loaded in the memory.
        /// </summary>
        bool IsValueCreated { get; }

        /// <summary>
        /// Gets the object value.
        /// If the object value is not loaded, the object will be constructed.
        /// </summary>
        TObj Value { get; }
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
    public class LazyMemoryCache<TKey, TValue> : IEnumerable<KeyValuePair<TKey, ILazy<TValue>>>
        where TValue: class
    {
        /// <summary>
        /// Represents lazy cache item. 
        /// The object value can be loaded on demand and also unloaded. The appropriate events are also given.
        /// </summary>
        /// <typeparam name="ObjKey">Object key.</typeparam>
        /// <typeparam name="TObj">Object value.</typeparam>
        private class LazyCacheItem<ObjKey, TObj> : ILazy<TObj>, IDisposable
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
            public bool IsValueCreated
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
        /// Structure that contains all objects (objects that consume memory + reference objects).
        /// </summary>
        private ConcurrentDictionary<TKey, ILazy<TValue>> cache;
        /// <summary>
        /// Management strategy (LRU) that is responsible for automatically object unloading.
        /// </summary>
        private LRUCache<TKey, LazyCacheItem<TKey, TValue>> managmentStrategy;
        /// <summary>
        /// Forces GC.Collect()  (user option).
        /// </summary>
        private bool forceCollectionOnRemoval;
        /// <summary>
        /// Sync object, needed for non-concurrent structures.
        /// </summary>
        object syncObj = new object();

        /// <summary>
        /// Constructs lazy memory cache which caches object constructor and destructor.
        /// <para>Value loading is handled in a lazy way (JIT), and it is automatically unloaded from memory when a specified capacity is reached.</para>
        /// <para>The memory management is handled by LRU strategy. See: LRUCache.</para>
        /// </summary>
        /// <param name="isCapacityReached">Function that returns true if the cache limit is reached and the cache should start to unload items.</param>
        /// <param name="objectSizeFunc">Function to determine object size.</param>
        /// <param name="forceCollectionOnRemoval">
        /// <para>When set to true calls GC.Collect() when a value is unloaded due to capacity reach, but the CPU consumption can be high and accessing / adding elements can be temporary delay due to garbage collector.</para>
        /// <para>If false the GC.Collect() is not called which can lead to cache evict values more aggressively which could be avoided by setting this flag to true.
        /// Also the capacity will be probably breached but the memory overflow exception should not be thrown.
        /// </para>
        /// </param>
        /// <example>
        /// ComputerInfo computerInfo = new ComputerInfo(); //reference to Microsoft.VisualBasic assembly.
        ///
        /// //construction
        /// var memCache = new LazyMemoryCache ;lt int, Image ;lt Gray, byte ;gt;gt
        ///  (
        ///   (currentSize) =>
        ///    {
        ///        var occupied = computerInfo.TotalPhysicalMemory - computerInfo.AvailablePhysicalMemory;
        ///        var occupiedPercentage = (float)occupied / computerInfo.TotalPhysicalMemory;
        ///
        ///        if (occupiedPercentage > 0.55)
         ///           return true;
        ///
        ///        return false;
        ///    },
        ///    
        ///    (image) => (ulong)(image.Stride * image.Height * image.ColorInfo.Size));
        ///    
        /// //adding elements (you can also use stream as IEnumerable to populate cache)
        /// memCache.AddOrUpdate(2, () =>   
        ///        {
        ///            var image = new Image ;ltGray, byte ;gt(1 * 1024 * 1024, 1, 0);
        ///            image.SetValue(252);
        ///            return image;
        ///        }, 
        ///        (img) => img.Dispose());
        ///        
        /// //... add second element
        /// //... add third element
        ///        
        /// //accessing elements
        /// foreach (var item in memCache)
         /// {
         ///    Console.WriteLine(item.Value.Value);
         ///    Console.WriteLine(memCache.HardFaults);
        ///  }
        /// </example>
        public LazyMemoryCache(Func<ulong, bool> isCapacityReached, Func<TValue, ulong> objectSizeFunc, bool forceCollectionOnRemoval = true)
        {
            cache = new ConcurrentDictionary<TKey, ILazy<TValue>>();
            this.forceCollectionOnRemoval = forceCollectionOnRemoval;

            managmentStrategy = new LRUCache<TKey, LazyCacheItem<TKey, TValue>>(isCapacityReached, (lazyContainer) => lazyContainer.IsValueCreated ? objectSizeFunc(lazyContainer.Value): 0);
            managmentStrategy.OnRemoveItem += managmentStrategy_OnRemoveItem;
        }

        void managmentStrategy_OnRemoveItem(LRUCache<TKey, LazyCacheItem<TKey, TValue>> sender, KeyValuePair<TKey, LazyCacheItem<TKey, TValue>> item, bool userRequested)
        {
            item.Value.Unload();
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

        /*
        /// <summary>
        /// Adds or updates reference to already existent object in cache.
        /// </summary>
        /// <param name="key">Key to add.</param>
        /// <param name="referenceKey">Reference key. Must exist.</param>
        public bool AddOrUpdateReference(TKey key, TKey referenceKey)
        {
            ILazy<TValue> val;
            if (!cache.TryGetValue(key, out val))
                return false;

            cache.GetOrAdd(key, val);
            return true;
        }*/

        void item_OnValueUnloaded(object sender, EventArgs e)
        {
            //the item is automatically unloaded (LRU) or by the user
            if (forceCollectionOnRemoval)
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
        public bool TryRemove(TKey key)
        {
            if (!cache.ContainsKey(key))
                return false;

            managmentStrategy.Remove(key);

            //if the key is the allocated object's key....
            ILazy<TValue> val;
            cache.TryGetValue(key, out val);

            //unload from memory
            (val as LazyCacheItem<TKey, TValue>).Dispose();

            //remove item
            cache.TryRemove(key, out val);

            return true;
        }

        /// <summary>
        /// Tries to get value under the specified key.
        /// </summary>
        /// <param name="key">Object key.</param>
        /// <param name="value">Object key.</param>
        /// <returns>True if the specified key exist, false otherwise.</returns>
        public bool TryGetValue(TKey key, out ILazy<TValue> value)
        {
            ILazy<TValue> val;
            bool contains = cache.TryGetValue(key, out val);

            value = val;
            return contains;
        }

        /// <summary>
        /// Gets the enumerator for the cache.
        /// <para>By enumerating the collection objects are loaded only if the value property from <see cref="Accord.Extensions.Caching.ILazy{T}"/> is read.</para>
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator<KeyValuePair<TKey, ILazy<TValue>>> GetEnumerator()
        {
            return cache.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator for the cache.
        /// <para>By enumerating the collection objects are loaded only if the value property from <see cref="Accord.Extensions.Caching.ILazy{T}"/> is read.</para>
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
            get { return cache.Count; }
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
