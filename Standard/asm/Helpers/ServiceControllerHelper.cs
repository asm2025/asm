﻿using System.ServiceProcess;
using asm.Extensions;
using JetBrains.Annotations;

namespace asm.Helpers
{
	public static class ServiceControllerHelper
	{
		[NotNull]
		public static ServiceController GetController(string serviceName) { return GetController(serviceName, null); }

		[NotNull]
		public static ServiceController GetController(string serviceName, string machineName)
		{
			machineName = machineName.ToNullIfEmpty() ?? string.Empty;
			return string.IsNullOrEmpty(machineName)
						? new ServiceController(serviceName)
						: new ServiceController(serviceName, machineName);
		}
	}
}
