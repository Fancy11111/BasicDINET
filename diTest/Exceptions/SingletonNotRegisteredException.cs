using System;
using System.Collections.Generic;
using System.Text;

namespace diTest.Exceptions
{

	[Serializable]
	public class SingletonNotRegisteredException : Exception
	{
		public SingletonNotRegisteredException() { }
		public SingletonNotRegisteredException(string message) : base(message) { }
		public SingletonNotRegisteredException(string message, Exception inner) : base(message, inner) { }
		protected SingletonNotRegisteredException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
