using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserInputManager.Lib
{
    public class KeyDataCollection : KeyHandlerDataCollection<KeyHandler, KeyData>{}
   
    public class PointDataCollection:KeyHandlerDataCollection<PointerKeyHandler,PointerData>{}
  //  public delegate void KeyBindhandlerChanged(Ihandler t, bool isAdd);
    public abstract class KeyHandlerDataCollection<TKeyHandler,TData> : IEnumerable<TKeyHandler> where TKeyHandler :IKeyHandler<TData> where TData:KeyData

    {
        private LinkedList<TKeyHandler> keyHandlers = new LinkedList<TKeyHandler>();
        private int blockLayer = -1;
        private bool isDirty;
        private Dictionary<KeyData,List<KeyPointAction>> dispatchDict = new Dictionary<KeyData, List<KeyPointAction>>();
        public virtual void Dispatch(KeyData keyData)
        {
            Rebuild();
            List<KeyPointAction> actions;
            if (dispatchDict.TryGetValue(keyData, out actions))
            {
                foreach (var action in actions)
                {
                    DoDispatch(action, keyData);
                }
            }
        }

        protected virtual void DoDispatch(KeyPointAction action, KeyData data)
        {
            if (action != null)
                action(data);
        }
     
        private void Rebuild()
        {
            if (!isDirty) return;
            foreach (var keyValuePair in dispatchDict)
            {
                keyValuePair.Value.Clear();
            }
            foreach (TKeyHandler keyData in this)
            {
                var bindings = keyData.GetBindingDict();
                foreach (KeyValuePair<TData, KeyPointAction> binding in bindings)
                {
                    List<KeyPointAction> keyActions;
                    if (!dispatchDict.TryGetValue(binding.Key,out keyActions))
                    {
                        keyActions = new List<KeyPointAction>();
                        dispatchDict.Add(binding.Key,keyActions);
                    }
                    keyActions.Add(binding.Value);
                }
            }
            isDirty = false;
        }
        

        public void AddOne(TKeyHandler handler)
        {
            Console.WriteLine("add handler " + handler);
            if (null == handler)
            {
                return;
            }
            if (keyHandlers.Contains(handler))
            {
                return;
            }
            var newLayer = handler.GetLayer();
            var node = keyHandlers.First;
            var added = false;
            for (; null != node; node = node.Next)
            {
                var layer = node.Value.GetLayer();
                if (newLayer >= layer)
                {
                    continue;
                }

                keyHandlers.AddBefore(node, handler);
                added = true;
                break;
            }

            if (!added)
            {
                keyHandlers.AddLast(handler);
            }

            isDirty = true;
        }

        public void Remove(TKeyHandler handler)
        {
            Console.WriteLine("remove handler " + handler);
            if (null == handler)
            {
                return;
            }

            if (keyHandlers.Remove(handler))
            {
                isDirty = false;
            }

            
        }

        public IEnumerator<TKeyHandler> GetEnumerator()
        {
            blockLayer = -1;
            var node = keyHandlers.First;
            for (; null != node; node = node.Next)
            {
                var layer = node.Value.GetLayer();
                if (blockLayer > -1 && layer > blockLayer)
                {
                    yield break;
                }

                if (node.Value.GetBlockType() == BlockType.All)
                {
                    blockLayer = node.Value.GetLayer();
                }

                yield return node.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
