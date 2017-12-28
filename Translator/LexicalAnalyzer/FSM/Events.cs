namespace Translator.LexicalAnalyzer.FSM
{
    enum Events
    {
        Letter,
        Digit,
        Delimeter,
        MultiDelimStart,
        Whitespace,
        CommentOpenSymbol,
        CommentAdditionSymbol,
        CommentCloseSymbol,
        Eof,
        Other
    }
}
