using Moq.Proxy;
using System.Reflection;
using System;

namespace FakeMoq
{
	internal class MockBehavior : IProxyBehavior
	{
		public IMethodReturn Invoke (IMethodInvocation invocation, GetNextBehavior getNext)
		{
			if (invocation.MethodBase.DeclaringType.GetTypeInfo ().IsInterface)
				return invocation.CreateValueReturn (null);

			return getNext () (invocation, getNext);
		}
	}
}
