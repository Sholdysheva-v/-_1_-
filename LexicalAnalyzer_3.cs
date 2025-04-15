using System;
using System.Collections.Generic;
using System.Text;

namespace Лабораторная_1_компиляторы
{
    public class LexicalAnalyzer
    {
        // Перечисление типов токенов с их числовыми кодами
        public enum TokenType
        {
            UnsignedInt = 1,         // Целое число без знака
            SignedInt = 3,           // Целое число со знаком
            Identifier = 2,          // Идентификатор (начинается с $)
            Keyword = 14,            // Ключевое слово
            Separator = 11,          // Разделитель (пробел)
            AssignmentOperator = 10, // Оператор присваивания
            EndOfStatement = 16,     // Конец оператора (;)
            TypeKeyword = 17,        // Ключевое слово, обозначающее тип данных
            StructKeyword = 18,      // Ключевое слово struct
            OpenBrace = 19,          // Открывающая фигурная скобка {
            CloseBrace = 20,         // Закрывающая фигурная скобка }
            DollarSign = 21,         // Знак доллара $
            StringLiteral = 22,      // Строковый литерал (в кавычках)
            PlusOperator = 23,      // Оператор +
            MinusOperator = 24,      // Оператор -
            InvalidChar = 99         // Недопустимый символ
        }

        // Класс для представления токена
        public class Token
        {
            public int Code { get; set; }       // Код токена (из TokenType)
            public string Type { get; set; }    // Текстовое описание типа
            public string Value { get; set; }   // Значение токена
            public int StartPos { get; set; }   // Начальная позиция в тексте
            public int EndPos { get; set; }     // Конечная позиция в тексте

            // Конструктор токена
            public Token(int code, string type, string value, int startPos, int endPos)
            {
                Code = code;
                Type = type;
                Value = value;
                StartPos = startPos;
                EndPos = endPos;
            }

            // Переопределение метода ToString для удобного вывода
            public override string ToString()
            {
                return $"{Code} - {Type} - {Value} - с {StartPos} по {EndPos} символ";
            }
        }

        // Словарь ключевых слов и соответствующих им типов токенов
        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            {"struct", TokenType.StructKeyword},  // Ключевое слово struct
            {"string", TokenType.TypeKeyword},     // Ключевое слово для типа string
            {"int", TokenType.TypeKeyword},       // Ключевое слово для типа int
            {"bool", TokenType.TypeKeyword},      // Ключевое слово для типа bool
            {"float", TokenType.TypeKeyword},      // Ключевое слово для типа float
            {"double", TokenType.TypeKeyword}      // Ключевое слово для типа float
        };

