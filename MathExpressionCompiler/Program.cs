using System.Linq.Expressions;
using System.Numerics;

namespace MathExpressionCompiler;

public class Program
{
    private static bool HadError = false;
    private static bool HadRuntimeError = false;
    static void Main(string[] args)
    {
        /* if (args.Length > 1) {
            Console.WriteLine("Usage: jlox [script]");
            Environment.Exit(64);
        } else if (args.Length == 1) {
            Run(args[0]);
        } else {
            RunPrompt();
        } */
        var test = new MathExpressionParser();
        MathExpressionParser.GetExpression<Complex>("((1234i+i123)),.2i/test+ii34.4+.44");
        //MathExpressionParser.GetExpression<Complex>("bla");
        //MathExpressionParser.GetExpression<int>("bla");
    }


}
