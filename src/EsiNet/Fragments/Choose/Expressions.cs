using System;

namespace EsiNet.Fragments.Choose
{
    public class ComparisonExpression
    {
        public ComparisonExpression(ValueExpression left, ValueExpression right, ComparisonOperator comparisonOperator)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
            ComparisonOperator = comparisonOperator;
        }

        public ValueExpression Left { get; }
        public ValueExpression Right { get; }
        public ComparisonOperator ComparisonOperator { get; }
    }

    public enum ComparisonOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
    }

    public class ValueExpression
    {
    }

    public class ConstantExpression : ValueExpression
    {
        public ConstantExpression(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public class VariableExpression : ValueExpression
    {
        public VariableExpression(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}