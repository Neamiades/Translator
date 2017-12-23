using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Translator.LexicalAnalyzer.FSM;
using static System.Console;

namespace Translator.LexicalAnalyzer
{
    class Lexer
    {
       

        private States CurrentState;

        private string _globalWord;

        private readonly Table _table;

        protected Lexer(InitTable initTable)
        {
            _table = new Table(initTable);
            CurrentState = States.Initial;
        }

        private States NextState(char symbol)
        {

        }

        public (Table, List<Lexem>) ParseFile(string fileName)
        {
            var line = 0;
            var column = 0;

            var resultLexems = new List<Lexem>();
            var errorLexems = new List<Lexem>();

            using (var sr = new StreamReader(fileName))
            {
                var symbol = (char)sr.Read();

                while (!sr.EndOfStream)
                {
                    if (Char.IsLetter(symbol))
                    {
                        var ident = symbol.ToString();
                        var identCol = column;

                        symbol = (char) sr.Read();
                        while (Char.IsLetter(symbol) || Char.IsDigit(symbol))
                        {
                            ident += symbol;
                            column++;
                            symbol = (char)sr.Read();
                        }

                        if (_table.CoreWords.TryGetValue(ident, out int code))
                        {
                            resultLexems.Add(new Lexem(ident, code, line, identCol));
                        }
                        else if(_table.Identifiers.TryGetValue(ident, out code))
                        {
                            resultLexems.Add(new Lexem(ident, code, line, identCol));
                        }
                        else
                        {
                            _table.AddIdentifier(ident);
                            resultLexems.Add(new Lexem(ident, _table.Identifiers[ident], line, identCol));
                        }
                    }
                    else if (Char.IsDigit(symbol))
                    {
                        var constant = symbol.ToString();
                        var constantCol = column;

                        symbol = (char)sr.Read();
                        while (Char.IsDigit(symbol))
                        {
                            constant += symbol;
                            column++;
                            symbol = (char)sr.Read();
                        }
                        if (Char.IsLetter(symbol))
                        {
                            errorLexems.Add(new Lexem("Identifier can not start with number", -1, line, constantCol));
                            column++;
                            symbol = (char)sr.Read();
                            while (Char.IsDigit(symbol) || Char.IsDigit(symbol))
                            {
                                column++;
                                symbol = (char)sr.Read();
                            }
                        }
                        else if (_table.Constants.TryGetValue(constant, out int code))
                        {
                            resultLexems.Add(new Lexem(constant, code, line, constantCol));
                        }
                        else
                        {
                            _table.AddConstant(constant);
                            resultLexems.Add(new Lexem(constant, _table.Identifiers[constant], line, constantCol));
                        }
                    }
                    else if (symbol == '(')
                    {
                        var prevSymbol = symbol;
                        column++;
                        if ((symbol = (char)sr.Read()) == '*')
                        {
                            while (!sr.EndOfStream)
                            {
                                column++;
                                if (symbol == '*')
                                {
                                    column++;
                                    if ((symbol = (char) sr.Read()) == ')')
                                    {
                                        symbol = (char) sr.Read();
                                        break;
                                    }
                                }
                                else if (symbol == '*')
                                {
                                    
                                }
                                symbol = (char)sr.Read();
                            }
                        }
                    }
                    else if (_table.Delimeters.ContainsKey(symbol))
                    {
                        return Events.Delimeter;
                    }
                    else if (Char.IsWhiteSpace(symbol))
                    {
                        return Events.Whitespace;
                    }
                    return Events.Other;
                }
            }
        }

