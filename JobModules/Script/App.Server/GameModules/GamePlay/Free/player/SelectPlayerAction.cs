using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.data.sort;
using com.cpkf.yyjd.tools.util;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.para;
using com.wd.free.para.exp;
using com.wd.free.util;
using gameplay.gamerule.free.rule;
using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Player;

namespace gameplay.gamerule.free.player
{
    [System.Serializable]
    public class SelectPlayerAction : AbstractGameAction
    {
        private const long serialVersionUID = -5711184360474846389L;

        private string condition;

        [System.NonSerialized]
        private ExpParaCondition con;

        private IGameAction action;

        private int count;

        private string selectedName;

        private bool observer;

        private string order;

        [System.NonSerialized]
        private SelectMethod method;

        private IGameAction noneAction;

        //import gameplay.RoomSession;
        public virtual string GetCondition()
        {
            return condition;
        }

        public virtual void SetCondition(string condition)
        {
            this.condition = condition;
        }

        public virtual IGameAction GetAction()
        {
            return action;
        }

        public virtual void SetAction(IGameAction action)
        {
            this.action = action;
        }

        public virtual int GetCount()
        {
            return count;
        }

        public virtual void SetCount(int count)
        {
            this.count = count;
        }

        public virtual string GetSelectedName()
        {
            return selectedName;
        }

        public virtual void SetSelectedName(string selectedName)
        {
            this.selectedName = selectedName;
        }

        public override void DoAction(IEventArgs args)
        {
            Ini(args);

            List<FreeData> list = new List<FreeData>();

            foreach (PlayerEntity unit in args.GameContext.player.GetInitializedPlayerEntities())
            {
                if (unit.hasFreeData)
                {
                    FreeData fd = (FreeData)unit.freeData.FreeData;
                    args.TempUse(selectedName, fd);
                    if (con == null || con.Meet(args))
                    {
                        list.Add(fd);
                    }
                    args.Resume(selectedName);
                }
            }

            if (!StringUtil.IsNullOrEmpty(order))
            {
                DataBlock bl = new DataBlock();
                foreach (FreeData fd in list)
                {
                    bl.AddData(fd);
                }
                if (method == null || FreeUtil.IsVar(order))
                {
                    method = new SelectMethod(order);
                }
                list.Clear();
                foreach (IFeaturable fe in method.Select(bl).GetAllDatas())
                {
                    list.Add((FreeData)fe);
                }
            }
            if (list.Count > 0)
            {
                if (count > 0)
                {
                    int[] ids = RandomUtil.Random(0, list.Count - 1, count);
                    if (!StringUtil.IsNullOrEmpty(order))
                    {
                        ids = new int[(int)MyMath.Min(count, list.Count)];
                        for (int i = 0; i < ids.Length; i++)
                        {
                            ids[i] = i;
                        }
                    }
                    for (int i = 0; i < ids.Length; i++)
                    {
                        int id = ids[i];
                        FreeData unit = list[id];
                        args.TempUsePara(new IntPara("index", i + 1));
                        args.TempUsePara(new IntPara("count", ids.Length));
                        args.TempUse(selectedName, unit);
                        action.Act(args);
                        args.Resume(selectedName);
                        args.ResumePara("index");
                        args.ResumePara("count");
                    }
                }
                else
                {
                    int i = 1;
                    foreach (FreeData unit in list)
                    {
                        args.TempUsePara(new IntPara("index", i++));
                        args.TempUsePara(new IntPara("count", list.Count));
                        args.TempUse(selectedName, unit);
                        action.Act(args);
                        args.Resume(selectedName);
                        args.ResumePara("index");
                        args.ResumePara("count");
                    }
                }
            }
            else
            {
                if (noneAction != null)
                {
                    noneAction.Act(args);
                }
            }
        }

        private void Ini(IEventArgs args)
        {
            if (con == null || (condition != null && condition.Contains(FreeUtil.VAR_START) && condition.Contains(FreeUtil.VAR_END)))
            {
                if (!StringUtil.IsNullOrEmpty(condition))
                {
                    con = new ExpParaCondition(FreeUtil.ReplaceVar(condition, args));
                }
                if (StringUtil.IsNullOrEmpty(selectedName))
                {
                    selectedName = "current";
                }
            }
        }
    }
}
