using System;
using System.Collections.Generic;
using System.Reflection;

namespace Moq.Proxy
{
	public interface IMethodInvocation
	{
		IParameterCollection Arguments { get; }

		IDictionary<string, object> Context { get; }

		MethodBase MethodBase { get; }

		object Target { get; }

		IMethodReturn CreateValueReturn(object returnValue, params object[] allArguments);

		IMethodReturn CreateExceptionReturn(Exception exception);
	}
}