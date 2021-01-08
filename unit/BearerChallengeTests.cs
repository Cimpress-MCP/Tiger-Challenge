// <copyright file="BearerChallengeTests.cs" company="Cimpress, Inc.">
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
using Xunit;
using static System.StringComparer;
using static System.UriKind;

namespace Tiger.Challenge.Tests
{
    /// <summary>Tests related to <see cref="BearerChallenge"/>.</summary>
    public class BearerChallengeTests
    {
        [Fact(DisplayName = "An empty string can be parsed as a challenge.")]
        public void EmptyChallenge_Parses()
        {
            var actual = Record.Exception(() => BearerChallenge.Parse(string.Empty));

            Assert.Null(actual);
        }

        [Fact(DisplayName = "An empty string can be parsed as a challenge.")]
        public void EmptyChallenge_Parses_Try()
        {
            var success = BearerChallenge.TryParse(string.Empty, out var actual);

            Assert.True(success);
            Assert.NotNull(actual);
        }

        [Fact(DisplayName = "An empty string produces a default challenge.")]
        public void EmptyChallenge_NoResults()
        {
            var actual = BearerChallenge.Parse(string.Empty);

            Assert.NotNull(actual);
            Assert.Null(actual.Realm);
            Assert.Empty(actual.Scope);
            Assert.Null(actual.Error);
            Assert.Null(actual.ErrorDescription);
            Assert.Null(actual.ErrorUri);
            Assert.NotNull(actual.Extensions);
            Assert.Empty(actual.Extensions);
        }

        [Fact(DisplayName = "An empty string produces a default challenge.")]
        public void EmptyChallenge_NoResults_Try()
        {
            var success = BearerChallenge.TryParse(string.Empty, out var actual);

            Assert.True(success);
            Assert.NotNull(actual);
            Assert.Null(actual.Realm);
            Assert.Empty(actual.Scope);
            Assert.Null(actual.Error);
            Assert.Null(actual.ErrorDescription);
            Assert.Null(actual.ErrorUri);
            Assert.NotNull(actual.Extensions);
            Assert.Empty(actual.Extensions);
        }

        [Fact(DisplayName = "A challenge with an empty scope otherwise parses.")]
        public void EmptyScope_Parses()
        {
            const string Challenge = @"realm=""cimpress.auth0.com"", scope="""", error=""invalid_token"", error_description=""The token is expired""";

            var actual = Record.Exception(() => BearerChallenge.Parse(Challenge));

            Assert.Null(actual);
        }

        [Fact(DisplayName = "A challenge with an empty scope otherwise parses.")]
        public void EmptyScope_Parses_Try()
        {
            const string Challenge = @"realm=""cimpress.auth0.com"", scope="""", error=""invalid_token"", error_description=""The token is expired""";

            var success = BearerChallenge.TryParse(Challenge, out var actual);

            Assert.True(success);
            Assert.NotNull(actual);
        }

        [Fact(DisplayName = "A challenge with an empty scope produces empty scope data.")]
        public void EmptyScope_Results_EmptyScope()
        {
            const string Realm = "cimpress.auth0.com";
            const string Error = "invalid_token";
            const string ErrorDescription = "The token is expired";
            var challenge = $@"realm=""{Realm}"", scope="""", error=""{Error}"", error_description=""{ErrorDescription}""";

            var actual = BearerChallenge.Parse(challenge);

            Assert.NotNull(actual);
            Assert.Equal(Realm, actual.Realm, Ordinal);
            Assert.Empty(actual.Scope);
            Assert.Equal(Error, actual.Error, Ordinal);
            Assert.Equal(ErrorDescription, actual.ErrorDescription, Ordinal);
            Assert.Null(actual.ErrorUri);
            Assert.NotNull(actual.Extensions);
            Assert.Empty(actual.Extensions);
        }

        [Fact(DisplayName = "A challenge with an empty scope produces empty scope data.")]
        public void EmptyScope_Results_EmptyScope_Try()
        {
            const string Realm = "cimpress.auth0.com";
            const string Error = "invalid_token";
            const string ErrorDescription = "The token is expired";
            var challenge = $@"realm=""{Realm}"", scope="""", error=""{Error}"", error_description=""{ErrorDescription}""";

            var success = BearerChallenge.TryParse(challenge, out var actual);

            Assert.True(success);
            Assert.NotNull(actual);
            Assert.Equal(Realm, actual.Realm, Ordinal);
            Assert.Empty(actual.Scope);
            Assert.Equal(Error, actual.Error, Ordinal);
            Assert.Equal(ErrorDescription, actual.ErrorDescription, Ordinal);
            Assert.Null(actual.ErrorUri);
            Assert.NotNull(actual.Extensions);
            Assert.Empty(actual.Extensions);
        }

