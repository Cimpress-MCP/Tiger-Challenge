using System;
using Xunit;
using static System.StringComparer;
using static System.UriKind;
// ReSharper disable All

namespace Tiger.Challenge.Tests
{
    /// <summary>Tests related to <see cref="BearerChallenge"/>.</summary>
    public class BearerChallengeTests
    {
        [Fact(DisplayName = "An empty string can be parsed as a challenge.")]
        public void EmptyChallenge_Parses()
        {
            // arrange
            var challenge = string.Empty;

            // act
            var actual = Record.Exception(() => BearerChallenge.Parse(challenge));

            // assert
            Assert.Null(actual);
        }

        [Fact(DisplayName = "An empty string can be parsed as a challenge.")]
        public void EmptyChallenge_Parses_Try()
        {
            // arrange
            var challenge = string.Empty;

            // act
            var success = BearerChallenge.TryParse(challenge, out var actual);

            // assert
            Assert.True(success);
            Assert.NotNull(actual);
        }

        [Fact(DisplayName = "An empty string produces a default challenge.")]
        public void EmptyChallenge_NoResults()
        {
            // arrange
            var challenge = string.Empty;

            // act
            var actual = BearerChallenge.Parse(challenge);

            // assert
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
            // arrange
            var challenge = string.Empty;

            // act
            var success = BearerChallenge.TryParse(challenge, out var actual);

            // assert
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
            // arrange
            var challenge = @"realm=""cimpress.auth0.com"", scope="""", error=""invalid_token"", error_description=""The token is expired""";

            // act
            var actual = Record.Exception(() => BearerChallenge.Parse(challenge));

            // assert
            Assert.Null(actual);
        }

        [Fact(DisplayName = "A challenge with an empty scope otherwise parses.")]
        public void EmptyScope_Parses_Try()
        {
            // arrange
            var challenge = @"realm=""cimpress.auth0.com"", scope="""", error=""invalid_token"", error_description=""The token is expired""";

            // act
            var success = BearerChallenge.TryParse(challenge, out var actual);

            // assert
            Assert.True(success);
            Assert.NotNull(actual);
        }

        [Fact(DisplayName = "A challenge with an empty scope produces empty scope data.")]
        public void EmptyScope_Results_EmptyScope()
        {
            // arrange
            var realm = "cimpress.auth0.com";
            var error = "invalid_token";
            var errorDescription = "The token is expired";
            var challenge = $@"realm=""{realm}"", scope="""", error=""{error}"", error_description=""{errorDescription}""";

            // act
            var actual = BearerChallenge.Parse(challenge);

            // assert
            Assert.NotNull(actual);
            Assert.Equal(realm, actual.Realm, Ordinal);
            Assert.Empty(actual.Scope);
            Assert.Equal(error, actual.Error, Ordinal);
            Assert.Equal(errorDescription, actual.ErrorDescription, Ordinal);
            Assert.Null(actual.ErrorUri);
            Assert.NotNull(actual.Extensions);
            Assert.Empty(actual.Extensions);
        }

        [Fact(DisplayName = "A challenge with an empty scope produces empty scope data.")]
        public void EmptyScope_Results_EmptyScope_Try()
        {
            // arrange
            var realm = "cimpress.auth0.com";
            var error = "invalid_token";
            var errorDescription = "The token is expired";
            var challenge = $@"realm=""{realm}"", scope="""", error=""{error}"", error_description=""{errorDescription}""";

            // act
            var success = BearerChallenge.TryParse(challenge, out var actual);

            // assert
            Assert.True(success);
            Assert.NotNull(actual);
            Assert.Equal(realm, actual.Realm, Ordinal);
            Assert.Empty(actual.Scope);
            Assert.Equal(error, actual.Error, Ordinal);
            Assert.Equal(errorDescription, actual.ErrorDescription, Ordinal);
            Assert.Null(actual.ErrorUri);
            Assert.NotNull(actual.Extensions);
            Assert.Empty(actual.Extensions);
        }

        [Fact(DisplayName = "A challenge with no scope otherwise parses.")]
        public void ThreeSixtyNoScope_Parses()
        {
            // arrange
            var challenge = @"realm=""cimpress.auth0.com"", error=""invalid_token"", error_description=""The token is expired""";

            // act
            var actual = Record.Exception(() => BearerChallenge.Parse(challenge));

            // assert
            Assert.Null(actual);
        }

        [Fact(DisplayName = "A challenge with no scope otherwise parses.")]
        public void ThreeSixtyNoScope_Parses_Try()
        {
            // arrange
            var challenge = @"realm=""cimpress.auth0.com"", error=""invalid_token"", error_description=""The token is expired""";

            // act
            var success = BearerChallenge.TryParse(challenge, out var actual);

            // assert
            Assert.True(success);
        }

