using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EsiNet
{
    public static class RegexExtensions
    {
        public static async Task<string> ReplaceAsync(
            this Regex regex, string input, Func<Match, Task<string>> evaluator)
        {
            if (regex == null) throw new ArgumentNullException(nameof(regex));
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (evaluator == null) throw new ArgumentNullException(nameof(evaluator));

            var tasks = new List<Task<string>>();
            var lastIndex = 0;

            foreach (Match match in regex.Matches(input))
            {
                var beforeContent = input.Substring(lastIndex, match.Index - lastIndex);
                tasks.Add(Task.FromResult(beforeContent));
                tasks.Add(evaluator(match));

                lastIndex = match.Index + match.Length;
            }

            var lastContent = input.Substring(lastIndex, input.Length - lastIndex);
            tasks.Add(Task.FromResult(lastContent));

            var all = await Task.WhenAll(tasks).ConfigureAwait(false);
            return string.Concat(all);
        }
    }
}