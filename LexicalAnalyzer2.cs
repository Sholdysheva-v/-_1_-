using System;
using System.Collections.Generic;
using System.Text;

namespace Лабораторная_1_компиляторы
{
    public class LexicalAnalyzer_2
    {
        public enum TokenType
        {
            UnsignedInt = 1,
            SignedInt = 3,
            FloatNumber = 4,
            Identifier = 2,
            Keyword = 14,
            Separator = 11,
            AssignmentOperator = 10,
            EndOfStatement = 16,
            TypeKeyword = 17,
            StructKeyword = 18,
            OpenBrace = 19,
            CloseBrace = 20,
            DollarSign = 21,
            StringLiteral = 22,
            PlusOperator = 23,
            MinusOperator = 24,
            Whitespace = 25,
            InvalidChar = 99
        }

        public class Token
        {
            public int Code { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
            public int StartPos { get; set; }
            public int EndPos { get; set; }

            public Token(int code, string type, string value, int startPos, int endPos)
            {
                Code = code;
                Type = type;
                Value = value;
                StartPos = startPos;
                EndPos = endPos;
            }

            public override string ToString()
            {
                return $"{Code} - {Type} - {Value} - с {StartPos} по {EndPos} символ";
            }
        }

        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            {"struct", TokenType.StructKeyword},
            {"string", TokenType.TypeKeyword},
            {"int", TokenType.TypeKeyword},
            {"bool", TokenType.TypeKeyword},
            {"float", TokenType.TypeKeyword},
            {"double", TokenType.TypeKeyword}
        };

