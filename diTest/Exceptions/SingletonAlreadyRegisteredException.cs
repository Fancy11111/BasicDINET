using System;
using System.Collections.Generic;
using System.Text;

namespace diTest.Exceptions
{

	[Serializable]
	public class SingletonAlreadyRegisteredException : Exception
	{
		public SingletonAlreadyRegisteredException() { }
		public SingletonAlreadyRegisteredException(string message) : base(message) { }
		public SingletonAlreadyRegisteredException(string message, Exception inner) : base(message, inner) { }
		protected SingletonAlreadyRegisteredException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
