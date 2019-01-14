using System;
using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.para.exp;
using com.wd.free.trigger;
using App.Server.GameModules.GamePlay.Free.entity;
using com.wd.free.skill;
using com.wd.free.map;

namespace com.wd.free.action
{
    public class FreeLog
    {
        private static int index = 0;

        private static IList<ParaExp> paras;

        private static bool enable = false;

        private static LinkedHashMap<FreeLog.LogKey, FreeLog.LogKey> value;

        private static IList<FreeLog.LogKey> logs;

        private static int lastSeq;

        public static List<string> vars;

        public static List<string> errors;

        public static List<string> messages;

        public static List<string> funcs;

        public static List<string> ais;

        private static int aiCount;

        public static int messageCount;

        private static object trigger;

        static FreeLog()
        {
            value = new LinkedHashMap<FreeLog.LogKey, FreeLog.LogKey>();
            logs = new List<FreeLog.LogKey>();
            vars = new List<string>();
            errors = new List<string>();
            messages = new List<string>();
            paras = new List<ParaExp>();
            funcs = new List<string>();
            ais = new List<string>();
        }

        public static void SetParas(string para)
        {
            if (enable)
            {
                paras = new List<ParaExp>();
                if (!StringUtil.IsNullOrEmpty(para))
                {
                    string[] ss = StringUtil.Split(para, ",");
                    foreach (string s in ss)
                    {
                        ParaExp pe = new ParaExp(s);
                        paras.Add(pe);
                    }
                }
                vars.Clear();
                errors.Clear();
                messages.Clear();
                funcs.Clear();
                ais.Clear();
                messageCount = 0;
            }
        }

        public static void Message(string msg, IEventArgs args)
        {
            if (enable)
            {
                IList<string> list = new List<string>();
                foreach (ParaExp pe in paras)
                {
                    IPara pa = GetPara(args, pe);
                    if (pa != null)
                    {
                        list.Add(pa.GetName() + "=" + pa.GetValue());
                    }
                }
                messageCount++;
                messages.Add(messageCount + "  " + DateTime.Now.ToString("u") + " " + msg + "\n   观察值:" + StringUtil.GetStringFromStrings(list, ", "));
                if (messages.Count > 1000)
                {
                    messages.Remove(0);
                }
            }
        }

        public static void Print()
        {
            if (enable)
            {
                foreach (FreeLog.LogKey log in logs)
                {
                    vars.Add(DateTime.Now.ToString("u") + " " + log.ToLog());
                }
                if (vars.Count > 1000)
                {
                    for (int i = 0; i < MyMath.Min(100, logs.Count); i++)
                    {
                        vars.Remove(0);
                    }
                }
            }
        }

        private static string ToTrigger()
        {
            if (trigger is GameTrigger)
            {
                GameTrigger gt = (GameTrigger)trigger;
                return string.Format("触发器:{0} 触发类型:{1} 分类:{2} 规则:{3}", gt.GetName(), gt.GetKey(), gt.GetGroup(), gt.GetRule());
            }
            else if (trigger is string)
            {
                return (string)trigger;
            }
            else if (trigger is FreeEntityData)
            {
                FreeEntityData move = (FreeEntityData)trigger;
                return string.Format("FreeMove:{0}", move.name);
            }
            else if (trigger is ISkill)
            {
                ISkill skill = (ISkill)trigger;
                string key = skill.GetKey() != null ? skill.GetKey() : string.Empty;
                if (skill is AbstractCoolDownSkill)
                {
                    PlayerActionSkill cooldown = (PlayerActionSkill)skill;
                    string skillTrigger = cooldown.trigger != null ? cooldown.trigger.ToString() : string.Empty;
                    return string.Format("动作技能:{0}, 触发:{1}", cooldown.GetKey(), skillTrigger);
                }
                else
                {
                    return string.Format("技能:{0}", skill.GetKey());
                }
            }
            else if (trigger is FreeBuf)
            {
                FreeBuf buf = (FreeBuf)trigger;
                return string.Format("Buf:{0}", buf.GetKey());
            }

            return string.Empty;
        }

