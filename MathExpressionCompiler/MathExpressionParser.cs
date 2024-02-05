using System.ComponentModel;
using System.Linq.Expressions;
using System.Numerics;

namespace MathExpressionCompiler;

public class MathExpressionParser()
{
    public static Expression GetExpressionTree<T>(string source)
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
        Parser<T> parser = new(tokens);
        Term<T> root = parser.Parse();

        ASTPrinter<T> printer = new();
        printer.Print(root);

        Console.WriteLine(root.GetType());
        Console.WriteLine(((Binary<T>)root).BinaryOperator);

        Console.WriteLine("DONE");

        throw new NotImplementedException();
    }

    internal static void Error(string message) { Report("", message); }
    internal static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF) Report(" at end", message);
        else Report($" at <{token.Lexeme}>", message);
    }

    private static void Report(string where, string message)
    {
        Console.Error.WriteLine($"Error{where}: {message}");
    }

}
