using System.Text;
using JetBrains.Annotations;

namespace asm.Cryptography.Asymmetric
{
	public class RSAAlgorithm<T> : RSAAlgorithmBase<T>
		where T : System.Security.Cryptography.RSA
	{
		/// <inheritdoc />
		public RSAAlgorithm([NotNull] T algorithm)
			: base(algorithm)
		{
		}

		/// <inheritdoc />
		public RSAAlgorithm([NotNull] T algorithm, [NotNull] Encoding encoding)
			: base(algorithm, encoding)
		{
		}

		public override object Clone() { return new RSAAlgorithm<T>(Algorithm, Encoding); }
	}
}