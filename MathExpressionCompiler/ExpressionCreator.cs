using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Xml;

namespace MEC;

internal class ExpressionCreator<Space> : Term<Space>.IVisitor<Expression>
{
    private ASTVariableNameCollector<Space> NameCollector;
    private VariableManager<Space> VariableManager;

    private readonly List<Term<Space>> Terms;
    private Term<Space> RootTerm => Terms[0];
    internal Expression? RootExpression { get; private set; }
    

    private NumberSpace NumberSpace;

    internal ExpressionCreator(List<Term<Space>> terms)
    {
        Terms = terms;
        NameCollector = new(terms);
        NameCollector.CollectNames();

        //NameCollector.PrintBound();
        //NameCollector.PrintUnbound();

        VariableManager = new(NameCollector.UnboundVariableNames, NameCollector.BoundVariableNames);
        NumberSpace = Helper.GetNumberSpace(typeof(Space));
    }

    internal Delegate Compile()
    {
        if (RootExpression is null)
        {
            CreateExpression();
        }
        var body = RootExpression;
        var parameters = VariableManager.GetUnboundParameters();
        var lambda = Expression.Lambda(body!, parameters);
        return lambda.Compile();
    }

    internal ParameterExpression[] GetParameters()
    {
        return [.. VariableManager.GetUnboundParameters()];
    }

    internal void CreateExpression()
    {
        RootExpression = RootTerm.Accept(this);
    }

    public Expression VisitBinaryTerm(Binary<Space> term)
    {
        var left = term.Left.Accept(this);
        var right = term.Right.Accept(this);
        TokenType type = term.BinaryOperator.Type;
        switch (type)
        {
            case TokenType.PLUS:
                return Expression.Add(left, right);
            case TokenType.MINUS:
                return Expression.Subtract(left, right);
            case TokenType.STAR:
                return Expression.Multiply(left, right);
            case TokenType.SLASH:
                return Expression.Divide(left, right);
            case TokenType.CARET:
                return Pow(left, right);
            default:
                throw new NotSupportedException($"Unsupported operator {type}");
        }
    }

    private MethodCallExpression Pow(Expression left, Expression right)
    {
        return NumberSpace switch
        {
            NumberSpace.REAL => PowReal(left, right),
            NumberSpace.COMPLEX => PowComplex(left, right),
            _ => throw new NotImplementedException()
        };
    }
    private MethodCallExpression PowComplex(Expression left, Expression right)
    {
        MethodInfo info = typeof(Complex).GetMethod("Pow", [typeof(Complex), typeof(Complex)])!;
        MethodCallExpression expr = Expression.Call(info, left, right);
        return expr;
    }
    private MethodCallExpression PowReal(Expression left, Expression right) 
    {
        MethodInfo info = typeof(Math).GetMethod("Pow", [typeof(double), typeof(double)])!;
        MethodCallExpression expr = Expression.Call(info, left, right);
        return expr;
    }

    public Expression VisitVariableTerm(Variable<Space> term)
    {
        if (VariableManager.IsActive) return VariableManager.GetVariable(term.Name);
        return Expression.Parameter(typeof(Space), term.Name);
    }

    public Expression VisitConstantTerm(Constant<Space> term)
    {
        return Expression.Constant(term.Value);
    }

    public Expression VisitFunctionTerm(Function<Space> term)
    {
        TokenType type = term.FunctionOperator.Type;
        return (type) switch
        {
            TokenType.SIN => Sinus(term),
            TokenType.COS => Cosinus(term),
            TokenType.TAN => Tangent(term),
            TokenType.SQRT => SquareRoot(term),
            TokenType.POLAR => Polar(term),
            TokenType.SUM => Sum(term),
            _ => throw new NotImplementedException()
        };
    }

