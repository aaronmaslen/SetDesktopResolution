namespace SetDesktopResolution.Common.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public static class CollectionExtensions
	{
		public static void AddAll<T>(this ICollection<T> collection, IEnumerable<T> items)
		{
			foreach (var i in items)
				collection.Add(i);
		}
	}
}
