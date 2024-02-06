using System.Linq.Expressions;

namespace MathExpressionCompiler;

public class VariableManager<Space>
{
    private List<ParameterExpression> Bound = [];
    private List<string> BoundNames = [];
    private Dictionary<string, int> CurrentBoundIndex = [];
    private Dictionary<string, int> NextBoundIndex = [];

    private List<ParameterExpression> Parameters = [];
    private List<string> ParameterNames = [];
    private Dictionary<string, ParameterExpression> ParameterLookup = [];

    private List<ParameterExpression> CurrentEnvironment = [];

    public VariableManager(IEnumerable<string> unboundVariables, IEnumerable<string> boundVariableNames)
    {
        foreach (string parameterName in unboundVariables)
        {
            var parameterExpression = Expression.Parameter(typeof(Space), parameterName);
            Parameters.Add(parameterExpression);
            ParameterNames.Add(parameterName);
            ParameterLookup[parameterName] = parameterExpression;
        }

        foreach (string boundName in boundVariableNames)
        {
            Bound.Add(Expression.Parameter(typeof(Space), boundName));
            BoundNames.Add(boundName);
            NextBoundIndex.TryAdd(boundName, Helper.IndexOfFirstOccurrence(BoundNames, boundName));
            CurrentBoundIndex.TryAdd(boundName, -1);
        }
    }

    public List<ParameterExpression> GetUnboundParameters()
    {
        return Parameters;
    }

    public ParameterExpression GetVariable(string name)
    {
        var enviromentVariables = CurrentEnvironment.FindAll(p => p.Name!.Equals(name));
        if (enviromentVariables.Count == 1)
        {
            return enviromentVariables[0];
        }
        if (enviromentVariables.Count > 1)
        {
            throw new Exception($"Multiple enviroment variables are named {name}.");
        }
        return ParameterLookup[name];
    }

    public void LeaveEnvironment(ParameterExpression boundVariable)
    {
        CurrentBoundIndex[boundVariable.Name!] = -1;
        CurrentEnvironment.Remove(boundVariable);
    }

    public ParameterExpression EnterEnvironment(string boundVariableName)
    {
        int index = NextBoundIndex[boundVariableName];
        CurrentBoundIndex[boundVariableName] = index;
        NextBoundIndex[boundVariableName] = Helper.IndexOfNextOccurrence(BoundNames, boundVariableName, index);
        CurrentEnvironment.Add(Bound[index]);
        return Bound[index];
    }
    
}

