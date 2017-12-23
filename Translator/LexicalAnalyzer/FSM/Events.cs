namespace Translator.LexicalAnalyzer.FSM
{
    enum Events
    {
        Letter,
        Digit,
        Delimeter,
        Whitespace,
        OpenParenthesis,
        Asterisk,
        CloseParenthesis,
        Eof,
        Other
    }
}
