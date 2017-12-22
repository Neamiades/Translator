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
            foreach (var delim in _table.Delimeters)
            {
                if (c == delim.Value)
                {
                    WordDetector(_globalWord);
                    _globalWord = null;
                    _table.Tokens.Add(new Token(delim.Value.ToString(), delim.Key, _line, _column));
                    return;
                }
            }
            WordDetector(_globalWord);
            _globalWord = null;
            _table.Errors.Add(new Token($"LEKS ERR IMPOSS SYMBOL [{c}]", 0, _line, _column));
        }

        static private void WordDetector(string stroke)
        {
            if (stroke == null)
                return;
                
            foreach (var coreWord in _table.CoreWords)
            {
                if (stroke == coreWord.Value)
                {
                    _table.Tokens.Add(new Token(stroke, coreWord.Key, _line, _column));
                    return;
                }
            }
            if (Char.IsDigit(stroke[0]))
            {
                int a;
                if (Int32.TryParse(stroke, out a))
                {
                    if (!_table.Constants.ContainsValue(stroke))
                    {
                        _table.Constants.Add(_constCode, stroke);
                        _table.Tokens.Add(new Token(stroke, _constCode, _line, _column));
                        _constCode++;
                        return;
                    }
                    else
                    {
                        foreach (var con in _table.Constants)
                        {
                            if (stroke == con.Value)
                            {
                                _table.Tokens.Add(new Token(stroke, con.Key, _line, _column));
                                return;
                            }
                        }
                    }
                }
                else
                {
                    _table.Errors.Add(new Token($"LEKS ERR IDENT CANT START WITH NUM [{stroke}] ", 0, _line, _column));
                    return;
                }
            }
            if (!_table.Identifiers.ContainsValue(stroke))
            {
                _table.Identifiers.Add(_identifierCode, stroke);
                _table.Tokens.Add(new Token(stroke, _identifierCode, _line, _column));
                _identifierCode++;
                return;
            }
            else
            {
                foreach (var ident in _table.Identifiers)
                {
                    if (stroke == ident.Value)
                    {
                        _table.Tokens.Add(new Token(stroke, ident.Key, _line, _column));
                        return;
                    }
                }
            }
        }
    }
}
