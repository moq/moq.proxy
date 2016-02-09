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
			return (T)proxyFactory.CreateProxy (new MockInterceptor (), typeof (T), new Type[0], new object[0]);
		}
	}
}