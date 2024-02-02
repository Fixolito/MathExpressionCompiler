namespace MathExpressionCompiler;

public class Parser(List<Token> tokens)
{
    public class ParseError : Exception
    {
        public ParseError() : base() { }
        public ParseError(string message) : base(message) { }
        public ParseError(string message, Exception inner) : base(message, inner) { }
    }

    private readonly List<Token> Tokens = tokens;
    private int Current = 0;

    public List<Statement> Parse()
    {
        //this needs to be done next after creating the AST code generation part
    }

}