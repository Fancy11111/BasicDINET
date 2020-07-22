using System;

namespace diTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var services = new ServiceCollection();
			services.AddService<ISimpleDummyService, SimpleDummyService>();
			services.AddService<IComplexDummyService, ComplexDummyService>();
			var simple = services.GetServiceInstance<ISimpleDummyService>();
			var complex = services.GetServiceInstance<IComplexDummyService>();
		}
	}
}
