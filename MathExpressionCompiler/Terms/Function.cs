using System.Collections.Generic;

namespace MathExpressionCompiler;

public class Function<Space> : Term<Space>
{
    public readonly Token FunctionOperator;
    public readonly List<Term<Space>> Parameters;

    public Function(Token functionOperator, List<Term<Space>> parameters)
    {
        this.FunctionOperator = functionOperator;
        this.Parameters = parameters;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitFunctionTerm(this);
    }
}
