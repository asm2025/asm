using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace asm.Extensions
{
	public static class ValueTaskExtension
	{
		public static ValueTask AsValueTask<T>(this ValueTask<T> thisValue)
		{
			if (!thisValue.IsCompletedSuccessfully) return new ValueTask(thisValue.AsTask());
			thisValue.GetAwaiter().GetResult();
			return default(ValueTask);
		}
	}
}