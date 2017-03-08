using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;
using Sprache;
using static Sprache.Parse;
using static System.Linq.Enumerable;
using static System.StringComparer;
using static System.UriKind;

namespace Tiger.Challenge
{
    /// <summary>
    /// Parses the challenge following the "Bearer" auth. scheme
    /// in a WWW-Authenticate header.
    /// </summary>
    [PublicAPI]
    public sealed partial class BearerChallenge
    {
        const string RealmKey = "realm";
        const string ScopeKey = "scope";
        const string ErrorKey = "error";
        const string ErrorDescriptionKey = "error_description";
        const string ErrorUriKey = "error_uri";

        static readonly ImmutableArray<string> _knownKeys =
            ImmutableArray.Create(RealmKey, ScopeKey, ErrorKey, ErrorDescriptionKey, ErrorUriKey);

        [NotNull]
        static Parser<char> HTab => Char('\t').Named("HTAB");

        [NotNull]
        static Parser<char> Sp => Char(' ').Named("SP");

        [NotNull]
        static Parser<char> VChar =>
            Char(c => c >= 0x21 && c <= 0x7E, "visible (printing) characters").Named("VCHAR");

        static Parser<char> ObsText =>
            Char(c => c >= 0x80 && c <= 0xFF, "observable text").Named("obs-text");

        [NotNull]
        static Parser<char> QdText =>
            HTab.XOr(Sp)
                .XOr(Char('!'))
                .XOr(Char(c => c >= 0x23 && c <= 0x5B, "%x23-5B"))
                .XOr(Char(c => c >= 0x5D && c <= 0x7E, "%x5D-7E"))
                .XOr(ObsText)
                .Named("QDTEXT");

        [NotNull]
        static Parser<char> DQuote => Char('"').Named("DQUOTE");

        [NotNull]
        static Parser<char> QuotedPair =>
            from escape in Char('\\')
            from next in HTab.XOr(Sp).XOr(VChar).XOr(ObsText)
            select next;

        [NotNull]
        static Parser<string> QuotedString =>
            QdText
                .XOr(QuotedPair)
                .XMany()
                .Text()
                .Contained(DQuote, DQuote)
                .Named("quoted-string");

        [NotNull]
        static Parser<string> Token =>
            LetterOrDigit
                .XOr(Chars(@"!#$%&'*+-.^_`|~"))
                .XAtLeastOnce()
                .Text()
                .Named("token");

        [NotNull]
        static Parser<string> ScopeToken =>
            Char('!')
                .XOr(Char(c => c >= 0x23 && c <= 0x5B, "%x23-5B"))
                .XOr(Char(c => c >= 0x5D && c <= 0x7E, "%x5D-7E"))
                .XAtLeastOnce()
                .Text()
                .Named("scope-token");

        [NotNull]
        static Parser<KeyValuePair<string, string>> AuthParam =>
            from key in Token
            from equalsSign in Char('=').Token()
            from value in Token.XOr(QuotedString)
            select new KeyValuePair<string, string>(key, value);

        [NotNull]
        static Parser<IImmutableDictionary<string, string>> AuthParams =>
            AuthParam.XDelimitedBy(Char(',').Token())
                .XOr(Return(Empty<KeyValuePair<string, string>>()))
                .End()
                .WithoutDuplicates(new ChallengeKeyComparer<string>(OrdinalIgnoreCase))
                .Select(aps => aps.ToImmutableDictionary(OrdinalIgnoreCase));

        [NotNull]
        static Parser<IImmutableList<string>> Scopes =>
            ScopeToken.XDelimitedBy(Sp.XAtLeastOnce())
                .XOr(Return(Empty<string>()))
                .End()
                .WithoutDuplicates(OrdinalIgnoreCase)
                .Select(ImmutableList.CreateRange);

        /// <summary>Parses a string to produce a <see cref="BearerChallenge"/>.</summary>
        /// <param name="challengeParameter">
        /// The challenge from a WWW-Authenticate header, following the "Bearer" auth. scheme.
        /// </param>
        /// <returns>A <see cref="BearerChallenge"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="challengeParameter"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">The challenge could not be parsed.</exception>
        /// <exception cref="FormatException">The scope could not be parsed.</exception>
        /// <exception cref="FormatException">The error URI could not be parsed.</exception>
        [NotNull, Pure]
        public static BearerChallenge Parse([NotNull] string challengeParameter)
        {
            if (challengeParameter == null) { throw new ArgumentNullException(nameof(challengeParameter)); }

            IImmutableDictionary<string, string> authParams;
            try
            {
                authParams = AuthParams.Parse(challengeParameter);
            }
            catch (ParseException pe)
            {
                throw new FormatException("Input string was not in a correct format.", pe);
            }

            var realm = authParams.GetValueOrDefault(RealmKey);
            IImmutableList<string> scope;
            try
            {
                scope = Scopes.Parse(authParams.GetValueOrDefault(ScopeKey, string.Empty));
            }
            catch (ParseException pe)
            {
                throw new FormatException("Input string was not in a correct format.", pe);
            }

            var error = authParams.GetValueOrDefault(ErrorKey);
            var errorDescription = authParams.GetValueOrDefault(ErrorDescriptionKey);

            Uri errorUri = null;
            var rawErrorUri = authParams.GetValueOrDefault(ErrorUriKey);
            if (rawErrorUri != null)
            {
                try
                { // ReSharper disable once ExceptionNotDocumentedOptional
                    errorUri = new Uri(rawErrorUri, RelativeOrAbsolute);
                }
                catch (UriFormatException ufe)
                {
                    throw new FormatException(@"Input string was not in a correct format.", ufe);
                }
            }

            var extensions = authParams.RemoveRange(_knownKeys);

            return new BearerChallenge(realm, scope, error, errorDescription, errorUri, extensions);
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
        /// <exception cref="ArgumentNullException"><paramref name="challengeParameter"/> is <see langword="null"/>.</exception>
        [ContractAnnotation("=>true,result:notnull; =>false,result:null"), Pure]
        public static bool TryParse(
            [NotNull] string challengeParameter,
            out BearerChallenge result)
        {
            if (challengeParameter == null) { throw new ArgumentNullException(nameof(challengeParameter)); }

            var authParams = AuthParams.TryParse(challengeParameter);
            if (!authParams.WasSuccessful)
            {
                result = default(BearerChallenge);
                return false;
            }

            var realm = authParams.Value.GetValueOrDefault(RealmKey);
            var scope = Scopes.TryParse(authParams.Value.GetValueOrDefault(ScopeKey, string.Empty));
            if (!scope.WasSuccessful)
            {
                result = default(BearerChallenge);
                return false;
            }

            var error = authParams.Value.GetValueOrDefault(ErrorKey);
            var errorDescription = authParams.Value.GetValueOrDefault(ErrorDescriptionKey);

            Uri errorUri = null;
            var rawErrorUri = authParams.Value.GetValueOrDefault(ErrorUriKey);
            if (rawErrorUri != null)
            {
                if (!Uri.TryCreate(authParams.Value.GetValueOrDefault(ErrorUriKey), RelativeOrAbsolute, out errorUri))
                {
                    result = default(BearerChallenge);
                    return false;
                }
            }

            var extensions = authParams.Value.RemoveRange(_knownKeys);

            result = new BearerChallenge(realm, scope.Value, error, errorDescription, errorUri, extensions);
            return true;
        }
    }
}