        [Fact(DisplayName = "A challenge with no scope otherwise parses.")]
        public void ThreeSixtyNoScope_Parses()
        {
            const string Challenge = @"realm=""cimpress.auth0.com"", error=""invalid_token"", error_description=""The token is expired""";

            var actual = Record.Exception(() => BearerChallenge.Parse(Challenge));

            Assert.Null(actual);
        }

        [Fact(DisplayName = "A challenge with no scope otherwise parses.")]
        public void ThreeSixtyNoScope_Parses_Try()
        {
            const string Challenge = @"realm=""cimpress.auth0.com"", error=""invalid_token"", error_description=""The token is expired""";

            var success = BearerChallenge.TryParse(Challenge, out var _);

            Assert.True(success);
        }

        [Fact(DisplayName = "A challenge with no scope produces empty scope data.")]
        public void ThreeSixtyNoScope_Results_EmptyScope()
        {
            const string Realm = "cimpress.auth0.com";
            const string Error = "invalid_token";
            const string ErrorDescription = "The token is expired";
            var challenge = $@"realm=""{Realm}"", error=""{Error}"", error_description=""{ErrorDescription}""";

            var actual = BearerChallenge.Parse(challenge);

            Assert.NotNull(actual);
            Assert.Equal(Realm, actual.Realm, Ordinal);
            Assert.Empty(actual.Scope);
            Assert.Equal(Error, actual.Error, Ordinal);
            Assert.Equal(ErrorDescription, actual.ErrorDescription, Ordinal);
            Assert.Null(actual.ErrorUri);
            Assert.NotNull(actual.Extensions);
            Assert.Empty(actual.Extensions);
        }

        [Fact(DisplayName = "A challenge with no scope produces empty scope data.")]
        public void ThreeSixtyNoScope_Results_EmptyScope_Try()
        {
            const string Realm = "cimpress.auth0.com";
            const string Error = "invalid_token";
            const string ErrorDescription = "The token is expired";
            var challenge = $@"realm=""{Realm}"", error=""{Error}"", error_description=""{ErrorDescription}""";

            var success = BearerChallenge.TryParse(challenge, out var actual);

            Assert.True(success);
            Assert.NotNull(actual);
            Assert.Equal(Realm, actual.Realm, Ordinal);
            Assert.Empty(actual.Scope);
            Assert.Equal(Error, actual.Error, Ordinal);
            Assert.Equal(ErrorDescription, actual.ErrorDescription, Ordinal);
            Assert.Null(actual.ErrorUri);
            Assert.NotNull(actual.Extensions);
            Assert.Empty(actual.Extensions);
        }

        [Fact(DisplayName = "A typical challenge parses.")]
        public void TypicalChallenge_Parses()
        {
            const string Realm = "cimpress.auth0.com";
            var scope = new[] { "openid" };
            const string Error = "invalid_token";
            const string ErrorDescription = "The token is expired";
            var authorizationUri = new Uri("https://cimpress.auth0.com/oauth/token", Absolute);
            var challenge = $@"realm=""{Realm}"", authorization_uri=""{authorizationUri}"", scope=""{string.Join(" ", scope)}"", error=""{Error}"", error_description=""{ErrorDescription}""";

            var actual = Record.Exception(() => BearerChallenge.Parse(challenge));

            Assert.Null(actual);
        }

        [Fact(DisplayName = "A typical challenge parses.")]
        public void TypicalChallenge_Parses_Try()
        {
            const string Realm = "cimpress.auth0.com";
            var scope = new[] { "openid" };
            const string Error = "invalid_token";
            const string ErrorDescription = "The token is expired";
            var authorizationUri = new Uri("https://cimpress.auth0.com/oauth/token", Absolute);
            var challenge = $@"realm=""{Realm}"", authorization_uri=""{authorizationUri}"", scope=""{string.Join(" ", scope)}"", error=""{Error}"", error_description=""{ErrorDescription}""";

            var success = BearerChallenge.TryParse(challenge, out var actual);

            Assert.True(success);
            Assert.NotNull(actual);
        }

        [Fact(DisplayName = "A typical challenge produces the correct data.")]
        public void TypicalChallenge_Results()
        {
            const string Realm = "cimpress.auth0.com";
            var scope = new[] { "openid", "profile" };
            const string Error = "invalid_token";
            const string ErrorDescription = "The token is expired";
            var authorizationUri = new Uri("https://cimpress.auth0.com/oauth/token", Absolute);
            var challenge = $@"realm=""{Realm}"", authorization_uri=""{authorizationUri}"", scope=""{string.Join(" ", scope)}"", error=""{Error}"", error_description=""{ErrorDescription}""";

            var actual = BearerChallenge.Parse(challenge);

            Assert.NotNull(actual);
            Assert.Equal(Realm, actual.Realm, Ordinal);
            Assert.Equal(scope, actual.Scope, Ordinal);
            Assert.Equal(Error, actual.Error, Ordinal);
            Assert.Equal(ErrorDescription, actual.ErrorDescription, Ordinal);
            Assert.Null(actual.ErrorUri);
            Assert.NotNull(actual.Extensions);
            var value = Assert.Contains("authorization_uri", actual.Extensions);
            Assert.Equal(authorizationUri.AbsoluteUri, value, Ordinal);
        }

