using System;
using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.exception;
using com.wd.free.para;
using com.wd.free.para.exp;
using com.wd.free.util;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Player;

namespace gameplay.gamerule.free.player
{
    [System.Serializable]
    public class PlayerHonorAction : AbstractGameAction
    {
        private const long serialVersionUID = 7354887359200319872L;

        private int max;

        private IList<PlayerHonorAction.PlayerHonor> honors;

        public override void DoAction(IEventArgs args)
        {
            if (honors != null)
            {
                IList<FreeData> fds = new List<FreeData>();
                Contexts contexts = (Contexts)args.GameContext;

                foreach (PlayerEntity player in contexts.player.GetEntities())
                {
                    fds.Add((FreeData)player.freeData.FreeData);
                }

                foreach (PlayerHonorAction.PlayerHonor ph in honors)
                {
                    ph.SetHonor(args, Sharpen.Collections.ToArray(fds, new FreeData[0]));
                }
                IDictionary<int, IList<PlayerHonorAction.PlayerHonor>> map = new Dictionary<int, IList<PlayerHonorAction.PlayerHonor>>();
                foreach (PlayerHonorAction.PlayerHonor ph_1 in honors)
                {
                    foreach (int id in ph_1.GetIds())
                    {
                        if (!map.ContainsKey(id))
                        {
                            map[id] = new List<PlayerHonorAction.PlayerHonor>();
                        }
                        map[id].Add(ph_1);
                    }
                }
                foreach (PlayerEntity player in contexts.player.GetInitializedPlayerEntities())
                {
                    int id_1 = (int)player.playerInfo.PlayerId;
                    FreeData fd = (FreeData)player.freeData.FreeData;
                    Reset(fd);
                    fd.GetParameters().AddPara(new IntPara("honorCount", 0));
                    if (map.ContainsKey(id_1))
                    {
                        IList<string> list1 = new List<string>();
                        IList<string> list2 = new List<string>();
                        IList<string> list3 = new List<string>();
                        PlayerHonorAction.PlayerHonor[] phs = Optimize(map[id_1]);
                        for (int i = 0; i < MyMath.Min(max, phs.Length); i++)
                        {
                            fd.GetParameters().AddPara(new BoolPara("is" + StringUtil.GetPascalString(phs[i].name), true));
                            list1.Add(phs[i].img1);
                            list2.Add(phs[i].img2);
                            list3.Add(phs[i].img3);
                        }
                        fd.GetParameters().AddPara(new IntPara("honorCount", list1.Count));
                        AddHornorPara(fd, list1, "honors1");
                        AddHornorPara(fd, list2, "honors2");
                        AddHornorPara(fd, list3, "honors3");
                    }
                }
            }

        }
        private void AddHornorPara(FreeData fd, IList<string> list, string name)
        {
            fd.GetParameters().AddPara(new StringPara(name, StringUtil.GetStringFromStrings(list, StringMultiAction.FIELD_SPLITER)));
        }

        private void Reset(FreeData fd)
        {
            foreach (PlayerHonorAction.PlayerHonor ph in honors)
            {
                fd.GetParameters().AddPara(new BoolPara("is" + StringUtil.GetPascalString(ph.name), false));
            }
        }

        private PlayerHonorAction.PlayerHonor[] Optimize(IList<PlayerHonorAction.PlayerHonor> honors)
        {
            IDictionary<int, PlayerHonorAction.PlayerHonor> map = new Dictionary<int, PlayerHonorAction.PlayerHonor>();
            foreach (PlayerHonorAction.PlayerHonor ph in honors)
            {
                map[ph.priority] = ph;
            }
            IList<PlayerHonorAction.PlayerHonor> r = new List<PlayerHonorAction.PlayerHonor>();
            Sharpen.Collections.AddAll(r, map.Values);
            r.Sort();
            return Sharpen.Collections.ToArray(r, new PlayerHonorAction.PlayerHonor[0]);
        }

        [System.Serializable]
        public class PlayerHonor : IComparable<PlayerHonorAction.PlayerHonor>
        {
            private const long serialVersionUID = -3681162991562941760L;

            public string name;

            public string formula;

            public string condition;

            public bool desc;

            public string order;

            public int priority;

            public string img1;

            public string img2;

            public string img3;

            [System.NonSerialized]
            private ICollection<int> ids;

            private IParaCondition con;

            public PlayerHonor()
                : base()
            {
            }

            public virtual int[] GetIds()
            {
                return Sharpen.Collections.ToArray(ids, new int[0]);
            }

            public virtual void SetHonor(IEventArgs args, FreeData[] fds)
            {
                if (ids == null)
                {
                    ids = new HashSet<int>();
                }
                if (con == null || (condition != null && condition.Contains(FreeUtil.VAR_START) && condition.Contains(FreeUtil.VAR_END)))
                {
                    if (!StringUtil.IsNullOrEmpty(condition))
                    {
                        con = new ExpParaCondition(condition);
                    }
                }
                ids.Clear();
                IList<PlayerHonorAction.PlayerValue> pvs = new List<PlayerHonorAction.PlayerValue>();
                foreach (FreeData fd in fds)
                {
                    args.TempUse("player", fd);
                    try
                    {
                        if (con == null || con.Meet(args))
                        {
                            PlayerHonorAction.PlayerValue pv = new PlayerHonorAction.PlayerValue(fd, FreeUtil.ReplaceDouble(formula, args), desc);
                            pvs.Add(pv);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new GameConfigExpception(e.Message);
                    }
                    finally
                    {
                        args.Resume("player");
                    }
                }
                pvs.Sort();
                int order = GetOrder(args);
                if (order > 0)
                {
                    if (order <= pvs.Count)
                    {
                        ids.Add((int)pvs[order - 1].data.Player.playerInfo.PlayerId);
                    }
                }
                else
                {
                    foreach (PlayerHonorAction.PlayerValue pv in pvs)
                    {
                        ids.Add((int)pv.data.Player.playerInfo.PlayerId);
                    }
                }
            }

            private int GetOrder(IEventArgs args)
            {
                return FreeUtil.ReplaceInt(order, args);
            }

            public virtual int GetPriority()
            {
                return priority;
            }

            public virtual string GetName()
            {
                return name;
            }

            public virtual int CompareTo(PlayerHonorAction.PlayerHonor arg0)
            {
                return this.priority - arg0.priority;
            }
        }

        internal class PlayerValue : IComparable<PlayerHonorAction.PlayerValue>
        {
            public FreeData data;

            public double value;

            public bool desc;

            public PlayerValue()
                : base()
            {
            }

            public PlayerValue(FreeData data, double value, bool desc)
                : base()
            {
                this.data = data;
                this.value = value;
                this.desc = desc;
            }

            public virtual FreeData GetData()
            {
                return data;
            }

            public virtual double GetValue()
            {
                return value;
            }

            public virtual int CompareTo(PlayerHonorAction.PlayerValue o)
            {
                if (value > o.value)
                {
                    if (desc)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    if (value == o.value)
                    {
                        return 0;
                    }
                    else
                    {
                        if (!desc)
                        {
                            return -1;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
            }
        }
    }
}
