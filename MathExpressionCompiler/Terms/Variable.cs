using System.Collections.Generic;

namespace MathExpressionCompiler;

public class Variable<Space> : Term<Space>
{
    public readonly string Name;
    public readonly Space? Value;

    public Variable(string name, Space? value)
    {
        this.Name = name;
        Console.WriteLine($"NewName: {name} {Name}");
        this.Value = value;
    }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitVariableTerm(this);
    }
}
