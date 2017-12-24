namespace Translator.LexicalAnalyzer.FSM
{
    enum States
    {
        Initial,
        Identifier,
        Number,
        Out,
        Input,
        Delimeter,
        Whitespace,
        BeginComment,
        Comment,
        EndComment,
        Exit,
        Error
    }
}
