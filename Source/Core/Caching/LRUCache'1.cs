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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Accord.Extensions.Caching
{
    /// <summary>
    /// Represents key value pair cache collection which is using "Least Recently Used (LRU)" element replace when the capacity is reached.
    /// </summary>
    /// <typeparam name="K">Key.</typeparam>
    /// <typeparam name="V">Value.</typeparam>
    public class LRUCache<K, V>: IDictionary<K, V>
    {
        /// <summary>
        /// Occurs when the item is about to be removed.
        /// </summary>
        /// <param name="sender">LRU cache instance.</param>
        /// <param name="item">Item to be removed.</param>
        /// <param name="userRequested">Is the removal user request or is it performed automatically by cache.</param>
        public delegate void RemovedItem(LRUCache<K, V> sender, KeyValuePair<K, V> item, bool userRequested);

        /// <summary>
        /// Occurs when the LRUCache is about to discard its oldest item
        /// because its capacity has been reached and a new item is being added.  
        /// </summary>
        /// <remarks>The item has not been discarded yet, and thus is still contained in 
        /// the Oldest property.</remarks>
        public event RemovedItem OnRemoveItem;

        // The index into the list, used by Add, Remove, and Contains.
        Dictionary<K, V> dict;

        //The key size mapping
        Dictionary<K, ulong> sizeContainer;

        // The list of items in the cache.  New items are added to the end of the list;
        // existing items are moved to the end when added; the items thus appear in
        // the list in the order they were added/used, with the least recently used
        // item being the first.  This is internal because the LRUCacheEnumerator
        // needs to access it.
        
        LinkedList<K> queue = null;

        /// <summary>
        /// Add, Clear, CopyTo, and Remove lock on this object to keep them thread-safe.
        /// </summary>
        object syncObj = new object();

        /// <summary>
        /// User-defined function for getting an object expense (size).
        /// </summary>
        Func<V, ulong> objectSizeFunc;

        /// <summary>
        /// User-defined function for getting an stopping condition. 
        /// Parameters: total cache size (user defined) - see: <see cref="objectSizeFunc"/>.
        /// </summary>
        Func<ulong, bool> isCapacityReached;

        /// <summary>
        /// Initializes a new instance of the LRUCache class that is empty and has the specified
        /// initial capacity.
        /// </summary>
        /// <param name="isCapacityReached">Func that return true if the capacity is reached.</param>
        /// <param name="objectSizeFunc">Func that returns the object size in bytes.</param>
        public LRUCache(Func<ulong, bool> isCapacityReached, Func<V, ulong> objectSizeFunc)
        {
            dict = new Dictionary<K, V>();
            sizeContainer = new Dictionary<K, ulong>();
            queue = new LinkedList<K>();
            
            this.isCapacityReached = isCapacityReached;
            this.objectSizeFunc = objectSizeFunc;
        }

        /// <summary>
        /// Gets the number of items contained in the LRUCache.
        /// </summary>
        public int Count
        {
            get { lock (syncObj) return dict.Count; }
        }

        /// <summary>
        /// Gets the total size of all items in user defined units.
        /// <para>Default: 1 per item. </para>
        /// </summary>
        public ulong TotalSize
        {
            get;
            private set;
        }

		/// <summary>
        /// The oldest (i.e. least recently used) item in the LRUCache.
        /// </summary>
        public KeyValuePair<K, V> Oldest
        {
            get
            {
                var oldestKey = queue.First.Value;

                V val;
                dict.TryGetValue(oldestKey, out val);

                return new KeyValuePair<K, V>(oldestKey, val);
            }
        }

		/// <summary>
        /// Add an item to the LRUCache, making it the newest item (i.e. the last
        /// item in the list). If the key is already in the LRUCache, its value is replaced.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <remarks>If the LRUCache has a nonzero capacity, and it is at its capacity, this 
        /// method will discard the oldest item, raising the DiscardingOldestItem event before 
        /// it does so.</remarks>
        public void Add(K key, V value)
        {
            Add(new KeyValuePair<K, V>(key, value));
        }

		/// <summary>
        /// Add an item to the LRUCache, making it the newest item (i.e. the last
        /// item in the list). If the key is already in the LRUCache, an exception is thrown.
        /// </summary>
        /// <param name="pair">The item that is being used.</param>
        /// <remarks>If the LRUCache has a nonzero capacity, and it is at its capacity, this 
        /// method will discard the oldest item, raising the DiscardingOldestItem event before 
        /// it does so.</remarks>
        public void Add(KeyValuePair<K, V> pair)
        {
            bool contains = false;
            lock (syncObj) 
                contains = dict.ContainsKey(pair.Key);

            if (contains)
                throw new Exception("The key already exist. Use AddOrUpdate command to update existing key.");
        }

        /// <summary>
        /// Add an item to the LRUCache, making it the newest item (i.e. the last
        /// item in the list). If the key is already in the LRUCache, its value is replaced.
        /// </summary>
        /// <param name="key">Data key value.</param>
        /// <param name="value">Value.</param>
        ///  /// <remarks>If the LRUCache has a nonzero capacity, and it is at its capacity, this 
        /// method will discard the oldest item, raising the DiscardingOldestItem event before 
        /// it does so.</remarks>
        public void AddOrUpdate(K key, V value)
        {
            lock (syncObj)
            {
                //if the key is already inside remove it
                tryRemove(key, false);

                queue.AddLast(key);
                dict[key] = value;

                var objSize = objectSizeFunc(value);
                sizeContainer[key] = objSize;
                TotalSize += objSize;

                while (isCapacityReached(this.TotalSize) && dict.Count != 0)
                {
                    // cache full, so re-use the oldest node
                    var node = queue.First;
                    tryRemove(node.Value, false);
                }
            }
        }

        private bool tryRemove(K key, bool userRequested)
        {
            lock (syncObj)
            {
                V val;
                var success = dict.TryGetValue(key, out val);

                if (success)
                {
                    if (OnRemoveItem != null)
                        OnRemoveItem(this, this.Oldest, userRequested);

                    var objSize = sizeContainer[key];
                    TotalSize -= objSize;

                    dict.Remove(key);
                    sizeContainer.Remove(key);
                    queue.Remove(key);
                }

                return success;
            }
        }

		/// <summary>
        /// Remove the specified item from the LRUCache.
        /// </summary>
        /// <param name="key">The key of the item to remove from the LRUCache.</param>
        /// <returns>true if the item was successfully removed from the LRUCache,
        /// otherwise false.  This method also returns false if the item was not
        /// found in the LRUCache.</returns>
        public bool Remove(K key)
        {
            return tryRemove(key, true);
        }

		/// <summary>
        /// Clear the contents of the LRUCache.
        /// </summary>
        public void Clear()
        {
            lock (syncObj)
            {
                queue.Clear();
                dict.Clear();
                this.TotalSize = 0;
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <returns> 
        /// True if the System.Collections.Generic.Dictionary{TKey,TValue} contains an 
        /// element with the specified key; otherwise, false.
        /// </returns>
        public bool TryGetValue(K key, out V value)
        {
            lock (syncObj)
            {
                if (dict.TryGetValue(key, out value))           
                {
                    queue.Remove(key);                        
                    queue.AddLast(key);                   
                    return true;
                }

                return false;
            }
        }

		/// <summary>
        /// Determines whether the LRUCache contains a specific value.
        /// </summary>
        /// <param name="key">The key of the item to locate in the LRUCache.</param>
        /// <returns>true if the item is in the LRUCache, otherwise false.</returns>
        public bool ContainsKey(K key)
        {
            return dict.ContainsKey(key);
        }

        /// <summary>
        /// Returns keys.
        /// </summary>
        public ICollection<K> Keys
        {
            get { return dict.Keys; }
        }

        /// <summary>
        /// Returns values.
        /// </summary>
        public ICollection<V> Values
        {
            get { return dict.Values; }
        }

        /// <summary>
        /// Gets or sets value which is associated with specified key.
        /// If the key already exist a previous value will be updated. 
        /// The appropriate events will be fired.
        /// </summary>
        /// <param name="key">Object key.</param>
        /// <returns>Value.</returns>
        public V this[K key]
        {
            get
            {
                lock(syncObj) return dict[key];
            }
            set
            {
                AddOrUpdate(key, value);
            }
        }

        /// <summary>
        /// Determines whether the cache contains specified item or not.
        /// </summary>
        /// <param name="item">Specified item.</param>
        /// <returns>True if the item is in cache, false otherwise.</returns>
        public bool Contains(KeyValuePair<K, V> item)
        {
            lock (syncObj)
            {
                V value;
                if (!this.TryGetValue(item.Key, out value))
                    return false;

                return EqualityComparer<V>.Default.Equals(value, item.Value);
            }
        }

		/// <summary>
        /// Copies the elements of the LRUCache to an array, starting at a particular 
        /// array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of
        /// items copied from the LRUCache.</param>
        /// <param name="arrayIndex">The index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            lock (syncObj)
            {
                if (array == null)
                    throw new ArgumentNullException("array");

                if (arrayIndex < 0 || arrayIndex > array.Length)
                    throw new ArgumentOutOfRangeException("arrayIndex");

                if ((array.Length - arrayIndex) < dict.Count)
                    throw new ArgumentException("Destination array is not large enough. Check array.Length and arrayIndex.");

                foreach (var item in dict)
                    array[arrayIndex++] = item;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the LRUCache is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

	    /// <summary>
        /// Remove the specified item from the LRUCache.
        /// </summary>
        /// <param name="item">The item to remove from the LRUCache.</param>
        /// <returns>true if the item was successfully removed from the LRUCache,
        /// otherwise false. This method also returns false if the item was not
        /// found in the LRUCache.</returns>
        public bool Remove(KeyValuePair<K, V> item)
        {
            lock (syncObj)
            {
                if (!dict.Contains(item))
                    return false;

                return this.Remove(item.Key);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the items in the LRUCache.
        /// </summary>
        /// <returns>An IEnumerator object that may be used to iterate through the 
        /// LRUCache./></returns>
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            LinkedListNode<K> node = queue.First;
            while (node != null)
            {
                yield return new KeyValuePair<K, V>(node.Value, dict[node.Value]);
                node = node.Next;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the items in the LRUCache.
        /// </summary>
        /// <returns>An IEnumerator object that may be used to iterate through the 
        /// LRUCache./></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)dict.GetEnumerator();
        }
    }
}