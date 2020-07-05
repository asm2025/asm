﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace asm.Collections
{
	// https://www.growingwiththeweb.com/data-structures/fibonacci-heap/overview/
	// https://brilliant.org/wiki/fibonacci-heap/
	[DebuggerDisplay("{Key} = {Value} :D{Degree}")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public abstract class FibonacciNode<TNode, TKey, TValue>
		where TNode : FibonacciNode<TNode, TKey, TValue>
	{
		private const int PARENT = 0;
		private const int CHILD = 1;
		private const int PREVIOUS = 2;
		private const int NEXT = 3;

		// The previous/next nodes are not exactly a doubly linked list but rather a circular reference
		private readonly TNode[] _nodes = new TNode[4];

		protected FibonacciNode([NotNull] TValue value)
		{
			Value = value;
			_nodes[PREVIOUS] = _nodes[NEXT] = (TNode)this;
		}

		public TNode Parent
		{
			get => _nodes[PARENT];
			internal set
			{
				if (ReferenceEquals(value, this)) throw new InvalidOperationException("Circular reference detected.");
				_nodes[PARENT] = value;
			}
		}

		public TNode Child
		{
			get => _nodes[CHILD];
			internal set
			{
				if (ReferenceEquals(value, this)) throw new InvalidOperationException("Circular reference detected.");
				_nodes[CHILD] = value;
			}
		}

		/// <summary>
		/// The previous/next nodes are not exactly a doubly linked list but rather
		/// a circular reference so will have to watch out for this.
		/// </summary>
		public TNode Previous
		{
			get => ReferenceEquals(_nodes[PREVIOUS], this)
						? null
						: _nodes[PREVIOUS];
			internal set => _nodes[PREVIOUS] = value ?? (TNode)this;
		}

		/// <summary>
		/// The previous/next nodes are not exactly a doubly linked list but rather
		/// a circular reference so will have to watch out for this.
		/// </summary>
		public TNode Next
		{
			get => ReferenceEquals(_nodes[NEXT], this)
						? null
						: _nodes[NEXT];
			internal set => _nodes[NEXT] = value ?? (TNode)this;
		}

		[NotNull]
		public abstract TKey Key { get; set; }

		[NotNull]
		public TValue Value { get; set; }

		public int Degree { get; internal set; }

		public bool Marked { get; internal set; }

		public bool IsLeaf => _nodes[NEXT] == null && _nodes[CHILD] == null;

		/// <inheritdoc />
		[NotNull]
		public override string ToString() { return Convert.ToString(Value); }

		[NotNull]
		internal virtual string ToString(int level)
		{
			return $"{Key} = {Value} :D{Degree}L{level}";
		}

		[ItemNotNull]
		public IEnumerable<TNode> Backwards(TNode stopAt = null)
		{
			TNode node = (TNode)this;

			do
			{
				yield return node;
				node = node._nodes[PREVIOUS];
			}
			while (node != stopAt && node != this);
		}

		[ItemNotNull]
		public IEnumerable<TNode> Forwards(TNode stopAt = null)
		{
			TNode node = (TNode)this;

			do
			{
				yield return node;
				node = node._nodes[NEXT];
			}
			while (node != stopAt && node != this);
		}

		[ItemNotNull]
		public IEnumerable<TNode> Ancestors()
		{
			TNode parent = _nodes[PARENT];

			while (parent != null)
			{
				yield return parent;
				parent = parent._nodes[PARENT];
			}
		}

		[ItemNotNull]
		public IEnumerable<TNode> Children()
		{
			TNode child = _nodes[CHILD];

			while (child != null)
			{
				yield return child;
				child = child._nodes[CHILD];
			}
		}

		[MethodImpl(MethodImplOptions.ForwardRef | MethodImplOptions.AggressiveInlining)]
		public void Swap([NotNull] TNode other)
		{
			TKey otherKey = other.Key;
			TValue otherValue = other.Value;
			other.Key = Key;
			other.Value = Value;
			Key = otherKey;
			Value = otherValue;
		}
	}

	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class FibonacciNode<TKey, TValue> : FibonacciNode<FibonacciNode<TKey, TValue>, TKey, TValue>
	{
		private TKey _key;

		/// <inheritdoc />
		public FibonacciNode([NotNull] TKey key, [NotNull] TValue value)
			: base(value)
		{
			_key = key;
		}

		/// <inheritdoc />
		public override TKey Key
		{
			get => _key;
			set => _key = value;
		}
	}

	[DebuggerDisplay("{Value} :D{Degree}")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class FibonacciNode<T> : FibonacciNode<FibonacciNode<T>, T, T>
	{
		/// <inheritdoc />
		public FibonacciNode([NotNull] T value)
			: base(value)
		{
		}

		/// <inheritdoc />
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override T Key
		{
			get => Value;
			set => Value = value;
		}

		internal override string ToString(int level)
		{
			return $"{Value} :D{Degree}L{level}";
		}
	}
}