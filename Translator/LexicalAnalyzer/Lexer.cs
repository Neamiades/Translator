using System;
using System.Collections.Generic;
using System.IO;
using Translator.LexicalAnalyzer.FSM;

namespace Translator.LexicalAnalyzer
{
    class Lexer
    {
        private readonly Table _table;

        private List<Lexem> _resultLexems;

        private List<Lexem> _errorLexems;

        private States CurrentState;

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
            //    case States.Input:
            //        break;
            //    case States.Whitespace:
            //        CurrentState = States.Input;
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

        private void NextState(char symbol)
        {
            //if (Char.IsLetter(symbol))
            //{
            //    LetterEvent();
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

        public (Table InformationTable, List<Lexem> Lexems, List<Lexem> Errors) ParseFile(string fileName)
        {
            _line = 0;
            _column = 0;
            _resultLexems = new List<Lexem>();
            _errorLexems = new List<Lexem>();

            using (var sr = new StreamReader(fileName))
            {
                var symbol = (char)sr.Read();
                
                //while (!sr.EndOfStream)
                while (CurrentState != States.Exit)
                {
                    NextState(symbol);

                    if (CurrentState == States.Input)
                        symbol = (char)sr.Read();
                }
            }

            return (_table, _resultLexems, _errorLexems);
        }
    }
}
