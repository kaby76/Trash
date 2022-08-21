using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
///*****************************************************************************
/// Copyright (c) 2011 Jesper Moller, and others
/// All rights reserved. This program and the accompanying materials
/// are made available under the terms of the Eclipse Public License 2.0
/// which accompanies this distribution, and is available at
/// https://www.eclipse.org/legal/epl-2.0/
/// 
/// SPDX-License-Identifier: EPL-2.0
/// 
/// Contributors:
///     Jesper Moller - initial API and implementation
///     Jesper Steen Moller  - bug 340933 - Migrate to new XPath2 API
/// ******************************************************************************
/// </summary>

namespace org.eclipse.wst.xml.xpath2.api
{


	using ItemType = org.eclipse.wst.xml.xpath2.api.typesystem.ItemType;
	using SimpleAtomicItemTypeImpl = org.eclipse.wst.xml.xpath2.processor.@internal.types.SimpleAtomicItemTypeImpl;
	using BuiltinTypeLibrary = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.BuiltinTypeLibrary;
	using SingleItemSequence = org.eclipse.wst.xml.xpath2.processor.@internal.types.builtin.SingleItemSequence;

	/// <summary>
	/// @since 2.0
	/// </summary>
	public class ResultBuffer
	{

		private List<Item> values = new List<Item>();

		public virtual ResultSequence Sequence
		{
			get
			{
				if (values.Count == 0)
				{
					return EMPTY;
				}
				if (values.Count == 1)
				{
					return wrap(values[0]);
				}
    
				return new ArrayResultSequence(values.ToArray());
			}
		}

		public virtual void clear()
		{
			values.Clear();
		}

		public virtual ResultBuffer add(Item at)
		{
			values.Add(at);
			return this;
		}

		public virtual ResultBuffer append(Item at)
		{
			values.Add(at);
			return this;
		}

		public virtual ResultBuffer concat(ResultSequence rs)
		{
            for (var i = rs.iterator(); i.MoveNext(); )
            {
                var s = i.Current;
                values.Add(s);
            }
			// KED KED values.AddRange(collectionWrapper(rs));
			return this;
		}

		public sealed class SingleResultSequence : ResultSequence
		{

			public SingleResultSequence(Item at)
			{
				value_Renamed = at;
			}

			internal readonly Item value_Renamed;

			/* (non-Javadoc)
			 * @see org.eclipse.wst.xml.xpath2.api.ResultSequence#getLength()
			 */
			public int size()
			{
				return 1;
			}

			/* (non-Javadoc)
			 * @see org.eclipse.wst.xml.xpath2.api.ResultSequence#getItem(int)
			 */
			public Item item(int index)
			{
				if (index != 0)
				{
					throw new System.IndexOutOfRangeException("Length is one, you looked up number " + index);
				}
				return value_Renamed;
			}

			/* (non-Javadoc)
			 * @see org.eclipse.wst.xml.xpath2.api.ResultSequence#first()
			 */
			public Item first()
			{
				return item(0);
			}

			/* (non-Javadoc)
			 * @see org.eclipse.wst.xml.xpath2.api.ResultSequence#getItem(int)
			 */
			public object value(int index)
			{
				if (index != 0)
				{
					throw new System.IndexOutOfRangeException("Length is one, you looked up number " + index);
				}
				return value_Renamed.NativeValue;
			}

			/* (non-Javadoc)
			 * @see org.eclipse.wst.xml.xpath2.api.ResultSequence#empty()
			 */
			public bool empty()
			{
				return false;
			}

			/* (non-Javadoc)
			 * @see org.eclipse.wst.xml.xpath2.api.ResultSequence#first()
			 */
			public object firstValue()
			{
				return value_Renamed.NativeValue;
			}

			/* (non-Javadoc)
			 * @see org.eclipse.wst.xml.xpath2.api.ResultSequence#iterator()
			 */
			public IEnumerator<Item> iterator()
			{
				return new IteratorAnonymousInnerClass(this);
			}

			private class IteratorAnonymousInnerClass : IEnumerator<Item>
			{
				private readonly SingleResultSequence outerInstance;

				public IteratorAnonymousInnerClass(SingleResultSequence outerInstance)
				{
					this.outerInstance = outerInstance;
					seenIt = false;
				}

				internal bool seenIt;

                object IEnumerator.Current
                {
                    get
                    {
                        seenIt = true;
                        return outerInstance.value_Renamed;
                    }
                }

				public void remove()
				{
					throw new System.NotSupportedException("ResultSequences are immutable");
				}

