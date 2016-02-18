using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Moq.Proxy
{
	public class ParameterCollection : IParameterCollection
	{
		List<ParameterInfo> infos = new List<ParameterInfo>();
		List<object> values = new List<object>();

		public ParameterCollection (object[] values, ParameterInfo[] infos)
		{
			this.infos = infos.ToList ();
			this.values = values.ToList ();
		}

		public ParameterCollection (IEnumerable<object> values, IEnumerable<ParameterInfo> infos)
		{
			this.infos = infos.ToList ();
			this.values = values.ToList ();
		}

		public object this[int index] 
		{
			get { return values[index]; }
			set { values[index] = value; }
		}

		public object this[string parameterName]
		{
			get { return values[IndexOf(parameterName)]; }
			set { values[IndexOf(parameterName)] = value; }
		}

		public int Count => infos.Count;

		public bool Contains (string parameterName) => IndexOf (parameterName) != -1;

		public IEnumerator<object> GetEnumerator () => values.GetEnumerator ();

		public ParameterInfo GetInfo (string parameterName) => infos[IndexOf (parameterName)];

		public ParameterInfo GetInfo (int index) => infos[index];

		public string GetName (int index) => infos[index].Name;

		public int IndexOf (string parameterName)
		{
			for (int i = 0; i < infos.Count; ++i) {
				if (infos[i].Name == parameterName) {
					return i;
				}
			}

			return -1;
		}

		IEnumerator IEnumerable.GetEnumerator () => values.GetEnumerator ();
	}
}
