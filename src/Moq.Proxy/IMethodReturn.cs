using System;
using System.Collections.Generic;

namespace Moq.Proxy
{
	public interface IMethodReturn
	{
		IDictionary<string, object> Context { get; }

		Exception Exception { get; }

		IParameterCollection Outputs { get; }

		object ReturnValue { get; set; }
	}
}