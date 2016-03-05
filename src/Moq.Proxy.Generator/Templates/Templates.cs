using System.Collections.Generic;
using System.Reflection;

namespace Moq.Proxy.Generator.Templates
{
	partial class CsInterfaceProxy
	{
		public CsInterfaceProxy (string targetNamespace, string className,
			TypeInfo baseType, IEnumerable<TypeInfo> implementedInterfaces)
			: base (targetNamespace, className, baseType, implementedInterfaces)
		{
		}
	}

	partial class VbInterfaceProxy
	{
		public VbInterfaceProxy (string targetNamespace, string className,
			TypeInfo baseType, IEnumerable<TypeInfo> implementedInterfaces)
			: base (targetNamespace, className, baseType, implementedInterfaces)
		{
		}
	}
}
