using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Moq.Proxy;

namespace FakeMoq
{
	/// <summary>
	/// Exposes the actual Moq creation behavior.
	/// </summary>
	/// <remarks>
	/// Since GetCallingAssembly isn't reliable due to inlining and is 
	/// therefore not even available in PCL, we resort to including the 
	/// the entry point API as source via the FakeMoq.targets, which 
	/// means we can use it to inspect the "current" assembly since it 
	/// will be the one where this package was installed, which is the 
	/// test project typically.
	/// </remarks>
	public static class Mock
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static T Of<T>(IProxyFactory proxyFactory)
		{
			var proxy = (T)proxyFactory.CreateProxy(typeof(T), new Type[0], new object[0]);

			// NOTE: this means anyone can add new behaviors to the proxy, 
			// reaccomodate the behaviors list, etc.
			((IProxy)proxy).Behaviors.Add(new MockBehavior());

			return proxy;
		}
	}
}