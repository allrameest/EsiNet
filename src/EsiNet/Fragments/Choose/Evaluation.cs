using System;
using System.Collections.Generic;

namespace EsiNet.Fragments.Choose
{
    public class WhenEvaluator
    {
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
                    return left.Equals(right);
                case ComparisonOperator.NotEqual:
                    return !left.Equals(right);
                case ComparisonOperator.GreaterThan:
                case ComparisonOperator.GreaterThanOrEqual:
                case ComparisonOperator.LessThan:
                case ComparisonOperator.LessThanOrEqual:
                    throw new NotImplementedException(); //TODO
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

            throw new Exception(); //TODO
        }
    }
}