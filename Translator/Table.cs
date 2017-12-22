using System.Collections.Generic;

namespace Translator
{
    class Table
    {
        public List<Token> Errors = new List<Token>();

        public List<Token> Tokens = new List<Token>();

        public Dictionary<string, int> Constants        = new Dictionary<string, int>();

        public Dictionary<string, int> Identifiers      = new Dictionary<string, int>();

        public Dictionary<char, int>   Delimeters       = new Dictionary<char, int>()
        {
            { '.', 0 },
            { ';', 1 },
            { '+', 4 },
            { '-', 5 },
            { '/', 6 },
            { '*', 7 },
            { ':', 8 },
            { ')', 9 },
            {'(' , 10},
        };

        public Dictionary<int, char> Delimeters2 = new Dictionary<int, char>()
        {
            { 0,  '.' },
            { 1,  ';' },
            { 4,  '+' },
            { 5,  '-' },
            { 6,  '/' },
            { 7,  '*' },
            { 8,  ':' },
            { 9,  ')' },
            { 10, '(' },
        };

        public Dictionary<string, int> DoubleDelimeters = new Dictionary<string, int>()
        {
            { ":=", 300}
        };

        public Dictionary<string, int> CoreWords        = new Dictionary<string, int>()
        {
            {"PROGRAM", 401 },
            {"END"    , 402 },
            {"BEGIN"  , 403 },
            {"VAR"    , 404 },
            {"INTEGER", 405 }
        };
    }
}
