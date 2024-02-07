using System.Text;
using System.Xml;

namespace MEC;

internal class ASTPrinter<Space> : Term<Space>.IVisitor<string>
{
    internal string Print(Term<Space> input)
    {
        string tmp = input.Accept(this);
        Console.WriteLine(tmp);
        return tmp;
    }
    public string VisitBinaryTerm(Binary<Space> term)
    {
        return Parenthesize(term.BinaryOperator.Lexeme, term.Left, term.Right);
    }

    public string VisitConstantTerm(Constant<Space> term)
    {
        if (term.Value is null) return "NULL";
        return term.Value.ToString()!;
    }

    public string VisitFunctionTerm(Function<Space> term)
    {
        StringBuilder builder = new();
        builder.Append(term.FunctionOperator.Type.ToString());
        builder.Append('(').Append(term.Parameters[0].Accept(this));
        for (int i = 1; i < term.Parameters.Count - 1; i++)
        {
            builder.Append(", ").Append(term.Parameters[i].Accept(this));
        }
        builder.Append(')');
        return builder.ToString();
    }

    public string VisitGroupingTerm(Grouping<Space> term)
    {
        return Parenthesize("Group", term.Expression);
    }

    public string VisitUnaryTerm(Unary<Space> term)
    {
        return Parenthesize(term.UnaryOperator.Lexeme, term.Right);
    }

    public string VisitVariableTerm(Variable<Space> term)
    {
        return term.Name;
    }

    private string Parenthesize(string name, params Term<Space>[] terms)
    {
        StringBuilder builder = new();
        builder.Append('(').Append(name);
        foreach(Term<Space> t in terms)
        {
            builder.Append(' ');
            builder.Append(t.Accept(this));
        }
        builder.Append(')');
        return builder.ToString();
    }
}

