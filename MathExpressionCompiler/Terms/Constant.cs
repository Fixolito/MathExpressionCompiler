using System.Collections.Generic;

namespace MEC;

internal class Constant<Space> : Term<Space>
{
    internal readonly Space Value;

    internal Constant(Space value)
    {
        this.Value = value;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitConstantTerm(this);
    }
}
