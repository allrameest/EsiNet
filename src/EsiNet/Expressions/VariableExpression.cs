using System;

namespace EsiNet.Expressions
{
    [Serializable]
    public abstract class VariableExpression : ValueExpression
    {
        protected VariableExpression(string name, string defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
        }

        public string Name { get; }
        public string DefaultValue { get; }
    }

    [Serializable]
    public class SimpleVariableExpression : VariableExpression
    {
        public SimpleVariableExpression(string name, string defaultName = null) : base(name, defaultName)
        {
        }
    }

    [Serializable]
    public class DictionaryVariableExpression : VariableExpression
    {
        public DictionaryVariableExpression(string name, string key, string defaultName = null) : base(name, defaultName)
        {
            Key = key;
        }

        public string Key { get; }
    }
}