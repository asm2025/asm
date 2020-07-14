﻿using System.Collections.Generic;
using JetBrains.Annotations;

namespace asm.Collections
{
	public interface IHeap<T> : ICollection<T>
	{
		void Add([NotNull] IEnumerable<T> values);
	
		[NotNull]
		T Value();
		
		[NotNull]
		T ExtractValue();
		
		[NotNull]
		T ElementAt(int k);
	}
}