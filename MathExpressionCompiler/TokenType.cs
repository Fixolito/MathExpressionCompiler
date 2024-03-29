namespace MEC;

internal enum TokenType
{
    // Single characters
    LEFT_PARENTHESIS, RIGHT_PARENTHESIS,
    COMMA, MINUS, PLUS, SLASH, STAR, CARET, EQUALITY,

    SPACE, IMAGINARY_UNIT, //DUAL_UNIT would be cool to implement as well

    // Multiple characters
    SIN, COS, TAN, 
    SUM, POLAR, SQRT,

    // Literals
    CONSTANT,
    VARIABLE,

    // other
    EOF

}