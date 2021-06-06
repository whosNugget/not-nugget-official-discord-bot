using System;
using System.Collections.Generic;
using System.Text;

namespace NuggetTwitchIntegration.Exceptions
{
	public class APIWrapperNotInstantiatedException : Exception
	{
		public APIWrapperNotInstantiatedException() : base() { }
		public APIWrapperNotInstantiatedException(string? message) : base(message) { }
		public APIWrapperNotInstantiatedException(string? message, Exception? innerException) : base(message, innerException) { }
	}
}
