using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EsiNet.Expressions
{
    [Serializable]
    public class VariableString
    {
        public IReadOnlyCollection<object> Parts { get; }

        public VariableString(IReadOnlyCollection<object> parts)
        {
            Parts = parts;
        }
    }

    public static class VariableStringParser
    {
        public static VariableString Parse(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var result = new List<object>();
            var basicString = new StringBuilder();

            using (var reader = new ExpressionReader(text))
            {
                while (reader.Peek() != -1)
                {
                    switch (reader.PeekChar())
                    {
                        case '$':
                            FlushString();
                            result.Add(VariableParser.ParseVariable(reader));
                            break;
                        default:
                            var c = reader.ReadChar();
                            basicString.Append(c);
                            break;
                    }
                }
            }

            FlushString();

            return new VariableString(result);

            void FlushString()
            {
                if (basicString.Length <= 0) return;

                result.Add(basicString.ToString());
                basicString.Clear();
            }
        }
    }

    public static class VariableStringResolver
    {
        public static IEnumerable<string> Resolve(
            EsiExecutionContext executionContext, VariableString variableString)
        {
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));
            if (variableString == null) throw new ArgumentNullException(nameof(variableString));
            
            return variableString.Parts
                .Select(o => ResolveStringPart(o, executionContext) ?? string.Empty);
        }

        private static string ResolveStringPart(object obj, EsiExecutionContext executionContext)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            switch (obj)
            {
                case string s:
                    return s;
                case VariableExpression v:
                    return VariableResolver.ResolveValue(v, executionContext.Variables);
                default:
                    throw new Exception($"Unknown part type '{obj.GetType().Name}'.");
            }
        }
    }
}