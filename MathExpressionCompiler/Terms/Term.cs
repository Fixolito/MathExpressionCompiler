using System.Collections.Generic;

namespace MEC;

internal abstract class Term<Space>
{
    internal interface IVisitor<T>
    {
        T VisitBinaryTerm(Binary<Space> term);
        T VisitVariableTerm(Variable<Space> term);
        T VisitConstantTerm(Constant<Space> term);
        T VisitFunctionTerm(Function<Space> term);
        T VisitUnaryTerm(Unary<Space> term);
        T VisitGroupingTerm(Grouping<Space> term);
    }

    internal abstract T Accept<T>(IVisitor<T> visitor);
}
