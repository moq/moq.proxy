using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Moq.Proxy
{
	public static class MethodInfoExtensions
	{
		public static bool IsVoid (this MethodInfo method) => 
			method.ReturnType == typeof (void);

		public static bool HasOutOrRef (this MethodInfo method) => 
			method.GetParameters ().Any (p => p.IsOut || p.ParameterType.IsByRef);
	}
}
