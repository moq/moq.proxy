using System;
using System.Collections.Generic;
using System.Reflection;

namespace Moq.Proxy
{
	/// <summary>
	/// Default implementation of <see cref="IMethodInvocation"/>.
	/// </summary>
	public class MethodInvocation : IMethodInvocation
	{
		public MethodInvocation (object target, MethodBase method, params object[] arguments)
		{
			Target = target;
			MethodBase = method;
			Arguments = new ParameterCollection (arguments, method.GetParameters ());
			Context = new Dictionary<string, object> ();
		}

		public IParameterCollection Arguments { get; }

		public IDictionary<string, object> Context { get; }

		public MethodBase MethodBase { get; }

		public object Target { get; }

		public IMethodReturn CreateExceptionReturn (Exception exception) => new MethodReturn (this, exception);

		public IMethodReturn CreateValueReturn (object returnValue, params object[] allArguments) => new MethodReturn (this, returnValue, allArguments);
	}
}