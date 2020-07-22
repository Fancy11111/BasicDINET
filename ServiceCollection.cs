using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace diTest
{
	public class ServiceCollection
	{
		private Dictionary<Type, Type> coll { get; set; } = new Dictionary<Type, Type>();

		public void AddService<TInt, TConc>() where TInt : class where TConc : class, TInt
		{
			if (!typeof(TInt).IsInterface) throw new NonInterfaceException(typeof(TInt).Name);
			coll.Add(typeof(TInt), typeof(TConc));
		}

		public Type GetServiceType<TInt>() where TInt : class
		{
			return coll.GetValueOrDefault(typeof(TInt));
		}

		public TInt GetServiceInstance<TInt>() where TInt : class
		{
			Type concServiceType = GetServiceType<TInt>();
			var ctorInfo = concServiceType.GetConstructors()[0];
			var ctorParams = ctorInfo.GetParameters();
			object[] paramArr = { };
			if (ctorParams != null && ctorParams.Any())
			{
				foreach (var ctorParam in ctorParams)
				{
					var ctorType = ctorParam.ParameterType.GetTypeInfo();
					MethodInfo method = GetType().GetMethod("GetServiceInstance")
						.MakeGenericMethod(new Type[] { ctorType });
					object param = method.Invoke(this, new object[] { });
					paramArr = paramArr.Append(param).ToArray();
				}
			}
			TInt instance = (TInt)Activator.CreateInstance(concServiceType, paramArr);
			return instance;
		}
	}
}
