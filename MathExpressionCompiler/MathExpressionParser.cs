using System.ComponentModel;
using System.Linq.Expressions;
using System.Numerics;

namespace MathExpressionCompiler;

public class MathExpressionParser()
{
    public static Expression GetExpression<T>(string source)
    {
        if(typeof(T) != typeof(double) && typeof(T) != typeof(Complex))
        {
            throw new Exception("Only <double> for real numbers or <Complex> for complex numbers are allowed.");
        }
        return Run<T>(source); 
    }

    private static Expression Run<T>(string source)
    {
        Scanner<T> scanner = new(source);
        List<Token> tokens = scanner.ScanTokens();
        foreach(var t in tokens)
        {
            Console.WriteLine(t);
        }
        // Implement parsing logic here
        throw new NotImplementedException();
    }

    internal static void Error(string message) { Report("", message); }
    private static void Report(string where, string message)
    {
        Console.Error.WriteLine($"Error{where}: {message}");
    }

}
