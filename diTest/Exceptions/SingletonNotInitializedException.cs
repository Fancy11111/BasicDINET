using System;
using System.Collections.Generic;
using System.Text;

namespace diTest.Exceptions
{

	[Serializable]
	public class SingletonNotInitializedException : Exception
	{
		public SingletonNotInitializedException() { }
		public SingletonNotInitializedException(string message) : base(message) { }
		public SingletonNotInitializedException(string message, Exception inner) : base(message, inner) { }
		protected SingletonNotInitializedException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
