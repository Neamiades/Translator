namespace Translator.LexicalAnalyzer.FSM
{
    enum States
    {
        Initial,
        Identifier,
        Number,
        Out,
        Input,
        Whitespace,
        BeginComment,
        Comment,
        EndComment,
        Exit,
        Error
    }
}
