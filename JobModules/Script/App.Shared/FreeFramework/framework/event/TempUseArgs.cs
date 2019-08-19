using com.wd.free.para;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.wd.free.@event
{
    public class TempUseArgs
    {
        public const int UseCount = 3;
        public const string buf = "buf";
        public const string creator = "creator";
        public const string DEFAULT = "default";

        // Stack
        private Stack<IParable> bufStack = new Stack<IParable>();
        private IParable bufParas;

        private Stack<IParable> creatorStack = new Stack<IParable>();
        private IParable creatorParas;

        private Stack<IParable> defaultStack = new Stack<IParable>();
        private IParable defaultParas;

        private BaseEventArgs baseEventArgs;

        public TempUseArgs(BaseEventArgs args)
        {
            baseEventArgs = args;
        }

        public bool TempUse(string key, IParable paras)
        {
            switch (key)
            {
                case buf:
                    bufStack.Push(baseEventArgs.GetUnit(key));
                    bufParas = paras;
                    return true;
                case creator:
                    creatorStack.Push(baseEventArgs.GetUnit(key));
                    creatorParas = paras;
                    return true;
            }
            return false;
        }

        public bool Resume(string key)
        {
            IParable old = null;
            switch (key)
            {
                case buf:
                    old = bufStack.Pop();
                    bufParas = old;
                    return true;
                case creator:
                    old = creatorStack.Pop();
                    creatorParas = old;
                    return true;
            }
            return false;
        }

        public bool GetUnit(string key, out IParable parable)
        {
            parable = null;
            switch (key)
            {
                case DEFAULT:
                    parable = defaultParas;
                    return true;
                case buf:
                    parable = bufParas;
                    return true;
                case creator:
                    parable = creatorParas;
                    return true;
            }
            return false;
        }

        public bool RemovePara(string key, out IParable parable)
        {
            parable = null;
            switch (key)
            {
                case DEFAULT:
                    defaultStack.Clear();
                    parable = defaultParas;
                    defaultParas = null;
                    return true;
                case buf:
                    bufStack.Clear();
                    parable = bufParas;
                    bufParas = null;
                    return true;
                case creator:
                    creatorStack.Clear();
                    parable = creatorParas;
                    creatorParas = null;
                    return true;
            }
            return false;
        }

        public void AddDefault(IParable paras)
        {
            defaultParas = paras;
        }

        public void TempUsePara(IPara para)
        {
            if (null == defaultParas) return;
            defaultParas.GetParameters().TempUse(para);
        }

        public void ResumePara(string paraName)
        {
            if (null == defaultParas) return;
            defaultParas.GetParameters().Resume(paraName);
        }
    }
}
