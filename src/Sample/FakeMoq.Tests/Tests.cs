using Xunit;

namespace Tests
{
	public class Misc
	{
		[Fact]
		public void when_creating_proxy_then_succeeds ()
		{
			var proxy = Mock.Of<IFoo>();

			Assert.NotNull (proxy);
			proxy.Do ();
		}
	}
}