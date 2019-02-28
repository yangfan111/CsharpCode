using gameplay.gamerule.free.ui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Core.Free;
using com.wd.free.map.position;
using com.wd.free.util;
using com.wd.free.unit;
using UnityEngine;
using App.Server.GameModules.GamePlay.free.player;
using Free.framework;
using App.Server.GameModules.GamePlay;

namespace App.Shared.FreeFramework.Free.Map
{
    [Serializable]
    public class PlaySoundAction : SendMessageAction
    {
        public string stop;
        public string loop;
        public string key;
        public string id;
        public string entity;
        public IPosSelector pos;

        protected override void BuildMessage(IEventArgs args)
        {
            builder.Key = FreeMessageConstant.PlaySound;

            builder.Bs.Add(FreeUtil.ReplaceBool(stop, args));
            builder.Bs.Add(FreeUtil.ReplaceBool(loop, args));
            builder.Bs.Add(pos != null);

            builder.Ins.Add(FreeUtil.ReplaceInt(id, args));

            if (!string.IsNullOrEmpty(entity))
            {
                foreach (FreeMoveEntity freeMoveEntity in args.GameContext.freeMove.GetEntities())
                {
                    if (freeMoveEntity.freeData.Key == entity)
                    {
                        builder.Ins.Add(freeMoveEntity.entityKey.Value.EntityId);
                    }
                }
            }

            if(key != null)
            {
                builder.Ss.Add(FreeUtil.ReplaceVar(key, args));
            }
            else
            {
                builder.Ss.Add("");
            }
            

            if (pos != null)
            {
                UnitPosition up = pos.Select(args);
                builder.Fs.Add(up.GetX());
                builder.Fs.Add(up.GetY());
                builder.Fs.Add(up.GetZ());
            }
        }
    }
}
