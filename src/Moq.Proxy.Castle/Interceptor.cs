using DynamicProxy = Castle.DynamicProxy;

namespace Moq.Proxy.Castle
{
	internal class Interceptor : DynamicProxy.IInterceptor
	{
		IInterceptor interceptor;

		internal Interceptor (IInterceptor interceptor)
		{
			this.interceptor = interceptor;
		}

		public void Intercept (DynamicProxy.IInvocation invocation)
		{
			interceptor.Intercept (new MethodCall (invocation));
		}
	}
}
