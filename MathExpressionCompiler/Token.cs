namespace MathExpressionCompiler;

public class Token
{
    public readonly TokenType Type;
    public readonly string Lexeme;
    public readonly Object? Literal;

    public Token(TokenType type, string lexeme, Object? literal)
    {
        Type = type;
        Lexeme = lexeme;
        Literal = literal;
    }

    public override string ToString()
    {
        return $"{Type.ToString().PadRight(18)}  {Lexeme.PadLeft(12)}  {(Literal == null ? "" : Literal.ToString()!.PadLeft(10))}";
    }
}