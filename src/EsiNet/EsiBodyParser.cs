using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using EsiNet.Fragments;
using EsiNet.Fragments.Composite;
using EsiNet.Fragments.Ignore;
using EsiNet.Fragments.Text;

namespace EsiNet
{
    public class EsiBodyParser
    {
        private const int TagGroupIndex = 1;
        private const int AttributesGroupIndex = 2;
        private const int TagBodyIndex = 3;
        private const int AttributeNameGroupIndex = 1;
        private const int AttributeValueGroupIndex = 2;

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

        private readonly EsiFragmentParser _fragmentParser;

        public EsiBodyParser(EsiFragmentParser fragmentParser)
        {
            _fragmentParser = fragmentParser ?? throw new ArgumentNullException(nameof(fragmentParser));
        }

        public IEsiFragment Parse(string body)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));

            if (body.Length == 0)
            {
                return new EsiIgnoreFragment();
            }

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
                if (beforeContent.Length > 0)
                {
                    fragments.Add(new EsiTextFragment(beforeContent));
                }

                var fragment = ParseTag(match);
                fragments.Add(fragment);

                lastIndex = match.Index + match.Length;
            }

            var lastContent = body.Substring(lastIndex, body.Length - lastIndex);
            if (lastContent.Length > 0)
            {
                fragments.Add(new EsiTextFragment(lastContent));
            }

            if (fragments.Count == 1)
            {
                return fragments.Single();
            }

            return new EsiCompositeFragment(fragments);
        }

        private IEsiFragment ParseTag(Match match)
        {
            var tag = match.Groups[TagGroupIndex].Value;
            var attributes = ParseAttributes(match.Groups[AttributesGroupIndex].Value);
            var tagBody = match.Groups[TagBodyIndex].Value;
            var outerBody = match.Value;

            return _fragmentParser.Parse(tag, attributes, tagBody, outerBody);
        }

        private static IReadOnlyDictionary<string, string> ParseAttributes(string attributesContent)
        {
            return EsiAttributeRegex.Matches(attributesContent)
                .Cast<Match>()
                .ToDictionary(
                    match => match.Groups[AttributeNameGroupIndex].Value,
                    match => WebUtility.HtmlDecode(match.Groups[AttributeValueGroupIndex].Value),
                    StringComparer.OrdinalIgnoreCase);
        }
    }
}