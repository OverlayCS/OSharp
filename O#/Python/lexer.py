#OSharp Lexer In Python

import re
from enum import Enum, auto

class O_TokenType(Enum):
    KEYWORD = auto()
    IDENTIFIER = auto()
    NUMBER = auto()
    STRING = auto()
    SYMBOL = auto()
    EOF = auto()

class O_Token:
    def __init__(self, type_: O_TokenType, value: str):
        self.type = type_
        self.value = value

    def __repr__(self):
        return f"{self.type.name}({self.value})"
    
class O_Lexer:
    SYMBOLS = r"[\{\}\(\)\[\];,:\=\+\-\*/\.]"
    KEYWORDS = {
        "public", "private", "static", "internal", "void", "int", "float",
        "double", "long", "short", "for", "foreach", "if", "else", "try",
        "error", "final", "while", "continue", "end", "parallel", "new",
        "OnChanged"
    }

    def __init__(self, source: str):
        self.source = source
        self.position = 0
        self.length = len(source)

    def tokenize(self):
        tokens = []
        while self.position < self.length:
            current_char = self.source[self.position]

            if current_char.isspace():
                self.position += 1
                continue

            if current_char.isalpha() or current_char == '_':
                start = self.position
                while (self.position < self.length and 
                       (self.source[self.position].isalnum() or self.source[self.position] == '_')):
                    self.position += 1
                value = self.source[start:self.position]
                token_type = O_TokenType.KEYWORD if value in self.KEYWORDS else O_TokenType.IDENTIFIER
                tokens.append(O_Token(token_type, value))
                continue

            if current_char.isdigit():
                start = self.position
                while (self.position < self.length and self.source[self.position].isdigit()):
                    self.position += 1
                if (self.position < self.length and self.source[self.position] == '.'):
                    self.position += 1
                    while (self.position < self.length and self.source[self.position].isdigit()):
                        self.position += 1
                value = self.source[start:self.position]
                tokens.append(O_Token(O_TokenType.NUMBER, value))
                continue

            if current_char == '"':
                start = self.position + 1
                self.position += 1
                while (self.position < self.length and self.source[self.position] != '"'):
                    if self.source[self.position] == '\\':
                        self.position += 2
                    else:
                        self.position += 1
                value = self.source[start:self.position]
                tokens.append(O_Token(O_TokenType.STRING, value))
                self.position += 1
                continue

            if re.match(self.SYMBOLS, current_char):
                tokens.append(O_Token(O_TokenType.SYMBOL, current_char))
                self.position += 1
                continue

            raise Exception(f"Unexpected character: {current_char}")

        tokens.append(O_Token(O_TokenType.EOF, ""))
        return tokens
    
if __name__ == "__main__":
    code = '''
    public int age = 20, : OnChanged
    {
        Console.Message(age.ToUsableString());
    }
    '''

    lexer = O_Lexer(code)
    tokens = lexer.tokenize()
    for t in tokens:
        print(t)