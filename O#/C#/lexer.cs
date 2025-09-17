
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OSharpLexer
{
    public enum O_TokenType
    {
        KEYWORD,
        IDENTIFIER,
        NUMBER,
        STRING,
        SYMBOL,
        EOF
    }

    public class O_Token
    {
        public O_TokenType Type { get; }
        public string Value { get; }

        public O_Token(O_TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString() => $"{Type}({Value})";
    }

    public class O_Lexer
    {
        private static readonly string SYMBOLS = @"[\{\}\(\)\[\];,:\=\+\-\*/\.]";
        private static readonly HashSet<string> KEYWORDS = new()
        {
            "public", "private", "static", "internal", "void", "int", "float",
            "double", "long", "short", "for", "foreach", "if", "else", "try",
            "error", "final", "while", "continue", "end", "parallel", "new",
            "OnChanged"
        };

        private readonly string source;
        private int position;
        private readonly int length;

        public O_Lexer(string source)
        {
            this.source = source;
            this.position = 0;
            this.length = source.Length;
        }

        public List<O_Token> Tokenize()
        {
            var tokens = new List<O_Token>();

            while (position < length)
            {
                char currentChar = source[position];

                if (char.IsWhiteSpace(currentChar))
                {
                    position++;
                    continue;
                }

                // Identifier or keyword
                if (char.IsLetter(currentChar) || currentChar == '_')
                {
                    int start = position;
                    while (position < length && (char.IsLetterOrDigit(source[position]) || source[position] == '_'))
                        position++;
                    string value = source[start..position];
                    O_TokenType type = KEYWORDS.Contains(value) ? O_TokenType.KEYWORD : O_TokenType.IDENTIFIER;
                    tokens.Add(new O_Token(type, value));
                    continue;
                }

                // Number
                if (char.IsDigit(currentChar))
                {
                    int start = position;
                    while (position < length && char.IsDigit(source[position]))
                        position++;

                    // Decimal part
                    if (position < length && source[position] == '.')
                    {
                        position++;
                        while (position < length && char.IsDigit(source[position]))
                            position++;
                    }

                    string value = source[start..position];
                    tokens.Add(new O_Token(O_TokenType.NUMBER, value));
                    continue;
                }

                // String
                if (currentChar == '"')
                {
                    int start = position + 1;
                    position++;
                    while (position < length && source[position] != '"')
                    {
                        if (source[position] == '\\' && position + 1 < length)
                            position += 2; // skip escaped character
                        else
                            position++;
                    }
                    string value = source[start..position];
                    tokens.Add(new O_Token(O_TokenType.STRING, value));
                    position++; // skip closing quote
                    continue;
                }

                // Symbols
                if (Regex.IsMatch(currentChar.ToString(), SYMBOLS))
                {
                    tokens.Add(new O_Token(O_TokenType.SYMBOL, currentChar.ToString()));
                    position++;
                    continue;
                }

                throw new Exception($"Unexpected character: {currentChar}");
            }

            tokens.Add(new O_Token(O_TokenType.EOF, ""));
            return tokens;
        }
    }

    class Program
    {
        static void Main()
        {
            string code = @"
            public int age = 20, : OnChanged
            {
                Console.Message(age.ToUsableString());
            }
            ";

            var lexer = new O_Lexer(code);
            var tokens = lexer.Tokenize();
            foreach (var t in tokens)
                Console.WriteLine(t);

            Console.ReadKey();
        }
    }
}