        // Основной метод анализа входной строки
        public List<Token> Analyze(string input)
        {
            List<Token> tokens = new List<Token>(); // Список для хранения токенов
            int currentPos = 0;                     // Текущая позиция в строке
            int lineNumber = 1;                    // Номер текущей строки (для отслеживания переносов)

            // Основной цикл обработки входной строки
            while (currentPos < input.Length)
            {
                char currentChar = input[currentPos]; // Текущий символ

                // Обработка пробельных символов
                if (char.IsWhiteSpace(currentChar))
                {
                    if (currentChar == '\n')
                    {
                        lineNumber++; // Увеличиваем номер строки при переносе
                    }
                    currentPos++;
                    continue; // Пропускаем пробелы
                }

                // Обработка строковых литералов (в кавычках)
                if (currentChar == '"')
                {
                    StringBuilder sb = new StringBuilder();
                    int startPos = currentPos + 1; // Позиция начала строки (без кавычки)
                    sb.Append(currentChar); // Добавляем открывающую кавычку
                    currentPos++;

                    bool escape = false; // Флаг экранирования
                    bool closed = false; // Флаг закрытия строки

                    // Собираем содержимое строкового литерала
                    while (currentPos < input.Length)
                    {
                        currentChar = input[currentPos];
                        sb.Append(currentChar);

                        // Обработка экранированных символов
                        if (currentChar == '\\' && !escape)
                        {
                            escape = true;
                        }
                        else if (currentChar == '"' && !escape)
                        {
                            closed = true; // Найдена закрывающая кавычка
                            currentPos++;
                            break;
                        }
                        else
                        {
                            escape = false;
                        }

                        currentPos++;
                    }

                    // Если строка не закрыта, добавляем ошибку
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
                        // Добавляем корректный строковый литерал
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
                    // Специальная проверка для минуса (может быть частью числа)
                    if (currentChar == '-' && currentPos + 1 < input.Length && char.IsDigit(input[currentPos + 1]))
                    {
                        StringBuilder sb = new StringBuilder();
                        int startPos = currentPos + 1;
                        sb.Append(currentChar); // Добавляем минус
                        currentPos++;

                        // Собираем все цифры после минуса
                        while (currentPos < input.Length && char.IsDigit(input[currentPos]))
                        {
                            sb.Append(input[currentPos]);
                            currentPos++;
                        }

                        // Добавляем число со знаком
                        tokens.Add(new Token(
                            (int)TokenType.SignedInt,
                            "целое со знаком",
                            sb.ToString(),
                            startPos,
                            currentPos
                        ));
                    }
                    else
                    {
                        // Добавляем оператор + или -
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

                // Обработка идентификаторов (начинаются с $)
                if (currentChar == '$')
                {
                    int startPos = currentPos + 1;
                    currentPos++;

                    // Проверяем, что после $ идет латинская буква
                    if (currentPos < input.Length && IsLatinLetter(input[currentPos]))
                    {
                        StringBuilder sb = new StringBuilder();

                        // Собираем все допустимые символы идентификатора
                        while (currentPos < input.Length && (IsLatinLetterOrDigit(input[currentPos]) || input[currentPos] == '_'))
                        {
                            sb.Append(input[currentPos]);
                            currentPos++;
                        }

                        // Добавляем идентификатор
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
                        // Ошибка - после $ нет буквы
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

                // Обработка ключевых слов и других буквенных последовательностей
                if (char.IsLetter(currentChar) && IsLatinLetter(currentChar))
                {
                    StringBuilder sb = new StringBuilder();
                    int startPos = currentPos + 1;

                    // Собираем последовательность латинских букв
                    while (currentPos < input.Length && IsLatinLetter(input[currentPos]))
                    {
                        sb.Append(input[currentPos]);
                        currentPos++;
                    }

                    string word = sb.ToString();
                    int endPos = currentPos;

                    // Проверяем, является ли слово ключевым
                    bool isKeyword = Keywords.TryGetValue(word, out TokenType keywordType);

                    // Проверка на частичное совпадение с ключевыми словами
                    bool hasPartialKeyword = false;
                    foreach (var kw in Keywords.Keys)
                    {
                        if (word.StartsWith(kw) && word != kw && word.Length > kw.Length)
                        {
                            // Разделяем на ключевое слово и остаток
                            string keywordPart = kw;
                            string remainingPart = word.Substring(kw.Length);

                            // Добавляем часть, которая является ключевым словом
                            tokens.Add(new Token(
                                (int)Keywords[kw],
                                Keywords[kw] == TokenType.StructKeyword ? "ключевое слово (struct)" : "ключевое слово типа",
                                keywordPart,
                                startPos,
                                startPos + kw.Length - 1
                            ));

                            // Добавляем остаток как ошибку
                            tokens.Add(new Token(
                                (int)TokenType.InvalidChar,
                                "недопустимые символы",
                                remainingPart,
                                startPos + kw.Length,
                                endPos
                            ));

                            hasPartialKeyword = true;
                            break;
                        }
                    }

                    if (hasPartialKeyword)
                    {
                        continue;
                    }

                    // Если это ключевое слово
                    if (isKeyword)
                    {
                        tokens.Add(new Token(
                            (int)keywordType,
                            keywordType == TokenType.StructKeyword ? "ключевое слово (struct)" : "ключевое слово типа",
                            word,
                            startPos,
                            endPos
                        ));

                        // Обработка идентификатора после ключевого слова и пробела
                        if (currentPos < input.Length && input[currentPos] == ' ')
                        {
                            currentPos++;

                            if (currentPos < input.Length && IsLatinLetter(input[currentPos]))
                            {
                                sb = new StringBuilder();
                                startPos = currentPos + 1;

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
                        }
                    }
                    else
                    {
                        // Если это не ключевое слово, добавляем как ошибку
                        tokens.Add(new Token(
                            (int)TokenType.InvalidChar,
                            "недопустимые символы",
                            word,
                            startPos,
                            endPos
                        ));
                    }
                    continue;
                }

                // Обработка целых чисел без знака
                if (char.IsDigit(currentChar))
                {
                    StringBuilder sb = new StringBuilder();
                    int startPos = currentPos + 1;

                    // Собираем последовательность цифр
                    while (currentPos < input.Length && char.IsDigit(input[currentPos]))
                    {
                        sb.Append(input[currentPos]);
                        currentPos++;
                    }

                    // Добавляем число без знака
                    tokens.Add(new Token(
                        (int)TokenType.UnsignedInt,
                        "целое без знака",
                        sb.ToString(),
                        startPos,
                        currentPos
                    ));
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
                        // Обработка русских букв как ошибки
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
                            // Любой другой недопустимый символ
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

            return tokens; // Возвращаем список распознанных токенов
        }

        // Проверка, является ли символ латинской буквой
        private bool IsLatinLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        // Проверка, является ли символ латинской буквой или цифрой
        private bool IsLatinLetterOrDigit(char c)
        {
            return IsLatinLetter(c) || char.IsDigit(c);
        }

        // Проверка, является ли символ кириллическим
        private bool IsCyrillic(char c)
        {
            return (c >= 'А' && c <= 'я') || c == 'ё' || c == 'Ё';
        }
    }
}