namespace Translator
{
    struct Token
    {
        public int Line    { get; set; }

        public int Column  { get; set; }

        public int Code    { get; set; }

        public string Name { get; set; }

        public Token(string name, int code, int line, int column)
        {
            Name = name;
            Code = code;
            Line = line;
            Column = column;
        }

        public override string ToString() => $"<{Code}>\t[{Line}][{Column}] - {Name}";
    }
}
