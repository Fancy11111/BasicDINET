using System;
using System.Collections.Generic;
using System.Text;

namespace Testing.Classes
{
	public interface IGenericSimpleDummy<T>
	{
		T GetValue();
		void SetValue(T newVal);
	}

	public class GenericSimpleDummy<T> : IGenericSimpleDummy<T>
	{
		private T Value { get; set; }

		public T GetValue()
		{
			return Value;
		}

		public void SetValue(T newVal)
		{
			Value = newVal;
		}
	}
}