    private BlockExpression Sum(Function<Space> term)
    {
        VariableManager.IsActive = false;
        string loopVariableName = ((ParameterExpression)term.Parameters[0].Accept(this)).Name!;
        VariableManager.IsActive = true;
        ParameterExpression loopVariable = VariableManager.EnterEnvironment(loopVariableName);
        Expression start = term.Parameters[1].Accept(this);
        Expression end = term.Parameters[2].Accept(this);
        Expression body = term.Parameters[3].Accept(this);

        var returnTarget = Expression.Label(typeof(Space));
        var resultVariable = Expression.Variable(typeof(Space), "RESULTSUM");
        var breakLabel = Expression.Label("LoopBreak");
        Space zero = (Space)Helper.Zero(NumberSpace);
        Space one = (Space)Helper.One(NumberSpace);
        var initializeResult = Expression.Assign(resultVariable, Expression.Constant(zero));
        var initializeIterationVariable = Expression.Assign(loopVariable, start);

        Expression condition = Expression.Variable(typeof(Space), "DUMMY");
        switch (NumberSpace)
        {
            case NumberSpace.REAL:
                condition = Expression.LessThanOrEqual(loopVariable, end);
                break;
            case NumberSpace.COMPLEX:
                Expression loopVariableReal = Expression.Property(loopVariable, nameof(System.Numerics.Complex.Real));
                Expression endReal = Expression.Property(end, nameof(System.Numerics.Complex.Real));
                condition = Expression.LessThanOrEqual(loopVariableReal, endReal);
                break;
            default:
                break;
        };

        var incrementIterationVariable = Expression.Assign(loopVariable, Expression.Add(loopVariable, Expression.Constant(one)));

        var updateResultVariable = Expression.Assign(resultVariable, Expression.Add(resultVariable, body));

        var loopBlock = Expression.Block(body, updateResultVariable, incrementIterationVariable);//, condition);

        var loop = Expression.Loop(
            Expression.IfThenElse(
                condition,
                loopBlock,
                Expression.Break(breakLabel)
            ),
            breakLabel
        );

        var block = Expression.Block(
            [resultVariable, loopVariable],
            initializeResult,
            initializeIterationVariable,
            loop,
            resultVariable,
            Expression.Label(returnTarget, resultVariable)
        );

        VariableManager.LeaveEnvironment(loopVariable);

        return block;
    }

    public Expression CreateSumExpressionWithVariables(
        ParameterExpression iterationVariable,
        Expression start,
        Expression end,
        Func<ParameterExpression, IEnumerable<Expression>> bodyFunc,
        //Func<ParameterExpression, IEnumerable<Expression>> bodyFunc,
        IEnumerable<ParameterExpression> innerVariables)
    {
        // Create a label target for returning the final result
        var returnTarget = Expression.Label(typeof(double));
        var resultVariable = Expression.Variable(typeof(double), "result");
        var breakLabel = Expression.Label("LoopBreak");

        // Initialize the result variable and iteration variable
        var initializeResult = Expression.Assign(resultVariable, Expression.Constant(0.0));
        var initializeIterationVariable = Expression.Assign(iterationVariable, start);

        // Construct the loop body using the provided bodyFunc
        var loopBody = bodyFunc(iterationVariable);

        // Loop condition
        var condition = Expression.LessThanOrEqual(iterationVariable, end);

        // Increment iteration variable
        var incrementIterationVariable = Expression.Assign(iterationVariable, Expression.Add(iterationVariable, Expression.Constant(1.0)));

        // The loop block, including the body and increment
        var loopBlock = Expression.Block(
            new[] { iterationVariable }.Union(innerVariables),
            loopBody.Concat(new[] { incrementIterationVariable })
        );

        // Construct the loop using a conditional expression for iteration
        var loop = Expression.Loop(
            Expression.IfThenElse(
                condition,
                loopBlock,
                Expression.Break(breakLabel)
            ),
            breakLabel
        );

        // The overall block, including initialization, loop, and return
        var block = Expression.Block(
            new[] { resultVariable }.Union(innerVariables),
            initializeResult,
            initializeIterationVariable,
            loop,
            Expression.Label(returnTarget, resultVariable)
        );

        return Expression.Lambda(block);
    }

    private MethodCallExpression Polar(Function<Space> term)
    {
        return NumberSpace switch
        {
            NumberSpace.REAL => throw new InvalidCastException("Can't convert into complex numbers, because number space is real."),
            NumberSpace.COMPLEX => PolarComplex(term),
            _ => throw new NotImplementedException()
        };
    }