        //public static Table ParseFile(string fileName, InitTable initTable)
        //{
        //    InitVars(initTable);
        //    using (var sr = new StreamReader(fileName))
        //    {
        //        while (!sr.EndOfStream)
        //        {
        //            char c = (char)sr.Read();
        //            _column++;
        //            if (c == '(')
        //            {
        //                char c1 = c;
        //                c = (char)sr.Read();
        //                if (c == '*')
        //                {
        //                    while (true)
        //                    {
        //                        var cNext = (char)sr.Read();
        //                        _column++;
        //                        if (sr.Peek() == -1)
        //                        {
        //                            _table.Errors.Add(new Lexem($"NOT CLOSED COMMENT", 0, _line, _column));
        //                            break;
        //                        }
        //                        if (cNext == '\n')
        //                        {
        //                            _column = -1;
        //                            _line++;
        //                        }
        //                        if (cNext == '*' && sr.Read() == ')')
        //                        {
        //                            _column++;
        //                            break;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    SymbolWorker(c1, sr);
        //                    SymbolWorker(c, sr);
        //                }
        //            }
        //            else
        //                SymbolWorker(c, sr);
        //        }
        //    }

        //    WordDetector(_globalWord);
        //    return _table;
        //}

        //private static void SymbolWorker(char c, StreamReader sr)
        //{
        //    Write(c);
        //    if (c == '\n')
        //    {
        //        _line++;
        //        _column = -1;
        //        WordDetector(_globalWord);
        //        _globalWord = null;

        //    }
        //    else if (char.IsLetterOrDigit(c))
        //    {
        //        _globalWord += c;
        //    }
        //    else if (char.IsWhiteSpace(c))
        //    {
        //        WordDetector(_globalWord);
        //        _globalWord = null;
        //    }
        //    else if (c == ':')
        //    {
        //        c = (char)sr.Read();
        //        if (c == '=')
        //        {
        //            Write(c);
        //            WordDetector(_globalWord);
        //            _globalWord = null;
        //            _table.Tokens.Add(new Lexem(":=", 300, _line, _column));
        //        }
        //        else
        //        {
        //            _table.Tokens.Add(new Lexem(":", 8, _line, _column));
        //            SymbolWorker(c, sr);
        //        }
        //    }
        //    else if (_table.Delimeters.TryGetValue(c, out int value))
        //    {
        //        WordDetector(_globalWord);
        //        _globalWord = null;
        //        _table.Tokens.Add(new Lexem(c.ToString(), value, _line, _column));
        //    }
        //    else
        //    {
        //        WordDetector(_globalWord);
        //        _globalWord = null;
        //        _table.Errors.Add(new Lexem($"LEKS ERR IMPOSS SYMBOL [{c}]", 0, _line, _column));
        //    }
        //}

        //private static void WordDetector(string stroke)
        //{
        //    if (stroke == null)
        //        return;

        //    if (_table.CoreWords.TryGetValue(stroke, out int value))
        //    {
        //        _table.Tokens.Add(new Lexem(stroke, value, _line, _column));
        //    }
        //    else if (char.IsDigit(stroke[0]))
        //    {
        //        if (int.TryParse(stroke, out _))
        //        {
        //            if (_table.Constants.TryGetValue(stroke, out value))
        //            {
        //                _table.Tokens.Add(new Lexem(stroke, value, _line, _column));
        //            }
        //            else
        //            {
        //                _table.Constants.Add(stroke, _constCode);
        //                _table.Tokens.Add(new Lexem(stroke, _constCode, _line, _column));
        //                _constCode++;
        //            }
        //        }
        //        else
        //        {
        //            _table.Errors.Add(new Lexem($"LEKS ERR IDENT CANT START WITH NUM [{stroke}] ", 0, _line, _column));
        //        }
        //    }
        //    else if (_table.Identifiers.TryGetValue(stroke, out value))
        //    {
        //        _table.Tokens.Add(new Lexem(stroke, value, _line, _column));
        //    }
        //    else
        //    {
        //        _table.Identifiers.Add(stroke, _identifierCode);
        //        _table.Tokens.Add(new Lexem(stroke, _identifierCode++, _line, _column));
        //    }
        //}
    }
}
