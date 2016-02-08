using System;

namespace Moq.Proxy
{
	public interface IProxyFactory
	{
		object CreateProxy(IInterceptor interceptor, Type baseType, Type[] implementedInterfaces, object[] construtorArguments);
	}
}