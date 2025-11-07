using System;
using System.Collections.Generic;

namespace ShopifyAccess.Extensions
{
	/// <summary>
	/// Contains extensions methods to easier work on IEnumerable collections
	/// </summary>
	public static class IEnumerableExtensions
	{
		/// <summary>
		/// Splits sequence in a number of batches of specified size. Could be used when we want to handle big collection in a chunks/batches
		/// for some reasons - e.g. when requesting something from API.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="chunkSize"></param>
		/// <typeparam name="TSource"></typeparam>
		/// <exception cref="ArgumentException"></exception>
		//TODO GUARD-3954 On feature cleanup (hopefully, after Core's .NET Standard conversion is in the trunk),
		//	remove this method and use the SkuVault.Integrations.Core.Extensions.IEnumerableExtensions.SplitInBatches() in the SkuVault.Integrations.Core package
		//See this thread for context - https://linnworks.slack.com/archives/C05P96HMZLG/p1762362613525109
		public static IEnumerable<IList<TSource>> SplitInBatches<TSource>(this IEnumerable<TSource> source, int chunkSize)
		{
			if (source == null)
			{
				throw new ArgumentException("Source shouldn't be nullable");
			}

			if (chunkSize < 1)
			{
				throw new ArgumentException("Chunk size should be positive");
			}

			return IteratorImpl();

			// An iterator method (a method that contains `yield`) will not validate arguments until the caller begins to enumerate the result items.
			// To ensure that arguments are validated immediately (when the method is called), move the iterator to a separate method (local function).
			// https://github.com/JosefPihrt/Roslynator/blob/main/docs/analyzers/RCS1227.md
			IEnumerable<IList<TSource>> IteratorImpl()
			{
				var result = new List<TSource>(chunkSize);

				foreach (var s in source)
				{
					result.Add(s);
					if (result.Count >= chunkSize)
					{
						yield return result.ToArray();
						result.Clear();
					}
				}

				if (result.Count > 0)
				{
					yield return result.ToArray();
				}
			}
		}
	}
}