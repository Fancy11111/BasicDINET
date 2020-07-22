using System;
using System.Collections.Generic;
using System.Text;

namespace diTest
{
	public interface IComplexDummyService
	{
		bool Invert();
	}

	public class ComplexDummyService : IComplexDummyService
	{
		private ISimpleDummyService _simpleDummyService;
		private bool value = false;

		public bool Invert() {
			value = _simpleDummyService.Invert(value);
			return value;
		}

		public ComplexDummyService(ISimpleDummyService simpleDummyService)
		{
			_simpleDummyService = simpleDummyService;
		}
	}
}
