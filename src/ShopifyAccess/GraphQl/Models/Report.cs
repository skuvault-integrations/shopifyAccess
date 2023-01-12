using System.Collections.Generic;
using System.Linq;

namespace ShopifyAccess.GraphQl.Models
{
	public class Report: List< object >
	{
		public IEnumerable<T> As<T> () where T : class
		{
			return from obj in this select obj as T;
		}
	}
}