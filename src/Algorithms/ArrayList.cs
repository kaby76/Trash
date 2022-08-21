using System;
using System.Collections.Generic;
using System.Text;

namespace Algorithms
{
	public class ArrayList<T> : List<T>
	{

		public ArrayList()
		{
		}

		public ArrayList(int count)
			: base(count)
		{
		}

		public override int GetHashCode()
		{
			int hash = MurmurHash.Initialize(1);
			foreach (T t in this)
				hash = MurmurHash.Update(hash, t.GetHashCode());
			hash = MurmurHash.Finish(hash, this.Count);
			return hash;
		}

		public override bool Equals(object o)
		{
			return o == this
				|| (o is List<T> list && Equals(list));
		}


		public bool Equals(List<T> o)
		{
			if (this.Count != o.Count)
				return false;
			IEnumerator<T> thisItems = this.GetEnumerator();
			IEnumerator<T> otherItems = o.GetEnumerator();
			while (thisItems.MoveNext() && otherItems.MoveNext())
			{
				if (!thisItems.Current.Equals(otherItems.Current))
					return false;
			}
			return true;

		}

	}
}
