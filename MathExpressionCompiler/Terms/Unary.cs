using System.Collections.Generic;

namespace MEC;

internal class Unary<Space> : Term<Space>
{
    internal readonly Token UnaryOperator;
    internal readonly Term<Space> Right;

    internal Unary(Token unaryOperator, Term<Space> right)
    {
        this.UnaryOperator = unaryOperator;
        this.Right = right;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitUnaryTerm(this);
    }
}
