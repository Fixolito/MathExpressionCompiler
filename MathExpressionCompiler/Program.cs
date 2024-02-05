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
        //var tmp = MathExpressionParser.GetExpression<double>("2+test/(2+sin(ball))");
        var tmp = MathExpressionParser.GetExpressionTree<double>("2+sin(ball)");
        
        MathExpressionParser.GetExpressionTree<Complex>("-3/sin * polar(-4^sum(i=0,200,i+2)+test)");
        MathExpressionParser.GetExpressionTree<Complex>("((1234i+i123)),.2i/test+ii34.4+.44");
        //MathExpressionParser.GetExpression<Complex>("bla");
        //MathExpressionParser.GetExpression<int>("bla");
    }


}
