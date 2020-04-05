using System;
using System.Text;

namespace EsiNet.Expressions
{
    public static class VariableParser
    {
        public static VariableExpression ParseVariable(ExpressionReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            if (reader.ReadChar() != '$') throw reader.UnexpectedCharacterException();
            if (reader.ReadChar() != '(') throw reader.UnexpectedCharacterException();

            reader.SkipWhitespace();

            var name = ParseVariableName(reader);

            reader.SkipWhitespace();

            VariableExpression variable;
            if (reader.PeekChar() == '{')
            {
                var dictionaryKey = ParseDictionaryKey(reader);
                var defaultValue = ParseDefaultValue(reader);
                variable = new DictionaryVariableExpression(name, dictionaryKey, defaultValue);
            }
            else
            {
                var defaultValue = ParseDefaultValue(reader);
                variable = new SimpleVariableExpression(name, defaultValue);
            }

            if (reader.ReadChar() != ')') throw reader.UnexpectedCharacterException();

            return variable;
        }

        private static string ParseVariableName(ExpressionReader reader)
        {
            var name = new StringBuilder();
            while (IsNameChar(reader.PeekChar()))
            {
                name.Append(reader.ReadChar());
            }

            if (name.Length == 0) throw reader.UnexpectedCharacterException();
            
            return name.ToString();
        }

        private static bool IsNameChar(char c) => char.IsLetter(c) || c == '_';

        private static string ParseDictionaryKey(ExpressionReader reader)
        {
            reader.Read(); //Skip {

            var key = new StringBuilder();
            while (reader.Peek() != -1 && reader.PeekChar() != '}')
            {
                key.Append(reader.ReadChar());
            }

            if (key.Length == 0) throw reader.UnexpectedCharacterException();

            if (reader.Read() != '}') throw reader.UnexpectedCharacterException();

            reader.SkipWhitespace();

            return key.ToString();
        }

        private static string ParseDefaultValue(ExpressionReader reader)
        {
            if (reader.PeekChar() != '|')
            {
                return null;
            }

            reader.Read();
            
            if (reader.PeekChar() == '\'')
            {
                return ConstantParser.Parse(reader);
            }

            var value = new StringBuilder();
            while (IsValidDefaultChar(reader.PeekChar()))
            {
                value.Append(reader.ReadChar());
            }

            return value.ToString();
        }
        
        private static bool IsValidDefaultChar(char c) => char.IsLetter(c) || char.IsDigit(c);
    }
}