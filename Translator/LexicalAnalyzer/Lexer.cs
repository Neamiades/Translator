using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Translator.LexicalAnalyzer.FSM;

namespace Translator.LexicalAnalyzer
{
    class Lexer
    {
        private readonly Table _table;

        private List<Lexem> _resultLexems;

        private List<Lexem> _errorLexems;

        private States _currentState;

        private const int ErrorCode = -1;

        private const char NextLine = '\n';

        private int _line;

        private int _column;

        private string _word;

        private char _prevSymbol;

        public Lexer(InitTable initTable) => _table = new Table(initTable);

        public (Table informationTable, List<Lexem> lexems, List<Lexem> errors) ParseFile(string fileName)
        {
            _line = 1;
            _column = 1;
            _currentState = States.Initial;

            _resultLexems = new List<Lexem>();
            _errorLexems = new List<Lexem>();

            using (var sr = new StreamReader(fileName))
            {
                var symbol = sr.Read();

                while (_currentState != States.Exit)
                {
                    NextState((char)symbol, RecognizeEvent(symbol));

                    if (   _currentState == States.Read
                        || _currentState == States.Identifier
                        || _currentState == States.Number
                        || _currentState == States.Whitespace
                        || _currentState == States.Comment
                        || _currentState == States.BeginComment
                        || _currentState == States.EndComment
                        || _currentState == States.MultiDelimeter)
                    {
                        Console.Write((char)symbol);
                        symbol = sr.Read();

                        if (symbol == NextLine)
                        {
                            _column = 1;
                            _line++;
                        }
                        else
                            _column++;
                    }
                }
            }

            return (_table, _resultLexems, _errorLexems);
        }

        private Events RecognizeEvent(int symbol)
        {
            return symbol == -1                                                ? Events.Eof
                 : Char.IsLetter((char)symbol)                                 ? Events.Letter
                 : Char.IsDigit((char)symbol)                                  ? Events.Digit
                 : Char.IsWhiteSpace((char)symbol)                             ? Events.Whitespace
                 : (char)symbol == _table.CommentOpenSymbol                    ? Events.CommentOpenSymbol
                 : (char)symbol == _table.CommentAdditionSymbol                ? Events.CommentAdditionSymbol
                 : (char)symbol == _table.CommentCloseSymbol                   ? Events.CommentCloseSymbol
                 : _table.MultiDelimeters.Any(md => md.Key[0] == (char)symbol) ? Events.MultiDelimStart
                 : _table.Delimeters.ContainsKey((char)symbol)                 ? Events.Delimeter
                 : Events.Other;
        }

        private void NextState(char symbol, Events eventType)
        {
            switch (_currentState)
            {
                case States.Initial:
                case States.Read:
                    ReadHandler(symbol, eventType);
                    break;
                case States.Identifier:
                    IdentifierHandler(symbol, eventType);
                    break;
                case States.Number:
                    NumberHandler(symbol, eventType);
                    break;
                case States.Whitespace:
                    if (eventType != Events.Whitespace)
                        _currentState = States.Initial;

                    break;
                case States.BeginComment:
                    switch (eventType)
                    {
                        case Events.CommentAdditionSymbol:
                            _currentState = States.Comment;
                            break;
                        default:
                            _resultLexems.Add(new Lexem(_prevSymbol.ToString(), _table.Delimeters[_prevSymbol], _line, _column - 1));
                            _currentState = States.Initial;
                            break;
                    }
                    break;
                case States.Comment:
                    switch (eventType)
                    {
                        case Events.CommentAdditionSymbol:
                            _currentState = States.EndComment;
                            break;
                        case Events.Eof:
                            _errorLexems.Add(new Lexem("Not closed comment", ErrorCode, _line, _column - 1));
                            _currentState = States.Exit;
                            break;
                    }
                    break;
                case States.EndComment:
                    _currentState =   eventType == Events.CommentCloseSymbol    ? States.Read
                                    : eventType == Events.CommentAdditionSymbol ? States.EndComment
                                    : States.Comment;
                    break;
                case States.MultiDelimeter:
                    MultiDelimHandler(symbol, eventType);
                    break;
            }
        }

