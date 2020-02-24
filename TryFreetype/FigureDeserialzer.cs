using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TryFreetype.Model;

namespace TryFreetype
{
    public class FigureDeserialzer
    {
        private struct Token
        {
            public TokenType Type;
            public ValueUnion V;
            public RefUnion R;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ValueUnion
        {
            [FieldOffset(0)]
            public long IntValue;
            [FieldOffset(0)]
            public double a;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct RefUnion
        {
            [FieldOffset(0)]
            public string str;
        }

        private enum TokenType
        {
            Bof,
            Eof,
            Eol,
            Word,
            Integer,
            Float,
        }

        private TextReader _reader;
        private int _iChar;
        private int _nestingLevel;

        private StringBuilder _tokenString = new StringBuilder();
        private TokenType _tokenType;
        private long _intVal;
        private double _floatVal;

        public Figure Figure { get; private set; }

        internal FigureDeserialzer(TextReader reader)
        {
            _reader = reader;

            ReadChar();
        }

        internal void Deserialize()
        {
            Figure figure = ReadFigure();

            Figure = figure;
        }

        private Figure ReadFigure()
        {
            while (!IsAtEof())
            {
                ReadRecord();
            }

            if (_nestingLevel > 0)
                throw new ApplicationException();

            return null;
        }

        private void ReadRecord()
        {
            SkipWhitespace();

            long id = -1;

            if (CharIsDigit(_iChar))
            {
                id = ReadInteger();
            }

            ReadRecord(id);
        }

        private void ReadRecord(long id)
        {
            ReadToken();

            if (IsAtEof())
            {
                if (id >= 0)
                    throw new ApplicationException();

                return;
            }

            if (_tokenType == TokenType.Eol)
                return;

            if (_tokenType != TokenType.Word)
                throw new ApplicationException();

            if (WordMatches("end"))
            {
                OnEndRecord();

                _nestingLevel--;

                if (_nestingLevel < 0)
                    throw new ApplicationException();
            }
            else
            {
                OnBeginRecord(id, _tokenString.ToString());

                ReadToken();

                while (_tokenType != TokenType.Eol && _tokenType != TokenType.Eof)
                {
                    if (_tokenType == TokenType.Word && WordMatches("begin"))
                    {
                        ReadToken();

                        if (_tokenType != TokenType.Eol)
                            throw new ApplicationException();

                        _nestingLevel++;
                    }
                    else
                    {
                        Token token = new Token();
                        token.Type = _tokenType;
                        switch (_tokenType)
                        {
                            case TokenType.Word:
                                token.R.str = _tokenString.ToString();
                                break;

                            case TokenType.Integer:
                                token.V.IntValue = _intVal;
                                break;

                            case TokenType.Float:
                                token.V.a = _floatVal;
                                break;
                        }

                        OnAttribute(token);

                        ReadToken();
                    }
                }

                //OnEndRecord();
            }
        }

        private bool WordMatches(string str)
        {
            if (_tokenType != TokenType.Word)
                throw new ApplicationException();

            if (_tokenString.Length != str.Length)
                return false;

            for (int i = 0; i < str.Length; i++)
            {
                if (_tokenString[i] != str[i])
                    return false;
            }

            return true;
        }

        private void ReadToken()
        {
            SkipWhitespace();

            if (_iChar < 0)
                _tokenType = TokenType.Eof;
            else if (_iChar == '\r' || _iChar == '\n')
                ReadEol();
            else if (CharIsLetter(_iChar))
                ReadWord();
            else if (_iChar == '-' || CharIsDigit(_iChar))
                ReadNumber();
            else
                throw new ApplicationException();
        }

        private static bool CharIsLetter(int c)
        {
            if (c < 0)
                return false;

            return char.IsLetter((char) c);
        }

        private static bool CharIsDigit(int c)
        {
            if (c < 0)
                return false;

            return char.IsDigit((char) c);
        }

        private static bool CharIsLetterOrDigit(int c)
        {
            if (c < 0)
                return false;

            return char.IsLetterOrDigit((char) c);
        }

        private static bool CharIsWhiteSpace(int c)
        {
            if (c < 0)
                return false;

            char ch = (char) c;

            return ch == ' ' || ch == '\t';
        }

        private static bool CharIsSeparator(int c)
        {
            if (c < 0)
                return true;

            char ch = (char) c;

            return ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n';
        }

        private void ReadEol()
        {
            if (_iChar == '\n')
            {
                _tokenType = TokenType.Eol;
                ReadChar();
                return;
            }

            if (_iChar == '\r')
            {
                _tokenType = TokenType.Eol;
                ReadChar();

                if (_iChar == '\n')
                    ReadChar();

                return;
            }
        }

        private void ReadNumber()
        {
            bool negate = false;
            bool dotFound = false;

            _tokenString.Clear();

            if (_iChar == '-')
            {
                negate = true;
                ReadChar();
            }

            do
            {
                if (_iChar != '.' && !CharIsDigit(_iChar))
                    throw new ApplicationException();

                if (_iChar == '.')
                {
                    if (dotFound)
                        throw new ApplicationException();

                    dotFound = true;
                }

                _tokenString.Append((char) _iChar);
                ReadChar();
            }
            while (!CharIsSeparator(_iChar));

            if (dotFound)
            {
                _tokenType = TokenType.Float;
                _floatVal = Convert.ToDouble(_tokenString.ToString());
                if (negate)
                    _floatVal = -_floatVal;
            }
            else
            {
                _tokenType = TokenType.Integer;
                _intVal = Convert.ToInt64(_tokenString.ToString());
                if (negate)
                    _intVal = -_intVal;
            }
        }

        private long ReadInteger()
        {
            ReadNumber();

            if (_tokenType != TokenType.Integer)
                throw new ApplicationException();

            return _intVal;
        }

        private void ReadWord()
        {
            _tokenString.Clear();

            do
            {
                if (!CharIsLetterOrDigit(_iChar) && _iChar != '-')
                    throw new ApplicationException();

                _tokenString.Append((char) _iChar);
                ReadChar();
            }
            while (!CharIsSeparator(_iChar));

            _tokenType = TokenType.Word;
        }

        private void SkipWhitespace()
        {
            while (CharIsWhiteSpace(_iChar))
            {
                ReadChar();
            }
        }

        private bool IsAtEof()
        {
            return _tokenType == TokenType.Eof;
        }

        private void ReadChar()
        {
            _iChar = _reader.Read();

            if (_iChar < 0)
                return;

            if (char.IsControl((char) _iChar) && _iChar != '\r' && _iChar != '\n' && _iChar != '\t')
                throw new ApplicationException();
        }

        public static Figure Deserialize(TextReader reader)
        {
            var deserializer = new FigureDeserialzer(reader);

            deserializer.Deserialize();

            return deserializer.Figure;
        }

        private void OnEndRecord()
        {
            Console.WriteLine("end");
        }

        private void OnBeginRecord(long id, string v)
        {
            Console.WriteLine("begin {0} ({1})", v, id);
        }

        private void OnAttribute(Token token)
        {
            Console.WriteLine("attr {0}", token.Type);
        }
    }
}
