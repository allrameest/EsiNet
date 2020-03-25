using System;
using System.Collections.Generic;

namespace EsiNet.Expressions
{
    public static class VariableResolver
    {
        public static string ResolveValue(
            VariableExpression variableExpression, IReadOnlyDictionary<string, IVariableValueResolver> variables)
        {
            if (!variables.TryGetValue(variableExpression.Name, out var resolver))
            {
                return null;
            }

            return resolver.TryGetValue(variableExpression);
        }
    }

    public interface IVariableValueResolver
    {
        string TryGetValue(VariableExpression variable);
    }

    public class SimpleVariableValueResolver : IVariableValueResolver
    {
        private readonly Lazy<string> _value;

        public SimpleVariableValueResolver(Lazy<string> value)
        {
            _value = value;
        }

        public string TryGetValue(VariableExpression variable)
        {
            return _value.Value;
        }
    }

    public class DictionaryVariableValueResolver : IVariableValueResolver
    {
        private readonly Lazy<IReadOnlyDictionary<string, string>> _values;

        public DictionaryVariableValueResolver(Lazy<IReadOnlyDictionary<string, string>> values)
        {
            _values = values;
        }

        public string TryGetValue(VariableExpression variable)
        {
            return variable is DictionaryVariableExpression dictionaryVariable &&
                   _values.Value.TryGetValue(dictionaryVariable.Key, out var value)
                ? value
                : null;
        }
    }
}