using System.ComponentModel;
using System.Linq.Expressions;
using System.Numerics;

namespace MathExpressionCompiler;

public class MathExpressionCreator()
{
    public static ParameterExpression[] LastParameters { get; private set; }=[];
    public static Expression? LastRoot { get; private set; } = null;
    public static Delegate? LastCompiled { get; private set; } = null;
    public static Delegate? LastCompiledDoubleInput  { get; private set; } = null;

    public static Delegate CreateRegularInputs<T>(string source, out string[] variableOrder)
    {
        return Create<T>(source, false, out variableOrder);
    }

    public static Delegate CreateDoubleInputs<T>(string source, out string[] variableOrder)
    {
        return Create<T>(source, true, out variableOrder);
    }


    private static Delegate Create<T>(string source, bool doubleAsInputs, out string[] variableOrder)
    {
        if(typeof(T) != typeof(double) && typeof(T) != typeof(Complex))
        {
            throw new Exception("Only <double> for real numbers or <Complex> for complex numbers are allowed.");
        }
        return Run<T>(source, doubleAsInputs, out variableOrder);
    }


    private static Delegate Run<T>(string source, bool doubleAsInputs, out string[] variableOrder)
    {
        var space = Helper.GetNumberSpace(typeof(T));

        Scanner<T> scanner = new(source);
        List<Token> tokens = scanner.ScanTokens();

        Parser<T> parser = new(tokens);
        List<Term<T>> terms = parser.Parse();

        ExpressionCreator<T> expressionCreator = new(terms);
        expressionCreator.CreateExpression();
        variableOrder = Helper.VariableNames(expressionCreator.GetParameters());
        return space switch
        {
            NumberSpace.REAL => expressionCreator.Compile(),
            NumberSpace.COMPLEX => doubleAsInputs
                ? CompileToDoubleInputs(expressionCreator.RootExpression!, expressionCreator.GetParameters())
                : expressionCreator.Compile(),
            _ => throw new NotImplementedException()
        };
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


    //
    public static Delegate CompileToDoubleInputs(Expression complexExpression, params ParameterExpression[] complexParameters)
    {
        var doubleParameters = new ParameterExpression[complexParameters.Length];
        for (int i = 0; i < complexParameters.Length; i++)
        {
            doubleParameters[i] = Expression.Parameter(typeof(double), complexParameters[i].Name);
        }

        var complexConversions = new Expression[complexParameters.Length];
        for (int i = 0; i < complexConversions.Length; i++)
        {
            var constructorInfo = typeof(Complex).GetConstructor(new[] { typeof(double), typeof(double) });
            complexConversions[i] = Expression.New(constructorInfo!, doubleParameters[i], Expression.Constant(0.0, typeof(double)));
        }

        var bodyWithConversions = new ExpressionReplacer(complexParameters, complexConversions).Visit(complexExpression);

        var lambda = Expression.Lambda(bodyWithConversions, doubleParameters);
        return lambda.Compile();
    }

    

}