        public List<Token> Analyze(string input)
        {
            List<Token> tokens = new List<Token>();
            int currentPos = 0;
            int lineNumber = 1;

            while (currentPos < input.Length)
            {
                char currentChar = input[currentPos];

                // Обработка пробельных символов
                if (char.IsWhiteSpace(currentChar))
                {
                    StringBuilder sb = new StringBuilder();
                    int startPos = currentPos + 1;

                    while (currentPos < input.Length && char.IsWhiteSpace(input[currentPos]))
                    {
                        sb.Append(input[currentPos]);
                        if (input[currentPos] == '\n') lineNumber++;
                        currentPos++;
                    }

                    tokens.Add(new Token(
                        (int)TokenType.Whitespace,
                        "пробельный символ",
                        sb.ToString(),
                        startPos,
                        currentPos
                    ));
                    continue;
                }

                // Обработка строковых литералов
                if (currentChar == '"')
                {
                    StringBuilder sb = new StringBuilder();
                    int startPos = currentPos + 1;
                    sb.Append(currentChar);
                    currentPos++;

                    bool escape = false;
                    bool closed = false;

                    while (currentPos < input.Length)
                    {
                        currentChar = input[currentPos];
                        sb.Append(currentChar);

                        if (currentChar == '\\' && !escape)
                        {
                            escape = true;
                        }
                        else if (currentChar == '"' && !escape)
                        {
                            closed = true;
                            currentPos++;
                            break;
                        }
                        else
                        {
                            escape = false;
                        }

                        currentPos++;
                    }

                    if (!closed)
                    {
                        tokens.Add(new Token(
                            (int)TokenType.InvalidChar,
                            "незакрытый строковый литерал",
                            sb.ToString(),
                            startPos,
                            currentPos
                        ));
                    }
                    else
                    {
                        tokens.Add(new Token(
                            (int)TokenType.StringLiteral,
                            "строковый литерал",
                            sb.ToString(),
                            startPos,
                            currentPos
                        ));
                    }
                    continue;
                }

                // Обработка операторов + и -
                if (currentChar == '+' || currentChar == '-')
                {
                    // Проверка на начало числа со знаком
                    if ((currentChar == '-' || currentChar == '+') &&
                        currentPos + 1 < input.Length &&
                        (char.IsDigit(input[currentPos + 1]) || input[currentPos + 1] == '.'))
                    {
                        StringBuilder sb = new StringBuilder();
                        int startPos = currentPos + 1;
                        sb.Append(currentChar);
                        currentPos++;

                        bool hasDecimalPoint = false;

                        // Собираем цифры и десятичную точку
                        while (currentPos < input.Length &&
                              (char.IsDigit(input[currentPos]) ||
                              (input[currentPos] == '.' && !hasDecimalPoint)))
                        {
                            if (input[currentPos] == '.')
                            {
                                hasDecimalPoint = true;
                            }
                            sb.Append(input[currentPos]);
                            currentPos++;
                        }

                        // Определяем тип числа
                        TokenType numberType = hasDecimalPoint ? TokenType.FloatNumber : TokenType.SignedInt;
                        string typeDescription = hasDecimalPoint ? "дробное число" : "целое со знаком";

                        tokens.Add(new Token(
                            (int)numberType,
                            typeDescription,
                            sb.ToString(),
                            startPos,
                            currentPos
                        ));
                    }
                    else
                    {
                        tokens.Add(new Token(
                            currentChar == '+' ? (int)TokenType.PlusOperator : (int)TokenType.MinusOperator,
                            currentChar == '+' ? "оператор +" : "оператор -",
                            currentChar.ToString(),
                            currentPos + 1,
                            currentPos + 1
                        ));
                        currentPos++;
                    }
                    continue;
                }

                // Обработка чисел (целых и дробных)
                if (char.IsDigit(currentChar) || currentChar == '.')
                {
                    StringBuilder sb = new StringBuilder();
                    int startPos = currentPos + 1;
                    bool hasDecimalPoint = false;
                    bool isNumberValid = true;

                    // Обработка случая, когда число начинается с точки
                    if (currentChar == '.')
                    {
                        if (currentPos + 1 < input.Length && char.IsDigit(input[currentPos + 1]))
                        {
                            hasDecimalPoint = true;
                            sb.Append(currentChar);
                            currentPos++;
                        }
                        else
                        {
                            isNumberValid = false;
                        }
                    }

                    if (isNumberValid)
                    {
                        // Собираем цифры и десятичную точку
                        while (currentPos < input.Length &&
                              (char.IsDigit(input[currentPos]) ||
                              (input[currentPos] == '.' && !hasDecimalPoint)))
                        {
                            if (input[currentPos] == '.')
                            {
                                hasDecimalPoint = true;
                            }
                            sb.Append(input[currentPos]);
                            currentPos++;
                        }

                        // Проверяем, что число не заканчивается на точку
                        if (sb.Length > 0 && sb[sb.Length - 1] == '.')
                        {
                            tokens.Add(new Token(
                                (int)TokenType.InvalidChar,
                                "некорректное число",
                                sb.ToString(),
                                startPos,
                                currentPos
                            ));
                        }
                        else
                        {
                            // Определяем тип числа
                            TokenType numberType = hasDecimalPoint ? TokenType.FloatNumber : TokenType.UnsignedInt;
                            string typeDescription = hasDecimalPoint ? "дробное число" : "целое без знака";

                            tokens.Add(new Token(
                                (int)numberType,
                                typeDescription,
                                sb.ToString(),
                                startPos,
                                currentPos
                            ));
                        }
                    }
                    else
                    {
                        tokens.Add(new Token(
                            (int)TokenType.InvalidChar,
                            "некорректное число",
                            currentChar.ToString(),
                            startPos,
                            startPos
                        ));
                        currentPos++;
                    }
                    continue;
                }

                // Обработка идентификаторов (начинаются с $)
                if (currentChar == '$')
                {
                    int startPos = currentPos + 1;
                    currentPos++;

                    if (currentPos < input.Length && IsLatinLetter(input[currentPos]))
                    {
                        StringBuilder sb = new StringBuilder();

                        while (currentPos < input.Length && (IsLatinLetterOrDigit(input[currentPos]) || input[currentPos] == '_'))
                        {
                            sb.Append(input[currentPos]);
                            currentPos++;
                        }

                        tokens.Add(new Token(
                            (int)TokenType.Identifier,
                            "идентификатор",
                            sb.ToString(),
                            startPos,
                            currentPos
                        ));
                    }
                    else
                    {
                        tokens.Add(new Token(
                            (int)TokenType.InvalidChar,
                            "недопустимый символ после $",
                            "$",
                            startPos,
                            currentPos
                        ));
                    }
                    continue;
                }

                // Обработка ключевых слов и идентификаторов
                if (char.IsLetter(currentChar) && IsLatinLetter(currentChar))
                {
                    StringBuilder sb = new StringBuilder();
                    int startPos = currentPos + 1;

                    while (currentPos < input.Length && IsLatinLetter(input[currentPos]))
                    {
                        sb.Append(input[currentPos]);
                        currentPos++;
                    }

                    string word = sb.ToString();
                    int endPos = currentPos;

                    // Проверка на ключевые слова
                    if (Keywords.ContainsKey(word))
                    {
                        tokens.Add(new Token(
                            (int)Keywords[word],
                            Keywords[word] == TokenType.StructKeyword ? "ключевое слово (struct)" : "ключевое слово типа",
                            word,
                            startPos,
                            endPos
                        ));
                    }
                    else
                    {
                        tokens.Add(new Token(
                            (int)TokenType.Identifier,
                            "идентификатор",
                            word,
                            startPos,
                            endPos
                        ));
                    }
                    continue;
                }

                // Обработка специальных символов
                switch (currentChar)
                {
                    case '{':
                        tokens.Add(new Token(
                            (int)TokenType.OpenBrace,
                            "открывающая фигурная скобка",
                            "{",
                            currentPos + 1,
                            currentPos + 1
                        ));
                        currentPos++;
                        continue;
                    case '}':
                        tokens.Add(new Token(
                            (int)TokenType.CloseBrace,
                            "закрывающая фигурная скобка",
                            "}",
                            currentPos + 1,
                            currentPos + 1
                        ));
                        currentPos++;
                        continue;
                    case ';':
                        tokens.Add(new Token(
                            (int)TokenType.EndOfStatement,
                            "конец оператора",
                            ";",
                            currentPos + 1,
                            currentPos + 1
                        ));
                        currentPos++;
                        continue;
                    default:
                        if (IsCyrillic(currentChar))
                        {
                            StringBuilder sb = new StringBuilder();
                            int startPos = currentPos + 1;

                            while (currentPos < input.Length && IsCyrillic(input[currentPos]))
                            {
                                sb.Append(input[currentPos]);
                                currentPos++;
                            }

                            tokens.Add(new Token(
                                (int)TokenType.InvalidChar,
                                "недопустимые символы (русские буквы)",
                                sb.ToString(),
                                startPos,
                                currentPos
                            ));
                        }
                        else
                        {
                            tokens.Add(new Token(
                                (int)TokenType.InvalidChar,
                                "недопустимый символ",
                                currentChar.ToString(),
                                currentPos + 1,
                                currentPos + 1
                            ));
                            currentPos++;
                        }
                        continue;
                }
            }

            return tokens;
        }

        private bool IsLatinLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        private bool IsLatinLetterOrDigit(char c)
        {
            return IsLatinLetter(c) || char.IsDigit(c);
        }

        private bool IsCyrillic(char c)
        {
            return (c >= 'А' && c <= 'я') || c == 'ё' || c == 'Ё';
        }
    }
}