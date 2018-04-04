using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EsiNet
{
    public class EsiBodyParser
    {
        private const int TagGroupIndex = 1;
        private const int AttributesGroupIndex = 2;
        private const int TagBodyIndex = 3;
        private const int AttributeNameGroupIndex = 1;
        private const int AttributeValueGroupIndex = 1;

        private static readonly Regex EsiTagRegex = new Regex(@"
<
(esi\:[a-z]+)       # Tag name
\b
([^>]+[^\/>])?      # Tag attributes
(?:\/
|
>([\s\S]*?)<\/\1)   # Tag body
>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        private static readonly Regex EsiAttributeRegex = new Regex(@"
\b
([^\s=]+)   # Attribute name
=
""
([^""""]*)  # Attribute value
""
",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        private readonly IReadOnlyDictionary<string, IEsiParser> _parsers;

        public EsiBodyParser(IReadOnlyDictionary<string, IEsiParser> parsers)
        {
            _parsers = parsers;
        }

        public IEsiFragment Parse(string body)
        {
            var matches = EsiTagRegex.Matches(body);
            if (matches.Count == 0)
            {
                return new EsiTextFragment(body);
            }

            var fragments = new List<IEsiFragment>();
            var lastIndex = 0;

            foreach (Match match in matches)
            {
                var beforeContent = body.Substring(lastIndex, match.Index - lastIndex);
                fragments.Add(new EsiTextFragment(beforeContent));

                var fragment = ParseTag(match);
                fragments.Add(fragment);

                lastIndex = match.Index + match.Length;
            }

            var lastContent = body.Substring(lastIndex, body.Length - lastIndex);
            fragments.Add(new EsiTextFragment(lastContent));

            return new EsiCompositeFragment(fragments);
        }

        private IEsiFragment ParseTag(Match match)
        {
            var tag = match.Groups[TagGroupIndex].Value;
            var attributes = ParseAttributes(match.Groups[AttributesGroupIndex].Value);
            var tagBody = match.Groups[TagBodyIndex].Value;

            return _parsers.TryGetValue(tag, out var parser)
                ? parser.Parse(attributes, tagBody)
                : new EsiTextFragment(match.Value);
        }

        private static IReadOnlyDictionary<string, string> ParseAttributes(string attributesContent)
        {
            return EsiAttributeRegex.Matches(attributesContent)
                .Cast<Match>()
                .ToDictionary(
                    match => match.Groups[AttributeNameGroupIndex].Value,
                    match => match.Groups[AttributeValueGroupIndex].Value,
                    StringComparer.OrdinalIgnoreCase);
        }
    }
}