        public static void CallAiNode(string node)
        {
            if (enable)
            {
                if (ais.Count == 0)
                {
                    ais.Add(node);
                    aiCount = 1;
                }
                else
                {
                    string last = ais[ais.Count - 1];
                    if (last.Contains(node))
                    {
                        aiCount++;
                    }
                    else
                    {
                        ais.Set(ais.Count - 1, last + ", " + aiCount);
                        aiCount = 1;
                        ais.Add(node);
                    }
                }
                if (ais.Count > 10000)
                {
                    for (int i = 0; i < MyMath.Min(100, ais.Count); i++)
                    {
                        ais.Remove(0);
                    }
                }
            }
        }

        public static void CallFunc(string msg)
        {
            if (enable)
            {
                funcs.Add(DateTime.Now.ToString("u") + " " + msg);
                if (funcs.Count > 1000)
                {
                    funcs.Remove(0);
                }
            }
        }

        public static void FuncArg(string arg)
        {
            if (enable)
            {
                funcs.Add(arg);
                if (funcs.Count > 1000)
                {
                    funcs.Remove(0);
                }
            }
        }

        public static void Error(string msg, IGameAction action)
        {
            if (enable)
            {
                errors.Add(msg + "\n    at 动作 '" + (action == null ? string.Empty : action.ToString()) + "'\n    at " + ToTrigger());
                if (errors.Count > 1000)
                {
                    errors.Remove(0);
                }
            }
        }

        public static long Start(IGameAction action, IEventArgs args)
        {
            if (enable)
            {
                long s = Runtime.NanoTime();
                foreach (ParaExp pe in paras)
                {
                    IPara p = GetPara(args, pe);
                    if (p != null)
                    {
                        FreeLog.LogKey key = new FreeLog.LogKey(s, pe.ToString(), action.ToString());
                        key.index = index;
                        key.seq = value.Count + 1;
                        key.from = (IPara)p.Copy();
                        value[key] = key;
                    }
                }
                index++;
                return s;
            }
            return 0L;
        }

        private static IPara GetPara(IEventArgs args, ParaExp pe)
        {
            if (pe.GetUnit().Equals("robot"))
            {
                foreach (string key in args.GetUnitKeys())
                {
                    if (key.Equals("robot"))
                    {
                        IParable parable = args.GetUnit(key);
                        if (parable != null && parable.GetType().Name.Equals("FreeData"))
                        {
                            IPara para = parable.GetParameters().Get(pe.GetPara());
                            if (para != null)
                            {
                                return para;
                            }
                        }
                    }
                }
            }
            else
            {
                if (pe.GetUnit().Equals("player"))
                {
                    foreach (string key in args.GetUnitKeys())
                    {
                        if (!key.Equals("robot"))
                        {
                            IParable parable = args.GetUnit(key);
                            if (parable != null && parable.GetType().Name.Equals("FreeData"))
                            {
                                IPara para = parable.GetParameters().Get(pe.GetPara());
                                if (para != null)
                                {
                                    return para;
                                }
                            }
                        }
                    }
                }
                else
                {
                    IParable parable = args.GetUnit(pe.GetUnit());
                    if (parable != null)
                    {
                        return pe.GetSourcePara(args);
                    }
                }
            }
            return null;
        }

        public static void Stop(long s, IGameAction action, IEventArgs args)
        {
            if (enable)
            {
                foreach (ParaExp pe in paras)
                {
                    IPara p = GetPara(args, pe);
                    if (p != null)
                    {
                        FreeLog.LogKey key = new FreeLog.LogKey(s, pe.ToString(), action.ToString());
                        if (value.ContainsKey(key))
                        {
                            if (!value[key].GetFrom().Meet("==", p))
                            {
                                // 去除外层的改变action
                                FreeLog.LogKey v = value[key];
                                if (v.seq > lastSeq)
                                {
                                    v.to = (IPara)p.Copy();
                                    logs.Add(v);
                                    lastSeq = v.seq;
                                }
                                value[key].to = (IPara)p.Copy();
                            }
                        }
                    }
                }
                index--;
            }
        }

