using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.IO;
using System.Reflection;

#pragma warning disable CA1012 // Abstract types should not have constructors
public abstract class LexerAdaptor : Lexer
#pragma warning restore CA1012 // But Lexer demands it - old 
{
    // I copy a reference to the stream, so It can be used as a Char Stream, not as a IISStream
    readonly ICharStream stream;
    // Tokens are read only so I hack my way
    readonly FieldInfo tokenInput = typeof(CommonToken).GetField("_type", BindingFlags.NonPublic | BindingFlags.Instance);
    protected LexerAdaptor(ICharStream input)
            : base(input, Console.Out, Console.Error)
    {
        stream = input;
    }

    protected LexerAdaptor(ICharStream input, TextWriter output, TextWriter errorOutput)
            : base(input, output, errorOutput)
    {
        stream = input;
    }
    /**
        * Track whether we are inside of a rule and whether it is lexical parser. _currentRuleType==TokenConstants.InvalidType
        * means that we are outside of a rule. At the first sign of a rule name reference and _currentRuleType==invalid, we
        * can assume that we are starting a parser rule. Similarly, seeing a token reference when not already in rule means
        * starting a token rule. The terminating ';' of a rule, flips this back to invalid type.
        *
        * This is not perfect logic but works. For example, "grammar T;" means that we start and stop a lexical rule for
        * the "T;". Dangerous but works.
        *
        * The whole point of this state information is to distinguish between [..arg actions..] and [charsets]. Char sets
        * can only occur in lexical rules and arg actions cannot occur.
        */
    private static int PREQUEL_CONSTRUCT = -10;
    private int CurrentRuleType { get; set; } = TokenConstants.InvalidType;
    private bool insideOptionsBlock = false;

    protected void handleBeginArgument()
    {
        if (InLexerRule)
        {
            PushMode(AntlrPrimeLexer.LexerCharSet);
            More();
        }
        else
        {
            PushMode(AntlrPrimeLexer.Argument);
        }
    }

    protected void handleEndArgument()
    {
        PopMode();
        if (ModeStack.Count > 0)
        {
            CurrentRuleType = (AntlrPrimeLexer.ARGUMENT_CONTENT);
        }
    }

    protected void handleEndAction()
    {
        int oldMode = CurrentMode;
        int newMode = PopMode();
        bool isActionWithinAction = ModeStack.Count > 0
            && newMode == AntlrPrimeLexer.TargetLanguageAction
            && oldMode == newMode;

        if (isActionWithinAction)
        {
            Type = (AntlrPrimeLexer.ACTION_CONTENT);
        }
    }

    public void handleOptionsLBrace()
    {
        if (insideOptionsBlock)
        {
            Type = (AntlrPrimeLexer.BEGIN_ACTION);
            PushMode(AntlrPrimeLexer.TargetLanguageAction);
        }
        else
        {
            Type = (AntlrPrimeLexer.LBRACE);
            insideOptionsBlock = true;
        }
    }

    public override IToken Emit()
    {
        if ((Type == AntlrPrimeLexer.OPTIONS || Type == AntlrPrimeLexer.TOKENS || Type == AntlrPrimeLexer.CHANNELS)
                && CurrentRuleType == TokenConstants.InvalidType)
        { // enter prequel construct ending with an RBRACE
            CurrentRuleType = PREQUEL_CONSTRUCT;
        }
        else if (Type == AntlrPrimeLexer.RBRACE && CurrentRuleType == PREQUEL_CONSTRUCT)
        { // exit prequel construct
            CurrentRuleType = TokenConstants.InvalidType;
        }
        else if (Type == AntlrPrimeLexer.AT && CurrentRuleType == TokenConstants.InvalidType)
        { // enter action
            CurrentRuleType = AntlrPrimeLexer.AT;
        }
        else if (Type == AntlrPrimeLexer.END_ACTION && CurrentRuleType == AntlrPrimeLexer.AT)
        { // exit action
            CurrentRuleType = TokenConstants.InvalidType;
        }
        else if (Type == AntlrPrimeLexer.ID)
        {
            char firstChar = stream.GetText(Interval.Of(TokenStartCharIndex, TokenStartCharIndex))[0];
            if (char.IsUpper(firstChar))
            {
                Type = AntlrPrimeLexer.TOKEN_REF;
            }
            else
            {
                Type = AntlrPrimeLexer.RULE_REF;
            }

            if (CurrentRuleType == TokenConstants.InvalidType)
            { // if outside of rule def
                CurrentRuleType = Type; // set to inside lexer or parser rule
            }
        }
        else if (Type == AntlrPrimeLexer.SEMI)
        { // exit rule def
            CurrentRuleType = TokenConstants.InvalidType;
        }
        return base.Emit();
    }

    private bool InLexerRule => CurrentRuleType == AntlrPrimeLexer.TOKEN_REF;


    private bool InParserRule => CurrentRuleType == AntlrPrimeLexer.RULE_REF;

}
