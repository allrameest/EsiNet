using System;

namespace EsiNet.Expressions
{
    [Serializable]
    public abstract class VariableExpression : ValueExpression
    {
        protected VariableExpression(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    [Serializable]
    public class SimpleVariableExpression : VariableExpression
    {
        public SimpleVariableExpression(string name) : base(name)
        {
        }
    }

    [Serializable]
    public class DictionaryVariableExpression : VariableExpression
    {
        public DictionaryVariableExpression(string name, string key) : base(name)
        {
            Key = key;
        }

        public string Key { get; }
    }
}