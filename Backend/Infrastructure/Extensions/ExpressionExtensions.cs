using System;
using System.Linq.Expressions;
namespace Infrastructure.Extensions;

/// <summary>
/// Extension methods for combining Expression predicates
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// Combines two expressions with a logical AND
    /// </summary>
    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var param = Expression.Parameter(typeof(T), "x");

        var body = Expression.AndAlso(
            Expression.Invoke(expr1, param),
            Expression.Invoke(expr2, param)
        );

        return Expression.Lambda<Func<T, bool>>(body, param);
    }
    
    /// <summary>
    /// Combines two expressions with a logical OR
    /// </summary>
    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var param = Expression.Parameter(typeof(T), "x");

        var body = Expression.OrElse(
            Expression.Invoke(expr1, param),
            Expression.Invoke(expr2, param)
        );

        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}
