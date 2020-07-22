using System;
using System.Collections.Generic;
using System.Text;

namespace Testing.Classes
{
	public interface IValueDependencyDummy
	{
		bool Invert();
	}

	public class ValueDependencyDummy : IValueDependencyDummy
	{
		private bool Value { get; set; }
		public ValueDependencyDummy(bool initValue)
		{
			Value = initValue;
		}
		public bool Invert() => !Value;
	}
}
