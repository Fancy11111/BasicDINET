using System;
using System.Collections.Generic;
using System.Text;

namespace Testing.Classes
{
	public interface ISingletonDummy
	{
		void SetValue(string value);
		string GetValue();
	}

	public class SingletonDummy : ISingletonDummy
	{
		private string Value = "";

		public string GetValue()
		{
			return Value;
		}

		public void SetValue(string value)
		{
			Value = value;
		}
	}
}
