using System.Collections.Generic;

namespace Moq.Proxy
{
	public interface IProxy
	{
		IList<IProxyBehavior> Behaviors { get; }
	}
}