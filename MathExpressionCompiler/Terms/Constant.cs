using System.Collections.Generic;

namespace MathExpressionCompiler;

public class Constant<Space> : Term<Space>
{
    public readonly Space Value;

    public Constant(Space value)
    {
        this.Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitConstantTerm(this);
    }
}
