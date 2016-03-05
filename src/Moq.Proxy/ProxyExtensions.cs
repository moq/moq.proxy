using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Moq.Proxy
{
	public static class ProxyExtensions
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static MethodInfo GetBaseMethod<TDelegate> (this IProxy proxy, TDelegate method)
			where TDelegate : class
		{
			var target = method as Delegate;

			return target.GetMethodInfo ();
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		public static MethodInfo GetInterfaceMethod<TInterface> (this IProxy proxy, MethodInfo method)
		{
			// Convert the class implementation method into the interface definition method.
			var mapping = proxy.GetType().GetTypeInfo().GetRuntimeInterfaceMap(typeof(TInterface));
			for (int i = 0; i < mapping.TargetMethods.Length; i++) {
				if (mapping.TargetMethods[i] == method) {
					return mapping.InterfaceMethods[i];
				}
			}

			return method;
		}

		[EditorBrowsable (EditorBrowsableState.Never)]
		public static MethodInfo GetInterfaceMethod<TDelegate, TInterface>(this IProxy proxy, TDelegate @delegate)
			where TDelegate : class
		{
			var target = @delegate as Delegate;
			var method = target.GetMethodInfo();
			// Convert the class implementation method into the interface definition method.
			var mapping = proxy.GetType().GetTypeInfo().GetRuntimeInterfaceMap(typeof(TInterface));
			for (int i = 0; i < mapping.TargetMethods.Length; i++) {
				if (mapping.TargetMethods[i] == method) {
					return mapping.InterfaceMethods[i];
				}
			}

			return target.GetMethodInfo ();
		}

		public static void AddBehavior (this IProxy proxy, InvokeBehavior behavior)
		{
			proxy.Behaviors.Add (new AnonymousBehavior (behavior));
		}

		public static void InsertBehavior (this IProxy proxy, int index, InvokeBehavior behavior)
		{
			proxy.Behaviors.Insert(index, new AnonymousBehavior (behavior));
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
