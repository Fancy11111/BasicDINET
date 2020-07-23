using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace diTest
{
	public class DependencyNode
	{
		public Type Value { get; set; }
		public DependencyNode[] References { get; set; }
		public DependencyNode[] Dependency { get; set; }
	}

	public class DependencyTree
	{
		
		private IEnumerable<Type> registered = new Type[] { };

		public void Insert(Type t)
		{
			if (registered.Contains<Type>(t)) return;
			registered = registered.Append<Type>(t);

		}
	}
}
