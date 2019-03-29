using com.wd.free.item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.skill;
using com.wd.free.action;
using Sharpen;
using com.wd.free.para;
using gameplay.gamerule.free.ui;
using gameplay.gamerule.free.ui.component;

namespace App.Server.GameModules.GamePlay.Free.item
{
    [Serializable]
    public class UnityEmptyInventoryUi : IInventoryUI
    {
        private IGameAction errorAction;

        private IGameAction moveOutAction;

        private IGameAction canNotMoveAction;

        private IGameAction moveAction;

        [System.NonSerialized]
        private long lastErrorTime;

        public IGameAction MoveAction
        {
            get { return moveAction; }
        }

        public IGameAction CanNotMoveAction
        {
            get { return canNotMoveAction; }
        }

        public IGameAction MoveOutAction
        {
            get { return MoveOutAction; }
        }

        public IGameAction ErrorAction
        {
            get { return errorAction; }
        }

        public void AddItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip)
        {
            
        }

        public void DeleteItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip)
        {
            
        }

        public void Error(ISkillArgs args, ItemInventory inventory, string msg)
        {
            if (errorAction != null)
            {
                if (Runtime.CurrentTimeMillis() - lastErrorTime > 2000)
                {
                    if (args != null)
                    {
                        args.GetDefault().GetParameters().TempUse(new StringPara("message", msg));
                        errorAction.Act(args);
                        args.GetDefault().GetParameters().Resume("message");
                    }
                    lastErrorTime = Runtime.CurrentTimeMillis();
                }
            }
        }

        public void ReDraw(ISkillArgs args, ItemInventory inventory, bool includeBack)
        {
            
        }

        public void UpdateItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip)
        {
        }

        public void UseItem(ISkillArgs args, ItemInventory inventory, ItemPosition ip)
        {
        }
    }
}