    private MethodCallExpression PolarComplex(Function<Space> term)
    {
        MemberExpression magnitudeReal = Expression.Property(term.Parameters[0].Accept(this), "Real");
        MemberExpression angleReal = Expression.Property(term.Parameters[1].Accept(this), "Real");
        MethodInfo info = typeof(Complex).GetMethod("FromPolarCoordinates", [typeof(double), typeof(double)])!;
        MethodCallExpression expr = Expression.Call(info, magnitudeReal, angleReal);
        return expr;
    }

    private MethodCallExpression SquareRoot(Function<Space> term)
    {
        return NumberSpace switch
        {
            NumberSpace.REAL => SquareRootReal(term),
            NumberSpace.COMPLEX => SquareRootComplex(term),
            _ => throw new NotImplementedException()
        };
    }
    private MethodCallExpression SquareRootComplex(Function<Space> term)
    {
        MethodInfo info = typeof(Complex).GetMethod("Sqrt", [typeof(Complex)])!;
        MethodCallExpression expr = Expression.Call(info, term.Parameters[0].Accept(this));
        return expr;
    }
    private MethodCallExpression SquareRootReal(Function<Space> term) 
    {
        MethodInfo info = typeof(Math).GetMethod("Sqrt", [typeof(double)])!;
        MethodCallExpression expr = Expression.Call(info, term.Parameters[0].Accept(this));
        return expr;
    }

    private MethodCallExpression Tangent(Function<Space> term)
    {
        return NumberSpace switch
        {
            NumberSpace.REAL => TangentReal(term),
            NumberSpace.COMPLEX => TangentComplex(term),
            _ => throw new NotImplementedException()
        };
    }
    private MethodCallExpression TangentComplex(Function<Space> term)
    {
        MethodInfo info = typeof(Complex).GetMethod("Tan", [typeof(Complex)])!;
        MethodCallExpression expr = Expression.Call(info, term.Parameters[0].Accept(this));
        return expr;
    }
    private MethodCallExpression TangentReal(Function<Space> term)
    {
        MethodInfo info = typeof(Math).GetMethod("Tan", [typeof(double)])!;
        MethodCallExpression expr = Expression.Call(info, term.Parameters[0].Accept(this));
        return expr;
    }

    private MethodCallExpression Cosinus(Function<Space> term)
    {
        return NumberSpace switch
        {
            NumberSpace.REAL => CosinusReal(term),
            NumberSpace.COMPLEX => CosinusComplex(term),
            _ => throw new NotImplementedException()
        };
    }
    private MethodCallExpression CosinusComplex(Function<Space> term)
    {
        MethodInfo info = typeof(Complex).GetMethod("Cos", [typeof(Complex)])!;
        MethodCallExpression expr = Expression.Call(info, term.Parameters[0].Accept(this));
        return expr;
    }
    private MethodCallExpression CosinusReal(Function<Space> term)
    {
        MethodInfo info = typeof(Math).GetMethod("Cos", [typeof(double)])!;
        MethodCallExpression expr = Expression.Call(info, term.Parameters[0].Accept(this));
        return expr;
    }

    private MethodCallExpression Sinus(Function<Space> term)
    {
        return NumberSpace switch
        {
            NumberSpace.REAL => SinusReal(term),
            NumberSpace.COMPLEX => SinusComplex(term),
            _ => throw new NotImplementedException()
        };
    }
    private MethodCallExpression SinusComplex(Function<Space> term)
    {
        MethodInfo info = typeof(Complex).GetMethod("Sin", [typeof(Complex)])!;
        MethodCallExpression expr = Expression.Call(info, term.Parameters[0].Accept(this));
        return expr;
    }
    private MethodCallExpression SinusReal(Function<Space> term)
    {
        MethodInfo info = typeof(Math).GetMethod("Sin", [typeof(double)])!;
        MethodCallExpression expr = Expression.Call(info, term.Parameters[0].Accept(this));
        return expr;
    }
    

    public Expression VisitUnaryTerm(Unary<Space> term)
    {
        TokenType type = term.UnaryOperator.Type;
        return (type) switch
        {
            TokenType.MINUS => Expression.Negate(term.Right.Accept(this)),
            _ => throw new NotImplementedException()
        };
    }

    public Expression VisitGroupingTerm(Grouping<Space> term)
    {
        return term.Expression.Accept(this);
    }
}