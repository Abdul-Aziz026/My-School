using System.Linq.Expressions;

namespace Application.Common.Extensions;

public static class ExpressionExtensions
{
    /// <summary>
    /// Combines two expressions with AND (&&) logic
    /// </summary>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, 
                                                   Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var leftBody = new ParameterReplacer(expr1.Parameters[0], parameter)
            .Visit(expr1.Body);
        var rightBody = new ParameterReplacer(expr2.Parameters[0], parameter)
            .Visit(expr2.Body);

        // combine the two bodies with AND
        var body = Expression.AndAlso(leftBody, rightBody);
        // create and return the combined lambda expression
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    /// <summary>
    /// Combines two expressions with OR (||) logic
    /// </summary>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                  Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var leftBody = new ParameterReplacer(expr1.Parameters[0], parameter)
            .Visit(expr1.Body);
        var rightBody = new ParameterReplacer(expr2.Parameters[0], parameter)
            .Visit(expr2.Body);
        var body = Expression.OrElse(leftBody, rightBody);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    /// <summary>
    /// Negates an expression (NOT logic)
    /// </summary>
    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expr)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var body = new ParameterReplacer(expr.Parameters[0], parameter)
            .Visit(expr.Body);
        var notBody = Expression.Not(body);
        return Expression.Lambda<Func<T, bool>>(notBody, parameter);
    }
}

/// <summary>
/// Helper class to replace parameter references in expression trees
/// </summary>
public class ParameterReplacer : ExpressionVisitor
{
    private readonly ParameterExpression _oldParameter;
    private readonly ParameterExpression _newParameter;
    public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        _oldParameter = oldParameter;
        _newParameter = newParameter;
    }
    protected override Expression VisitParameter(ParameterExpression node)
    {
        // Replace old parameter with new parameter
        return (node == _oldParameter) ? _newParameter : base.VisitParameter(node);
    }
}
