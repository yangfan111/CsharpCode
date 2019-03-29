using System.Collections.Generic;
using App.Protobuf;
using Core.ObjectPool;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace App.Shared.Network
{
    public class UserCmdMessageConverter
    {
        public static ReusableList<IUserCmd> FromProtoBuf(UserCmdMessage message)
        {
            ReusableList<IUserCmd> list = ReusableList<IUserCmd>.Allocate();
            var count = message.UserCmdList.Count;
            for(int i = 0; i < count; i++)
            {
                var item = message.UserCmdList[i];
                UserCmd cmd = UserCmd.Allocate();
                cmd.RenderTime = item.RenderTime;
                cmd.FrameInterval = item.FrameInterval;
                cmd.MoveHorizontal = item.MoveHorizontal;
                cmd.MoveVertical = item.MoveVertical;
                cmd.MoveUpDown = item.MoveUpDown;
                cmd.Seq = item.Seq;
                cmd.DeltaPitch = item.DeltaPitch;
                cmd.DeltaYaw = item.DeltaYaw;
                cmd.Buttons = item.Buttons;
                cmd.BeState = item.BeState;                
                cmd.SwitchNumber = item.SwitchNumber;
                cmd.ChangedSeat = item.ChangedSeat;
                cmd.ChangeChannel = item.ChangeChannel;
                cmd.SnapshotId = item.SnapshotId;
                cmd.CurWeapon = item.CurWeapon;
                cmd.PickUpEquip = item.PickUpEquip;
                cmd.UseEntityId = item.UseVehicleId;
                cmd.UseVehicleSeat = item.UseVehicleSeat;
                cmd.UseType = item.UseType;
                cmd.BagIndex = item.BagIndex;
                list.Value.Add(cmd);
            }
            return list;
        }

        public static void ToProtoBuf(UserCmdMessage rc, ReusableList<IUserCmd> list)
        {
            foreach (var item in list.Value)
            {
	            UserCmdItem cmd = UserCmdItem.Allocate();
	            {
		            cmd.RenderTime = item.RenderTime;
                    cmd.FrameInterval = item.FrameInterval;
					cmd.Buttons= item.Buttons;
					cmd.MoveHorizontal = item.MoveHorizontal;
					cmd.MoveVertical = item.MoveVertical;
	                cmd.MoveUpDown = item.MoveUpDown;
					cmd.Seq = item.Seq;
					cmd.DeltaPitch = item.DeltaPitch;
					cmd.DeltaYaw = item.DeltaYaw;
	                cmd.SwitchNumber = item.SwitchNumber;
					cmd.ChangedSeat =  item.ChangedSeat;
	                cmd.ChangeChannel = item.ChangeChannel;
					cmd.SnapshotId = item.SnapshotId;
	                cmd.CurWeapon = item.CurWeapon;
	                cmd.PickUpEquip = item.PickUpEquip;
                    cmd.UseVehicleId = item.UseEntityId;
                    cmd.UseVehicleSeat = item.UseVehicleSeat;
                    cmd.UseType = item.UseType;
                    cmd.BagIndex = item.BagIndex;
	            };
                rc.UserCmdList.Add(cmd);
            }
        }
    }
}