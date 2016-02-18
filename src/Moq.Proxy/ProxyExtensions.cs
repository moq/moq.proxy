using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moq.Proxy
{
	public static class ProxyExtensions
	{
		public static void InsertBehavior (this IProxy proxy, int index, InvokeBehavior behavior)
		{
			proxy.Behaviors.Insert(index, new AnonymousBehavior (behavior));
		}

		public static void AddBehavior(this IProxy proxy, InvokeBehavior behavior)
		{
			proxy.Behaviors.Add (new AnonymousBehavior (behavior));
		}

		class AnonymousBehavior : IProxyBehavior
		{
			InvokeBehavior behavior;

			public AnonymousBehavior (InvokeBehavior behavior)
			{
				this.behavior = behavior;
			}

			public IMethodReturn Invoke (IMethodInvocation invocation, GetNextBehavior getNext) =>
				behavior (invocation, getNext);
		}
	}
}
