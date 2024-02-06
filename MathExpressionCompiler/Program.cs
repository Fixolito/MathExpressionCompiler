using System.Linq.Expressions;
using System.Numerics;

namespace MathExpressionCompiler;

public class Program
{
    static void Main(string[] args)
    {
        var test = new MathExpressionCreator();
        Delegate complex = MathExpressionCreator.CreateRegularInputs<Complex>("-2^3^2", out _);
        Delegate real = MathExpressionCreator.CreateDoubleInputs<Complex>("-2^3^2", out _);
        Console.WriteLine(complex.DynamicInvoke());
        Console.WriteLine(real.DynamicInvoke());
        //Console.WriteLine(Complex.FromPolarCoordinates(8, 3));
    }


}
