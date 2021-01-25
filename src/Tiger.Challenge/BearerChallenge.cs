// <copyright file="BearerChallenge.cs" company="Cimpress, Inc.">
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
using System.Collections.Immutable;
using System.Linq;
using static System.StringComparer;

namespace Tiger.Challenge
{
    /// <summary>
    /// Represents the challenge following the "Bearer" auth. scheme
    /// in a WWW-Authenticate header.
    /// </summary>
    public sealed partial class BearerChallenge
    {
        BearerChallenge(
            string? realm,
            ImmutableArray<string> scope,
            string? error,
            string? errorDescription,
            Uri? errorUri,
            ImmutableDictionary<string, string> extensions)
        {
            Realm = realm;
            Scope = scope;
            Error = error;
            ErrorDescription = errorDescription;
            ErrorUri = errorUri;
            Extensions = extensions;
        }

        /// <summary>Gets the name of the scope of authenticated protection.</summary>
        /// <remarks><para>Corresponds to the key <c>realm</c>.</para></remarks>
        public string? Realm { get; }

        /// <summary>
        /// Gets a collection of space-delimited values in the "scope" attribute
        /// of the challenge.
        /// </summary>
        /// <remarks>Corresponds to the key <c>scope</c>.</remarks>
        public ImmutableArray<string> Scope { get; }

        /// <summary>Gets the title of the authentication error.</summary>
        /// <remarks>Corresponds to the key <c>error</c>.</remarks>
        public string? Error { get; }

        /// <summary>Gets the description of the authentication error.</summary>
        /// <remarks>Corresponds to the key <c>error_description</c>.</remarks>
        public string? ErrorDescription { get; }

        /// <summary>Gets a URI corresponding to a description of the authentication error.</summary>
        /// <remarks>Corresponds to the key <c>error_uri</c>.</remarks>
        public Uri? ErrorUri { get; }

        /// <summary>
        /// Gets a mapping of keys to values for auth. parameters not explicitly defined
        /// by the Bearer challenge specification.
        /// </summary>
        public IImmutableDictionary<string, string> Extensions { get; }
    }

    /// <content>Overrides and override-equivalents.</content>
    public sealed partial class BearerChallenge
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            var output = new List<string>(8);
            if (Realm is not null)
            {
                output.Add($@"{RealmKey}=""{Realm}""");
            }

            if (Scope.Length != 0)
            {
                output.Add($@"{ScopeKey}=""{string.Join(" ", Scope)}""");
            }

            if (Error is not null)
            {
                output.Add($@"{ErrorKey}=""{Error}""");
            }

            if (ErrorDescription is not null)
            {
                output.Add($@"{ErrorDescriptionKey}=""{ErrorDescription}""");
            }

            if (ErrorUri is not null)
            {
                output.Add($@"{ErrorUriKey}=""{ErrorUri}""");
            }

            if (Extensions.Count != 0)
            {
                output.AddRange(Extensions.Select(kvp => $@"{kvp.Key}=""{kvp.Value}"""));
            }

            return string.Join(", ", output);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hash = default(HashCode);
            hash.Add(Realm, Ordinal);
            hash.Add(Scope);
            hash.Add(Error, Ordinal);
            hash.Add(ErrorDescription, Ordinal);
            hash.Add(ErrorUri);
            hash.Add(Extensions);
            return hash.ToHashCode();
        }
    }
}
