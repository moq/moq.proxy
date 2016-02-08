using System.Reflection;
using DynamicProxy = LinFu.DynamicProxy;

namespace Moq.Proxy.LinFu
{
	internal class MethodCall : IMethodCall
	{
		DynamicProxy.InvocationInfo invocation;

		internal MethodCall (DynamicProxy.InvocationInfo invocation)
		{
			this.invocation = invocation;
		}

		public object[] Arguments => invocation.Arguments;

		public MethodInfo Method => invocation.TargetMethod;

		public object ReturnValue { get; set; }

		public void InvokeTarget ()
		{
			ReturnValue = invocation.TargetMethod.Invoke (invocation.Target, invocation.Arguments);
		}

		public void SetArgument (int index, object value)
		{
			invocation.SetArgument (index, value);
		}
	}
}
