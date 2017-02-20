using System;

namespace Moq.Proxy
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public class ProxyFactoryAttribute : Attribute
	{
		public ProxyFactoryAttribute(Type factoryType)
		{
			FactoryType = factoryType;
		}

		public Type FactoryType { get; }
	}
}
