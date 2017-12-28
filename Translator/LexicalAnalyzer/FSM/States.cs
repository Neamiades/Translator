namespace Translator.LexicalAnalyzer.FSM
{
    enum States
    {
        Initial,
        Identifier,
        Number,
        Read,
        MultiDelimeter,
        Whitespace,
        BeginComment,
        Comment,
        EndComment,
        Exit
    }
}
