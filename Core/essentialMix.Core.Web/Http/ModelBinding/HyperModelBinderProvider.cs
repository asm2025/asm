﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using essentialMix.Collections;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using essentialMix.Json.Abstraction;
using JetBrains.Annotations;

namespace essentialMix.Core.Web.Http.ModelBinding
{
	public static class HyperModelBinderProvider
	{
		public static Collections.IReadOnlySet<Type> ExcludedTypes { get; } = new ReadOnlySet<Type>(new HashSet<Type>
		{
			typeof(object)
		});

		public static ConcurrentDictionary<Type, HyperModelBinder> Types { get; } = new ConcurrentDictionary<Type, HyperModelBinder>();
	}

	public class HyperModelBinderProvider<T> : IModelBinderProvider
	{
		public HyperModelBinderProvider([NotNull] IJsonSerializer serializer)
		{
			Type type = typeof(T);

			while (type != null && !HyperModelBinderProvider.ExcludedTypes.Contains(type))
			{
				HyperModelBinderProvider.Types.TryAdd(type, new HyperModelBinder(type, serializer));
				type = type.BaseType;
			}
		}

		public IModelBinder GetBinder([NotNull] ModelBinderProviderContext context)
		{
			HyperModelBinderProvider.Types.TryGetValue(context.Metadata.ModelType, out HyperModelBinder binder);
			return binder;
		}
	}
}