/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KiriFig
{
    internal abstract class Parser
    {
        protected struct Token
        {
            public TokenType Type;
            public int IntValue;
            public string StringValue;

            public int GetInteger()
            {
                if ( Type != TokenType.Integer )
                    throw new ApplicationException();

                return IntValue;
            }

            public string GetWord()
            {
                if ( Type != TokenType.Word )
                    throw new ApplicationException();

                return StringValue;
            }
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

        private StringBuilder _tokenChars = new StringBuilder();
        private TokenType _tokenType;
        private int _intVal;

        internal Parser( TextReader reader )
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
            while ( !IsAtEof() )
            {
                ReadRecord();
            }

            if ( _nestingLevel != 0 )
                throw new ApplicationException();
        }

        private void ReadRecord()
        {
            SkipWhitespace();

            int id = -1;

            if ( _iChar == '-' || CharIsDigit( _iChar ) )
            {
                id = ReadInteger();
            }

            ReadRecord( id );
        }

        private void ReadRecord( int id )
        {
            ReadToken();

            if ( IsAtEof() || _tokenType == TokenType.Eol )
            {
                if ( id >= 0 )
                    throw new ApplicationException();

                return;
            }

            if ( _tokenType != TokenType.Word )
                throw new ApplicationException();

            if ( WordMatches( "end" ) )
            {
                OnEndRecord();

                _nestingLevel--;

                if ( _nestingLevel < 0 )
                    throw new ApplicationException();
            }
            else
            {
                string head = _tokenChars.ToString();
                bool openRecord = false;
                var attrs = new List<Token>();

                ReadToken();

                while ( _tokenType != TokenType.Eol && _tokenType != TokenType.Eof )
                {
                    if ( _tokenType == TokenType.Word && WordMatches( "begin" ) )
                    {
                        ReadToken();

                        if ( _tokenType != TokenType.Eol )
                            throw new ApplicationException();

                        _nestingLevel++;
                        openRecord = true;
                    }
                    else
                    {
                        var token = WrapCurrentToken();

                        attrs.Add( token );

                        ReadToken();
                    }
                }

                OnBeginRecord( id, head, attrs, openRecord );
            }
        }

        private Token WrapCurrentToken()
        {
            Token token = new Token();
            token.Type = _tokenType;

            switch ( _tokenType )
            {
                case TokenType.Word:
                    token.StringValue = _tokenChars.ToString();
                    break;

                case TokenType.Integer:
                    token.IntValue = _intVal;
                    break;
            }

            return token;
        }

        private bool WordMatches( string str )
        {
            if ( _tokenType != TokenType.Word )
                throw new ApplicationException();

            if ( _tokenChars.Length != str.Length )
                return false;

            for ( int i = 0; i < str.Length; i++ )
            {
                if ( _tokenChars[i] != str[i] )
                    return false;
            }

            return true;
        }

        private void ReadToken()
        {
            SkipWhitespace();

            if ( _iChar < 0 )
                _tokenType = TokenType.Eof;
            else if ( _iChar == '\r' || _iChar == '\n' )
                ReadEol();
            else if ( CharIsLetter( _iChar ) )
                ReadWord();
            else if ( _iChar == '-' || CharIsDigit( _iChar ) )
                ReadNumber();
            else
                throw new ApplicationException();
        }

        private static bool CharIsLetter( int c )
        {
            if ( c < 0 )
                return false;

            return char.IsLetter( (char) c );
        }

        private static bool CharIsDigit( int c )
        {
            if ( c < 0 )
                return false;

            return char.IsDigit( (char) c );
        }

        private static bool CharIsLetterOrDigit( int c )
        {
            if ( c < 0 )
                return false;

            return char.IsLetterOrDigit( (char) c );
        }

        private static bool CharIsWhiteSpace( int c )
        {
            if ( c < 0 )
                return false;

            char ch = (char) c;

            return ch == ' ' || ch == '\t';
        }

        private static bool CharIsSeparator( int c )
        {
            if ( c < 0 )
                return true;

            char ch = (char) c;

            return ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n';
        }

        private void ReadEol()
        {
            if ( _iChar == '\r' )
            {
                _tokenType = TokenType.Eol;
                ReadChar();
            }

            if ( _iChar == '\n' )
            {
                _tokenType = TokenType.Eol;
                ReadChar();
            }
        }

        private void ReadNumber()
        {
            bool negate = false;

            _tokenChars.Clear();

            if ( _iChar == '-' )
            {
                negate = true;
                ReadChar();
            }

            do
            {
                if ( !CharIsDigit( _iChar ) )
                    throw new ApplicationException();

                TokenAppendChar( _iChar );
                ReadChar();
            }
            while ( !CharIsSeparator( _iChar ) );

            _tokenType = TokenType.Integer;
            _intVal = Convert.ToInt32( _tokenChars.ToString() );
            if ( negate )
                _intVal = -_intVal;
        }

        private int ReadInteger()
        {
            ReadNumber();

            if ( _tokenType != TokenType.Integer )
                throw new ApplicationException();

            return _intVal;
        }

        private void ReadWord()
        {
            _tokenChars.Clear();

            do
            {
                if ( !CharIsLetterOrDigit( _iChar ) && _iChar != '-' )
                    throw new ApplicationException();

                TokenAppendChar( _iChar );
                ReadChar();
            }
            while ( !CharIsSeparator( _iChar ) );

            _tokenType = TokenType.Word;
        }

        private void TokenAppendChar( int iChar )
        {
            _tokenChars.Append( (char) iChar );
        }

        private void SkipWhitespace()
        {
            while ( CharIsWhiteSpace( _iChar ) )
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

            if ( _iChar < 0 )
                return;

            if ( char.IsControl( (char) _iChar ) && _iChar != '\r' && _iChar != '\n' && _iChar != '\t' )
                throw new ApplicationException();
        }

        protected abstract void OnBeginRecord( int id, string head, IList<Token> attrs, bool open );
        protected abstract void OnEndRecord();
    }
}
