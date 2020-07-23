using diTest.Exceptions;
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
		private Dictionary<Type, object> singletons { get; set; } = new Dictionary<Type, object>();

		public ServiceCollection AddService<TInt, TConc>() where TInt : class where TConc : class, TInt
		{
			if (!typeof(TInt).IsInterface) throw new NonInterfaceException(typeof(TInt).Name);
			coll.Add(typeof(TInt), typeof(TConc));
			return this;
		}

		public ServiceCollection AddService(Type intType, Type concType)
		{
			if (!intType.IsInterface) throw new NonInterfaceException(intType.Name);
			coll.Add(intType, concType);
			return this;
		}

		public ServiceCollection AddService<TInt, TConc>(Func<TConc> initializer) where TInt : class where TConc : class, TInt
		{
			if (!typeof(TInt).IsInterface) throw new NonInterfaceException(typeof(TInt).Name);
			coll.Add(typeof(TInt), typeof(TConc));
			initializers.Add(typeof(TInt), initializer);
			return this;
		}

		public ServiceCollection AddSingleton<TInt, TConc>() where TInt : class where TConc : class, TInt
		{
			AddService<TInt, TConc>();
			RegisterSingleton(typeof(TInt));
			return this;
		}

		public ServiceCollection AddSingleton(Type intType, Type concType)
		{
			AddService(intType, concType);
			RegisterSingleton(intType);
			return this;
		}

		public ServiceCollection AddSingleton<TInt, TConc>(Func<TConc> initializer) where TInt : class where TConc : class, TInt
		{
			AddService<TInt, TConc>(initializer);
			RegisterSingleton(typeof(TInt));
			return this;
		}

		public TInt GetServiceInstance<TInt>() where TInt : class
		{
			TInt instance;
			Type intType = typeof(TInt);
			Type[] genericParams = intType.GetGenericArguments();
			Type concServiceType;
			Func<object> initializer;
			bool isSingleton = IsSingleton(intType);
			bool isInitialized = isSingleton && IsRegisteredInitializedSingleton(intType);

			if (isInitialized)
			{
				instance = (TInt)GetSingleton(intType);
			}
			else if ((initializer = GetInitializer<TInt>()) != null)
			{
				instance = (TInt)initializer();
			}
			else
			{
				if (intType.IsGenericType)
				{
					intType = intType.GetGenericTypeDefinition();
					concServiceType = GetServiceType(intType).MakeGenericType(genericParams);
				}
				else
				{
					concServiceType = GetServiceType<TInt>();
				}

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
				instance = (TInt)Activator.CreateInstance(concServiceType, paramArr);
			}

			if (isSingleton && !isInitialized)
			{
				InitializeSingleton(intType, instance);
			}

			return instance;
		}

		private Type GetServiceType<TInt>() where TInt : class
		{
			return coll.GetValueOrDefault(typeof(TInt));
		}

		private Type GetServiceType(Type intType)
		{
			return coll.GetValueOrDefault(intType);
		}

		private Func<object> GetInitializer<TInt>()
		{
			return initializers.TryGetValue(typeof(TInt), out Func<object> initializer) ? initializer : null;
		}

		private void RegisterSingleton(Type intType)
		{
			singletons.Add(intType, null);
		}

		private void InitializeSingleton(Type intType, object singleton)
		{
			if(singletons.ContainsKey(intType) && singletons[intType] == null)
			{
				singletons[intType] = singleton;
			}
			else if (!singletons.ContainsKey(intType))
			{
				throw new SingletonNotRegisteredException(intType.Name);
			}
			else
			{
				throw new SingletonOverwriteException(intType.Name);
			}
		}

		private object GetSingleton(Type intType)
		{
			if (singletons.ContainsKey(intType) && singletons[intType] != null)
			{
				return singletons[intType];
			}
			else
			{
				throw new SingletonNotInitializedException(intType.Name);
			}
		}

		private bool IsSingleton(Type intType)
		{
			return singletons.ContainsKey(intType);
		}

		private bool IsRegisteredInitializedSingleton(Type intType)
		{
			var res = singletons.TryGetValue(intType, out object instance);
			return res && instance != null;
		}
	}
}
