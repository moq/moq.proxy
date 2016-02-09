using System.Reflection;
using DynamicProxy = Castle.DynamicProxy;

namespace Moq.Proxy.Castle
{
	internal class MethodCall : IMethodCall
	{
		DynamicProxy.IInvocation invocation;

		internal MethodCall (DynamicProxy.IInvocation invocation)
		{
			this.invocation = invocation;
		}

		public object[] Arguments => invocation.Arguments;

		public MethodInfo Method => invocation.Method;

		public object ReturnValue
		{
			get { return invocation.ReturnValue; }
			set { invocation.ReturnValue = value; }
		}

		public void InvokeTarget ()
		{
			invocation.Proceed ();
		}

		public void SetArgument (int index, object value)
		{
			invocation.SetArgumentValue (index, value);
		}

		public override string ToString () => invocation.InvocationTarget?.GetType ().Name + "." + Method.Name;
	}
}
