using Xunit;
using FakeMoq;
using System.Reflection;
using System.Linq;
using Castle.DynamicProxy;
using System.IO;
using System;

namespace Moq.Proxy.Tests
{
	public class Misc
	{
		[Fact]
		public void when_creating_proxy_then_succeeds ()
		{
			var proxy = Mock.Of<IFoo>();

			Assert.NotNull (proxy);
			proxy.Do ();
			// TODO: ToString doesn't currently work with LinFu, investigate why?
			// System.Console.WriteLine (proxy.ToString());
		}
		
		[Fact]
		public void when_adding_behavior_then_can_override_mock ()
		{
			var foo = Mock.Of<IFoo>();
			var proxy = (IProxy)foo;

			proxy.InsertBehavior(0, (invocation, getNext) => {
				if (invocation.MethodBase.Name == "Add")
					return invocation.CreateValueReturn ((int)invocation.Arguments[0] + (int)invocation.Arguments[1], invocation.Arguments.ToArray());

				return getNext () (invocation, getNext);
			});

			var result = foo.Add(1, 3);

			Assert.Equal (4, result);
		}

		public interface IFoo
		{
			void Do();

			int Add(int x, int y);
		}
	}
}