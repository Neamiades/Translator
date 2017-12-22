using System;
using System.IO;
using static System.Console;

namespace Translator
{
    static class Lexer
    {
        static private int _line;

        static private int _column;

        static private int _identifierCode;

        static private int _constCode;

        static private string _globalWord;

        static private Table _table;

        static public Table ParseFile(string fileName)
        {
            InitVars();
            using (var sr = new StreamReader(fileName))
            {
                while (!sr.EndOfStream)
                {
                    char c = (char)sr.Read();
                    _column++;
                    if (c == '(')
                    {
                        char c1 = c;
                        c = (char)sr.Read();
                        if (c == '*')
                        {
                            char cNext = ' ';
                            while (true)
                            {
                                cNext = (char)sr.Read();
                                _column++;
                                if (sr.Peek() == -1)
                                {
                                    _table.Errors.Add(new Token($"NOT CLOSED COMMENT", 0, _line, _column));
                                    break;
                                }
                                if (cNext == '\n')
                                {
                                    _column = -1;
                                    _line++;
                                }
                                if (cNext == '*' && sr.Read() == ')')
                                {
                                    _column++;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            SymbolWorker(c1, sr);
                            SymbolWorker(c, sr);
                        }
                    }
                    else
                        SymbolWorker(c, sr);
                }
            }

            WordDetector(_globalWord);
            return _table;
        }

        static private void InitVars()
        {
            _line = 1;
            _column = -1;
            _identifierCode = 1000;
            _constCode = 500;
            _table = new Table();
        }

        static private void SymbolWorker(char c, StreamReader sr)
        {
            Write(c);
            if (c == '\n')
            {
                _line++;
                _column = -1;
                WordDetector(_globalWord);
                _globalWord = null;
                return;
                
            }
            else if (Char.IsLetterOrDigit(c))
            {
                _globalWord += c;
                return;
            }
            else if (Char.IsWhiteSpace(c))
            {
                WordDetector(_globalWord);
                _globalWord = null;
                return;
            }
            else if (c == ':')
            {
                char prev = c;
                c = (char)sr.Read();
                if (c == '=')
                {
                    Write(c);
                    WordDetector(_globalWord);
                    _globalWord = null;
                    _table.Tokens.Add(new Token(":=", 300, _line, _column));
                    return;
                }
                else
                {
                    _table.Tokens.Add(new Token(":", 8, _line, _column));
                    SymbolWorker(c, sr);
                    return;
                }
            }
            if (_table.Delimeters.TryGetValue(c, out int value))
            {
                WordDetector(_globalWord);
                _globalWord = null;
                _table.Tokens.Add(new Token(c.ToString(), value, _line, _column));
                return;
            }
            WordDetector(_globalWord);
            _globalWord = null;
            _table.Errors.Add(new Token($"LEKS ERR IMPOSS SYMBOL [{c}]", 0, _line, _column));
        }

        static private void WordDetector(string stroke)
        {
            if (stroke == null)
                return;

            if (_table.CoreWords.TryGetValue(stroke, out int value))
            {
                _table.Tokens.Add(new Token(stroke, value, _line, _column));
            }
            else if (Char.IsDigit(stroke[0]))
            {
                int a;
                if (Int32.TryParse(stroke, out a))
                {
                    if (_table.Constants.ContainsKey(stroke))
                    {
                        _table.Constants.Add(stroke, _constCode);
                        _table.Tokens.Add(new Token(stroke, _constCode, _line, _column));
                        _constCode++;
                    }
                    else if (_table.Constants.TryGetValue(stroke, out value))
                    {
                        _table.Tokens.Add(new Token(stroke, value, _line, _column));
                    }
                }
                else
                {
                    _table.Errors.Add(new Token($"LEKS ERR IDENT CANT START WITH NUM [{stroke}] ", 0, _line, _column));
                }
            }
            else if (_table.Identifiers.TryGetValue(stroke, out value))
            {
                _table.Tokens.Add(new Token(stroke, value, _line, _column));
            }
            else
            {
                _table.Identifiers.Add(stroke, value);
                _table.Tokens.Add(new Token(stroke, _identifierCode, _line, _column));
                _identifierCode++;
            }
        }
    }
}
