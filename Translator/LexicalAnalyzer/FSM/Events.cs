namespace Translator.LexicalAnalyzer.FSM
{
    enum Events
    {
        Letter,
        Digit,
        Delimeter,
        MultiDelimStart,
        Whitespace,
        OpenParenthesis,
        Asterisk,
        CloseParenthesis,
        Eof,
        Other
    }
}
