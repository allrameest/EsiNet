using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EsiNet.Fragments.Ignore;

namespace EsiNet.Fragments.Try
{
    public class EsiTryParser : IEsiFragmentParser
    {
        private readonly EsiBodyParser _bodyParser;

        private static readonly Regex TagRegex = new Regex(
            @"<(esi:[a-z]+)>([\s\S]*?)<\/\1>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        public EsiTryParser(EsiBodyParser bodyParser)
        {
            _bodyParser = bodyParser ?? throw new ArgumentNullException(nameof(bodyParser));
        }

        public IEsiFragment Parse(IReadOnlyDictionary<string, string> attributes, string body)
        {
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));
            if (body == null) throw new ArgumentNullException(nameof(body));

            var tags = TagRegex.Matches(body)
                .Cast<Match>()
                .ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value, StringComparer.OrdinalIgnoreCase);

            var attemptFound = tags.TryGetValue("esi:attempt", out var attemptBody);
            var exceptFound = tags.TryGetValue("esi:except", out var exceptBody);

            if (!attemptFound)
            {
                return new EsiIgnoreFragment();
            }

            var attemptFragment = _bodyParser.Parse(attemptBody);
            var exceptFragment = exceptFound
                ? _bodyParser.Parse(exceptBody)
                : new EsiIgnoreFragment();

            return new EsiTryFragment(attemptFragment, exceptFragment);
        }
    }
}