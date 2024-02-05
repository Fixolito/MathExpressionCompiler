using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.Json;

namespace MathExpressionCompiler;

public class Scanner<T>(string source)
{
    private static Dictionary<string, TokenType> Keywords { get; } = new Dictionary<string, TokenType>
    {
        {"sin", TokenType.SIN},
        {"cos", TokenType.COS},
        {"tan", TokenType.TAN},
        {"sum", TokenType.SUM},
        {"polar", TokenType.POLAR}
    };

    private readonly string Source = source.ToLower().Replace('=', ',');
    private readonly List<Token> Tokens = [];
    private int Start = 0;
    private int Current = 0;
    public NumberSpace NumberSpace = GetNumberSpace(typeof(T));

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            Start = Current;
            ScanToken();
        }
        Tokens.Add(new Token(TokenType.EOF, "", null));
        return Tokens;
    }

    private static NumberSpace GetNumberSpace(Type type)
    {
        if (type == typeof(double)) return NumberSpace.REAL;
        else if (type == typeof(Complex)) return NumberSpace.COMPLEX;
        else throw new Exception("Only <double> for real numbers or <Complex> for complex numbers are allowed.");
    }

    private void ScanToken()
    {
        char c = Advance();
        switch(c)
        {
            case '(': AddToken(TokenType.LEFT_PARENTHESIS); break;
            case ')': AddToken(TokenType.RIGHT_PARENTHESIS); break;
            case ',': AddToken(TokenType.COMMA); break;
            case '-': AddToken(TokenType.MINUS); break;
            case '+': AddToken(TokenType.PLUS); break;
            case '/': AddToken(TokenType.SLASH); break;
            case '*': AddToken(TokenType.STAR); break;
            case '^': AddToken(TokenType.CARET); break;
            case '=': AddToken(TokenType.EQUALITY); break;
            case 'i': HandleImaginaryUnit(); break;
            case >= 'a' and <= 'z' and not 'i': HandleLetters(); break;
            case >= '0' and <= '9': HandleDigits(); break;
            case '.': HandleShorthandNotation(); break;
            case ' ': break;
            default: MathExpressionParser.Error($"Unknown char: <{c}> at {Current - 1}\n{source}\n{new string(' ', (Current - 1))}^"); break;

        }

    }

    private void HandleShorthandNotation()
    {
        Advance();
        while (IsDigit(Peek())) Advance();
        if (Peek() == '.') MathExpressionParser.Error($"Unexpected <.> at {Current}\n{source}\n{new string(' ', (Current))}^");
        double value = Double.Parse(Source[Start..Current], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
        AddToken(TokenType.CONSTANT, value);
    }

    private void HandleDigits()
    {
        while (IsDigit(Peek())) Advance();
        if (Peek() == '.')
        {
            Advance();
            while (IsDigit(Peek())) Advance();
            if (Peek() == '.') MathExpressionParser.Error($"Unexpected <.> at {Current}\n{source}\n{new string(' ', (Current))}^");
        }
        if (NumberSpace == NumberSpace.REAL)
        {
            double value = Double.Parse(Source[Start..Current], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            AddToken(TokenType.CONSTANT, value);
        }
        if (NumberSpace == NumberSpace.COMPLEX)
        {
            double real = Double.Parse(Source[Start..Current], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            Complex value = new(real, 0);
            AddToken(TokenType.CONSTANT, value);
        }
    }

    private void HandleLetters()
    {
        while (IsLetter(Peek())) Advance();
        string text = Source[Start..Current];
        if (Keywords.TryGetValue(text, out TokenType type))
        {
            AddToken(type);
            return;
        }
        AddToken(TokenType.VARIABLE, text);
    }

    private void HandleImaginaryUnit()
    {
        if (NumberSpace == NumberSpace.REAL || IsLetter(Peek()))
        {
            HandleLetters();
            return;
        }
        Complex value = new(0, 1);
        AddToken(TokenType.IMAGINARY_UNIT, value);
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, Object? literal)
    {
        string text = Source[Start..Current];
        Tokens.Add(new(type, text, literal));
    }


    private char Advance()
    {
        return Source[Current++];
    }

    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return Source[Current];
    }

    private char PeekNext()
    {
        if (Current + 1 >= Source.Length) return '\0';
        return Source[Current + 1];
    }

    private char Previous()
    {
        if (Current - 1 < 0) return '\0';
        return Source[Current - 1];
    }

    private static bool IsLetter(char c)
    {
        return 'a' <= c && c <= 'z';
    }

    private static bool IsDigit(char c)
    {
        return '0' <= c && c <= '9';
    }

    private bool IsAtEnd()
    {
        return Current >= Source.Length;
    }
}