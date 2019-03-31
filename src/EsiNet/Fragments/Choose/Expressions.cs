using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EsiNet.Fragments.Choose
{
    public class GroupExpression : IWhenExpression
    {
        public GroupExpression(IReadOnlyCollection<IWhenExpression> booleanExpressions, BooleanOperator booleanOperator)
        {
            BooleanExpressions = booleanExpressions;
            BooleanOperator = booleanOperator;
        }

        public IReadOnlyCollection<IWhenExpression> BooleanExpressions { get; }
        public BooleanOperator BooleanOperator { get; }
    }

    public interface IWhenExpression
    {
        BooleanOperator BooleanOperator { get; }
    }

    public enum BooleanOperator
    {
        And,
        Or
    }

    public class ComparisonExpression : IWhenExpression
    {
        public ComparisonExpression(ValueExpression left, ValueExpression right, ComparisonOperator comparisonOperator, BooleanOperator booleanOperator)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
            ComparisonOperator = comparisonOperator;
            BooleanOperator = booleanOperator;
        }

        public ValueExpression Left { get; }
        public ValueExpression Right { get; }
        public ComparisonOperator ComparisonOperator { get; }
        public BooleanOperator BooleanOperator { get; }
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

    public abstract class ValueExpression
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