using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TryFreetype.Model;

namespace TryFreetype
{
    internal abstract class Parser
    {
        protected struct Token
        {
            public TokenType Type;
            public ValueUnion V;
            public RefUnion R;

            public long GetInteger()
            {
                if (Type != TokenType.Integer)
                    throw new ApplicationException();

                return V.IntValue;
            }

            public double GetFloat()
            {
                if (Type == TokenType.Float)
                    return V.FloatValue;
                else if (Type == TokenType.Integer)
                    return V.IntValue;

                throw new ApplicationException();
            }

            public string GetWord()
            {
                if (Type != TokenType.Word)
                    throw new ApplicationException();

                return R.String;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        protected struct ValueUnion
        {
            [FieldOffset(0)]
            public long IntValue;
            [FieldOffset(0)]
            public double FloatValue;
        }

        [StructLayout(LayoutKind.Explicit)]
        protected struct RefUnion
        {
            [FieldOffset(0)]
            public string String;
        }

        protected enum TokenType
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

        internal Parser(TextReader reader)
        {
            _reader = reader;

            ReadChar();
        }

        internal void Parse()
        {
            ReadRecords();
        }

        private void ReadRecords()
        {
            while (!IsAtEof())
            {
                ReadRecord();
            }

            if (_nestingLevel > 0)
                throw new ApplicationException();
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

            if (IsAtEof() || _tokenType == TokenType.Eol)
            {
                if (id >= 0)
                    throw new ApplicationException();

                return;
            }

            if (_tokenType != TokenType.Word)
                throw new ApplicationException();

            if (WordMatches("end"))
            {
                OnEndRecord(null);

                _nestingLevel--;

                if (_nestingLevel < 0)
                    throw new ApplicationException();
            }
            else
            {
                string head = _tokenString.ToString();
                bool openRecord = false;
                var attrs = new List<Token>();

                ReadToken();

                while (_tokenType != TokenType.Eol && _tokenType != TokenType.Eof)
                {
                    if (_tokenType == TokenType.Word && WordMatches("begin"))
                    {
                        ReadToken();

                        if (_tokenType != TokenType.Eol)
                            throw new ApplicationException();

                        _nestingLevel++;
                        openRecord = true;
                    }
                    else
                    {
                        var token = WrapCurrentToken();

                        attrs.Add(token);

                        ReadToken();
                    }
                }

                OnBeginRecord(id, head, attrs);

                if (!openRecord)
                    OnEndRecord(head);
            }
        }

        private Token WrapCurrentToken()
        {
            Token token = new Token();
            token.Type = _tokenType;

            switch (_tokenType)
            {
                case TokenType.Word:
                    token.R.String = _tokenString.ToString();
                    break;

                case TokenType.Integer:
                    token.V.IntValue = _intVal;
                    break;

                case TokenType.Float:
                    token.V.FloatValue = _floatVal;
                    break;
            }

            return token;
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

                TokenAppendChar(_iChar);
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

                TokenAppendChar(_iChar);
                ReadChar();
            }
            while (!CharIsSeparator(_iChar));

            _tokenType = TokenType.Word;
        }

        private void TokenAppendChar(int iChar)
        {
            _tokenString.Append((char) iChar);
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

        protected abstract void OnBeginRecord(long id, string head, IList<Token> attrs);
        protected abstract void OnEndRecord(string head);
    }

    internal class FigureParser : Parser
    {
        private enum Node
        {
            None,
            Figure,
            Contour,
        }

        private int _level;
        private Node[] _nodeStack = new Node[3];

        private Dictionary<int, PointGroup> _pointGroups = new Dictionary<int, PointGroup>();
        private Dictionary<int, Point> _points = new Dictionary<int, Point>();
        private Contour _curContour;
        private int _width;
        private int _height;
        private double _offsetX;
        private double _offsetY;

        public Figure Figure { get; private set; }

        public FigureParser(TextReader reader) :
            base(reader)
        {
            _nodeStack[0] = Node.None;
        }

        private void PushLevel(Node node)
        {
            if (_level == _nodeStack.Length - 1)
                throw new ApplicationException();

            _level++;
            _nodeStack[_level] = node;
        }

        private void PopLevel()
        {
            if (_level == 0)
                throw new ApplicationException();

            _level--;
        }

