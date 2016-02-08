using System;
using DynamicProxy = LinFu.DynamicProxy;

namespace Moq.Proxy.LinFu
{
	internal class Interceptor : DynamicProxy.IInterceptor
	{
		IInterceptor interceptor;

		internal Interceptor (IInterceptor interceptor)
		{
			this.interceptor = interceptor;
		}

		public object Intercept (DynamicProxy.InvocationInfo info)
		{
			var call = new MethodCall (info);

			interceptor.Intercept (call);

			return call.ReturnValue;
		}
	}
}
