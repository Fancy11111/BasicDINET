using System;
using System.Collections.Generic;
using System.Text;

namespace diTest.Exceptions
{
	[System.Serializable]
	public class NonInterfaceException : Exception
	{
		public NonInterfaceException() { }
		public NonInterfaceException(string message) : base(message) { }
		public NonInterfaceException(string message, Exception inner) : base(message, inner) { }
		protected NonInterfaceException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
