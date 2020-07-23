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
		/// <summary>
		/// Contains all pairs of interfaces and implementations
		/// </summary>
		private Dictionary<Type, Type> coll { get; set; } = new Dictionary<Type, Type>();
		/// <summary>
		/// contains pairs of interfaces and initializers for types that are registered with initializers
		/// </summary>
		private Dictionary<Type, Func<object>> initializers { get; set; } = new Dictionary<Type, Func<object>>();
		/// <summary>
		/// contains pairs of interfaces and concrete objects for singletons
		/// </summary>
		private Dictionary<Type, object> singletons { get; set; } = new Dictionary<Type, object>();

		/// <summary>
		/// Add a service via generic parameters
		/// </summary>
		/// <typeparam name="TInt">Interfacetype</typeparam>
		/// <typeparam name="TConc">Implementationtype</typeparam>
		/// <exception cref="NonInterfaceException">When TInt is not an Interface</exception>
		/// <returns>this object for fluent api</returns>
		public ServiceCollection AddService<TInt, TConc>() where TInt : class where TConc : class, TInt => 
			AddService(typeof(TInt), typeof(TConc));

		/// <summary>
		/// Add a service via generic parameters with an initializer Func
		/// </summary>
		/// <param name="initializer">Func that returns a <see cref="TConc"/>, is used to instanciate on request</param>
		/// <typeparam name="TInt">Interfacetype</typeparam>
		/// <typeparam name="TConc">Implementationtype</typeparam>
		/// <exception cref="NonInterfaceException">When TInt is not an Interface</exception>
		/// <returns>this object for fluent api</returns>
		public ServiceCollection AddService<TInt, TConc>(Func<TConc> initializer) where TInt : class where TConc : class, TInt
		{
			AddService(typeof(TInt), typeof(TConc));
			initializers.Add(typeof(TInt), initializer);
			return this;
		}

		/// <summary>
		/// Add a service via type parameters
		/// </summary>
		/// <param name="intType">Interfacetype</param>
		/// <param name="concType">Implementationtype</param>
		/// <exception cref="NonInterfaceException">When TInt is not an Interface</exception>
		/// <returns>this object for fluent api</returns>
		public ServiceCollection AddService(Type intType, Type concType)
		{
			if (!intType.IsInterface) throw new NonInterfaceException(intType.Name);
			coll.Add(intType, concType);
			return this;
		}

		/// <summary>
		/// Add a service as a singleton via generic types
		/// </summary>
		/// <typeparam name="TInt"></typeparam>
		/// <typeparam name="TConc"></typeparam>
		/// <returns></returns>
		public ServiceCollection AddSingleton<TInt, TConc>() where TInt : class where TConc : class, TInt
		{
			AddService<TInt, TConc>();
			RegisterSingleton(typeof(TInt));
			return this;
		}

		/// <summary>
		/// Add a service as a singleton via type parameters
		/// </summary>
		/// <param name="intType">Interfacetype</param>
		/// <param name="concType">Implementationtype</param>
		/// <exception cref="NonInterfaceException">When TInt is not an Interface</exception>
		/// <returns>this object for fluent api</returns>
		public ServiceCollection AddSingleton(Type intType, Type concType)
		{
			AddService(intType, concType);
			RegisterSingleton(intType);
			return this;
		}

		/// <summary>
		/// Add a service as a singleton via generic parameters with an initializer Func
		/// </summary>
		/// <param name="initializer">Func that returns a <see cref="TConc"/>, is used to instanciate on request</param>
		/// <typeparam name="TInt">Interfacetype</typeparam>
		/// <typeparam name="TConc">Implementationtype</typeparam>
		/// <exception cref="NonInterfaceException">When TInt is not an Interface</exception>
		/// <returns>this object for fluent api</returns>
		public ServiceCollection AddSingleton<TInt, TConc>(Func<TConc> initializer) where TInt : class where TConc : class, TInt
		{
			AddService<TInt, TConc>(initializer);
			RegisterSingleton(typeof(TInt));
			return this;
		}

		/// <summary>
		/// retrieve a serviceinstance via generic type
		/// </summary>
		/// <typeparam name="TInt">The interface used to retrieve the concrete implementation type</typeparam>
		/// <returns>An instance of a concrete implementation belonging to <typeparam name="TInt"/></returns>
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

		/// <summary>
		/// Get type of concrete implementation for Interface via typeparam
		/// </summary>
		/// <typeparam name="TInt">Generic type param as the interface to search for</typeparam>
		/// <returns>Concrete Type or null</returns>
		private Type GetServiceType<TInt>() where TInt : class => 
			GetServiceType(typeof(TInt));

		/// <summary>
		/// Get type of concrete implementation for Interface via param
		/// </summary>
		/// <param name="intType">Generic type param as the interface to search for</param>
		/// <returns>concrete Type or null</returns>
		private Type GetServiceType(Type intType) => 
			coll.GetValueOrDefault(intType);

		/// <summary>
		/// Get initializer or null
		/// </summary>
		/// <typeparam name="TInt"></typeparam>
		/// <returns>initializer func or null</returns>
		private Func<object> GetInitializer<TInt>() => 
			GetInitializer(typeof(TInt));

		/// <summary>
		/// Get initializer or null
		/// </summary>
		/// <typeparam name="TInt"></typeparam>
		/// <returns>initializer func or null</returns>
		private Func<object> GetInitializer(Type intType) =>
			initializers.TryGetValue(intType, out Func<object> initializer) ? initializer : null;

		/// <summary>
		/// Register singleton with null as value
		/// </summary>
		/// <param name="intType"></param>
		/// <exception cref="SingletonAlreadyRegisteredException">when singleton is already registered</exception>
		private void RegisterSingleton(Type intType)
		{
			if (singletons.ContainsKey(intType)) throw new SingletonAlreadyRegisteredException(intType.Name);
			singletons.Add(intType, null);
		}

		/// <summary>
		/// Initialize singleton
		/// </summary>
		/// <param name="intType"></param>
		/// <param name="singleton"></param>
		/// <exception cref="SingletonNotRegisteredException">when not registered as singleton</exception>
		/// <exception cref="SingletonOverwriteException">when already initialized singleton</exception>
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

		/// <summary>
		/// Retrieve singleton
		/// </summary>
		/// <param name="intType"></param>
		/// <returns></returns>
		/// <exception cref="SingletonNotRegisteredException">when not registered as singleton</exception>
		/// <exception cref="SingletonNotInitializedException">when not initialized</exception>
		private object GetSingleton(Type intType)
		{
			if (singletons.ContainsKey(intType) && singletons[intType] != null)
			{
				return singletons[intType];
			}
			else if (!singletons.ContainsKey(intType))
			{
				throw new SingletonNotRegisteredException(intType.Name);
			}
			else
			{
				throw new SingletonNotInitializedException(intType.Name);
			}
		}

		/// <summary>
		/// check if type is registered as singleton
		/// </summary>
		/// <param name="intType"></param>
		/// <returns></returns>
		private bool IsSingleton(Type intType) => 
			singletons.ContainsKey(intType);

		/// <summary>
		/// check if type is registered and initialized as singleton
		/// </summary>
		/// <param name="intType"></param>
		/// <returns></returns>
		private bool IsRegisteredInitializedSingleton(Type intType)
		{
			var res = singletons.TryGetValue(intType, out object instance);
			return res && instance != null;
		}
	}
}
