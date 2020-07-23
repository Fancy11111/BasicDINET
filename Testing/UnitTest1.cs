using diTest;
using NUnit.Framework;
using System;
using Testing.Classes;

namespace Testing
{
	public class Tests
	{
		public ISimpleDummyService Simple { get; set; }
		public IComplexDummyService Complex { get; set; }
		public IValueDependencyDummy Value { get; set; }
		public IGenericSimpleDummy<int> Generic { get; set; }
		public ISingletonDummy Singleton { get; set; }
		public ServiceCollection Services { get; set; }

		[SetUp]
		public void Setup()
		{
			Services = new ServiceCollection();
			Services.AddService<ISimpleDummyService, SimpleDummyService>();
			Services.AddService<IComplexDummyService, ComplexDummyService>();
			Services.AddService<IValueDependencyDummy, ValueDependencyDummy>(() => new ValueDependencyDummy(false));
			Services.AddService(typeof(IGenericSimpleDummy<>), typeof(GenericSimpleDummy<>));
			Services.AddSingleton(typeof(ISingletonDummy), typeof(SingletonDummy));
			Simple = Services.GetServiceInstance<ISimpleDummyService>();
			Complex = Services.GetServiceInstance<IComplexDummyService>();
			Value = Services.GetServiceInstance<IValueDependencyDummy>();
			Generic = Services.GetServiceInstance<IGenericSimpleDummy<int>>();
			Singleton = Services.GetServiceInstance<ISingletonDummy>();
		}

		[Test]
		public void IsTypeCorrect()
		{
			Assert.AreEqual(Simple.GetType(), typeof(SimpleDummyService));
			Assert.AreEqual(Complex.GetType(), typeof(ComplexDummyService));
			Assert.AreEqual(Value.GetType(), typeof(ValueDependencyDummy));
			Assert.AreEqual(Generic.GetType(), typeof(GenericSimpleDummy<int>));
			Assert.AreEqual(Singleton.GetType(), typeof(SingletonDummy));
		}

		[Test]
		public void IsFunctionalityCorrect()
		{
			Assert.IsFalse(Simple.Invert(true));
			Assert.IsTrue(Simple.Invert(false));
			Assert.IsTrue(Complex.Invert());
			Assert.IsFalse(Complex.Invert());
			Assert.IsTrue(Value.Invert());
		}

		[Test]
		public void GenericTest()
		{
			Generic.SetValue(1);
			Assert.AreEqual(1, Generic.GetValue());
			Generic.SetValue(20);
			Assert.AreEqual(20, Generic.GetValue());
			Generic.SetValue(-1000);
			Assert.AreEqual(-1000, Generic.GetValue());
		}

		[Test]
		public void SingletonTest()
		{
			Singleton.SetValue("test 1");
			var singleton2 = Services.GetServiceInstance<ISingletonDummy>();
			Assert.AreEqual(Singleton.GetValue(), singleton2.GetValue());
		}
	}
}