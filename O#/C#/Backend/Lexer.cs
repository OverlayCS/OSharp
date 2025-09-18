using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OSharp.Backend
{
    public enum TokenType
    {
        Identifier, Number, String, Symbol, Whitespace, Comment, EndOfFile, Keyword,

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
        public int GlobalIndex { get; set; } = 0;
        public string Source { get; set; }
        public int Line = 1;
        public int Column = 1;

        public readonly List<string> KEYWORDS = new()
        {
            "public","private","internal","static","void","return",
            "int","float","double","long","string","bool",
            "OnChanged",
            "safe","unsafe","io","net","all","parallel",
            "if","else","for","foreach","while","continue","end","try","error","final","new",
            "Console","NetworkServer","NetworkConnector","NetworkUtils"
        };

        public readonly HashSet<char> Symbols = new()
        {
            '{','}','(',')','[',']',';',',',':','=','+','-','*','/','.'
        };

        public Lexer(string source)
        {
            Source = source ?? string.Empty;
        }
    }
}