        [Fact(DisplayName = "A challenge with no scope produces empty scope data.")]
        public void ThreeSixtyNoScope_Results_EmptyScope()
        {
            // arrange
            var realm = "cimpress.auth0.com";
            var error = "invalid_token";
            var errorDescription = "The token is expired";
            var challenge = $@"realm=""{realm}"", error=""{error}"", error_description=""{errorDescription}""";

            // act
            var actual = BearerChallenge.Parse(challenge);

            // assert
            Assert.NotNull(actual);
            Assert.Equal(realm, actual.Realm, Ordinal);
            Assert.Empty(actual.Scope);
            Assert.Equal(error, actual.Error, Ordinal);
            Assert.Equal(errorDescription, actual.ErrorDescription, Ordinal);
            Assert.Null(actual.ErrorUri);
            Assert.NotNull(actual.Extensions);
            Assert.Empty(actual.Extensions);
        }

        [Fact(DisplayName = "A challenge with no scope produces empty scope data.")]
        public void ThreeSixtyNoScope_Results_EmptyScope_Try()
        {
            // arrange
            var realm = "cimpress.auth0.com";
            var error = "invalid_token";
            var errorDescription = "The token is expired";
            var challenge = $@"realm=""{realm}"", error=""{error}"", error_description=""{errorDescription}""";

            // act
            var success = BearerChallenge.TryParse(challenge, out var actual);

            // assert
            Assert.True(success);
            Assert.NotNull(actual);
            Assert.Equal(realm, actual.Realm, Ordinal);
            Assert.Empty(actual.Scope);
            Assert.Equal(error, actual.Error, Ordinal);
            Assert.Equal(errorDescription, actual.ErrorDescription, Ordinal);
            Assert.Null(actual.ErrorUri);
            Assert.NotNull(actual.Extensions);
            Assert.Empty(actual.Extensions);
        }

        [Fact(DisplayName = "A typical challenge parses.")]
        public void TypicalChallenge_Parses()
        {
            // arrange
            var realm = "cimpress.auth0.com";
            var scope = new [] { "openid" };
            var error = "invalid_token";
            var errorDescription = "The token is expired";
            var authorizationUri = new Uri(@"https://cimpress.auth0.com/oauth/token", Absolute);
            var challenge = $@"realm=""{realm}"", authorization_uri=""{authorizationUri}"", scope=""{string.Join(" ", scope)}"", error=""{error}"", error_description=""{errorDescription}""";

            // act
            var actual = Record.Exception(() => BearerChallenge.Parse(challenge));

            // assert
            Assert.Null(actual);
        }

        [Fact(DisplayName = "A typical challenge parses.")]
        public void TypicalChallenge_Parses_Try()
        {
            // arrange
            var realm = "cimpress.auth0.com";
            var scope = new[] { "openid" };
            var error = "invalid_token";
            var errorDescription = "The token is expired";
            var authorizationUri = new Uri(@"https://cimpress.auth0.com/oauth/token", Absolute);
            var challenge = $@"realm=""{realm}"", authorization_uri=""{authorizationUri}"", scope=""{string.Join(" ", scope)}"", error=""{error}"", error_description=""{errorDescription}""";

            // act
            var success = BearerChallenge.TryParse(challenge, out var actual);

            // assert
            Assert.True(success);
            Assert.NotNull(actual);
        }

        [Fact(DisplayName = "A typical challenge produces the correct data.")]
        public void TypicalChallenge_Results()
        {
            // arrange
            var realm = "cimpress.auth0.com";
            var scope = new[] { "openid", "profile" };
            var error = "invalid_token";
            var errorDescription = "The token is expired";
            var authorizationUri = new Uri(@"https://cimpress.auth0.com/oauth/token", Absolute);
            var challenge = $@"realm=""{realm}"", authorization_uri=""{authorizationUri}"", scope=""{string.Join(" ", scope)}"", error=""{error}"", error_description=""{errorDescription}""";

            // act
            var actual = BearerChallenge.Parse(challenge);

            // assert
            Assert.NotNull(actual);
            Assert.Equal(realm, actual.Realm, Ordinal);
            Assert.Equal(scope, actual.Scope, Ordinal);
            Assert.Equal(error, actual.Error, Ordinal);
            Assert.Equal(errorDescription, actual.ErrorDescription, Ordinal);
            Assert.Null(actual.ErrorUri);
            Assert.NotNull(actual.Extensions);
            var (key, value) = Assert.Single(actual.Extensions);
            Assert.Equal("authorization_uri", key, Ordinal);
            Assert.Equal(authorizationUri.AbsoluteUri, value, Ordinal);
        }

