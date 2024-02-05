using System.Collections.Generic;

namespace MathExpressionCompiler;

public abstract class Term<Space>
{
    public interface IVisitor<T>
    {
        T VisitBinaryTerm(Binary<Space> term);
        T VisitVariableTerm(Variable<Space> term);
        T VisitConstantTerm(Constant<Space> term);
        T VisitFunctionTerm(Function<Space> term);
        T VisitUnaryTerm(Unary<Space> term);
        T VisitGroupingTerm(Grouping<Space> term);
    }

    public abstract T Accept<T>(IVisitor<T> visitor);
}
