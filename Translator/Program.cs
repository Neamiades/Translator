using System.Collections.Generic;
using static System.Console;

namespace Translator
{
    class Program
    {
        static void Main(string[] args)
        {
            var table = Lexer.ParseFile("../../ExternalFiles/InputScript.txt");
            PrintTableToConsole(table);
        }

        private static void PrintTableToConsole(Table table)
        {
            WriteLine();
            PrintDictionary(nameof(table.Constants), table.Constants);
            PrintDictionary(nameof(table.Identifiers), table.Identifiers);
            PrintDictionary(nameof(table.Delimeters), table.Delimeters);
            PrintDictionary(nameof(table.DoubleDelimeters), table.DoubleDelimeters);
            PrintDictionary(nameof(table.CoreWords), table.CoreWords);

            PrintTokens(nameof(table.Tokens), table.Tokens);
            PrintTokens(nameof(table.Errors), table.Errors);

            //foreach (var c in Errors)
            //{
            //    Console.WriteLine(c.Name + "\t" + "\t" + $"[{c.Line}]" + $"[{c.Column}]");
            //}
        }

        private static void PrintTokens(string tokensName, List<Token> tokens)
        {
            WriteLine($"{tokensName}:");
            tokens.ForEach(t => WriteLine(t));
            WriteLine();
        }

        private static void PrintDictionary<TValue>(string dicName, Dictionary<int, TValue> dictionary)
        {
            WriteLine($"{dicName}:");

            foreach (var c in dictionary)
                WriteLine($"{c.Key} {c.Value}");

            WriteLine();
        }
    }
}
