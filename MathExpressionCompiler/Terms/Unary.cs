using System.Collections.Generic;

namespace MathExpressionCompiler;

public class Unary<Space> : Term<Space>
{
    public readonly Token UnaryOperator;
    public readonly Term<Space> Right;

    public Unary(Token unaryOperator, Term<Space> right)
    {
        this.UnaryOperator = unaryOperator;
        this.Right = right;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitUnaryTerm(this);
    }
}
