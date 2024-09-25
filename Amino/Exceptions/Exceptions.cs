using System;

namespace Amino
{
	public class ServiceNotPresentException : Exception
	{
		public ServiceNotPresentException(string serviceName)
			: base($"Service '{serviceName}' was required but wasn't present.")
		{

		}
	}
}
