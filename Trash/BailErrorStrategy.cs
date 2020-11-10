namespace Trash
{
    using Antlr4.Runtime;

    public class MyBailErrorStrategy : BailErrorStrategy
    {
        ITokenStream stream;

        public MyBailErrorStrategy(ITokenStream s)
        {
            stream = s;
        }

        public override void Recover(Parser recognizer, RecognitionException e)
        {
            for (; ; )
            {
                var p = stream.LA(1);
                if (p == -1) break;
                if (p != ReplParser.VWS)
                    stream.Consume();
            }
            base.Recover(recognizer, e);
        }
    }
}
