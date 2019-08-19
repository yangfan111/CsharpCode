using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserInputManager.Lib
{
    public class KeyDataCollection : AbsKeyDataCollection<KeyReceiver, KeyData>{}
   
    public class PointDataCollection:AbsKeyDataCollection<PointerReceiver,PointerData>{}
  //  public delegate void KeyBindhandlerChanged(Ihandler t, bool isAdd);
    public abstract class AbsKeyDataCollection<TKeyReceiver,TData> : IEnumerable<TKeyReceiver> where TKeyReceiver :IKeyReceiver<TData> where TData:KeyData

    {
        private LinkedList<TKeyReceiver> keyGroups = new LinkedList<TKeyReceiver>();
        private int blockLayer = -1;
        private bool isDirty = true;
        private Dictionary<KeyData,List<KeyPointAction>> dispatchDict = new Dictionary<KeyData, List<KeyPointAction>>(KeyDataComparer.Instance);
        public  void Dispatch(KeyData keyData)
        {
            Rebuild();
            List<KeyPointAction> actions;
            if (dispatchDict.TryGetValue(keyData, out actions))
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    DoDispatch(actions[i],keyData);
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
            foreach (TKeyReceiver keyData in this)
            {
                var bindings = keyData.GetBindingActions();
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
        

        public void AddOne(TKeyReceiver handler)
        {
            Console.WriteLine("add handler " + handler);
            if (null == handler)
            {
                return;
            }
            if (keyGroups.Contains(handler))
            {
                return;
            }
            var newLayer = handler.GetLayer();
            var node = keyGroups.First;
            var added = false;
            for (; null != node; node = node.Next)
            {
                var layer = node.Value.GetLayer();
                if (newLayer >= layer)
                {
                    continue;
                }

                keyGroups.AddBefore(node, handler);
                added = true;
                break;
            }

            if (!added)
            {
                keyGroups.AddLast(handler);
            }

            isDirty = true;
        }

        public void Remove(TKeyReceiver handler)
        {
            Console.WriteLine("remove handler " + handler);
            if (null == handler)
            {
                return;
            }

            if (keyGroups.Remove(handler))
            {
                isDirty = true;
            }

            
        }

        public IEnumerator<TKeyReceiver> GetEnumerator()
        {
            blockLayer = -1;
            var node = keyGroups.First;
            for (; null != node; node = node.Next)
            {
                int layer = node.Value.GetLayer();
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
