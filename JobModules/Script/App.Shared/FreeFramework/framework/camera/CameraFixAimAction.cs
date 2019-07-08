using System;
using App.Shared.Components.FreeMove;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.unit;
using Core.Components;
using Core.Configuration.Utils;
using Core.Free;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.FreeFramework.framework.camera
{
    [Serializable]
    public class CameraFixAim:AbstractPlayerAction, IRule
    {
        private string FreeMove;
        private int Type;
        private IPosSelector targetPos;
        
        public override void DoAction(IEventArgs args)
        {
            foreach (FreeMoveEntity freeMoveEntity in args.GameContext.freeMove.GetEntities())
            {
                if (freeMoveEntity.freeData.Key == FreeMove)
                {
                    UnitPosition aimPos = targetPos.Select(args);
                    if (!freeMoveEntity.hasFreeMoveController)
                    {
                        freeMoveEntity.AddFreeMoveController();
                    }
                    freeMoveEntity.freeMoveController.FocusOnPosition =
                        new Vector3(aimPos.GetX(), aimPos.GetY(), aimPos.GetZ()).ShiftedToFixedVector3();
                    freeMoveEntity.freeMoveController.ControllType = (byte)EFreeMoveControllType.FixFocusPos;
                }
            }
        }

        public int GetRuleID()
        {
            return (int) ERuleIds.CameraFixAimAction;
        }
    }
}