﻿using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace essentialMix.Caching
{
	public sealed class Cache : MemoryCache
	{
		public Cache()
			: this(new MemoryCacheOptions())
		{
		}

		/// <inheritdoc />
		public Cache(IOptions<MemoryCacheOptions> optionsAccessor)
			: base(optionsAccessor)
		{
		}
	}
}