using System;

namespace Moq.Proxy
{
	public interface IProxyFactory
	{
		object CreateProxy(Type baseType, Type[] implementedInterfaces, object[] construtorArguments);
	}
}