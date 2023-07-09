using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