				public object next()
				{
					if (!seenIt)
					{
						seenIt = true;
						return outerInstance.value_Renamed;
					}
					throw new System.InvalidOperationException("This iterator is at its end");
				}

				public bool hasNext()
				{
					return !seenIt;
				}

                bool IEnumerator.MoveNext()
                {
                    return !seenIt;
                }

                void IEnumerator.Reset()
                {
                    throw new System.NotImplementedException();
                }

                public Item Current
                {
                    get
                    {
                        seenIt = true;
                        return outerInstance.value_Renamed;
                    }
                }

                public void Dispose()
                {
                }
            }

			public ItemType itemType(int index)
			{
				return item(index).ItemType;
			}

			public ItemType sequenceType()
			{
				return value_Renamed.ItemType;
			}

            IEnumerator<Item> ResultSequence.iterator()
            {
                return GetEnumerator();
            }

            public IEnumerator<Item> GetEnumerator()
            {
                return iterator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

		public sealed class ArrayResultSequence : ResultSequence
		{

			internal readonly Item[] results;

			public ArrayResultSequence(Item[] results)
			{
				this.results = results;
			}

			/* (non-Javadoc)
			 * @see org.eclipse.wst.xml.xpath2.api.ResultSequence#getLength()
			 */
			public int size()
			{
				return results.Length;
			}

			/* (non-Javadoc)
			 * @see org.eclipse.wst.xml.xpath2.api.ResultSequence#getItem(int)
			 */
			public Item item(int index)
			{
				if (index < 0 && index >= results.Length)
				{
					throw new System.IndexOutOfRangeException("Index " + index + " is out of alllowed bounds (less that " + results.Length);
				}
				return results[index];
			}

			/* (non-Javadoc)
			 * @see org.eclipse.wst.xml.xpath2.api.ResultSequence#empty()
			 */
			public bool empty()
			{
				return false;
			}

			/* (non-Javadoc)
			 * @see org.eclipse.wst.xml.xpath2.api.ResultSequence#first()
			 */
			public object firstValue()
			{
				return item(0).NativeValue;
			}

			/* (non-Javadoc)
			 * @see org.eclipse.wst.xml.xpath2.api.ResultSequence#first()
			 */
			public Item first()
			{
				return item(0);
			}

			/* (non-Javadoc)
			 * @see org.eclipse.wst.xml.xpath2.api.ResultSequence#iterator()
			 */
			public IEnumerator<Item> iterator()
            {
                return results.ToList().GetEnumerator();
            }


			public ItemType itemType(int index)
			{
				if (index < 0 && index >= results.Length)
				{
					throw new System.IndexOutOfRangeException("Index " + index + " is out of alllowed bounds (less that " + results.Length);
				}
				return results[index].ItemType;
			}

			public ItemType sequenceType()
			{
				return new SimpleAtomicItemTypeImpl(BuiltinTypeLibrary.XS_ANYTYPE, org.eclipse.wst.xml.xpath2.api.typesystem.ItemType_Fields.OCCURRENCE_ONE_OR_MANY);
			}

			public object value(int index)
			{
				return item(index).NativeValue;
			}

            IEnumerator<Item> ResultSequence.iterator()
            {
                return results.ToList().GetEnumerator();
            }

            public IEnumerator<Item> GetEnumerator()
            {
                return results.ToList().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

		public virtual int size()
		{
			return values.Count;
		}

		public virtual IEnumerator iterator()
		{
			return values.GetEnumerator();
		}

		public virtual void prepend(ResultSequence rs)
		{
			values.AddRange(collectionWrapper(rs));
		}

		private ICollection<Item> collectionWrapper(ResultSequence rs)
		{
			// This is a dummy collections, solely exists for faster inserts into our array
			return new CollectionAnonymousInnerClass(this, rs);
		}

		private class CollectionAnonymousInnerClass : ICollection<Item>
		{
			private readonly ResultBuffer outerInstance;

			private org.eclipse.wst.xml.xpath2.api.ResultSequence rs;

			public CollectionAnonymousInnerClass(ResultBuffer outerInstance, org.eclipse.wst.xml.xpath2.api.ResultSequence rs)
			{
				this.outerInstance = outerInstance;
				this.rs = rs;
			}


			public virtual bool add(Item arg0)
			{
				return false;
			}

			public virtual bool addAll(ICollection arg0)
			{
				return false;
			}

			public virtual void clear()
			{
			}

			public virtual bool contains(object arg0)
			{
				return false;
			}

			public virtual bool containsAll(ICollection arg0)
			{
				return false;
			}

			public virtual bool Empty
			{
				get
				{
					return rs.empty();
				}
			}

            public int Count
            {
                get { return rs.size(); }
            }

            public bool IsReadOnly => throw new System.NotImplementedException();

            public virtual IEnumerator iterator()
			{
				return rs.iterator();
			}

			public virtual bool remove(object arg0)
			{
				return false;
			}

			public virtual bool removeAll(ICollection arg0)
			{
				return false;
			}

			public virtual bool retainAll(ICollection arg0)
			{
				return false;
			}

			public virtual int size()
			{
				return rs.size();
			}

			public virtual object[] toArray()
			{
				return toArray(new Item[outerInstance.size()]);
			}

			public virtual object[] toArray(object[] arg0)
			{
				if (arg0.Length < outerInstance.size())
				{
					arg0 = new Item[outerInstance.size()];
				}
				for (int i = 0; i < outerInstance.size(); ++i)
				{
					arg0[i] = rs.item(i);
				}
				return arg0;
			}

            public void Add(Item item)
            {
                throw new System.NotImplementedException();
            }

            public void Clear()
            {
                throw new System.NotImplementedException();
            }

            public bool Contains(Item item)
            {
                throw new System.NotImplementedException();
            }

            public void CopyTo(Item[] array, int arrayIndex)
            {
                throw new System.NotImplementedException();
            }

            public bool Remove(Item item)
            {
                throw new System.NotImplementedException();
            }

            public IEnumerator<Item> GetEnumerator()
            {
                throw new System.NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new System.NotImplementedException();
            }
        }

		public static readonly ResultSequence EMPTY = new ResultSequenceAnonymousInnerClass();

		private class ResultSequenceAnonymousInnerClass : ResultSequence
		{
			public ResultSequenceAnonymousInnerClass()
			{
			}


			public virtual int size()
			{
				return 0;
			}

			public virtual Item item(int index)
			{
				throw new System.IndexOutOfRangeException("Sequence is empty!");
			}

			public virtual bool empty()
			{
				return true;
			}

			public virtual ItemType itemType(int index)
			{
				throw new System.IndexOutOfRangeException("Sequence is empty!");
			}

			public virtual ItemType sequenceType()
			{
				return new SimpleAtomicItemTypeImpl(BuiltinTypeLibrary.XS_ANYTYPE, org.eclipse.wst.xml.xpath2.api.typesystem.ItemType_Fields.OCCURRENCE_ONE_OR_MANY);
			}

			public virtual object value(int index)
			{
				throw new System.IndexOutOfRangeException("Sequence is empty!");
			}

			public virtual object firstValue()
			{
				throw new System.IndexOutOfRangeException("Sequence is empty!");
			}

			public virtual Item first()
			{
				throw new System.IndexOutOfRangeException("Sequence is empty!");
			}

			public virtual IEnumerator iterator()
			{
				return new IteratorAnonymousInnerClass(this);
			}

            IEnumerator<Item> ResultSequence.iterator()
            {
                return new List<Item>().GetEnumerator();
            }

            private class IteratorAnonymousInnerClass : IEnumerator
			{
				private readonly ResultSequenceAnonymousInnerClass outerInstance;

                public object Current => throw new System.NotImplementedException();

                public IteratorAnonymousInnerClass(ResultSequenceAnonymousInnerClass outerInstance)
				{
					this.outerInstance = outerInstance;
				}


				public virtual void remove()
				{
					throw new System.NotSupportedException("ResultSequences are immutable");
				}

				public virtual object next()
				{
					throw new System.InvalidOperationException("This ResultSequence is empty");
				}

				public virtual bool hasNext()
				{
					return false;
				}

                public bool MoveNext()
                {
                    throw new System.NotImplementedException();
                }

                public void Reset()
                {
                    throw new System.NotImplementedException();
                }
            }

            public IEnumerator<Item> GetEnumerator()
            {
                return new List<Item>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

		public virtual ICollection<Item> Collection
		{
			get
			{
				return this.values;
			}
		}

		public virtual ResultBuffer concat(ICollection<Item> others)
		{
			this.values.AddRange(others);
			return this;
		}

		public static ResultSequence wrap(Item item)
		{
			if (item is SingleItemSequence)
			{
				return (SingleItemSequence)item;
			}

			return new SingleResultSequence(item);
		}

		public virtual Item item(int index)
		{
			return values[index];
		}

		public virtual void addAt(int pos, Item element)
		{
			values.Insert(pos, element);
		}
	}

}