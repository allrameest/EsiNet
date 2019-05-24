using System;
using System.Collections.Generic;

namespace EsiNet.Fragments.Choose
{
    public class WhenEvaluator
    {
        private static readonly StringComparer Comparer = StringComparer.CurrentCulture;

        public static bool Evaluate(IWhenExpression expression, IReadOnlyDictionary<string, string> variables)
        {
            if (expression is ComparisonExpression comparisonExpression)
            {
                return EvaluateComparison(comparisonExpression, variables);
            }

            if (expression is GroupExpression groupExpression)
            {
                return EvaluateGroup(groupExpression, variables);
            }

            throw new Exception($"Expression type {expression.GetType().Name} not supported");
        }

        private static bool EvaluateGroup(
            GroupExpression groupExpression,
            IReadOnlyDictionary<string, string> variables)
        {
            var groupEvaluated = true;
            foreach (var expression in groupExpression.BooleanExpressions)
            {
                if (expression.BooleanOperator == BooleanOperator.And && !groupEvaluated) continue;
                if (expression.BooleanOperator == BooleanOperator.Or && groupEvaluated) break;

                var expressionEvaluated = Evaluate(expression, variables);

                if (expression.BooleanOperator == BooleanOperator.And) groupEvaluated &= expressionEvaluated;
                if (expression.BooleanOperator == BooleanOperator.Or) groupEvaluated |= expressionEvaluated;
            }

            return groupEvaluated;
        }

        private static bool EvaluateComparison(
            ComparisonExpression comparisonExpression,
            IReadOnlyDictionary<string, string> variables)
        {
            var left = GetValue(comparisonExpression.Left, variables);
            var right = GetValue(comparisonExpression.Right, variables);

            switch (comparisonExpression.ComparisonOperator)
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
                    throw new ArgumentOutOfRangeException(
                        nameof(comparisonExpression.ComparisonOperator), comparisonExpression.ComparisonOperator, null);
            }
        }

        private static string GetValue(ValueExpression valueExpression, IReadOnlyDictionary<string, string> variables)
        {
            if (valueExpression is ConstantExpression constant)
            {
                return constant.Value;
            }

            if (valueExpression is SimpleVariableExpression variable)
            {
                return variables.TryGetValue(variable.Name, out var variableValue) ? variableValue : null;
            }

            throw new Exception($"Value type {valueExpression.GetType().Name} not supported");
        }
    }
}