using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Moq.Proxy;

namespace FakeMoq
{
	public static class Mock
	{
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static T Of<T> (IProxyFactory proxyFactory)
		{
			var proxy = (T)proxyFactory.CreateProxy(typeof(T), new Type[0], new object[0]);

			// NOTE: this means anyone can add new behaviors to the proxy, 
			// reaccomodate the behaviors list, etc.
			((IProxy)proxy).Behaviors.Add (new MockBehavior ());

			return proxy;
		}
	}
}