﻿/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Concurrent;
//using System.Runtime.Caching;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core.Extensions;

namespace Thinktecture.IdentityServer.Core.Services.Default
{
    /// <summary>
    /// In-memory, time based implementation of <see cref="ICache{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultCache<T> : ICache<T>
        where T : class
    {
        //readonly MemoryCache cache;
        private readonly ConcurrentDictionary<string, object> _cache;
        readonly TimeSpan duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCache{T}"/> class.
        /// </summary>
        public DefaultCache()
            : this(Constants.DefaultCacheDuration)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCache{T}"/> class.
        /// </summary>
        /// <param name="duration">The duration to cache items.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">duration;Duration must be greater than zero</exception>
        public DefaultCache(TimeSpan duration)
        {
            if (duration <= TimeSpan.Zero) throw new ArgumentOutOfRangeException("duration", "Duration must be greater than zero");

            //cache = new MemoryCache("cache");
            this._cache = new ConcurrentDictionary<string, object>();
            this.duration = duration;
        }

        /// <summary>
        /// Gets the cached data based upon a key index.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The cached item, or <c>null</c> if no item matches the key.
        /// </returns>
        public Task<T> GetAsync(string key)
        {
            //return Task.FromResult((T)cache.Get(key));
            object t;
            if(_cache.TryGetValue(key, out t))
                return Task.FromResult((T)t);

            return null;
        }

        /// <summary>
        /// Caches the data based upon a key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Task SetAsync(string key, T item)
        {
            var expiration = UtcNow.Add(this.duration);
            //cache.Set(key, item, expiration);
            _cache.TryAdd(key, item);

            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets the UTC now.
        /// </summary>
        /// <value>
        /// The UTC now.
        /// </value>
        protected virtual DateTimeOffset UtcNow
        {
            get { return DateTimeOffsetHelper.UtcNow; }
        }
    }
}
