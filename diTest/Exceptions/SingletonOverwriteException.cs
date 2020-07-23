using System;
using System.Collections.Generic;
using System.Text;

namespace diTest.Exceptions
{

	[Serializable]
	public class SingletonOverwriteException : Exception
	{
		public SingletonOverwriteException() { }
		public SingletonOverwriteException(string message) : base(message) { }
		public SingletonOverwriteException(string message, Exception inner) : base(message, inner) { }
		protected SingletonOverwriteException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
