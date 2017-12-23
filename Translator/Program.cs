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
            CoreWords       = new [] { "PROGRAM", "END", "BEGIN", "VAR", "INTEGER" }
        };

        static void Main()
        {
            var table = Lexer.ParseFile("../../ExternalFiles/InputScript.txt", InitTable);
            PrintTableToConsole(table);
        }

        private static void PrintTableToConsole(Table table)
        {
            WriteLine();
            PrintDictionary(nameof(table.Constants), table.Constants);
            PrintDictionary(nameof(table.Identifiers), table.Identifiers);
            PrintDictionary(nameof(table.Delimeters), table.Delimeters);
            PrintDictionary(nameof(table.MultiDelimeters), table.MultiDelimeters);
            PrintDictionary(nameof(table.CoreWords), table.CoreWords);

            PrintTokens(nameof(table.Tokens), table.Tokens);
            PrintTokens(nameof(table.Errors), table.Errors);
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
