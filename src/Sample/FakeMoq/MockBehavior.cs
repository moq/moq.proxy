using Moq.Proxy;
using System.Reflection;
using System;

namespace FakeMoq
{
	internal class MockBehavior : IProxyBehavior
	{
		public IMethodReturn Invoke (IMethodInvocation invocation, GetNextBehavior getNext)
		{
			// This is where the whole Moq matching, behavior, callbacks etc. 
			// would be evaluated and provide the actual behavior.
			if (invocation.MethodBase.DeclaringType.GetTypeInfo ().IsInterface)
				return invocation.CreateValueReturn (null);

			return getNext () (invocation, getNext);
		}
	}
}
