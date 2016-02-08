using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using DynamicProxy = LinFu.DynamicProxy;

namespace Moq.Proxy.LinFu
{
	public class ProxyFactory : IProxyFactory
	{
		static readonly DynamicProxy.ProxyFactory factory = new DynamicProxy.ProxyFactory();

		public object CreateProxy (IInterceptor interceptor, Type baseType, Type[] implementedInterfaces, object[] constructorArguments)
		{
			if (baseType.IsInterface)
				// TODO: Moq should guarantee that only class baseType is specified 
				// at all times. For interface only mocks, Moq.Proxy will provide a 
				// base type so that ToString can be always overriden.
				throw new NotSupportedException ();

			// TODO: the proxy factory should automatically detect requests to proxy 
			// delegates and generate an interface on the fly for them, without Moq 
			// having to know about it at all.

			// NOTE: LinFu doesn't support passing ctor args??!?!?!
			if (constructorArguments.Length != 0)
				throw new NotSupportedException ();

			return factory.CreateProxy(baseType, new Interceptor (interceptor), implementedInterfaces);
		}
	}
}