        protected override void OnBeginRecord(long id, string head, IList<Token> attrs)
        {
            Console.WriteLine("begin {0} ({1})", head, id);

            switch (_nodeStack[_level])
            {
                case Node.None:
                    if (head != "figure")
                        throw new ApplicationException();

                    PushLevel(Node.Figure);
                    break;

                case Node.Figure:
                    if (head == "width")
                    {
                        if (attrs.Count != 1)
                            throw new ApplicationException();

                        _width = (int) attrs[0].GetInteger();
                    }
                    else if (head == "height")
                    {
                        if (attrs.Count != 1)
                            throw new ApplicationException();

                        _height = (int) attrs[0].GetInteger();
                    }
                    else if (head == "offsetx")
                    {
                        if (attrs.Count != 1)
                            throw new ApplicationException();

                        _offsetX = (int) attrs[0].GetFloat();
                    }
                    else if (head == "offsety")
                    {
                        if (attrs.Count != 1)
                            throw new ApplicationException();

                        _offsetY = (int) attrs[0].GetFloat();
                    }
                    else if (head == "pointgroup")
                    {
                        if (attrs.Count < 1)
                            throw new ApplicationException();

                        bool isFixed = attrs[0].GetInteger() != 0;

                        var pointGroup = new PointGroup(isFixed);

                        _pointGroups.Add((int) id, pointGroup);
                    }
                    else if (head == "contour")
                    {
                        PushLevel(Node.Contour);
                        _curContour = new Contour();
                    }
                    else if (head == "original-edge")
                    {
                        if (attrs.Count < 3)
                            throw new ApplicationException();

                        string type = attrs[0].GetWord();
                        long id0 = attrs[1].GetInteger();
                        long id1 = attrs[2].GetInteger();
                        PointGroup pg0 = _pointGroups[(int) id0];
                        PointGroup pg1 = _pointGroups[(int) id1];
                        Edge edge;

                        switch (type)
                        {
                            case "line":
                                {
                                    LineEdge lineEdge = new LineEdge(pg0.Points[0], pg1.Points[0]);
                                    edge = lineEdge;
                                }
                                break;

                            case "conic":
                            case "cubic":
                                throw new NotImplementedException();

                            default:
                                throw new ApplicationException();
                        }

                        pg0.OriginalOutgoingEdge = edge;
                        pg1.OriginalIncomingEdge = edge;
                    }
                    else
                    {
                        throw new ApplicationException();
                    }
                    break;

                case Node.Contour:
                    if (head == "point")
                    {
                        if (attrs.Count < 3)
                            throw new ApplicationException();

                        double x = attrs[0].GetFloat();
                        double y = attrs[1].GetFloat();
                        long groupId = attrs[2].GetInteger();
                        Point point = new Point(x, y);
                        PointGroup group = _pointGroups[(int) groupId];

                        _points.Add((int) id, point);

                        point.Group = group;
                        point.Contour = _curContour;
                        group.Points.Add(point);

                        if (_curContour.FirstPoint == null)
                            _curContour.FirstPoint = point;
                    }
                    else if (head == "edge")
                    {
                        if (attrs.Count < 3)
                            throw new ApplicationException();

                        string type = attrs[0].GetWord();
                        long id0 = attrs[1].GetInteger();
                        long id1 = attrs[2].GetInteger();
                        Point p0 = _points[(int) id0];
                        Point p1 = _points[(int) id1];
                        Edge edge;

                        switch (type)
                        {
                            case "line":
                                {
                                    LineEdge lineEdge = new LineEdge(p0, p1);
                                    edge = lineEdge;
                                }
                                break;

                            case "conic":
                            case "cubic":
                                throw new NotImplementedException();

                            default:
                                throw new ApplicationException();
                        }

                        p0.OutgoingEdge = edge;
                        p1.IncomingEdge = edge;
                    }
                    else
                    {
                        throw new ApplicationException();
                    }
                    break;
            }
        }

        protected override void OnEndRecord(string head)
        {
            if (head == null)
                Console.WriteLine("end");

            if (head == null)
                PopLevel();
        }

        internal void Deserialize()
        {
            Parse();

            Figure = new Figure(_pointGroups.Values, _width, _height, _offsetX, _offsetY);
        }
    }

    public class FigureDeserialzer
    {
        public static Figure Deserialize(TextReader reader)
        {
            var deserializer = new FigureParser(reader);

            deserializer.Deserialize();

            return deserializer.Figure;
        }
    }
}
