using System.Linq.Expressions;

namespace MathExpressionCompiler;

public class Parser<T>(List<Token> tokens)
{
    public class ParseError : Exception
    {
        public ParseError() : base() { }
        public ParseError(string message) : base(message) { }
        public ParseError(string message, Exception inner) : base(message, inner) { }
    }

    private readonly List<Token> Tokens = tokens;
    private int Current = 0;

    private List<string> VariableNames = [];

    //public Term<T> Root;

    public Term<T> Parse()
    {
        List<Term<T>> terms = [];
        while (!IsAtEnd()) terms.Add(Function());
        return terms[0];
    }


    private Term<T> Function()
    {
        Console.WriteLine($"Function: {Peek().Type.ToString()}");
        
        return AddSub();
    }
    private Term<T> AddSub()
    {
        Console.WriteLine($"AddSub: {Peek().Type.ToString()}");
        Term<T> term = Factor();
        while (Match(TokenType.PLUS, TokenType.MINUS))
        {
            Token op = Previous();
            Term<T> right = Factor();
            term = new Binary<T>(term, op, right);
        }
        return term;
    }


    private Term<T> Factor()
    {
        Console.WriteLine($"Factor: {Peek().Type.ToString()}");
        Term<T> term = Power();
        while (Match(TokenType.STAR, TokenType.SLASH))
        {
            Token op = Previous();
            Term<T> right = Power();
            term = new Binary<T>(term, op, right);
        }
        return term;
    }

    private Term<T> Power()
    {
        Console.WriteLine($"Power: {Peek().Type.ToString()}");
        Term<T> term = Unary();
        while (Match(TokenType.CARET))
        {
            Token op = Previous();
            Term<T> exponent = Unary();
            term = new Binary<T>(term, op, exponent);
        }
        return term;
    }

    private Term<T> Unary()
    {
        Console.WriteLine($"Unary: {Peek().Type.ToString()}");
        if (Match(TokenType.MINUS))
        {
            Token op = Previous();
            Term<T> right = Function();
            return new Unary<T>(op, right);
        }
        return Primary();
    }

    private Term<T> Primary()
    {
        Console.WriteLine($"Primary: {Peek().Type.ToString()}");
        if (Match(TokenType.LEFT_PARENTHESIS))
        {
            Term<T> expression = Function();
            Consume(TokenType.RIGHT_PARENTHESIS, @"Expected <)> after expression.");
            return expression;
        }
        if (Match(TokenType.VARIABLE))
        {
            return new Variable<T>(Previous().Literal!.ToString()!);
        }
        if (Match(TokenType.CONSTANT, TokenType.IMAGINARY_UNIT))
        {
            return new Constant<T>((T) Previous().Literal!);
        }
        //sd
        if (Match(TokenType.SIN, TokenType.COS, TokenType.TAN))
        {
            Token op = Previous();
            Consume(TokenType.LEFT_PARENTHESIS, @"Expected <(> after function name.");
            Term<T> term = Function();
            Consume(TokenType.RIGHT_PARENTHESIS, @"Expected <)> after function term.");
            List<Term<T>> terms = [term];
            return new Function<T>(op, terms);
        }
        if(Match(TokenType.POLAR))
        {
            Token op = Previous();
            Consume(TokenType.LEFT_PARENTHESIS, @"Expected <(> after function name.");
            Term<T> arg1 = Function();
            Consume(TokenType.COMMA, @"Expected <,> between function arguments");
            Term<T> arg2 = Function();
            Consume(TokenType.RIGHT_PARENTHESIS, @"Expected <)> after function term.");
            List<Term<T>> terms = [arg1, arg2];
            return new Function<T>(op, terms);
        }
        if (Match(TokenType.SUM))
        {
            Token op = Previous();
            Consume(TokenType.LEFT_PARENTHESIS, @"Expected <(> after function name.");
            if (Peek().Type != TokenType.VARIABLE) throw Error(Peek(), "Expected variable");
            Term<T> variable = Primary();
            Consume(TokenType.COMMA, @"Expected <,> between function arguments");
            Term<T> start = Function();
            Consume(TokenType.COMMA, @"Expected <,> between function arguments");
            Term<T> finish = Function();
            Consume(TokenType.COMMA, @"Expected <,> between function arguments");
            Term<T> body = Function();
            Consume(TokenType.RIGHT_PARENTHESIS, @"Expected <)> after function term.");
            List<Term<T>> terms = [variable, start, finish, body];
            return new Function<T>(op, terms);
        }
        //sad
        throw Error(Peek(), $"Primary: Expected expression but got {Peek().Type}.");
    }

    private Token Consume(TokenType type, String message)
    {
        if (Check(type)) return Advance();
        throw Error(Peek(), message);
    }

    private ParseError Error(Token token, string message)
    {
        MathExpressionParser.Error(token, message);
        return new ParseError(message);
    }

    private bool Match(params TokenType[] types)
    {
        foreach(TokenType type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        return false;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) Current++;

        return Previous();
    }

    private Token Previous()
    {
        return Tokens[Current - 1];
    }

    private bool Check(TokenType type)
    {
        return !IsAtEnd() && Peek().Type == type;
    }

    private Token Peek()
    {
        return Tokens[Current];
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }

}