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

        private int _line;

        private int _column;

        private string _word;

        public Lexer(InitTable initTable) => _table = new Table(initTable);

        private void LetterEvent(char symbol)
        {
            //switch (CurrentState)
            //{
            //    case States.Initial:
            //        CurrentState = States.Identifier;
            //        _word += symbol;
            //        break;
            //    case States.Identifier:
            //        _word += symbol;
            //        break;
            //    case States.Number:
            //        CurrentState = States.Out;
            //        break;
            //    case States.Read:
            //        break;
            //    case States.Whitespace:
            //        CurrentState = States.Read;
            //        break;
            //    case States.BeginComment:
            //        if (_table.Delimeters.TryGetValue('(', out int code))
            //        {
            //            resultLexems.Add(new Lexem(ident, code, line, identCol));
            //        }
            //        else if (_table.Identifiers.TryGetValue(ident, out code))
            //        {
            //            resultLexems.Add(new Lexem(ident, code, line, identCol));
            //        }
            //        else
            //        {
            //            _table.AddIdentifier(ident);
            //            resultLexems.Add(new Lexem(ident, _table.Identifiers[ident], line, identCol));
            //        }
            //        break;
            //    case States.Comment:
            //        break;
            //    case States.EndComment:
            //        break;
            //    case States.Exit:
            //        break;
            //    case States.Error:
            //        break;
            //    default:
            //        break;
            //}
        }

        private Events RecognizeEvent(int symbol)
        {
            return symbol == -1                                                ? Events.Eof
                 : Char.IsLetter((char)symbol)                                 ? Events.Letter
                 : Char.IsDigit((char)symbol)                                  ? Events.Digit
                 : Char.IsWhiteSpace((char)symbol)                             ? Events.Whitespace
                 : (char)symbol == '('                                         ? Events.OpenParenthesis
                 : (char)symbol == '*'                                         ? Events.Asterisk
                 : (char)symbol == ')'                                         ? Events.CloseParenthesis
                 : _table.MultiDelimeters.Any(md => md.Key[0] == (char)symbol) ? Events.MultiDelimStart
                 : _table.Delimeters.ContainsKey((char)symbol)                 ? Events.Delimeter
                 : Events.Other;

        }

        private void NextState(char symbol, Events eventType)
        {
            switch (_currentState)
            {
                case States.Initial:
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
                            _currentState = States.Out;
                            break;
                        case Events.Whitespace:
                            _currentState = States.Whitespace;
                            break;

                        case Events.MultiDelimStart:
                        case Events.OpenParenthesis:
                            _currentState = States.InStack;
                            break;

                        case Events.Asterisk:
                        case Events.CloseParenthesis:
                            _currentState = States.Out;
                            break;

                        case Events.Eof:
                            _currentState = States.Exit;
                            break;
                        case Events.Other:
                            _currentState = States.Error;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
                    }
                    break;
                case States.Identifier:
                    switch (eventType)
                    {
                        case Events.Letter:
                        case Events.Digit:
                            _currentState = States.Identifier;
                            _word += symbol;
                            break;
                        default:
                            _currentState = States.Out;
                            break;
                    }
                    break;
                case States.Number:
                    switch (eventType)
                    {
                        case Events.Digit:
                            _currentState = States.Number;
                            _word += symbol;
                            break;
                        default:
                            _currentState = States.Out;
                            break;
                    }
                    break;
                case States.Out:
                    if (_table.CoreWords.TryGetValue(ident, out int code))
                    {
                        resultLexems.Add(new Lexem(ident, code, line, identCol));
                    }
                    else if (_table.Identifiers.TryGetValue(ident, out code))
                    {
                        resultLexems.Add(new Lexem(ident, code, line, identCol));
                    }
                    else
                    {
                        _table.AddIdentifier(ident);
                        resultLexems.Add(new Lexem(ident, _table.Identifiers[ident], line, identCol));
                    }
                    break;
                case States.Read:
                    break;
                case States.Delimeter:
                    break;
                case States.Whitespace:
                    break;
                case States.BeginComment:
                    break;
                case States.Comment:
                    break;
                case States.EndComment:
                    break;
                case States.Exit:
                    break;
                case States.Error:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //if (Char.IsLetter(symbol))
            //{
            //    var ident = symbol.ToString();
            //    var identCol = column;

            //    symbol = (char)sr.Read();
            //    while (Char.IsLetter(symbol) || Char.IsDigit(symbol))
            //    {
            //        ident += symbol;
            //        column++;
            //        symbol = (char)sr.Read();
            //    }

            //    if (_table.CoreWords.TryGetValue(ident, out int code))
            //    {
            //        resultLexems.Add(new Lexem(ident, code, line, identCol));
            //    }
            //    else if (_table.Identifiers.TryGetValue(ident, out code))
            //    {
            //        resultLexems.Add(new Lexem(ident, code, line, identCol));
            //    }
            //    else
            //    {
            //        _table.AddIdentifier(ident);
            //        resultLexems.Add(new Lexem(ident, _table.Identifiers[ident], line, identCol));
            //    }
            //}
            //else if (Char.IsDigit(symbol))
            //{
            //    LetterEvent();
            //    var constant = symbol.ToString();
            //    var constantCol = column;

            //    symbol = (char)sr.Read();
            //    while (Char.IsDigit(symbol))
            //    {
            //        constant += symbol;
            //        column++;
            //        symbol = (char)sr.Read();
            //    }
            //    if (Char.IsLetter(symbol))
            //    {
            //        errorLexems.Add(new Lexem("Identifier can not start with number", -1, line, constantCol));
            //        column++;
            //        symbol = (char)sr.Read();
            //        while (Char.IsDigit(symbol) || Char.IsDigit(symbol))
            //        {
            //            column++;
            //            symbol = (char)sr.Read();
            //        }
            //    }
            //    else if (_table.Constants.TryGetValue(constant, out int code))
            //    {
            //        resultLexems.Add(new Lexem(constant, code, line, constantCol));
            //    }
            //    else
            //    {
            //        _table.AddConstant(constant);
            //        resultLexems.Add(new Lexem(constant, _table.Identifiers[constant], line, constantCol));
            //    }
            //}
            //else if (symbol == '(')
            //{
            //    var prevSymbol = symbol;
            //    column++;
            //    if ((symbol = (char)sr.Read()) == '*')
            //    {
            //        while (!sr.EndOfStream)
            //        {
            //            column++;
            //            if (symbol == '*')
            //            {
            //                column++;
            //                if ((symbol = (char)sr.Read()) == ')')
            //                {
            //                    symbol = (char)sr.Read();
            //                    break;
            //                }
            //            }
            //            else if (symbol == '*')
            //            {

            //            }
            //            symbol = (char)sr.Read();
            //        }
            //    }
            //}
            //else if (_table.Delimeters.ContainsKey(symbol))
            //{
            //    return Events.Delimeter;
            //}
            //else if (Char.IsWhiteSpace(symbol))
            //{
            //    return Events.Whitespace;
            //}
            //return Events.Other;
        }

        public (Table informationTable, List<Lexem> lexems, List<Lexem> errors) ParseFile(string fileName)
        {
            _line = 0;
            _column = 0;
            _currentState = States.Initial;

            _resultLexems = new List<Lexem>();
            _errorLexems = new List<Lexem>();

            using (var sr = new StreamReader(fileName))
            {
                var symbol = sr.Read();

                while (_currentState != States.Exit)
                {
                    NextState((char)symbol, RecognizeEvent(symbol));

                    if (_currentState == States.Read
                        || _currentState == States.Identifier
                        || _currentState == States.Number
                        || _currentState == States.Whitespace
                        || _currentState == States.Comment
                        || _currentState == States.EndComment)
                    {
                        symbol = sr.Read();
                        _column++;

                    }
                }
            }

            return (_table, _resultLexems, _errorLexems);
        }
    }
}
