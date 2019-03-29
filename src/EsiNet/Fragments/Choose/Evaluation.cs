using System;
using System.Collections.Generic;

namespace EsiNet.Fragments.Choose
{
    public class WhenEvaluator
    {
        private static readonly StringComparer Comparer = StringComparer.CurrentCulture;

        public static bool Evaluate(ComparisonExpression comparisonExpression, IReadOnlyDictionary<string, string> variables)
        {
            var leftValue = GetValue(comparisonExpression.Left, variables);
            var rightValue = GetValue(comparisonExpression.Right, variables);

            return Compare(leftValue, rightValue, comparisonExpression.ComparisonOperator);
        }

        private static bool Compare(string left, string right, ComparisonOperator @operator)
        {
            switch (@operator)
            {
                case ComparisonOperator.Equal:
                    return Comparer.Equals(left, right);
                case ComparisonOperator.NotEqual:
                    return !Comparer.Equals(left, right);
                case ComparisonOperator.GreaterThan:
                    return Comparer.Compare(left, right) > 0;
                case ComparisonOperator.GreaterThanOrEqual:
                    return Comparer.Compare(left, right) >= 0;
                case ComparisonOperator.LessThan:
                    return Comparer.Compare(left, right) < 0;
                case ComparisonOperator.LessThanOrEqual:
                    return Comparer.Compare(left, right) <= 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(@operator), @operator, null);
            }
        }

        private static string GetValue(ValueExpression valueExpression, IReadOnlyDictionary<string, string> variables)
        {
            if (valueExpression is ConstantExpression constant)
            {
                return constant.Value;
            }

            if (valueExpression is VariableExpression variable)
            {
                return variables.TryGetValue(variable.Name, out var variableValue) ? variableValue : string.Empty;
            }

            throw new Exception($"Value type {valueExpression.GetType().Name} not supported");
        }
    }
}