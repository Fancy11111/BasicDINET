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
		private Dictionary<Type, Func<object>> initializers { get; set; } = new Dictionary<Type, Func<object>>();

		public void AddService<TInt, TConc>() where TInt : class where TConc : class, TInt
		{
			if (!typeof(TInt).IsInterface) throw new NonInterfaceException(typeof(TInt).Name);
			coll.Add(typeof(TInt), typeof(TConc));
		}

		public void AddService<TInt, TConc>(Func<TConc> initializer) where TInt : class where TConc : class, TInt
		{
			if (!typeof(TInt).IsInterface) throw new NonInterfaceException(typeof(TInt).Name);
			coll.Add(typeof(TInt), typeof(TConc));
			initializers.Add(typeof(TInt), initializer);
		}

		private Type GetServiceType<TInt>() where TInt : class
		{
			return coll.GetValueOrDefault(typeof(TInt));
		}

		public TInt GetServiceInstance<TInt>() where TInt : class
		{
			Type concServiceType = GetServiceType<TInt>();
			var initializer = GetInitializer<TInt>();
			if (initializer != null)
			{
				return (TInt)initializer();
			}
			else
			{
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

		private Func<object> GetInitializer<TInt>()
		{
			return initializers.TryGetValue(typeof(TInt), out Func<object> initializer) ? initializer : null;
		}
	}
}
