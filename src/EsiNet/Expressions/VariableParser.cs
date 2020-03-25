using System;
using System.Text;

namespace EsiNet.Expressions
{
    public static class VariableParser
    {
        public static VariableExpression ParseVariable(ExpressionReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            if (reader.Read() != '$') throw reader.UnexpectedCharacterException();
            if (reader.Read() != '(') throw reader.UnexpectedCharacterException();

            reader.SkipWhitespace();

            var name = new StringBuilder();
            while (IsNameChar(reader.PeekChar()))
            {
                name.Append(reader.ReadChar());
            }

            if (name.Length == 0) throw reader.UnexpectedCharacterException();

            reader.SkipWhitespace();

            VariableExpression variable;
            if (reader.PeekChar() == '{')
            {
                variable = ParseDictionaryVariable(reader, name.ToString());
            }
            else
            {
                variable = new SimpleVariableExpression(name.ToString());
            }

            if (reader.Read() != ')') throw reader.UnexpectedCharacterException();

            return variable;
        }

        private static DictionaryVariableExpression ParseDictionaryVariable(ExpressionReader reader, string name)
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

            return new DictionaryVariableExpression(name, key.ToString());
        }

        private static bool IsNameChar(char c) => char.IsLetter(c) || c == '_';
    }
}