using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.util;
using Core.EntityComponent;
using gameplay.gamerule.free.ui;
using UnityEngine;
using com.wd.free.map.position;
using com.wd.free.unit;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [Serializable]
    public class CreateFixEntityAction : AbstractGameAction
    {
        public string x;
        public string y;
        public string z;

        public string key;
        public string cat;
        public string value;

        public string lifeTime;

        public int distance;

        public IPosSelector pos;

        public override void DoAction(IEventArgs args)
        {
            FreeMoveEntity en = args.GameContext.freeMove.CreateEntity();
            en.AddEntityKey(new EntityKey(args.GameContext.session.commonSession.EntityIdGenerator.GetNextEntityId(), (short)EEntityType.FreeMove));
            if(pos == null)
            {
                en.AddPosition(new Vector3(FreeUtil.ReplaceFloat(x, args), FreeUtil.ReplaceFloat(y, args), FreeUtil.ReplaceFloat(z, args)));
            }
            else
            {
                UnitPosition up = pos.Select(args);
                en.AddPosition(new Vector3(up.GetX(), up.GetY(), up.GetZ()));
            }
            
            string realCat = FreeUtil.ReplaceVar(cat, args);
            if (realCat == null)
            {
                realCat = "";
            }
            string realValue = FreeUtil.ReplaceVar(value, args);
            if (realValue == null)
            {
                realValue = "";
            }
            en.AddFreeData(FreeUtil.ReplaceVar(key, args), null);
            en.freeData.Cat = realCat;
            en.freeData.Value = realValue;
            en.isFlagSyncNonSelf = true;

            en.AddFlagImmutability(args.GameContext.session.currentTimeObject.CurrentTime);

            if(distance > 0)
            {
                en.AddPositionFilter(Core.Components.PositionFilterType.Filter2D, distance);
            }

            if (!string.IsNullOrEmpty(lifeTime))
            {
                en.AddLifeTime(DateTime.Now, FreeUtil.ReplaceInt(lifeTime, args));
            }
        }
    }
}
