using System.Linq.Expressions;

namespace MEC;

internal class VariableManager<Space>
{
    internal bool IsActive = true;
    private List<ParameterExpression> Bound = [];
    private List<string> BoundNames = [];
    private Dictionary<string, int> CurrentBoundIndex = [];
    private Dictionary<string, int> NextBoundIndex = [];

    private List<ParameterExpression> Parameters = [];
    private List<string> ParameterNames = [];
    private Dictionary<string, ParameterExpression> ParameterLookup = [];

    private List<ParameterExpression> CurrentEnvironment = [];

    internal VariableManager(IEnumerable<string> unboundVariables, IEnumerable<string> boundVariableNames)
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
            Bound.Add(Expression.Variable(typeof(Space), boundName));
            BoundNames.Add(boundName);
            NextBoundIndex.TryAdd(boundName, Helper.IndexOfFirstOccurrence(BoundNames, boundName));
            CurrentBoundIndex.TryAdd(boundName, -1);
        }
    }

    internal List<ParameterExpression> PrecalculateEnvironment(List<string> unboundNames, List<string> boundNames, List<string> freshlyBoundNames)
    {
        foreach(string name in freshlyBoundNames)
        {
            if (unboundNames.Remove(name))
            {
                boundNames.Add(name);
            }
        }
        List<ParameterExpression> result = [];
        foreach (string name in unboundNames)
        {
            if (ParameterLookup.TryGetValue(name, out ParameterExpression? unbound))
            {
                result.Add(unbound);
            }
        }
        foreach (string name in boundNames)
        {
            var tmp = CurrentEnvironment.Find(p => p.Name!.Equals(name));
            if (tmp is not null)
            {
                result.Add(tmp);
            }
        }
        return result;
    }

    internal List<ParameterExpression> GetUnboundParameters()
    {
        return Parameters;
    }

    internal ParameterExpression GetVariable(string name)
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

    internal void LeaveEnvironment(ParameterExpression boundVariable)
    {
        CurrentBoundIndex[boundVariable.Name!] = -1;
        CurrentEnvironment.Remove(boundVariable);
    }

    internal ParameterExpression EnterEnvironment(string boundVariableName)
    {
        int index = NextBoundIndex[boundVariableName];
        CurrentBoundIndex[boundVariableName] = index;
        NextBoundIndex[boundVariableName] = Helper.IndexOfNextOccurrence(BoundNames, boundVariableName, index);
        CurrentEnvironment.Add(Bound[index]);
        return Bound[index];
    }
    
}

