namespace Translator.LexicalAnalyzer.FSM
{
    enum States
    {
        Initial,
        Identifier,
        InStack,
        OfStack,
        Number,
        Out,
        Read,
        Delimeter,
        Whitespace,
        BeginComment,
        Comment,
        EndComment,
        Exit,
        Error
    }
}
