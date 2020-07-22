using diTest;
using NUnit.Framework;
using Testing.Classes;

namespace Testing
{
	public class Tests
	{
		public ISimpleDummyService Simple { get; set; }
		public IComplexDummyService Complex { get; set; }
		public IValueDependencyDummy Value { get; set; }

		[SetUp]
		public void Setup()
		{
			var services = new ServiceCollection();
			services.AddService<ISimpleDummyService, SimpleDummyService>();
			services.AddService<IComplexDummyService, ComplexDummyService>();
			services.AddService<IValueDependencyDummy, ValueDependencyDummy>(() => new ValueDependencyDummy(false));
			Simple = services.GetServiceInstance<ISimpleDummyService>();
			Complex = services.GetServiceInstance<IComplexDummyService>();
			Value = services.GetServiceInstance<IValueDependencyDummy>();
		}

		[Test]
		public void IsTypeCorrect()
		{

			Assert.AreEqual(Simple.GetType(), typeof(SimpleDummyService));
			Assert.AreEqual(Complex.GetType(), typeof(ComplexDummyService));
			Assert.AreEqual(Value.GetType(), typeof(ValueDependencyDummy));
			Assert.Pass();
		}

		[Test]
		public void IsFunctionalityCorrect()
		{
			Assert.IsFalse(Simple.Invert(true));
			Assert.IsTrue(Simple.Invert(false));
			Assert.IsTrue(Complex.Invert());
			Assert.IsFalse(Complex.Invert());
			Assert.IsTrue(Value.Invert());
			Assert.Pass();
		}


	}
}