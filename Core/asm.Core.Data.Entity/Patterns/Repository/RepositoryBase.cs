﻿using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using asm.Data.Model;
using asm.Data.Patterns.Parameters;
using asm.Data.Patterns.Repository;
using asm.Extensions;
using asm.Helpers;
using asm.Patterns.Pagination;
using asm.Patterns.Sorting;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SystemDbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace asm.Core.Data.Entity.Patterns.Repository
{
	public abstract class RepositoryBase<TContext, TEntity> : RepositoryBase<TEntity>, IRepositoryBase<TContext, TEntity>
		where TContext : SystemDbContext
		where TEntity : class, IEntity
	{
		private TContext _context;

		/// <inheritdoc />
		protected RepositoryBase([NotNull] TContext context, [NotNull] IConfiguration configuration)
			: this(context, configuration, null, false)
		{
		}

		/// <inheritdoc />
		protected RepositoryBase([NotNull] TContext context, [NotNull] IConfiguration configuration, ILogger logger)
			: this(context, configuration, logger, false)
		{
		}

		/// <inheritdoc />
		protected RepositoryBase([NotNull] TContext context, [NotNull] IConfiguration configuration, ILogger logger, bool ownsContext)
			: base(configuration, logger)
		{
			_context = context;
			DbSet = _context.Set<TEntity>();
			OwnsContext = ownsContext;
		}

		/// <inheritdoc />
		public TContext Context => _context;

		/// <inheritdoc />
		public DbSet<TEntity> DbSet { get; }
		
		protected bool OwnsContext { get; }

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (disposing && OwnsContext) ObjectHelper.Dispose(ref _context);
			base.Dispose(disposing);
		}

		[NotNull]
		protected override IQueryable<TEntity> ListInternal(IPagination settings = null) { return PrepareListQuery(settings); }

		protected override ValueTask<IQueryable<TEntity>> ListAsyncInternal(IPagination settings = null, CancellationToken token = default(CancellationToken))
		{
			token.ThrowIfCancellationRequested();
			return new ValueTask<IQueryable<TEntity>>(PrepareListQuery(settings));
		}

		protected override TEntity GetInternal(params object[] keys) { return PrepareGetQuery(keys).FirstOrDefault(); }

		/// <inheritdoc />
		protected override ValueTask<TEntity> GetAsyncInternal(object[] keys, CancellationToken token = default(CancellationToken))
		{
			token.ThrowIfCancellationRequested();
			return new ValueTask<TEntity>(PrepareGetQuery(keys).FirstOrDefaultAsync(token));
		}

		/// <inheritdoc />

		/// <inheritdoc />
		protected override TEntity GetInternal(IGetSettings settings) { return PrepareGetQuery(settings).FirstOrDefault(); }

		/// <inheritdoc />
		protected override ValueTask<TEntity> GetAsyncInternal(IGetSettings settings, CancellationToken token = default(CancellationToken))
		{
			token.ThrowIfCancellationRequested();
			return new ValueTask<TEntity>(PrepareGetQuery(settings).FirstOrDefaultAsync(token));
		}

		/// <inheritdoc />
		public IQueryable<TEntity> SqlQuery([NotNull] string sql, [NotNull] params object[] parameters)
		{
			ThrowIfDisposed();
			return SqlQueryInternal(sql, parameters);
		}

		protected virtual IQueryable<TEntity> SqlQueryInternal([NotNull] string sql, [NotNull] params object[] parameters)
		{
			return DbSet.FromSqlRaw(sql, parameters);
		}

		[NotNull]
		protected virtual IQueryable<TEntity> PrepareListQuery(IPagination settings)
		{
			IQueryable<TEntity> query = DbSet;
			
			if (settings is IIncludeSettings includeSettings && includeSettings.Include?.Count > 0)
			{
				query = includeSettings.Include.SkipNullOrEmpty()
										.Aggregate(query, (current, path) => current.Include(path));
			}
			
			if (settings is IFilterSettings filterSettings && !string.IsNullOrEmpty(filterSettings.Filter.Expression))
			{
				query = query.Where(filterSettings.Filter.Expression, filterSettings.Filter.Arguments);
			}
			
			if (settings is ISortable sortable && sortable.OrderBy?.Count > 0)
			{
				string sortExpression = string.Join(",", sortable.OrderBy
																.Where(e => e.Type != SortType.None)
																.Select(e => $"{e.Name} {e.Type}"));
				if (!string.IsNullOrEmpty(sortExpression)) query = query.OrderBy(sortExpression);
			}

			return query;
		}

		[NotNull]
		protected virtual IQueryable<TEntity> PrepareGetQuery(object[] keys)
		{
			IQueryable<TEntity> query = DbSet;

			if (!(keys?.Length > 0)) return query;
			if (keys.Length != KeyProperties.Length) throw new ArgumentException("Wrong number of key values.", nameof(keys));
			string filter = string.Join(" and ", KeyProperties.Select((p, i) => $"{p.Name} == @{i}"));
			query = query.Where(filter, keys);
			return query;
		}

		[NotNull]
		protected virtual IQueryable<TEntity> PrepareGetQuery([NotNull] IGetSettings settings)
		{
			IQueryable<TEntity> query = PrepareGetQuery(settings.KeyValue);
			
			if (settings is IIncludeSettings includeSettings && includeSettings.Include?.Count > 0)
			{
				query = includeSettings.Include.SkipNullOrEmpty()
										.Aggregate(query, (current, path) => current.Include(path));
			}

			if (settings is IFilterSettings filterSettings && !string.IsNullOrEmpty(filterSettings.Filter.Expression))
			{
				query = query.Where(filterSettings.Filter.Expression, filterSettings.Filter.Arguments);
			}

			return query;
		}
	}
}