using System;
using System.IO;

namespace EsiNet.Fragments.Choose
{
    public class ExpressionReader : IDisposable
    {
        private int _currentPosition;
        private bool _lastAccessWasRead;
        private StringReader _reader;

        public ExpressionReader(string text)
        {
            OriginalText = text;
            _reader = new StringReader(text);
        }

        public string OriginalText { get; private set; }

        public int LastAccessedPosition => _lastAccessWasRead ? _currentPosition - 1 : _currentPosition;

        public int Peek()
        {
            _lastAccessWasRead = false;
            return _reader.Peek();
        }

        public int Read()
        {
            ++_currentPosition;
            _lastAccessWasRead = true;
            return _reader.Read();
        }

        public void Dispose()
        {
            OriginalText = null;
            _reader?.Dispose();
            _reader = null;
        }
    }
}