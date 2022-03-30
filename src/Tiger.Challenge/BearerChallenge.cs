// <copyright file="BearerChallenge.cs" company="Cimpress, Inc.">
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

using Microsoft.VisualBasic;

namespace Tiger.Challenge;

/// <summary>Represents the challenge following the "Bearer" auth. scheme in a WWW-Authenticate header.</summary>
/// <param name="Scope">A collection of space-delimited values from the "scope" attribute of the challenge.</param>
/// <param name="Extensions">
/// A mapping of keys to values for authn. parameters not explicitly defined
/// by the Bearer challenge specification.
/// </param>
public sealed partial record class BearerChallenge(StringSet Scope, StringMap Extensions)
{
    /// <summary>Gets the name of the scope of authenticated protection.</summary>
    /// <remarks><para>Corresponds to the key <c>realm</c>.</para></remarks>
    public string? Realm { get; init; }

    /// <summary>Gets the title of the authentication error.</summary>
    /// <remarks>Corresponds to the key <c>error</c>.</remarks>
    public string? Error { get; init; }

    /// <summary>Gets the description of the authentication error.</summary>
    /// <remarks>Corresponds to the key <c>error_description</c>.</remarks>
    public string? ErrorDescription { get; init; }

    /// <summary>Gets a URI corresponding to a description of the authentication error.</summary>
    /// <remarks>Corresponds to the key <c>error_uri</c>.</remarks>
    public Uri? ErrorUri { get; init; }

    /// <inheritdoc/>
    public override string ToString()
    {
        var output = new List<string>(8);
        if (Realm is { } r)
        {
            output.Add($@"{RealmKey}=""{r}""");
        }

        if (Scope is { Count: > 0 } s)
        {
            output.Add($@"{ScopeKey}=""{string.Join(" ", s)}""");
        }

        if (Error is { } e)
        {
            output.Add($@"{ErrorKey}=""{e}""");
        }

        if (ErrorDescription is { } ed)
        {
            output.Add($@"{ErrorDescriptionKey}=""{ed}""");
        }

        if (ErrorUri is { } eu)
        {
            output.Add($@"{ErrorUriKey}=""{eu}""");
        }

        if (Extensions is { Count: >= 0 } ee)
        {
            output.AddRange(ee.Select(kvp => $@"{kvp.Key}=""{kvp.Value}"""));
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
