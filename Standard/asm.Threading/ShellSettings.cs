using System;
using JetBrains.Annotations;

namespace asm.Threading
{
	public class ShellSettings : ExecutableSettingsBase
	{
		[NotNull]
		public static ShellSettings Default =>
			new ShellSettings
			{
				WindowStyle = ShowWindowEnum.SW_SHOW
			};

		public ShellSettings()
		{
		}

		public string Verb { get; set; }
		public string ClassName { get; set; }
		public IntPtr HKeyClass { get; set; }
		public ShowWindowEnum WindowStyle { get; set; }
		public bool ErrorDialog { get; set; }
		public IntPtr ErrorDialogParentHandle { get; set; }
		public bool InheritConsole { get; set; }
	}
}