using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.ai;
using Core.Free;
using Free.framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Shared.FreeFramework.framework.ai
{
    public class PlayerInterceptCommands
    {

        public static void AttackPlayer(PlayerEntity player, int targetId)
        {
            SimpleProto proto = FreePool.Allocate();
            proto.Key = FreeMessageConstant.PlayerCmd;
            proto.Ks.Add((int)PlayerActEnum.CmdType.Attack);

            proto.Ks.Add(player.entityKey.Value.EntityId);
            proto.Ks.Add(targetId);

            FreeMessageSender.SendMessage(player, proto);
        }

        public static void ClearKeys(PlayerEntity player)
        {
            SimpleProto proto = FreePool.Allocate();
            proto.Key = FreeMessageConstant.PlayerCmd;
            proto.Ks.Add((int)PlayerActEnum.CmdType.ClearKeys);
            proto.Ks.Add(player.entityKey.Value.EntityId);

            FreeMessageSender.SendMessage(player, proto);
        }

        public static void InterceptKeys(PlayerEntity player, int key, int time)
        {
            SimpleProto proto = FreePool.Allocate();
            proto.Key = FreeMessageConstant.PlayerCmd;
            proto.Ks.Add((int)PlayerActEnum.CmdType.InterceptKey);
            proto.Ks.Add(player.entityKey.Value.EntityId);

            proto.Ins.Add(time);
            proto.Ins.Add(key);

            FreeMessageSender.SendMessage(player, proto);
        }

        public static void PressKeys(PlayerEntity player, int key, int time)
        {
            SimpleProto proto = FreePool.Allocate();
            proto.Key = FreeMessageConstant.PlayerCmd;
            proto.Ks.Add((int)PlayerActEnum.CmdType.PressKey);
            proto.Ks.Add(player.entityKey.Value.EntityId);

            proto.Ins.Add(time);
            proto.Ins.Add(key);

            FreeMessageSender.SendMessage(player, proto);
        }

        public static void DoNothing(PlayerEntity player)
        {
            SimpleProto proto = FreePool.Allocate();
            proto.Key = FreeMessageConstant.PlayerCmd;
            proto.Ks.Add((int)PlayerActEnum.CmdType.Idle);
            proto.Ks.Add(player.entityKey.Value.EntityId);

            FreeMessageSender.SendMessage(player, proto);
        }

        public static void WalkTo(PlayerEntity player, Vector3 v)
        {
            SimpleProto proto = FreePool.Allocate();
            proto.Key = FreeMessageConstant.PlayerCmd;
            proto.Ks.Add((int)PlayerActEnum.CmdType.Walk);
            proto.Ks.Add(player.entityKey.Value.EntityId);

            proto.Fs.Add(v.x);
            proto.Fs.Add(v.y);
            proto.Fs.Add(v.z);

            FreeMessageSender.SendMessage(player, proto);
        }
    }
}
