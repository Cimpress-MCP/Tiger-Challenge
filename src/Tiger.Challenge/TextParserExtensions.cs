// <copyright file="TextParserExtensions.cs" company="Cimpress, Inc.">
//   Copyright 2020–2022 Cimpress, Inc.
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

namespace Superpower.Model;

/// <summary>Extensions to the functionality of <see cref="TextParser{T}"/>.</summary>
static class TextParserExtensions
{
    /// <summary>
    /// Specifies that a parse result is not to contain duplicates,
    /// as specified by the provided equality comparer.
    /// </summary>
    /// <typeparam name="TItem">The type of the items in the collection produced by <paramref name="parser"/>.</typeparam>
    /// <param name="parser">A parser.</param>
    /// <param name="comparer">A comparer for items.</param>
    /// <returns>A parser that fails on duplicates.</returns>
    public static TextParser<TItem[]> WithoutDuplicates<TItem>(
        this TextParser<TItem[]> parser,
        IEqualityComparer<TItem>? comparer = null) => WithoutDuplicatesBy(parser, static t => t, comparer);

    /// <summary>
    /// Specifies that a parse result is not to contain duplicates,
    /// as specified by the provided key selector and equality comparer.
    /// </summary>
    /// <typeparam name="TItem">The type of the items in the collection produced by <paramref name="parser"/>.</typeparam>
    /// <typeparam name="TKey">
    /// The type of the key by which elements of the collection produced by <paramref name="parser"/> will be distinguished.
    /// </typeparam>
    /// <param name="parser">A parser.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">An optional comparer for items.</param>
    /// <returns>A parser that fails on duplicates.</returns>
    public static TextParser<TItem[]> WithoutDuplicatesBy<TItem, TKey>(
        this TextParser<TItem[]> parser,
        Func<TItem, TKey> keySelector,
        IEqualityComparer<TKey>? comparer = null) => input =>
        {
            var result = parser(input);
            if (!result.HasValue)
            {
                return result;
            }

            var resultValue = result.Value.ToList();
            var originalCount = resultValue.Count;
            var distinctCount = resultValue.DistinctBy(keySelector, comparer).Count();
            return originalCount == distinctCount
                ? result
                : Result.Empty<TItem[]>(input, "Duplicates detected");
        };

    /// <summary>Constructs a parser that combines the results of two consecutive parsers.</summary>
    /// <typeparam name="TFirst">The result type of the first parser.</typeparam>
    /// <typeparam name="TSecond">The result type of the second parser.</typeparam>
    /// <typeparam name="TResult">The result type of the result parser.</typeparam>
    /// <param name="first">The parser to apply first.</param>
    /// <param name="second">The parser to apply second.</param>
    /// <param name="combiner">A function which combines the results of the provided parsers.</param>
    /// <returns>A parser which is the combination of the provided parsers.</returns>
    public static TextParser<TResult> Apply<TFirst, TSecond, TResult>(
        this TextParser<TFirst> first,
        TextParser<TSecond> second,
        Func<TFirst, TSecond, TResult> combiner) => input => first.Invoke(input) switch
        {
            { HasValue: true, Value: var v1, Remainder: var rm1 } r1 => second(rm1) switch
            {
                { HasValue: true, Value: var v2, Remainder: var rm2 } r2 => Result.Value(combiner(v1, v2), input, rm2),
                var r2 => Result.CastEmpty<TSecond, TResult>(r2),
            },
            var r1 => Result.CastEmpty<TFirst, TResult>(r1),
        };

    /// <summary>Parses optional whitespace before and after the provided parser.</summary>
    /// <typeparam name="T">The result type of the provided parser.</typeparam>
    /// <param name="parser">The parser to surround.</param>
    /// <returns>The resulting parser.</returns>
    public static TextParser<T> Token<T>(this TextParser<T> parser) => parser
        .Between(Character.WhiteSpace.IgnoreMany(), Character.WhiteSpace.IgnoreMany());
}
