// <copyright file="BearerChallenge.Parse.cs" company="Cimpress, Inc.">
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

namespace Tiger.Challenge;

/// <content>The parsing functionality of a bearer challenge.</content>
public sealed partial record class BearerChallenge
{
    const string RealmKey = "realm";
    const string ScopeKey = "scope";
    const string ErrorKey = "error";
    const string ErrorDescriptionKey = "error_description";
    const string ErrorUriKey = "error_uri";

    static readonly ChallengeKeyComparer<string> s_comparer = new(OrdinalIgnoreCase);

    static readonly StringSet s_knownKeys = Set.Create(RealmKey, ScopeKey, ErrorKey, ErrorDescriptionKey, ErrorUriKey);

    static readonly Parser<char> s_hTab = Char('\t').Named("HTAB");

    static readonly Parser<char> s_sp = Char(' ').Named("SP");

    static readonly Parser<char> s_vChar =
        Char(c => c is >= (char)0x21 and <= (char)0x7E, "visible (printing) characters").Named("VCHAR");

    static readonly Parser<char> s_obsText =
        Char(c => c is >= (char)0x80 and <= (char)0xFF, "observable text").Named("obs-text");

    static readonly Parser<char> s_qdText = s_hTab
        .XOr(s_sp)
        .XOr(Char('!'))
        .XOr(Char(c => c is >= (char)0x23 and <= (char)0x5B, "%x23-5B"))
        .XOr(Char(c => c is >= (char)0x5D and <= (char)0x7E, "%x5D-7E"))
        .XOr(s_obsText)
        .Named("QDTEXT");

    static readonly Parser<char> s_dQuote = Char('"').Named("DQUOTE");

    static readonly Parser<char> s_quotedPair =
        (from slash in Char('\\')
         from next in s_hTab.XOr(s_sp).XOr(s_vChar).XOr(s_obsText)
         select next).Named("quoted pair");

    static readonly Parser<string> s_quotedString = s_qdText
        .XOr(s_quotedPair)
        .XMany()
        .Text()
        .Contained(s_dQuote, s_dQuote)
        .Named("quoted-string");

    static readonly Parser<string> s_token = LetterOrDigit
        .XOr(Chars("!#$%&'*+-.^_`|~"))
        .XAtLeastOnce()
        .Text()
        .Named("token");

    static readonly Parser<string> s_scopeToken = Char('!')
        .XOr(Char(c => c is >= (char)0x23 and <= (char)0x5B, "%x23-5B"))
        .XOr(Char(c => c is >= (char)0x5D and <= (char)0x7E, "%x5D-7E"))
        .XAtLeastOnce()
        .Text()
        .Named("scope-token");

    static readonly Parser<StringPair> s_authParam =
        from key in s_token
        from equalsSign in Char('=').Token()
        from value in s_token.XOr(s_quotedString)
        select new StringPair(key, value);

    static readonly Parser<StringMap> s_authParams =
        s_authParam.XDelimitedBy(Char(',').Token())
            .XOr(Return(Empty<StringPair>()))
            .End()
            .WithoutDuplicates(s_comparer)
            .Select(aps => aps.ToImmutableDictionary(OrdinalIgnoreCase));

    static readonly Parser<StringSet> s_scopes =
        s_scopeToken.XDelimitedBy(s_sp.XAtLeastOnce())
            .XOr(Return(Empty<string>()))
            .End()
            .WithoutDuplicates(OrdinalIgnoreCase)
            .Select(Set.CreateRange);

    /// <summary>Parses a string to produce a <see cref="BearerChallenge"/>.</summary>
    /// <param name="challengeParameter">
    /// The challenge from a WWW-Authenticate header, following the "Bearer" auth. scheme.
    /// </param>
    /// <returns>A <see cref="BearerChallenge"/>.</returns>
    /// <exception cref="FormatException">The challenge could not be parsed.</exception>
    /// <exception cref="FormatException">The scope could not be parsed.</exception>
    /// <exception cref="FormatException">The error URI could not be parsed.</exception>
    public static BearerChallenge Parse(string challengeParameter)
    {
        StringMap authParams;
        try
        {
            authParams = s_authParams.Parse(challengeParameter);
        }
        catch (ParseException pe)
        {
            throw new FormatException("Input string was not in a correct format.", pe);
        }

        var realm = authParams.GetValueOrDefault(RealmKey);
        StringSet scope;
        try
        {
            scope = s_scopes.Parse(authParams.GetValueOrDefault(ScopeKey, string.Empty));
        }
        catch (ParseException pe)
        {
            throw new FormatException("Input string was not in a correct format.", pe);
        }

        var error = authParams.GetValueOrDefault(ErrorKey);
        var errorDescription = authParams.GetValueOrDefault(ErrorDescriptionKey);

        Uri? errorUri = null;
        var rawErrorUri = authParams.GetValueOrDefault(ErrorUriKey);
        if (rawErrorUri is not null)
        {
            try
            {
                errorUri = new(rawErrorUri, RelativeOrAbsolute);
            }
            catch (UriFormatException ufe)
            {
                throw new FormatException("Input string was not in a correct format.", ufe);
            }
        }

        var extensions = authParams.RemoveRange(s_knownKeys);

        return new(scope, extensions)
        {
            Realm = realm,
            Error = error,
            ErrorDescription = errorDescription,
            ErrorUri = errorUri,
        };
    }

    /// <summary>
    /// Parses a string to produce a <see cref="BearerChallenge"/>.
    /// A return value indicated whether the conversion succeeded.
    /// </summary>
    /// <param name="challengeParameter">
    /// The challenge from a WWW-Authenticate header, following the "Bearer" auth. scheme.
    /// </param>
    /// <param name="result">
    /// When this method returns, contains the challenge value equivalent of the challenge
    /// parameter contained in <paramref name="challengeParameter"/>. The conversion fails
    /// if the <paramref name="challengeParameter"/> is <see langword="null"/> or is not
    /// of the correct format. This parameter is passed uninitialized; any value originally
    /// supplied in <paramref name="result"/> will be overwritten.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="challengeParameter"/> was converted successfully;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryParse(
        string challengeParameter,
        [NotNullWhen(true)] out BearerChallenge? result)
    {
        var authParams = s_authParams.TryParse(challengeParameter);
        if (!authParams.WasSuccessful)
        {
            result = default;
            return false;
        }

        var realm = authParams.Value.GetValueOrDefault(RealmKey);
        var scope = s_scopes.TryParse(authParams.Value.GetValueOrDefault(ScopeKey, string.Empty));
        if (!scope.WasSuccessful)
        {
            result = default;
            return false;
        }

        var error = authParams.Value.GetValueOrDefault(ErrorKey);
        var errorDescription = authParams.Value.GetValueOrDefault(ErrorDescriptionKey);

        Uri? errorUri = null;
        var rawErrorUri = authParams.Value.GetValueOrDefault(ErrorUriKey);
        if (rawErrorUri is not null && !Uri.TryCreate(authParams.Value.GetValueOrDefault(ErrorUriKey), RelativeOrAbsolute, out errorUri))
        {
            result = default;
            return false;
        }

        var extensions = authParams.Value.RemoveRange(s_knownKeys);

        result = new(scope.Value, extensions)
        {
            Realm = realm,
            Error = error,
            ErrorDescription = errorDescription,
            ErrorUri = errorUri,
        };
        return true;
    }
}
