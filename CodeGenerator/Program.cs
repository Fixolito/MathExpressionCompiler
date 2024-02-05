using System.IO;

namespace CodeGenerator;

public class Program
{
    static void Main(string[] args)
    {
        string baseName = "Term";
        string[] children = [
            "Binary   : Term<Space> left, Token binaryOperator, Term<Space> right",
            "Variable : string name",
            "Constant : <Space> value",
            "Function : Token functionOperator, List<Term<Space>> parameters",
            "Unary    : Token unaryOperator, Term<Space> right",
            "Grouping : Term<Space> expression"
            ];
        CodeGenerator.CreateCode(baseName, children);
    }


}