using System.Security.Principal;
using asm.Extensions;

namespace asm.Helpers
{
	public static class WindowsIdentityHelper
	{
		public static bool HasElevatedPrivileges()
		{
			WindowsIdentity identity = null;

			try
			{
				identity = WindowsIdentity.GetCurrent();
				return identity.HasElevatedPrivileges();
			}
			finally
			{
				ObjectHelper.Dispose(ref identity);
			}
		}
	}
}