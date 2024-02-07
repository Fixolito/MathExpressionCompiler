using System.Collections.Generic;

namespace MEC;

internal class Function<Space> : Term<Space>
{
    internal readonly Token FunctionOperator;
    internal readonly List<Term<Space>> Parameters;

    internal Function(Token functionOperator, List<Term<Space>> parameters)
    {
        this.FunctionOperator = functionOperator;
        this.Parameters = parameters;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitFunctionTerm(this);
    }
}
