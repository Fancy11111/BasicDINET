using System;
using System.Collections.Generic;
using System.Text;

namespace Testing.Classes
{
	public interface ISimpleDummyService
	{
		bool Invert(bool input);
	}

	public class SimpleDummyService : ISimpleDummyService
	{
		public bool Invert(bool input) => !input;
	}
}
