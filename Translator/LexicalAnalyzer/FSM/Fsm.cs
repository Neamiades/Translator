using System;

namespace Translator.LexicalAnalyzer.FSM
{
    abstract class Fsm
    {
        protected States CurrentState;

        protected Fsm()
        {
            CurrentState = States.Initial;
        }

        protected Fsm(States currentState)
        {
            CurrentState = currentState;
        }

        private Events RecognizeEvent(char symbol)
        {
            if (Char.IsLetter(symbol))
            {
                return Events.Letter;
            }
            if (Char.IsDigit(symbol))
            {
                return Events.Digit;
            }
            if (Char.Is)
            {
                
            }
            if (Char.IsWhiteSpace(symbol))
            {
                return Events.Whitespace;
            }
        }

        protected virtual void Start() { }

        protected abstract States NextState(Events eventType);

        public bool ParseString(string expression)
        {
            for (int i = 0; i < expression.Length && CurrentState != States.Error; i++)
            {
                Start();
                CurrentState = NextState(RecognizeEvent(expression[i]));
            }
            return CurrentState == States.Exit;
        }
    }
}
