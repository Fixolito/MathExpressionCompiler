using System.Linq.Expressions;
using System.Numerics;

namespace MEC;

public static class MathExpressionCompiler
{
    public static Delegate CreateDelegate<T>(string term, out string[] variableNameList)
    {
        return MathExpressionCreator.CreateRegularInputs<T>(term, out variableNameList);
    }

    public static Delegate CreateDelegateWithDoubleParameters<T>(string term, out string[] variableNameList)
    {
        return MathExpressionCreator.CreateDoubleInputs<T>(term, out variableNameList);
    }

    static void Main(string[] args)
    {
        Delegate sum = MathExpressionCreator.CreateDoubleInputs<Complex>("sum(x,1,5,sum(y,1,x,sqrt(a+x)))* bla + sum(x,0,9,x)", out string[] tmp);
        Console.WriteLine($"[{string.Join(", ", tmp)}]");
        Console.WriteLine(sum.DynamicInvoke(4,7));

        var del = MathExpressionCompiler.CreateDelegateWithDoubleParameters<Complex>("polar(a,b)", out string[] variableNameList);
        Console.WriteLine(del.DynamicInvoke(4,7));
    }
}