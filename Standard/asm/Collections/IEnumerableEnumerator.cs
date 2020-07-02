﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace asm.Collections
{
	public interface IEnumerableEnumerator<out T> : IEnumerator<T>, IEnumerator, IEnumerable<T>, IEnumerable, IDisposable
	{
	}
}