using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EsiNet.Fragments.Choose;

namespace EsiNet.Fragments
{
    public class EsiChooseParser : IEsiFragmentParser
    {
        private readonly EsiBodyParser _bodyParser;

        private static readonly Regex WhenTagRegex = new Regex(
            @"<esi:when\s+test\=""([^""]*)"">([\s\S]*?)<\/esi:when>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        private static readonly Regex OtherwiseTagRegex = new Regex(
            @"<esi:otherwise>([\s\S]*?)<\/esi:otherwise>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        public EsiChooseParser(EsiBodyParser bodyParser)
        {
            _bodyParser = bodyParser ?? throw new ArgumentNullException(nameof(bodyParser));
        }

        public IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body)
        {
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));
            if (body == null) throw new ArgumentNullException(nameof(body));

            var whenFragments =
                from match in WhenTagRegex.Matches(body).Cast<Match>()
                let test = match.Groups[1].Value
                let innerBody = match.Groups[2].Value
                let innerFragment = _bodyParser.Parse(innerBody)
                select new EsiWhenFragment(ParseTestExpression(test), innerFragment);

            var otherwiseMatch = OtherwiseTagRegex.Match(body);

            var otherwiseFragment = otherwiseMatch.Success
                ? _bodyParser.Parse(otherwiseMatch.Groups[1].Value)
                : new EsiIgnoreFragment();

            return new EsiChooseFragment(whenFragments.ToArray(), otherwiseFragment);
        }

        private static ComparisonExpression ParseTestExpression(string test)
        {
            return WhenParser.Parse(test);
        }
    }
}