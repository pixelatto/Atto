
using System;

namespace Atto.Extensions
{
	public static class BoolExtensions
	{

		public static int ToInt(this bool boolean) => Convert.ToInt32(boolean);

	}
}