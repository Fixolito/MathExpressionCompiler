namespace MEC;

internal class Token
{
    internal readonly TokenType Type;
    internal readonly string Lexeme;
    internal readonly Object? Literal;

    internal Token(TokenType type, string lexeme, Object? literal)
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