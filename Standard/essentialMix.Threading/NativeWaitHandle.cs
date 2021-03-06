using System;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace essentialMix.Threading
{
	public class NativeWaitHandle : WaitHandle
	{
		public NativeWaitHandle(IntPtr handle, bool isOwning)
		{
			SafeWaitHandle = new SafeWaitHandle(handle, isOwning);
		}
	}
}