        public static GameTrigger GetTrigger()
        {
            if(trigger is GameTrigger)
            {
                return (GameTrigger)trigger;
            }

            return null;
        }

        public static void SetTrigger(object t)
        {
            trigger = t;
        }

        public static string ActionMark;

        public static void Reset()
        {
            if (enable)
            {
                index = 0;
                value.Clear();
                logs.Clear();
                lastSeq = 0;
            }
        }

        public static bool IsEnable()
        {
            return enable;
        }

        public static void Enable()
        {
            enable = true;
        }

        public static void Disable()
        {
            enable = false;
        }

        internal class LogKey
        {
            public long s;

            public string para;

            public string action;

            public int index;

            public int seq;

            public IPara from;

            public IPara to;

            public LogKey()
                : base()
            {
            }

            public LogKey(long s, string para, string action)
                : base()
            {
                this.s = s;
                this.para = para;
                this.action = action;
            }

            public virtual int GetSeq()
            {
                return seq;
            }

            public virtual void SetSeq(int seq)
            {
                this.seq = seq;
            }

            public virtual IPara GetFrom()
            {
                return from;
            }

            public virtual void SetFrom(IPara from)
            {
                this.from = from;
            }

            public virtual IPara GetTo()
            {
                return to;
            }

            public virtual void SetTo(IPara to)
            {
                this.to = to;
            }

            public virtual int GetIndex()
            {
                return index;
            }

            public virtual void SetIndex(int index)
            {
                this.index = index;
            }

            public virtual long GetS()
            {
                return s;
            }

            public virtual void SetS(long s)
            {
                this.s = s;
            }

            public virtual string GetPara()
            {
                return para;
            }

            public virtual void SetPara(string para)
            {
                this.para = para;
            }

            public virtual string GetAction()
            {
                return action;
            }

            public virtual void SetAction(string action)
            {
                this.action = action;
            }

            private IList<FreeLog.LogKey> GetParents()
            {
                IList<FreeLog.LogKey> list = new List<FreeLog.LogKey>();
                IList<FreeLog.LogKey> all = new List<FreeLog.LogKey>();
                foreach (FreeLog.LogKey lk in value.Values)
                {
                    all.Add(0, lk);
                }
                list.Add(this);
                int deep = index;
                for (int i = 0; i < all.Count; i++)
                {
                    if (all[i] == this)
                    {
                        for (int j = i + 1; j < all.Count; j++)
                        {
                            FreeLog.LogKey lk_1 = all[j];
                            if (lk_1.para.Equals(this.para) && lk_1.to != null)
                            {
                                if (lk_1.index == deep - 1)
                                {
                                    list.Add(0, lk_1);
                                    deep = lk_1.index;
                                }
                            }
                        }
                    }
                }
                return list;
            }

            public virtual string ToLog()
            {
                IList<string> list = new List<string>();
                list.Add(string.Format("\n    {0}", FreeLog.ToTrigger()));
                IList<FreeLog.LogKey> parents = GetParents();
                foreach (FreeLog.LogKey lk in parents)
                {
                    list.Add("\n    " + lk.action);
                }
                return from.GetName() + ":" + from.GetValue() + "->" + to.GetValue() + " at " + StringUtil.GetStringFromStrings(list, string.Empty);
            }

            public override int GetHashCode()
            {
                int prime = 31;
                int result = 1;
                result = prime * result + ((action == null) ? 0 : action.GetHashCode());
                result = prime * result + ((para == null) ? 0 : para.GetHashCode());
                result = prime * result + (int)(s ^ ((long)(((ulong)s) >> 32)));
                return result;
            }

            public override bool Equals(object obj)
            {
                if (this == obj)
                {
                    return true;
                }
                if (obj == null)
                {
                    return false;
                }
                if (GetType() != obj.GetType())
                {
                    return false;
                }
                FreeLog.LogKey other = (FreeLog.LogKey)obj;
                if (action == null)
                {
                    if (other.action != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if (!action.Equals(other.action))
                    {
                        return false;
                    }
                }
                if (para == null)
                {
                    if (other.para != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if (!para.Equals(other.para))
                    {
                        return false;
                    }
                }
                if (s != other.s)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
