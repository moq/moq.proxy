namespace Moq.Proxy
{
	public interface IInterceptor
	{
		void Intercept(IMethodCall context);
	}
}