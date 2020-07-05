﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace asm.Collections
{
	[Serializable]
	public class MinBinomialHeap<TKey, TValue> : BinomialHeap<TKey, TValue>
	{
		/// <inheritdoc />
		public MinBinomialHeap([NotNull] Func<TValue, TKey> getKeyForItem)
			: this(getKeyForItem, (IComparer<TKey>)null)
		{
		}

		/// <inheritdoc />
		public MinBinomialHeap([NotNull] Func<TValue, TKey> getKeyForItem, IComparer<TKey> comparer)
			: base(getKeyForItem, comparer)
		{
		}

		/// <inheritdoc />
		public MinBinomialHeap([NotNull] Func<TValue, TKey> getKeyForItem, [NotNull] BinomialNode<TKey, TValue> head)
			: this(getKeyForItem, head, null)
		{
		}

		/// <inheritdoc />
		public MinBinomialHeap([NotNull] Func<TValue, TKey> getKeyForItem, [NotNull] BinomialNode<TKey, TValue> head, IComparer<TKey> comparer)
			: base(getKeyForItem, head, comparer)
		{
		}

		/// <inheritdoc />
		public MinBinomialHeap([NotNull] Func<TValue, TKey> getKeyForItem, [NotNull] IEnumerable<TValue> enumerable)
			: this(getKeyForItem, enumerable, null)
		{
		}

		/// <inheritdoc />
		public MinBinomialHeap([NotNull] Func<TValue, TKey> getKeyForItem, [NotNull] IEnumerable<TValue> enumerable, IComparer<TKey> comparer)
			: base(getKeyForItem, enumerable, comparer)
		{
		}

		/// <inheritdoc />
		protected override BinomialHeap<BinomialNode<TKey, TValue>, TKey, TValue> MakeHeap(BinomialNode<TKey, TValue> head) { return new MinBinomialHeap<TKey, TValue>(_getKeyForItem, head, Comparer); }

		/// <inheritdoc />
		protected override int Compare(TKey x, TKey y)
		{
			return Comparer.Compare(x, y);
		}
	}

	[Serializable]
	public class MinBinomialHeap<T> : BinomialHeap<T>
	{
		/// <inheritdoc />
		public MinBinomialHeap()
			: this((IComparer<T>)null)
		{
		}

		/// <inheritdoc />
		public MinBinomialHeap(IComparer<T> comparer)
			: base(comparer)
		{
		}

		/// <inheritdoc />
		protected internal MinBinomialHeap([NotNull] BinomialNode<T> head)
			: this(head, null)
		{
		}

		/// <inheritdoc />
		protected internal MinBinomialHeap([NotNull] BinomialNode<T> head, IComparer<T> comparer)
			: base(head, comparer)
		{
		}

		/// <inheritdoc />
		public MinBinomialHeap([NotNull] IEnumerable<T> enumerable)
			: this(enumerable, null)
		{
		}

		/// <inheritdoc />
		public MinBinomialHeap([NotNull] IEnumerable<T> enumerable, IComparer<T> comparer)
			: base(enumerable, comparer)
		{
		}

		/// <inheritdoc />
		protected override BinomialHeap<BinomialNode<T>, T, T> MakeHeap(BinomialNode<T> head) { return new MinBinomialHeap<T>(head, Comparer); }

		/// <inheritdoc />
		protected override int Compare(T x, T y)
		{
			return Comparer.Compare(x, y);
		}
	}
}