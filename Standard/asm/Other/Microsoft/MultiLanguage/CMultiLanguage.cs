using System.Runtime.InteropServices;

namespace asm.Other.Microsoft.MultiLanguage
{
	[ComImport]
	[Guid("275C23E1-3747-11D0-9FEA-00AA003F8646")]
	[CoClass(typeof(CMultiLanguageClass))]
	internal interface CMultiLanguage : IMultiLanguage
	{
	}
}