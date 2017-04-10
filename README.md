# Tiger.Challenge

## What It Is

Tiger.Challenge is a library for parsing `WWW-Authenticate` authentication challenges.

## Why You Want It

The challenge portion of the HTTP header `WWW-Authenticate` is tricky to parse. A naïve solution using regular expressions can suffice for many situations, but falls down in the face of some of the more complex features of the header's grammar. These include, but are not limited to:

- Ability to express parameters in any order.
- Optional wrapping in double-quotes of parameter values.
- A whitelisted character range for parameter keys and values.
- A totally different whitelisted character range for "scope" values.
- Optional (though officially "bad") whitespace surrounding the "=" key–value separator.
  - Unless it's escaped.
    - But only before the "=", not after.
      - But only *then* if the value is wrapped in double-quotes.
        - Unless those themselves are escaped.

For this kind of parsing, an actual parser is required.

The grammar of the `WWW-Authenticate` header is specified in [RFC217](https://tools.ietf.org/html/rfc2617), which also establishes the challenge types Basic ("Basic") and Digest Access ("Digest"). It is described in ABNF, referencing other RFCs for smaller parsable structures, such as allowed values in HTTP headers or the structure of a URI. Other specifications specify other challenge types, such as [RFC6750](https://tools.ietf.org/html/rfc6750), which specifies Bearer Token ("Bearer").

<!--
### Hey, What about Other Challenge Types???

No one's asked for them yet. We standardized on "Bearer" here at Cimpress, so that's all that's needed for now. "Basic" and "Digest" should be easy enough to implement, if it comes down to it.
-->

## How You Develop It

This project is using the standard [`dotnet`](https://dot.net) build tool. A brief primer:

- Restore NuGet dependencies: `dotnet restore`
- Build the entire solution: `dotnet build`
- Run all unit tests: `dotnet test`
- Pack for publishing: `dotnet pack -o "$(pwd)/artifacts"`

The parameter `--configuration` (shortname `-c`) can be supplied to the `build`, `test`, and `pack` steps with the following meaningful values:

- “Debug” (the default)
- “Release”

This repository is attempting to use the [GitFlow](http://jeffkreeftmeijer.com/2010/why-arent-you-using-git-flow/) branching methodology. Results may be mixed, please be aware.

## Thank You

Seriously, though. Thank you for using this software. The author hopes it performs admirably for you.