        [Fact(DisplayName = "A typical challenge produces the correct data.")]
        public void TypicalChallenge_Results_Try()
        {
            const string Realm = "cimpress.auth0.com";
            var scope = new[] { "openid", "profile" };
            const string Error = "invalid_token";
            const string ErrorDescription = "The token is expired";
            var authorizationUri = new Uri("https://cimpress.auth0.com/oauth/token", Absolute);
            var challenge = $@"realm=""{Realm}"", authorization_uri=""{authorizationUri}"", scope=""{string.Join(" ", scope)}"", error=""{Error}"", error_description=""{ErrorDescription}""";

            var success = BearerChallenge.TryParse(challenge, out var actual);

            Assert.True(success);
            Assert.NotNull(actual);
            Assert.Equal(Realm, actual.Realm, Ordinal);
            Assert.Equal(scope, actual.Scope, Ordinal);
            Assert.Equal(Error, actual.Error, Ordinal);
            Assert.Equal(ErrorDescription, actual.ErrorDescription, Ordinal);
            Assert.Null(actual.ErrorUri);
            Assert.NotNull(actual.Extensions);
            var value = Assert.Contains("authorization_uri", actual.Extensions);
            Assert.Equal(authorizationUri.AbsoluteUri, value, Ordinal);
        }

        [Fact(DisplayName = "A challenge is valid when converted to a string.")]
        public void ToString_Valid()
        {
            const string Challenge = @"realm=""cimpress.auth0.com"", scope=""client_id=0NBlSon1C4tJEVWS6GJRjwju5NxKHfBF service=https://orr.fen.cimpress.io"", error=""invalid_token"", error_description=""The token is expired""";

            var actual = Record.Exception(() => BearerChallenge.Parse(BearerChallenge.Parse(Challenge).ToString()));

            Assert.Null(actual);
        }

        [Fact(DisplayName = "Bad quoting causes a parsing failure.")]
        public void BadQuotes_Failure()
        {
            const string Challenge = @"realm=""cimpress.auth0.com, scope=""client_id=0NBlSon1C4tJEVWS6GJRjwju5NxKHfBF service=https://orr.fen.cimpress.io"", error=""invalid_token"", error_description=""The token is expired""";

            var actual = Record.Exception(() => BearerChallenge.Parse(Challenge));

            Assert.NotNull(actual);
            var formatException = Assert.IsType<FormatException>(actual);
            Assert.Contains("correct format", formatException.Message, StringComparison.Ordinal);
        }

        [Fact(DisplayName = "Bad quoting causes a parsing failure.")]
        public void BadQuotes_Failure_Try()
        {
            const string Challenge = @"realm=""cimpress.auth0.com, scope=""client_id=0NBlSon1C4tJEVWS6GJRjwju5NxKHfBF service=https://orr.fen.cimpress.io"", error=""invalid_token"", error_description=""The token is expired""";

            var success = BearerChallenge.TryParse(Challenge, out var actual);

            Assert.False(success);
            Assert.Null(actual);
        }

        [Fact(DisplayName = "A duplicate key causes a parsing failure.")]
        public void DuplicateKey_Failure()
        {
            const string Challenge = @"realm=""cimpress.auth0.com"", realm=""cimpress.example.com"", scope=""client_id=0NBlSon1C4tJEVWS6GJRjwju5NxKHfBF service=https://orr.fen.cimpress.io"", error=""invalid_token"", error_description=""The token is expired""";

            var actual = Record.Exception(() => BearerChallenge.Parse(Challenge));

            Assert.NotNull(actual);
            var formatException = Assert.IsType<FormatException>(actual);
            Assert.Contains("correct format", formatException.Message, StringComparison.Ordinal);
        }

        [Fact(DisplayName = "A duplicate key causes a parsing failure.")]
        public void DuplicateKey_Failure_Try()
        {
            const string Challenge = @"realm=""cimpress.auth0.com"", realm=""cimpress.example.com"", scope=""client_id=0NBlSon1C4tJEVWS6GJRjwju5NxKHfBF service=https://orr.fen.cimpress.io"", error=""invalid_token"", error_description=""The token is expired""";

            var success = BearerChallenge.TryParse(Challenge, out var actual);

            Assert.False(success);
            Assert.Null(actual);
        }
    }
}

