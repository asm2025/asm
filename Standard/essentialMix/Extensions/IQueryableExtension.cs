using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using essentialMix.Linq;
using essentialMix.Patterns.Pagination;

namespace essentialMix.Extensions
{
	public static class IQueryableExtension
	{
		[NotNull]
		public static IQueryable<T> InterceptWith<T>([NotNull] this IQueryable<T> thisValue, [NotNull] params ExpressionVisitor[] visitors)
		{
			return new QueryTranslator<T>(thisValue, visitors);
		}

		[NotNull]
		public static IQueryable<T> AsExpandable<T>(this IQueryable<T> thisValue)
		{
			ExpandableQuery<T> query = thisValue as ExpandableQuery<T>;
			return query ?? new ExpandableQuery<T>(thisValue);
		}

		[NotNull]
		[MethodImpl(MethodImplOptions.ForwardRef | MethodImplOptions.AggressiveInlining)]
		public static IQueryable<T> Paginate<T>([NotNull] this IQueryable<T> thisValue, [NotNull] IPagination settings)
		{
			if (settings.PageSize < 1) settings.PageSize = Pagination.PAGE_SIZE;
			if (settings.Page < 1) settings.Page = 1;
			int start = (settings.Page - 1) * settings.PageSize;
			return thisValue.Skip(start).Take(settings.PageSize);
		}
	}
}