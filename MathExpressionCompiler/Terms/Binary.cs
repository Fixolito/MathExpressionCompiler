using System.Collections.Generic;

namespace MathExpressionCompiler;

public class Binary<Space> : Term<Space>
{
    public readonly Term<Space> Left;
    public readonly Token BinaryOperator;
    public readonly Term<Space> Right;

    public Binary(Term<Space> left, Token binaryOperator, Term<Space> right)
    {
        this.Left = left;
        this.BinaryOperator = binaryOperator;
        this.Right = right;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitBinaryTerm(this);
    }
}
