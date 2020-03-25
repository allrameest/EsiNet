using System;
using System.Collections.Generic;

namespace EsiNet.Expressions
{
    public class GroupExpression : IBooleanExpression
    {
        public GroupExpression(
            IReadOnlyCollection<IBooleanExpression> booleanExpressions,
            BooleanOperator booleanOperator)
        {
            BooleanExpressions = booleanExpressions;
            BooleanOperator = booleanOperator;
        }

        public IReadOnlyCollection<IBooleanExpression> BooleanExpressions { get; }
        public BooleanOperator BooleanOperator { get; }
    }

    public interface IBooleanExpression
    {
        BooleanOperator BooleanOperator { get; }
    }

    public enum BooleanOperator
    {
        And,
        Or
    }

    [Serializable]
    public class ComparisonExpression : IBooleanExpression
    {
        public ComparisonExpression(
            ValueExpression left,
            ValueExpression right,
            ComparisonOperator comparisonOperator,
            BooleanOperator booleanOperator)
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

    [Serializable]
    public abstract class ValueExpression
    {
    }

    [Serializable]
    public class ConstantExpression : ValueExpression
    {
        public ConstantExpression(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}