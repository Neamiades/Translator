using System.Collections.Generic;
using Translator.LexicalAnalyzer;

namespace Translator
{
    class Table
    {
        private int _delimetersCounter;

        private int _multiDelimetersCounter;

        private int _coreWordsCounter;

        private int _constantCounter;

        private int _identifiersCounter;

        public char CommentOpenSymbol     { get; }

        public char CommentAdditionSymbol { get; }

        public char CommentCloseSymbol    { get; }

        public readonly Dictionary<string, int> Constants;

        public readonly Dictionary<string, int> Identifiers;

        public readonly Dictionary<char, int> Delimeters;

        public readonly Dictionary<string, int> MultiDelimeters;

        public readonly Dictionary<string, int> CoreWords;


        public Table(InitTable initTable = null)
        {
            _delimetersCounter      = 0;
            _multiDelimetersCounter = 301;
            _coreWordsCounter       = 401;
            _constantCounter        = 501;
            _identifiersCounter     = 1001;

            Constants       = new Dictionary<string, int>();
            Identifiers     = new Dictionary<string, int>();
            Delimeters      = new Dictionary<char, int>();
            MultiDelimeters = new Dictionary<string, int>();
            CoreWords       = new Dictionary<string, int>();

            if (initTable != null)
            {
                CommentOpenSymbol     = initTable.CommentOpenSymbol     == default(char) ? '(' : initTable.CommentOpenSymbol;
                CommentAdditionSymbol = initTable.CommentAdditionSymbol == default(char) ? '*' : initTable.CommentAdditionSymbol;
                CommentCloseSymbol    = initTable.CommentCloseSymbol    == default(char) ? ')' : initTable.CommentCloseSymbol;

                if (initTable.Delimeters != null)
                    foreach (var delimeter in initTable.Delimeters)
                        AddDelimeter(delimeter);
                if (initTable.MultiDelimeters != null)
                    foreach (var multiDelimeter in initTable.MultiDelimeters)
                        AddMultiDelimeter(multiDelimeter);
                if (initTable.CoreWords != null)
                    foreach (var coreWord in initTable.CoreWords)
                        AddCoreWord(coreWord);
                if (initTable.Constants != null)
                    foreach (var constant in initTable.Constants)
                        AddConstant(constant);
                if (initTable.Identifiers != null)
                    foreach (var identifier in initTable.Identifiers)
                        AddIdentifier(identifier);
            }
        }

        private void AddDelimeter(char delimeter) => Delimeters.Add(delimeter, _delimetersCounter++);

        private void AddMultiDelimeter(string delimeter) => MultiDelimeters.Add(delimeter, _multiDelimetersCounter++);

        private void AddCoreWord(string coreWord) => CoreWords.Add(coreWord, _coreWordsCounter++);

        public void AddConstant(string constant) => Constants.Add(constant, _constantCounter++);

        public void AddIdentifier(string identifier) => Identifiers.Add(identifier, _identifiersCounter++);
    }
}
