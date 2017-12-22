using System.Collections.Generic;

namespace Translator
{
    class Table
    {
        public List<Token> Errors = new List<Token>();

        public List<Token> Tokens = new List<Token>();

        public Dictionary<int, string> Constants        = new Dictionary<int, string>();

        public Dictionary<int, string> Identifiers      = new Dictionary<int, string>();

        public Dictionary<int, char>   Delimeters       = new Dictionary<int, char>()
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

        public Dictionary<int, string> DoubleDelimeters = new Dictionary<int, string>()
        {
            { 300, ":="}
        };

        public Dictionary<int, string> CoreWords        = new Dictionary<int, string>()
        {
            {401, "PROGRAM" },
            {402, "END" },
            {403, "BEGIN" },
            {404, "VAR" },
            {405, "INTEGER" }
        };
    }
}
