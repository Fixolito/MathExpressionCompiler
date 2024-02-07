using System.Collections.Generic;

namespace MEC;

internal class Variable<Space> : Term<Space>
{
    internal readonly string Name;
    internal readonly Space? Value;

    internal Variable(string name, Space? value)
    {
        this.Name = name;
        this.Value = value;
    }

    internal override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitVariableTerm(this);
    }
}
