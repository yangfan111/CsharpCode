using System;
using App.Server.GameModules.GamePlay;
using App.Server.GameModules.GamePlay.free.player;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.item;
using com.wd.free.para;
using com.wd.free.skill;
using com.wd.free.unit;
using com.wd.free.util;
using gameplay.gamerule.free.rule;

namespace gameplay.gamerule.free.item
{
    [System.Serializable]
    public class CreateItemToPlayerAction : AbstractPlayerAction
    {
        private const long serialVersionUID = 6361325829890462542L;

        private const string INI_COUNT = "1";

        public string name;

        public string key;

        public string count;

        private IGameAction action;

        private IGameAction failAction;

        [System.NonSerialized]
        private IntPara addtimePara = new IntPara(ParaConstant.PARA_ITEM_ADD_TIME, 0);

        public override void DoAction(IEventArgs args)
        {
            if (addtimePara == null)
            {
                addtimePara = new IntPara(ParaConstant.PARA_ITEM_ADD_TIME, 0);
            }
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
            if (StringUtil.IsNullOrEmpty(count))
            {
                count = INI_COUNT;
            }
            FreeItem fi = FreeItemManager.GetItem(fr, FreeUtil.ReplaceVar(key, args), FreeUtil.ReplaceInt(count, args));
            if (StringUtil.IsNullOrEmpty(name))
            {
                name = InventoryManager.DEFAULT;
            }
            IGameUnit player = GetPlayer(args);
            if (player != null)
            {
                FreeData fd = (FreeData)player;
                args.TempUse(ParaConstant.PARA_PLAYER_CURRENT, fd);
                args.TempUse(ParaConstant.PARA_ITEM, fi);
                if (action != null)
                {
                    action.Act(args);
                }

                if (fd.freeInventory.GetInventoryManager().GetInventory(FreeUtil.ReplaceVar(name, args))
                    .AddItem((ISkillArgs)args, fi, true))
                {
                    //addtimePara.setValue(fr.room.getServerTime());
                    fi.GetParameters().AddPara(addtimePara);
                }
                else
                {
                    if (failAction != null)
                    {
                        fr.TempUse(ParaConstant.PARA_ITEM, fi);
                        failAction.Act(args);
                        fr.Resume(ParaConstant.PARA_ITEM);
                    }
                }

                fr.Resume(ParaConstant.PARA_PLAYER_CURRENT);
                fr.Resume(ParaConstant.PARA_ITEM);
            }
        }
    }
}
