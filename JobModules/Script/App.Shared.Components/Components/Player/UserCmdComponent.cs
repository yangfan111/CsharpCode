using System;
using System.Collections.Generic;
using Core.Components;
using Core.EntityComponent;
using Core.GameInputFilter;
using Core.Network;
using Core.Playback;
using Core.Prediction;
using Core.Prediction.UserPrediction;
using Core.Prediction.UserPrediction.Cmd;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Core.UpdateLatest;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using Utils.Utils;

// ReSharper disable PossibleNullReferenceException
namespace App.Shared.Components.Player
{
    [Player] 
    public class UserCmdSeqComponent : IUserPredictionComponent
    {
        public int GetComponentId() { { return (int)EComponentIds.UserCmd; } }

        [NetworkProperty] public int LastCmdSeq;
        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as UserCmdSeqComponent;
            LastCmdSeq = r.LastCmdSeq;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var r = right as UserCmdSeqComponent;
            return LastCmdSeq == r.LastCmdSeq;
        }

        protected bool Equals(UserCmdSeqComponent other)
        {
            return LastCmdSeq == other.LastCmdSeq;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UserCmdSeqComponent) obj);
        }

        public override int GetHashCode()
        {
            return LastCmdSeq;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }

    [Player]
    public class LatestAdjustCmdComponent : ISelfLatestComponent
    {
        [DontInitilize] [NetworkProperty] public Vector3 AdjustPos;
        [NetworkProperty] public int ServerSeq;
        public int ClientSeq = -1;
        
        public int GetComponentId()
        {
            return (int) EComponentIds.LatestAdjustCmd;
        }

        public Vector3? GetPos(int seq)
        {
            if (ClientSeq < ServerSeq)
            {
                ClientSeq = ServerSeq;
                return AdjustPos;
            }
                
            return null;
        }
        
        public void SetPos(Vector3 pos)
        {
            AdjustPos = pos;
        }   
        
        public void CopyFrom(object target)
        {
            var right = target as LatestAdjustCmdComponent;
            AdjustPos = right.AdjustPos;
            ServerSeq = right.ServerSeq;
        }
        
        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
    
    [Player]
    public class VehicleCmdSeqComponent : IGameComponent
    {
        public int GetComponentId() { { return (int)EComponentIds.VehicleCmd; } }

        [NetworkProperty] public int LastCmdSeq;
        public void RewindTo(object rightComponent)
        {
            LastCmdSeq = ((VehicleCmdSeqComponent)(rightComponent)).LastCmdSeq;
        }
        public bool IsApproximatelyEqual(object right)
        {
            return LastCmdSeq == (right as VehicleCmdSeqComponent).LastCmdSeq;
        }

        public override string ToString()
        {
            return string.Format("LastCmdSeq: {0}", LastCmdSeq);
        }
    }

    [Player]
    [Serializable]
    public class UserCmdComponent : CmdListComponent<IUserCmd>, IComponent
    {
        
    }

    [Player]
    
    public class RandomComponent : IUpdateComponent, IUserPredictionComponent
    {
        [DontInitilize] [NetworkProperty] public int Random;
        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as RandomComponent;
            Random = right.Random;
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.Random;
        }

        public bool IsApproximatelyEqual(object rightComponent)
        {
            var right = rightComponent as RandomComponent;
           return  Random == right.Random;
        }

        public override string ToString()
        {
            return string.Format("Random: {0}", Random);
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
    public static class UsercmdEx
    {
        public static void CopyForm(this SendUserCmdComponent component, IUserCmd right)
        {
            //component.Seq = right.Seq;
            component.FrameInterval = right.FrameInterval;
            component.MoveHorizontal = right.MoveHorizontal;
            component.MoveUpDown = right.MoveUpDown;
            component.MoveVertical = right.MoveVertical;
            component.DeltaYaw = right.DeltaYaw;
            component.DeltaPitch = right.DeltaPitch;
            component.RenderTime = right.RenderTime;
            component.ChangedSeat = right.ChangedSeat;
            //component.SnapshotId = right.SnapshotId;
            component.BeState = right.BeState;
            component.Buttons = right.Buttons;
            component.SwitchNumber = right.SwitchNumber;

            component.CurWeapon = right.CurWeapon;
            component.UseEntityId = right.UseEntityId;
            component.PickUpEquip = right.PickUpEquip;
            component.UseVehicleSeat = right.UseVehicleSeat;
            component.UseType = right.UseType;
            component.ChangeChannel = right.ChangeChannel;
            component.BagIndex = right.BagIndex;
        }
      
    }
    [Player]
    
    public class SendUserCmdComponent : IGameComponent,IUpdateComponent,IResetableComponent
    {
        //[DontInitilize] [NetworkProperty] public int Seq;
        [DontInitilize] [NetworkProperty] public int FrameInterval;
        [DontInitilize] [NetworkProperty] public float MoveHorizontal;
        [DontInitilize] [NetworkProperty] public float MoveUpDown;
        [DontInitilize] [NetworkProperty] public float MoveVertical;
        [DontInitilize] [NetworkProperty] public float DeltaYaw;
        [DontInitilize] [NetworkProperty] public float DeltaPitch;
        [DontInitilize] [NetworkProperty] public int RenderTime;
        [DontInitilize] [NetworkProperty] public int ClientTime;
        
        [DontInitilize] [NetworkProperty] public int ChangedSeat;
        //[DontInitilize] [NetworkProperty] public int SnapshotId;
        [DontInitilize] [NetworkProperty] public int BeState;
        [DontInitilize] [NetworkProperty] public int Buttons;
        [DontInitilize] [NetworkProperty] public int SwitchNumber;
        [DontInitilize] [NetworkProperty] public int CurWeapon;
        [DontInitilize] [NetworkProperty] public int PickUpEquip;
        [DontInitilize] [NetworkProperty] public int UseEntityId;
        [DontInitilize] [NetworkProperty] public int UseVehicleSeat;
        [DontInitilize] [NetworkProperty] public int UseType;
        [DontInitilize] [NetworkProperty] public int ChangeChannel;
        [DontInitilize] [NetworkProperty] public int BagIndex;
        public int GetComponentId()
        {
            return (int) EComponentIds.SendUserCmd;
        }

        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as SendUserCmdComponent;
            //Seq = right.Seq;
            FrameInterval = right.FrameInterval;
            MoveHorizontal = right.MoveHorizontal;
            MoveUpDown = right.MoveUpDown;
            MoveVertical = right.MoveVertical;
            DeltaYaw = right.DeltaYaw;
            DeltaPitch = right.DeltaPitch;
            RenderTime = right.RenderTime;
            ClientTime = right.ClientTime;
            ChangedSeat = right.ChangedSeat;
            //SnapshotId = right.SnapshotId;
            BeState = right.BeState;
            Buttons = right.Buttons;
            SwitchNumber = right.SwitchNumber;
           
            CurWeapon = right.CurWeapon;
            UseEntityId = right.UseEntityId;
            PickUpEquip = right.PickUpEquip;
            UseVehicleSeat = right.UseVehicleSeat;
            UseType = right.UseType;
            ChangeChannel = right.ChangeChannel;
            BagIndex = right.BagIndex;
        }
        

        public void Reset()
        {
            
        }

     } 

    [Player]
    public class NetworkComponent : IComponent
    {
        public INetworkChannel NetworkChannel;
    }

    [Player]
    public class UserCmdFilterComponent:IComponent
    {
        public IGameInputProcessor GameInputProcessor; 
    }

    [Player]
    public class FreeUserCmdComponent : IComponent
    {
        [DontInitilize] public bool ForceUnmountWeapon;
        [DontInitilize] public bool MountWeapon;

        public void Clear()
        {
            ForceUnmountWeapon = false;
            MountWeapon = false;
        }
    }
}