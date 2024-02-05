using System.Collections.Generic;

namespace MathExpressionCompiler;

public class Grouping<Space> : Term<Space>
{
    public readonly Term<Space> Expression;

    public Grouping(Term<Space> expression)
    {
        this.Expression = expression;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitGroupingTerm(this);
    }
}
