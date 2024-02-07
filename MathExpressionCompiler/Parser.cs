using System.Linq.Expressions;

namespace MEC;

internal class Parser<T>(List<Token> tokens)
{
    internal class ParseError : Exception
    {
        internal ParseError() : base() { }
        internal ParseError(string message) : base(message) { }
        internal ParseError(string message, Exception inner) : base(message, inner) { }
    }

    private readonly List<Token> Tokens = tokens;
    private int Current = 0;

    internal List<Term<T>> Parse()
    {
        List<Term<T>> terms = [];
        while (!IsAtEnd()) terms.Add(AddSub());
        return terms;
    }

    private Term<T> AddSub()
    {
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
        Term<T> left = Unary();
        while (Match(TokenType.CARET))
        {
            Token op = Previous();
            Term<T> right = Power();
            left = new Binary<T>(left, op, right);
        }
        return left;
    }

    private Term<T> Unary()
    {
        if (Match(TokenType.MINUS))
        {
            Token op = Previous();
            Term<T> right = AddSub();
            return new Unary<T>(op, right);
        }
        return Primary();
    }

    private Term<T> Primary()
    {
        if (Match(TokenType.LEFT_PARENTHESIS))
        {
            Term<T> expression = AddSub();
            Consume(TokenType.RIGHT_PARENTHESIS, @"Expected <)> after expression.");
            return expression;
        }
        if (Match(TokenType.VARIABLE))
        {
            return new Variable<T>(Previous().Literal!.ToString()!, default);
        }
        if (Match(TokenType.CONSTANT, TokenType.IMAGINARY_UNIT))
        {
            return new Constant<T>((T) Previous().Literal!);
        }
        if (Match(TokenType.SIN, TokenType.COS, TokenType.TAN, TokenType.SQRT))
        {
            Token op = Previous();
            Consume(TokenType.LEFT_PARENTHESIS, @"Expected <(> after function name.");
            Term<T> term = AddSub();
            Consume(TokenType.RIGHT_PARENTHESIS, @"Expected <)> after function term.");
            List<Term<T>> terms = [term];
            return new Function<T>(op, terms);
        }
        if(Match(TokenType.POLAR))
        {
            Token op = Previous();
            Consume(TokenType.LEFT_PARENTHESIS, @"Expected <(> after function name.");
            Term<T> arg1 = AddSub();
            Consume(TokenType.COMMA, @"Expected <,> between function arguments");
            Term<T> arg2 = AddSub();
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
            Term<T> start = AddSub();
            Consume(TokenType.COMMA, @"Expected <,> between function arguments");
            Term<T> finish = AddSub();
            Consume(TokenType.COMMA, @"Expected <,> between function arguments");
            Term<T> body = AddSub();
            Consume(TokenType.RIGHT_PARENTHESIS, @"Expected <)> after function term.");
            List<Term<T>> terms = [variable, start, finish, body];
            return new Function<T>(op, terms);
        }
        throw Error(Peek(), $"Primary: Expected expression but got {Peek().Type}.");
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();
        throw Error(Peek(), message);
    }

    private ParseError Error(Token token, string message)
    {
        MathExpressionCreator.Error(token, message);
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