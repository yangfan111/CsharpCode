using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.skill;
using com.wd.free.util;
using Core.Free;
using App.Server.GameModules.GamePlay.Free.entity;

namespace App.Server.GameModules.GamePlay
{
    public class FreeRuleEventArgs : BaseEventArgs, ISkillArgs
    {
        public IFreeRule rule;

        public SimplePlayerInput input = new SimplePlayerInput();

        public FreeRuleEventArgs(Contexts contexts)
        {
            this._gameContext = contexts;
        }

        public override IFreeRule Rule
        {
            get { return rule; }
            set
            {
                rule = value;
            }
        }

        public IPlayerInput GetInput()
        {
            return input;
        }

        public PlayerEntity GetPlayer(string player)
        {
            IParable unit = GetUnit(FreeUtil.ReplaceVar(player, this));
            if (unit is FreeData)
            {
                return ((FreeData)unit).Player;
            }

            return null;
        }

        public object GetEntity(string entity)
        {
            IParable unit = GetUnit(FreeUtil.ReplaceVar(entity, this));
            if (unit is FreeData)
            {
                return ((FreeData)unit).Player;
            }

            if (unit is FreeEntityData)
            {
                return ((FreeEntityData)unit).FreeMoveEntity;
            }

            foreach (FreeMoveEntity freeMoveEntity in GameContext.freeMove.GetEntities())
            {
                if (freeMoveEntity.freeData.Key == entity)
                {
                    return freeMoveEntity;
                }
            }

            return null;
        }
    }
}
