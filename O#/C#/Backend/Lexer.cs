using System;
using System.Collections.Generic;
using System.Text;

namespace OSharp.Backend
{
    public enum TokenType
    {
        Identifier, Number, String, Symbol, Whitespace, Comment, EndOfFile, Keyword,

        // Keywords
        Public, Private, Internal, Static, Void, Return,
        Int, Float, Double, Long, StringType, Bool,
        OnChanged,
        Safe, Unsafe, IO, Net, All, Parallel,
        If, Else, For, Foreach, While, Continue, End, Try, Error, Final, New,
        Console, NetworkServer, NetworkConnector, NetworkUtils
    }

    public class Token
    {
        public TokenType Type;
        public string Lexeme;
        public int Line;
        public int Column;

        public Token(TokenType type, string lexeme, int line, int column)
        {
            Type = type;
            Lexeme = lexeme;
            Line = line;
            Column = column;
        }

        public override string ToString() => $"{Type}('{Lexeme}') at {Line}:{Column}";
    }

    public class Lexer
    {
        private readonly string _source;
        private int _index = 0;
        private int _line = 1;
        private int _column = 1;

        private static readonly HashSet<string> Keywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "public","private","internal","static","void","return",
            "int","float","double","long","string","bool",
            "OnChanged",
            "safe","unsafe","io","net","all","parallel",
            "if","else","for","foreach","while","continue","end","try","error","final","new",
            "Console","NetworkServer","NetworkConnector","NetworkUtils"
        };

        private static readonly HashSet<char> Symbols = new()
        {
            '{','}','(',')','[',']',';',',',':','=','+','-','*','/','.'
        };

        public Lexer(string source) => _source = source ?? "";

        public IEnumerable<Token> Tokenize()
        {
            while (!IsAtEnd())
            {
                char c = Peek();
                if (char.IsWhiteSpace(c))
                    yield return ConsumeWhitespace();
                else if (char.IsLetter(c) || c == '_')
                    yield return ConsumeIdentifierOrKeyword();
                else if (char.IsDigit(c))
                    yield return ConsumeNumber();
                else if (c == '"')
                    yield return ConsumeString();
                else if (c == '/' && PeekNext() == '/')
                    yield return ConsumeComment();
                else if (Symbols.Contains(c))
                    yield return ConsumeSymbol();
                else
                    yield return ConsumeUnknown();
            }

            yield return new Token(TokenType.EndOfFile, string.Empty, _line, _column);
        }

        private Token ConsumeWhitespace()
        {
            int startLine = _line, startCol = _column;
            var sb = new StringBuilder();

            while (!IsAtEnd() && char.IsWhiteSpace(Peek()))
            {
                char c = Next();
                sb.Append(c);
                if (c == '\n') { _line++; _column = 1; }
            }

            return new Token(TokenType.Whitespace, sb.ToString(), startLine, startCol);
        }

        private Token ConsumeIdentifierOrKeyword()
        {
            int startLine = _line, startCol = _column;
            int startIndex = _index;

            while (!IsAtEnd() && (char.IsLetterOrDigit(Peek()) || Peek() == '_'))
                Next();

            string lexeme = _source.Substring(startIndex, _index - startIndex);

            TokenType type = Keywords.Contains(lexeme)
                ? Enum.TryParse<TokenType>(lexeme, true, out var kwType) ? kwType : TokenType.Keyword
                : TokenType.Identifier;

            if (lexeme.Equals("string", StringComparison.OrdinalIgnoreCase))
                type = TokenType.StringType;

            return new Token(type, lexeme, startLine, startCol);
        }

        private Token ConsumeNumber()
        {
            int startLine = _line, startCol = _column;
            int startIndex = _index;

            while (!IsAtEnd() && char.IsDigit(Peek()))
                Next();

            if (!IsAtEnd() && Peek() == '.')
            {
                Next();
                while (!IsAtEnd() && char.IsDigit(Peek()))
                    Next();
            }

            string lexeme = _source.Substring(startIndex, _index - startIndex);
            return new Token(TokenType.Number, lexeme, startLine, startCol);
        }

        private Token ConsumeString()
        {
            int startLine = _line, startCol = _column;
            var sb = new StringBuilder();
            Next(); // consume opening quote

            while (!IsAtEnd() && Peek() != '"')
            {
                char c = Next();
                if (c == '\\' && !IsAtEnd())
                {
                    char esc = Next();
                    c = esc switch
                    {
                        'n' => '\n',
                        't' => '\t',
                        '"' => '"',
                        '\\' => '\\',
                        _ => esc
                    };
                }
                sb.Append(c);
            }

            if (!IsAtEnd()) Next(); // consume closing quote
            return new Token(TokenType.String, sb.ToString(), startLine, startCol);
        }

        private Token ConsumeComment()
        {
            int startLine = _line, startCol = _column;
            Next(); Next(); // consume '//'

            var sb = new StringBuilder();
            while (!IsAtEnd() && Peek() != '\n')
                sb.Append(Next());

            return new Token(TokenType.Comment, sb.ToString(), startLine, startCol);
        }

        private Token ConsumeSymbol()
        {
            int startLine = _line, startCol = _column;
            char c = Next();
            if (c == '\n') { _line++; _column = 1; }
            return new Token(TokenType.Symbol, c.ToString(), startLine, startCol);
        }

        private Token ConsumeUnknown()
        {
            int startLine = _line, startCol = _column;
            char c = Next();
            return new Token(TokenType.Symbol, c.ToString(), startLine, startCol);
        }

        private char Peek() => !IsAtEnd() ? _source[_index] : '\0';
        private char PeekNext() => (_index + 1 < _source.Length) ? _source[_index + 1] : '\0';
        private char Next() { char c = _source[_index++]; _column++; return c; }
        private bool IsAtEnd() => _index >= _source.Length;
    }
}
