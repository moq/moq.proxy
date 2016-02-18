using System;
using System.Linq;

namespace Moq.Proxy.LinFu
{
	public class ProxyFactory : IProxyFactory
	{
		static readonly global::LinFu.DynamicProxy.ProxyFactory factory = new global::LinFu.DynamicProxy.ProxyFactory();

		public object CreateProxy (Type baseType, Type[] implementedInterfaces, object[] constructorArguments)
		{
			if (baseType.IsInterface) {
				// TODO: should Moq.Core do this work? It's the same for 
				// Castle and LinFu and presumably other (future?) libraries

				var fixedInterfaces = new Type[implementedInterfaces.Length + 1];
				fixedInterfaces[0] = baseType;
				implementedInterfaces.CopyTo (fixedInterfaces, 1);
				implementedInterfaces = fixedInterfaces;
				baseType = typeof (object);
			}

			// TODO: this code is duplicated exactly from the Castle proxy factory.
			if (!implementedInterfaces.Contains (typeof (IProxy))) {
				var fixedInterfaces = new Type[implementedInterfaces.Length + 1];
				fixedInterfaces[0] = typeof (IProxy);
				implementedInterfaces.CopyTo (fixedInterfaces, 1);
				implementedInterfaces = fixedInterfaces;
			}

			// TODO: the proxy factory should automatically detect requests to proxy 
			// delegates and generate an interface on the fly for them, without Moq 
			// having to know about it at all.

			// NOTE: LinFu doesn't support passing ctor args??!?!?!
			if (constructorArguments.Length != 0)
				throw new NotSupportedException ();

			return factory.CreateProxy(baseType, new Interceptor (), implementedInterfaces);
		}
	}
}
