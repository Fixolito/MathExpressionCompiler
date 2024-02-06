using System.Text;
using System.Xml;

namespace MathExpressionCompiler;

public class ASTVariableNameCollector<Space>(List<Term<Space>> terms) : Term<Space>.IVisitor<List<string>>
{
    private bool RemoveBoundVariables = true;
    public List<string> BoundVariableNames = [];
    public List<string> UnboundVariableNames = [];
    private List<string> EnvironmentVaribaleNames = [];
    private Term<Space> Root = terms[0];
    private bool IsAssgning = false;
    private bool hasRun;
    public List<string> CollectNames(bool removeBoundVariables = true)
    {
        RemoveBoundVariables = removeBoundVariables;
        BoundVariableNames.Clear();
        UnboundVariableNames.Clear();
        EnvironmentVaribaleNames.Clear();
        List<string> tmp = Root.Accept(this);
        hasRun = true;
        return tmp;
    }

    public List<string> CollectNames(Term<Space> node, bool removeBoundVariables = true)
    {
        var rememberRoot = Root;
        Root = node;
        RemoveBoundVariables = removeBoundVariables;
        BoundVariableNames.Clear();
        UnboundVariableNames.Clear();
        EnvironmentVaribaleNames.Clear();
        List<string> tmp = Root.Accept(this);
        Root = rememberRoot;
        hasRun = true;
        return tmp;
    }
    

    public List<string> VisitBinaryTerm(Binary<Space> term)
    {
        List<string> left = term.Left.Accept(this);
        List<string> right = term.Right.Accept(this);
        return left.Union(right).ToList();
    }

    public List<string> VisitConstantTerm(Constant<Space> term)
    {
        return [];
    }

    public List<string> VisitFunctionTerm(Function<Space> term)
    {
        TokenType type = term.FunctionOperator.Type;
        return (type) switch
        {
            TokenType.SIN or TokenType.COS or TokenType.TAN or TokenType.SQRT => term.Parameters[0].Accept(this),
            TokenType.POLAR => TwoParameters(term),
            TokenType.SUM => Sum(term),
            _ => throw new NotImplementedException()
        };
    }

    private List<string> Sum(Function<Space> term)
    {
        Console.WriteLine($"SumVisit:");
        IsAssgning = true;
        string loopVariable = term.Parameters[0].Accept(this)[0];
        if (EnvironmentVaribaleNames.Contains(loopVariable))
        {
            throw new Exception($"{loopVariable} has already been bound.");
        }
        BoundVariableNames.Add(loopVariable);
        EnvironmentVaribaleNames.Add(loopVariable);
        List<string> start = term.Parameters[1].Accept(this);
        List<string> end = term.Parameters[2].Accept(this);
        List<string> body = term.Parameters[3].Accept(this);
        if (start.Contains(loopVariable))
        {
            throw new ArgumentException($"Term defnining start must not contain the loop variable {loopVariable} itself.");
        }
        if (end.Contains(loopVariable))
        {
            throw new ArgumentException($"Term defnining end must not contain the loop variable {loopVariable} itself.");
        }
        Helper.AddToListANoDuplication(start, end, body);
        if (start.Any(p => BoundVariableNames.Contains(p) && !EnvironmentVaribaleNames.Contains(p)))
        {
            throw new ArgumentException("A bound variable is out of scope.");
        }
        start.Remove(loopVariable);
        Helper.AddToListANoDuplication(UnboundVariableNames, start);
        EnvironmentVaribaleNames.Reverse();
        EnvironmentVaribaleNames.Remove(loopVariable);
        EnvironmentVaribaleNames.Reverse();
        if (RemoveBoundVariables) return start;
        start.Add(loopVariable);
        return start;
    }

    private List<string> TwoParameters(Function<Space> term)
    {
        List<string> first = term.Parameters[0].Accept(this);
        List<string> second = term.Parameters[1].Accept(this);
        Helper.AddToListANoDuplication(first, second);
        return first;
    }

    public List<string> VisitGroupingTerm(Grouping<Space> term)
    {
        return term.Expression.Accept(this);
    }

    public List<string> VisitUnaryTerm(Unary<Space> term)
    {
        return term.Right.Accept(this);
    }

    public List<string> VisitVariableTerm(Variable<Space> term)
    {
        Console.WriteLine($"VariableVisit {term.Name}");
        string name = term.Name;
        if (IsAssgning)
        {
            IsAssgning = false;
            return [name];
        }

        if (BoundVariableNames.Contains(name) && !EnvironmentVaribaleNames.Contains(name))
        {
            throw new Exception($"Variable {name} is out of scope.");
        }
        if (!EnvironmentVaribaleNames.Contains(name))
        {
            Helper.AddToListANoDuplication(UnboundVariableNames, [name]);
            return [term.Name];
        }
        if (RemoveBoundVariables) return [];
        return [name];
    }

    public void PrintUnbound()
    {
        if (!hasRun) CollectNames();
        Console.WriteLine($"UnboundVariableNames: [{string.Join(", ", UnboundVariableNames)}]");
    }

    public void PrintBound()
    {
        if (!hasRun) CollectNames();
        Console.WriteLine($"BoundVariableNames: [{string.Join(", ", BoundVariableNames)}]");
    }
    
}

