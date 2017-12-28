using System.Collections.Generic;
using Translator.LexicalAnalyzer;
using static System.Console;

namespace Translator
{
    static class Program
    {
        private static readonly InitTable InitTable = new InitTable
        {
            Delimeters      = new [] {'.', ';', '+', '-', '/', '*', ':', ')', '('},
            MultiDelimeters = new [] { ":=" },
            CoreWords       = new [] { "PROGRAM", "END", "BEGIN", "VAR", "INTEGER" },
            CommentOpenSymbol = '(' ,
            CommentAdditionSymbol = '*',
            CommentCloseSymbol = ')'
        };

        static void Main()
        {
            var lexer = new Lexer(InitTable);
            (var informationTable, var lexems, var errors) = lexer.ParseFile("../../ExternalFiles/InputScript.txt");
            PrintTableToConsole(informationTable, lexems, errors);
        }

        private static void PrintTableToConsole(Table informationTable, List<Lexem> lexems, List<Lexem> errors)
        {
            WriteLine();
            PrintDictionary(nameof(informationTable.Constants), informationTable.Constants);
            PrintDictionary(nameof(informationTable.Identifiers), informationTable.Identifiers);
            PrintDictionary(nameof(informationTable.Delimeters), informationTable.Delimeters);
            PrintDictionary(nameof(informationTable.MultiDelimeters), informationTable.MultiDelimeters);
            PrintDictionary(nameof(informationTable.CoreWords), informationTable.CoreWords);

            PrintTokens(nameof(lexems), lexems);
            PrintTokens(nameof(errors), errors);
        }

        private static void PrintTokens(string tokensName, List<Lexem> tokens)
        {
            WriteLine($"{tokensName}:");
            tokens.ForEach(t => WriteLine(t));
            WriteLine();
        }

        private static void PrintDictionary<TKey, TValue>(string dicName, Dictionary<TKey, TValue> dictionary)
        {
            WriteLine($"{dicName}:");

            foreach (var c in dictionary)
                WriteLine($"{c.Key} {c.Value}");

            WriteLine();
        }
    }
}