        [Fact(DisplayName = "A typical challenge produces the correct data.")]
        public void TypicalChallenge_Results_Try()
        {
            // arrange
            var realm = "cimpress.auth0.com";
            var scope = new[] { "openid", "profile" };
            var error = "invalid_token";
            var errorDescription = "The token is expired";
            var authorizationUri = new Uri(@"https://cimpress.auth0.com/oauth/token", Absolute);
            var challenge = $@"realm=""{realm}"", authorization_uri=""{authorizationUri}"", scope=""{string.Join(" ", scope)}"", error=""{error}"", error_description=""{errorDescription}""";

            // act
            var success = BearerChallenge.TryParse(challenge, out var actual);

            // assert
            Assert.True(success);
            Assert.NotNull(actual);
            Assert.Equal(realm, actual.Realm, Ordinal);
            Assert.Equal(scope, actual.Scope, Ordinal);
            Assert.Equal(error, actual.Error, Ordinal);
            Assert.Equal(errorDescription, actual.ErrorDescription, Ordinal);
            Assert.Null(actual.ErrorUri);
            Assert.NotNull(actual.Extensions);
            var (key, value) = Assert.Single(actual.Extensions);
            Assert.Equal("authorization_uri", key, Ordinal);
            Assert.Equal(authorizationUri.AbsoluteUri, value, Ordinal);
        }

        [Fact(DisplayName = "A challenge is valid when converted to a string.")]
        public void ToString_Valid()
        {
            // arrange
            var challenge = @"realm=""cimpress.auth0.com"", scope=""client_id=0NBlSon1C4tJEVWS6GJRjwju5NxKHfBF service=https://orr.fen.cimpress.io"", error=""invalid_token"", error_description=""The token is expired""";

            // act
            var actual = Record.Exception(() => BearerChallenge.Parse(BearerChallenge.Parse(challenge).ToString()));

            // assert
            Assert.Null(actual);
        }

        [Fact(DisplayName = "Bad quoting causes a parsing failure.")]
        public void BadQuotes_Failure()
        {
            // arrange
            var challenge = @"realm=""cimpress.auth0.com, scope=""client_id=0NBlSon1C4tJEVWS6GJRjwju5NxKHfBF service=https://orr.fen.cimpress.io"", error=""invalid_token"", error_description=""The token is expired""";

            // act
            var actual = Record.Exception(() => BearerChallenge.Parse(challenge));

            // assert
            Assert.NotNull(actual);
            var formatException = Assert.IsType<FormatException>(actual);
            Assert.Contains("correct format", formatException.Message);
        }

        [Fact(DisplayName = "Bad quoting causes a parsing failure.")]
        public void BadQuotes_Failure_Try()
        {
            // arrange
            var challenge = @"realm=""cimpress.auth0.com, scope=""client_id=0NBlSon1C4tJEVWS6GJRjwju5NxKHfBF service=https://orr.fen.cimpress.io"", error=""invalid_token"", error_description=""The token is expired""";

            // act
            var success = BearerChallenge.TryParse(challenge, out var actual);

            // assert
            Assert.False(success);
            Assert.Null(actual);
        }

        [Fact(DisplayName = "A duplicate key causes a parsing failure.")]
        public void DuplicateKey_Failure()
        {
            // arrange
            var challenge = @"realm=""cimpress.auth0.com"", realm=""cimpress.example.com"", scope=""client_id=0NBlSon1C4tJEVWS6GJRjwju5NxKHfBF service=https://orr.fen.cimpress.io"", error=""invalid_token"", error_description=""The token is expired""";

            // act
            var actual = Record.Exception(() => BearerChallenge.Parse(challenge));

            // assert
            Assert.NotNull(actual);
            var formatException = Assert.IsType<FormatException>(actual);
            Assert.Contains("correct format", formatException.Message, StringComparison.Ordinal);
        }

        [Fact(DisplayName = "A duplicate key causes a parsing failure.")]
        public void DuplicateKey_Failure_Try()
        {
            // arrange
            var challenge = @"realm=""cimpress.auth0.com"", realm=""cimpress.example.com"", scope=""client_id=0NBlSon1C4tJEVWS6GJRjwju5NxKHfBF service=https://orr.fen.cimpress.io"", error=""invalid_token"", error_description=""The token is expired""";

            // act
            var success = BearerChallenge.TryParse(challenge, out var actual);

            // assert
            Assert.False(success);
            Assert.Null(actual);
        }
    }
}

