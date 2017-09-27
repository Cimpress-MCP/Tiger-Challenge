// <copyright file="ParserExtensions.cs" company="Cimpress, Inc.">
//   Copyright 2017 Cimpress, Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
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
using System.Linq;
using JetBrains.Annotations;
using Sprache;
using static JetBrains.Annotations.ImplicitUseTargetFlags;
using static Sprache.Result;
using static System.Linq.Enumerable;

namespace Tiger.Challenge
{
    /// <summary>Extensions to the functionality of <see cref="Parser{T}"/>.</summary>
    [UsedImplicitly(Members)]
    static class ParserExtensions
    {
        /// <summary>
        /// Specifies that a parse result is not to contain duplicates,
        /// as specified by the default equality comparer.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of the items in the collection produced by <paramref name="parser"/>.
        /// </typeparam>
        /// <param name="parser">A parser.</param>
        /// <returns>A parser that fails on duplicates.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parser"/> is <see langword="null"/>.</exception>
        [NotNull]
        public static Parser<IEnumerable<TItem>> WithoutDuplicates<TItem>(
            [NotNull] this Parser<IEnumerable<TItem>> parser)
        {
            if (parser == null) { throw new ArgumentNullException(nameof(parser)); }

            return parser.WithoutDuplicates(EqualityComparer<TItem>.Default);
        }

        /// <summary>
        /// Specifies that a parse result is not to contain duplicates,
        /// as specified by the provided equality comparer.
        /// </summary>
        /// <typeparam name="TItem">e
        /// The type of the items in the collection produced by <paramref name="parser"/>.
        /// </typeparam>
        /// <param name="parser">A parser.</param>
        /// <param name="comparer">A comparer for items.</param>
        /// <returns>A parser that fails on duplicates.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parser"/> is <see langword="null"/>.</exception>
        [NotNull]
        public static Parser<IEnumerable<TItem>> WithoutDuplicates<TItem>(
            [NotNull] this Parser<IEnumerable<TItem>> parser,
            [CanBeNull] IEqualityComparer<TItem> comparer)
        {
            if (parser == null) { throw new ArgumentNullException(nameof(parser)); }

            return input =>
            {
                var result = parser(input);
                if (!result.WasSuccessful) { return result; }

                var resultValue = result.Value.ToList();
                var originalCount = resultValue.Count;
                var distinctCount = resultValue.Distinct(comparer).Count();
                return originalCount == distinctCount
                    ? result
                    : Failure<IEnumerable<TItem>>(input, "Duplicates detected", Empty<string>());
            };
        }
    }
}