        private void MultiDelimHandler(char symbol, Events eventType)
        {
            var multiDelim = $"{_prevSymbol}{symbol}";

            switch (eventType)
            {
                case Events.Delimeter:
                case Events.MultiDelimStart:
                case Events.CommentAdditionSymbol:
                case Events.CommentCloseSymbol:
                    if (_table.MultiDelimeters.TryGetValue(multiDelim, out int code))
                    {
                        _resultLexems.Add(new Lexem(multiDelim, code, _line, _column - 1));
                    }
                    else
                    {
                        _resultLexems.Add(new Lexem(_prevSymbol.ToString(), _table.Delimeters[_prevSymbol], _line, _column));
                        _resultLexems.Add(new Lexem(symbol.ToString(), _table.Delimeters[symbol], _line, _column));
                    }
                    _currentState = States.Read;
                    break;

                case Events.Other:
                    if (_table.MultiDelimeters.TryGetValue(multiDelim, out code))
                    {
                        _resultLexems.Add(new Lexem(multiDelim, code, _line, _column - 1));
                    }
                    else
                    {
                        _resultLexems.Add(new Lexem(_prevSymbol.ToString(), _table.Delimeters[_prevSymbol], _line, _column));
                        _errorLexems.Add(new Lexem($"Unexpected symbol - {symbol}", ErrorCode, _line, _column));
                    }
                    _currentState = States.Read;
                    break;


                case Events.CommentOpenSymbol:
                    if (_table.MultiDelimeters.TryGetValue(multiDelim, out code))
                    {
                        _resultLexems.Add(new Lexem(multiDelim, code, _line, _column - 1));
                    }
                    else
                    {
                        _resultLexems.Add(new Lexem(_prevSymbol.ToString(), _table.Delimeters[_prevSymbol], _line, _column));
                    }

                    _currentState = States.BeginComment;
                    break;

                default:
                    _resultLexems.Add(new Lexem(_prevSymbol.ToString(), _table.Delimeters[_prevSymbol], _line, _column));
                    _currentState = States.Initial;
                    break;
            }
        }

        private void NumberHandler(char symbol, Events eventType)
        {
            switch (eventType)
            {
                case Events.Digit:
                    _currentState = States.Number;
                    _word += symbol;
                    break;

                case Events.Letter:
                    _errorLexems.Add(new Lexem("Identifier can't start from number", ErrorCode, _line, _column - _word.Length));
                    _word = null;
                    _currentState = States.Initial;
                    break;
                default:
                    if (_table.Constants.TryGetValue(_word, out int code))
                    {
                        _resultLexems.Add(new Lexem(_word, code, _line, _column - _word.Length));
                    }
                    else
                    {
                        _table.AddConstant(_word);
                        _resultLexems.Add(new Lexem(_word, _table.Constants[_word], _line, _column - _word.Length));
                    }
                    _word = null;
                    _currentState = States.Initial;
                    break;
            }
        }

        private void IdentifierHandler(char symbol, Events eventType)
        {
            switch (eventType)
            {
                case Events.Letter:
                case Events.Digit:
                    _word += symbol;
                    break;
                default:
                    if (_table.CoreWords.TryGetValue(_word, out int code))
                    {
                        _resultLexems.Add(new Lexem(_word, code, _line, _column - _word.Length));
                    }
                    else if (_table.Identifiers.TryGetValue(_word, out code))
                    {
                        _resultLexems.Add(new Lexem(_word, code, _line, _column - _word.Length));
                    }
                    else
                    {
                        _table.AddIdentifier(_word);
                        _resultLexems.Add(new Lexem(_word, _table.Identifiers[_word], _line, _column - _word.Length));
                    }
                    _word = null;
                    _currentState = States.Initial;
                    break;
            }
        }

        private void ReadHandler(char symbol, Events eventType)
        {
            switch (eventType)
            {
                case Events.Letter:
                    _currentState = States.Identifier;
                    _word += symbol;
                    break;
                case Events.Digit:
                    _currentState = States.Number;
                    _word += symbol;
                    break;
                case Events.Delimeter:
                    _resultLexems.Add(new Lexem(symbol.ToString(), _table.Delimeters[symbol], _line, _column));
                    _currentState = States.Read;
                    break;
                case Events.Whitespace:
                    _currentState = States.Whitespace;
                    break;
                case Events.MultiDelimStart:
                    _prevSymbol = symbol;
                    _currentState = States.MultiDelimeter;
                    break;
                case Events.CommentOpenSymbol:
                    _prevSymbol = symbol;
                    _currentState = States.BeginComment;
                    break;

                case Events.CommentAdditionSymbol:
                case Events.CommentCloseSymbol:
                    _resultLexems.Add(new Lexem(symbol.ToString(), _table.Delimeters[symbol], _line, _column));
                    _currentState = States.Read;
                    break;

                case Events.Eof:
                    _currentState = States.Exit;
                    break;
                case Events.Other:
                    _errorLexems.Add(new Lexem($"Unexpected symbol - {symbol}", ErrorCode, _line, _column));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            }
        }
    }
}
