using Xunit;
using FakeMoq;

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
			// TODO: ToString doesn't currently work with LinFu, investigate
			// System.Console.WriteLine (proxy.ToString());
		}
	}

	public interface IFoo
	{
		void Do ();
	}
}
