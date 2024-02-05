using System.Collections.Generic;

namespace MathExpressionCompiler;

public class Variable<Space> : Term<Space>
{
    public readonly string Name;

    public Variable(string name)
    {
        this.Name = name;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitVariableTerm(this);
    }
}
