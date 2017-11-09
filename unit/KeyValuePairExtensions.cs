using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Tiger.Challenge.Tests
{
    /// <summary>Extensions to the <see cref="KeyValuePair{TKey,TValue}"/> struct.</summary>
    static class KeyValuePairExtensions
    {
        /// <summary>Deconstructs a <see cref="KeyValuePair{TKey,TValue}"/> into its key and value.</summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="pair">The key-value pair to deconstruct.</param>
        /// <param name="key">
        /// When this method returns, contains the value of
        /// <see cref="KeyValuePair{TKey,TValue}.Key"/> for <paramref name="pair"/>.
        /// </param>
        /// <param name="value">
        /// When this method returns, contains the value of
        /// <see cref="KeyValuePair{TKey,TValue}.Value"/> for <paramref name="pair"/>.
        /// </param>
        public static void Deconstruct<TKey, TValue>(
            this KeyValuePair<TKey, TValue> pair,
            out TKey key,
            out TValue value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}
