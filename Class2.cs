//using System;
//using System.Collections.Generic;
//using System.Windows.Forms;

//namespace Лабораторная_1_компиляторы
//{
//    public class SyntaxAnalyzer
//    {
//        private enum State
//        {
//            Start,
//            AfterStruct,
//            AfterIdentifier,
//            AfterOpenBrace,
//            AfterFieldType,
//            AfterFieldName,
//            AfterSemicolon,
//            AfterCloseBrace,
//            Error
//        }

//        public class SyntaxError
//        {
//            public string Message { get; }
//            public string Fragment { get; }
//            public int Position { get; }
//            public string Recommendation { get; }

//            public SyntaxError(string message, string fragment, int position, string recommendation)
//            {
//                Message = message;
//                Fragment = fragment;
//                Position = position;
//                Recommendation = recommendation;
//            }
//        }

//        public List<SyntaxError> Analyze(List<LexicalAnalyzer.Token> tokens)
//        {
//            List<SyntaxError> errors = new List<SyntaxError>();
//            State currentState = State.Start;
//            int i = 0;
//            int structStart = -1;

//            while (i < tokens.Count)
//            {
//                var token = tokens[i];
//                bool transitionFound = false;

//                switch (currentState)
//                {
//                    case State.Start:
//                        if (token.Code == (int)LexicalAnalyzer.TokenType.StructKeyword)
//                        {
//                            currentState = State.AfterStruct;
//                            structStart = i;
//                            transitionFound = true;
//                        }
//                        else
//                        {
//                            errors.Add(new SyntaxError(
//                                "Ожидалось ключевое слово 'struct'",
//                                token.Value,
//                                token.StartPos,
//                                "Начните объявление структуры с ключевого слова 'struct'"));
//                            i++; // Пропускаем некорректный токен
//                        }
//                        break;

//                    case State.AfterStruct:
//                        if (token.Code == (int)LexicalAnalyzer.TokenType.Identifier)
//                        {
//                            currentState = State.AfterIdentifier;
//                            transitionFound = true;
//                        }
//                        else
//                        {
//                            errors.Add(new SyntaxError(
//                                "Ожидалось имя структуры",
//                                token.Value,
//                                token.StartPos,
//                                "Укажите имя структуры после 'struct'"));
//                            currentState = State.Error;
//                        }
//                        break;

//                    case State.AfterIdentifier:
//                        if (token.Code == (int)LexicalAnalyzer.TokenType.OpenBrace)
//                        {
//                            currentState = State.AfterOpenBrace;
//                            transitionFound = true;
//                        }
//                        else
//                        {
//                            errors.Add(new SyntaxError(
//                                "Ожидалась открывающая фигурная скобка '{'",
//                                token.Value,
//                                token.StartPos,
//                                "После имени структуры должна идти '{'"));
//                            currentState = State.Error;
//                        }
//                        break;

//                    case State.AfterOpenBrace:
//                        if (token.Code == (int)LexicalAnalyzer.TokenType.TypeKeyword)
//                        {
//                            currentState = State.AfterFieldType;
//                            transitionFound = true;
//                        }
//                        else if (token.Code == (int)LexicalAnalyzer.TokenType.CloseBrace)
//                        {
//                            currentState = State.AfterCloseBrace;
//                            transitionFound = true;
//                        }
//                        else
//                        {
//                            errors.Add(new SyntaxError(
//                                "Ожидался тип поля или закрывающая скобка '}'",
//                                token.Value,
//                                token.StartPos,
//                                "Добавьте тип поля или закройте структуру"));
//                            currentState = State.Error;
//                        }
//                        break;

//                    case State.AfterFieldType:
//                        if (token.Code == (int)LexicalAnalyzer.TokenType.Identifier)
//                        {
//                            currentState = State.AfterFieldName;
//                            transitionFound = true;
//                        }
//                        else
//                        {
//                            errors.Add(new SyntaxError(
//                                "Ожидалось имя поля",
//                                token.Value,
//                                token.StartPos,
//                                "Укажите имя поля после типа"));
//                            currentState = State.Error;
//                        }
//                        break;

//                    case State.AfterFieldName:
//                        if (token.Code == (int)LexicalAnalyzer.TokenType.EndOfStatement)
//                        {
//                            currentState = State.AfterSemicolon;
//                            transitionFound = true;
//                        }
//                        else
//                        {
//                            errors.Add(new SyntaxError(
//                                "Ожидалась точка с запятой ';'",
//                                token.Value,
//                                token.StartPos,
//                                "Завершите объявление поля точкой с запятой"));
//                            currentState = State.Error;
//                        }
//                        break;

//                    case State.AfterSemicolon:
//                        if (token.Code == (int)LexicalAnalyzer.TokenType.TypeKeyword)
//                        {
//                            currentState = State.AfterFieldType;
//                            transitionFound = true;
//                        }
//                        else if (token.Code == (int)LexicalAnalyzer.TokenType.CloseBrace)
//                        {
//                            currentState = State.AfterCloseBrace;
//                            transitionFound = true;
//                        }
//                        else
//                        {
//                            errors.Add(new SyntaxError(
//                                "Ожидался тип следующего поля или закрывающая скобка '}'",
//                                token.Value,
//                                token.StartPos,
//                                "Добавьте тип поля или закройте структуру"));
//                            currentState = State.Error;
//                        }
//                        break;

