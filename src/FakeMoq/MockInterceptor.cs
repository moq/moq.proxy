using Moq.Proxy;
using System.Reflection;

namespace FakeMoq
{
	internal class MockInterceptor : IInterceptor
	{
		public void Intercept (IMethodCall context)
		{
			if (context.Method.DeclaringType.GetTypeInfo ().IsInterface)
				return;

			context.InvokeTarget ();
		}
	}
}
