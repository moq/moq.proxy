using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moq.Proxy
{
	[AttributeUsage (AttributeTargets.Assembly, AllowMultiple = false)]
	public class ProxyFactoryAttribute : Attribute
	{
		public ProxyFactoryAttribute (Type factoryType)
		{
			FactoryType = factoryType;
		}

		public Type FactoryType { get; }
	}
}
