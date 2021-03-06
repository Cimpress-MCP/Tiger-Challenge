// <copyright file="ChallengeKeyComparer.cs" company="Cimpress, Inc.">
//   Copyright 2020 Cimpress, Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License") –
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;

namespace Tiger.Challenge
{
    /// <summary>Compares the keys of challenge key–value pairs for equality.</summary>
    /// <typeparam name="TValue">The type of the value of the <see cref="KeyValuePair{TKey,TValue}"/>.</typeparam>
    sealed class ChallengeKeyComparer<TValue>
        : IEqualityComparer<KeyValuePair<string, TValue>>
    {
        readonly IEqualityComparer<string> _keyComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeKeyComparer{TValue}"/> class.
        /// </summary>
        public ChallengeKeyComparer()
            : this(EqualityComparer<string>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeKeyComparer{TValue}"/> class.
        /// </summary>
        /// <param name="keyComparer">The equality comparer to use in comparing keys.</param>
        public ChallengeKeyComparer(IEqualityComparer<string> keyComparer)
        {
            _keyComparer = keyComparer;
        }

        /// <inheritdoc />
        public bool Equals(KeyValuePair<string, TValue> x, KeyValuePair<string, TValue> y) =>
            _keyComparer.Equals(x.Key, y.Key);

        /// <inheritdoc />
        public int GetHashCode(KeyValuePair<string, TValue> obj)
        {
            var hash = default(HashCode);
            hash.Add(obj.Key, _keyComparer);
            return hash.ToHashCode();
        }
    }
}
