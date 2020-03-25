namespace EsiNet.Expressions
{
    public class VariableExpression : ValueExpression
    {
        public VariableExpression(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public class SimpleVariableExpression : VariableExpression
    {
        public SimpleVariableExpression(string name) : base(name)
        {
        }
    }

    public class DictionaryVariableExpression : VariableExpression
    {
        public DictionaryVariableExpression(string name, string key) : base(name)
        {
            Key = key;
        }

        public string Key { get; }
    }
}