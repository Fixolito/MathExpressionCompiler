using System.Collections.Generic;

namespace MEC;

internal class Grouping<Space> : Term<Space>
{
    internal readonly Term<Space> Expression;

    internal Grouping(Term<Space> expression)
    {
        this.Expression = expression;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitGroupingTerm(this);
    }
}
