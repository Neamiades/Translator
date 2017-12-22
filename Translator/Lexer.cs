using System.IO;
using static System.Console;

namespace Translator
{
    static class Lexer
    {
        private static int _line;

        private static int _column;

        private static int _identifierCode;

        private static int _constCode;

        private static string _globalWord;

        private static Table _table;

        public static Table ParseFile(string fileName)
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
                            while (true)
                            {
                                var cNext = (char)sr.Read();
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

        private static void InitVars()
        {
            _line = 1;
            _column = -1;
            _identifierCode = 1000;
            _constCode = 500;
            _table = new Table();
        }

        private static void SymbolWorker(char c, StreamReader sr)
        {
            Write(c);
            if (c == '\n')
            {
                _line++;
                _column = -1;
                WordDetector(_globalWord);
                _globalWord = null;

            }
            else if (char.IsLetterOrDigit(c))
            {
                _globalWord += c;
            }
            else if (char.IsWhiteSpace(c))
            {
                WordDetector(_globalWord);
                _globalWord = null;
            }
            else if (c == ':')
            {
                c = (char)sr.Read();
                if (c == '=')
                {
                    Write(c);
                    WordDetector(_globalWord);
                    _globalWord = null;
                    _table.Tokens.Add(new Token(":=", 300, _line, _column));
                }
                else
                {
                    _table.Tokens.Add(new Token(":", 8, _line, _column));
                    SymbolWorker(c, sr);
                }
            }
            else if (_table.Delimeters.TryGetValue(c, out int value))
            {
                WordDetector(_globalWord);
                _globalWord = null;
                _table.Tokens.Add(new Token(c.ToString(), value, _line, _column));
            }
            else
            {
                WordDetector(_globalWord);
                _globalWord = null;
                _table.Errors.Add(new Token($"LEKS ERR IMPOSS SYMBOL [{c}]", 0, _line, _column));
            }
        }

        private static void WordDetector(string stroke)
        {
            if (stroke == null)
                return;

            if (_table.CoreWords.TryGetValue(stroke, out int value))
            {
                _table.Tokens.Add(new Token(stroke, value, _line, _column));
            }
            else if (char.IsDigit(stroke[0]))
            {
                if (int.TryParse(stroke, out _))
                {
                    if (_table.Constants.TryGetValue(stroke, out value))
                    {
                        _table.Tokens.Add(new Token(stroke, value, _line, _column));
                    }
                    else
                    {
                        _table.Constants.Add(stroke, _constCode);
                        _table.Tokens.Add(new Token(stroke, _constCode, _line, _column));
                        _constCode++;
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
                _table.Identifiers.Add(stroke, _identifierCode);
                _table.Tokens.Add(new Token(stroke, _identifierCode++, _line, _column));
            }
        }
    }
}
