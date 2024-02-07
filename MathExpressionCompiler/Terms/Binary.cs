using System.Collections.Generic;

namespace MEC;

internal class Binary<Space> : Term<Space>
{
    internal readonly Term<Space> Left;
    internal readonly Token BinaryOperator;
    internal readonly Term<Space> Right;

    internal Binary(Term<Space> left, Token binaryOperator, Term<Space> right)
    {
        this.Left = left;
        this.BinaryOperator = binaryOperator;
        this.Right = right;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitBinaryTerm(this);
    }
}
