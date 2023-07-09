using Microsoft.Xna.Framework;

namespace Amino.Utilities
{
	/// <summary> Extensions relating to services. </summary>
	public static class ServiceExtensions
	{
		/// <summary>
		/// Gets the service of the given type, throwing <see cref="ServiceNotPresentException"/> if it's not present.
		/// </summary>
		/// <exception cref="ServiceNotPresentException"></exception>
		public static T Require<T>(this GameServiceContainer services) where T : class
		{
			T service = services.GetService<T>();
			if (service == null)
			{
				throw new ServiceNotPresentException(typeof(T).Name);
			}
			return service;
		}
	}
}
