using System.Collections.Generic;
using System.Linq;
using Core.ObjectPool;
using Sharpen;

namespace com.graphbuilder.struc
{
	public class Stack : BaseRefCounter
	{
		public class ObjcetFactory :CustomAbstractObjectFactory
		{
			public ObjcetFactory() : base(typeof(Stack)){}
			public override object MakeObject()
			{
				return new Stack();
			}

		}
		public static Stack Allocate()
		{
			return ObjectAllocatorHolder<Stack>.Allocate();
		}

		private Stack()
		{
		}

		private List<object> _stack = new List<object>();
		protected override void OnCleanUp()
		{
			_stack.Clear();
			ObjectAllocatorHolder<Stack>.Free(this);
		}

		
		public void Clear()
		{
			_stack.Clear();
		}

		public object Peek()
		{
			return _stack[0];
		}

		public object Pop()
		{
			if (_stack.Count > 0)
			{
				var r = _stack[0];
				_stack.RemoveAt(0);
				return r;
			}

			return 0;

		}

		public void Push(object item)
		{
			_stack.AddFirst(item);
		}

		public int Count
		{
			get { return _stack.Count; }
		}

		public int Size()
		{
			return _stack.Count;
		}
		public bool IsEmpty()
		{
			return _stack.Count == 0;
		}

		public void AddToTail(object item)
		{
			_stack.Add(item);
		}

		public object RemoveTail()
		{
			var count = _stack.Count;
			if (count > 0)
			{
				var r = _stack[count - 1];
				_stack.RemoveAt(count - 1);
				return r;
			}

			return null;
		}
	}
}
