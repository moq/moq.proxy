namespace Moq.Proxy
{
	public interface IProxyBehavior
	{
		IMethodReturn Invoke(IMethodInvocation invocation, GetNextBehavior getNext);
	}

	public delegate InvokeBehavior GetNextBehavior();

	public delegate IMethodReturn InvokeBehavior(IMethodInvocation invocation, GetNextBehavior getNext);
}