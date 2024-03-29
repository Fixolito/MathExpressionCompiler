using System.Linq.Expressions;

namespace MEC;

internal class ExpressionReplacer : ExpressionVisitor
{
    private readonly ParameterExpression[] _originals;
    private readonly Expression[] _replacements;

    internal ExpressionReplacer(ParameterExpression[] originals, Expression[] replacements)
    {
        _originals = originals;
        _replacements = replacements;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        for (int i = 0; i < _originals.Length; i++)
        {
            if (node == _originals[i]) return _replacements[i];
        }
        return base.VisitParameter(node);
    }
}