//                    case State.AfterCloseBrace:
//                        if (token.Code == (int)LexicalAnalyzer.TokenType.EndOfStatement)
//                        {
//                            currentState = State.Start;
//                            transitionFound = true;
//                        }
//                        else
//                        {
//                            errors.Add(new SyntaxError(
//                                "Ожидалась точка с запятой ';'",
//                                token.Value,
//                                token.StartPos,
//                                "Завершите объявление структуры точкой с запятой"));
//                            currentState = State.Error;
//                        }
//                        break;

//                    case State.Error:
//                        // Пытаемся найти начало следующей структуры
//                        if (token.Code == (int)LexicalAnalyzer.TokenType.StructKeyword)
//                        {
//                            currentState = State.AfterStruct;
//                            structStart = i;
//                            transitionFound = true;
//                        }
//                        else
//                        {
//                            i++; // Продолжаем поиск
//                        }
//                        break;
//                }

//                if (transitionFound)
//                {
//                    i++;
//                }
//                else if (currentState == State.Error)
//                {
//                    // Пропускаем токены до следующей возможной структуры
//                    while (i < tokens.Count && tokens[i].Code != (int)LexicalAnalyzer.TokenType.StructKeyword)
//                    {
//                        i++;
//                    }

//                    if (i < tokens.Count)
//                    {
//                        currentState = State.Start;
//                    }
//                }
//            }

//            // Проверяем незавершенные конструкции
//            if (currentState != State.Start)
//            {
//                // Заменяем рекурсивный шаблон на обычный switch-case
//                string errorMessage;
//                switch (currentState)
//                {
//                    case State.AfterStruct:
//                        errorMessage = "Незавершенное объявление структуры: ожидалось имя структуры";
//                        break;
//                    case State.AfterIdentifier:
//                        errorMessage = "Незавершенное объявление структуры: ожидалась '{'";
//                        break;
//                    case State.AfterOpenBrace:
//                        errorMessage = "Незавершенное тело структуры";
//                        break;
//                    case State.AfterFieldType:
//                        errorMessage = "Незавершенное объявление поля: ожидалось имя поля";
//                        break;
//                    case State.AfterFieldName:
//                        errorMessage = "Незавершенное объявление поля: ожидалась ';'";
//                        break;
//                    case State.AfterSemicolon:
//                        errorMessage = "Незавершенное тело структуры";
//                        break;
//                    case State.AfterCloseBrace:
//                        errorMessage = "Незавершенное объявление структуры: ожидалась ';'";
//                        break;
//                    default:
//                        errorMessage = "Незавершенная конструкция";
//                        break;
//                }

//                int position = structStart >= 0 ? tokens[structStart].StartPos : 0;
//                errors.Add(new SyntaxError(
//                    errorMessage,
//                    "",
//                    position,
//                    "Завершите конструкцию согласно синтаксису"));
//            }

//            return errors;
//        }

//        public void PrintResultsToRichTextBox(List<SyntaxError> errors, RichTextBox richTextBox)
//        {
//            richTextBox.Clear();

//            if (errors.Count == 0)
//            {
//                richTextBox.SelectionColor = System.Drawing.Color.Green;
//                richTextBox.AppendText("Синтаксический анализ завершен успешно!\n");
//                richTextBox.AppendText("Ошибок не обнаружено.\n");
//                return;
//            }

//            richTextBox.SelectionColor = System.Drawing.Color.Red;
//            richTextBox.AppendText($"Найдено {errors.Count} ошибок:\n\n");

//            foreach (var error in errors)
//            {
//                richTextBox.SelectionColor = System.Drawing.Color.DarkRed;
//                richTextBox.AppendText("• Ошибка: ");
//                richTextBox.SelectionColor = System.Drawing.Color.Black;
//                richTextBox.AppendText($"{error.Message}\n");

//                richTextBox.SelectionColor = System.Drawing.Color.DarkBlue;
//                richTextBox.AppendText("  Позиция: ");
//                richTextBox.SelectionColor = System.Drawing.Color.Black;
//                richTextBox.AppendText($"{error.Position}\n");

//                if (!string.IsNullOrEmpty(error.Fragment))
//                {
//                    richTextBox.SelectionColor = System.Drawing.Color.DarkMagenta;
//                    richTextBox.AppendText("  Фрагмент: ");
//                    richTextBox.SelectionColor = System.Drawing.Color.Black;
//                    richTextBox.AppendText($"'{error.Fragment}'\n");
//                }

//                richTextBox.SelectionColor = System.Drawing.Color.DarkGreen;
//                richTextBox.AppendText("  Рекомендация: ");
//                richTextBox.SelectionColor = System.Drawing.Color.Black;
//                richTextBox.AppendText($"{error.Recommendation}\n\n");
//            }
//        }
//    }
//}