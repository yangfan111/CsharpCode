using Core.Components;
using Core.EntityComponent;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Core.Utils;
using Entitas.CodeGeneration.Attributes;
using System;
using Core.Free;
using Entitas;

namespace App.Shared.Components.Player
{
    public enum EPlayerLifeState
    {
        Alive = 0,
        Dying = 1,
        Dead = 2,
    }

    public enum EVehicleCollisionState
    {
        None = 0,
        Enter = 1,
        Stay = 2,
    }

    public enum EJobAttribute {
        EJob_EveryMan, /*普通人*/
        EJob_Variant, /*变异体*/
        EJob_Matrix, /*母体*/
        EJob_Hero, /*英雄*/
    }


    [Player]
    public class GamePlayComponent : ISelfLatestComponent, IPlaybackComponent, IRule, IResetableComponent
    {
        public static string[] LifeStateString = new[]
        {
            "Alive",
            "Dying",
            "Dead"
        };
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(GamePlayComponent));

        //最大血量有需求再同步
        [NetworkProperty] public int MaxHp;
        [NetworkProperty] public float CurHp;
        
        public int LastNewRoleId = -1;
        [NetworkProperty] public int NewRoleId = -1;

        [DontInitilize] [NetworkProperty] public float InHurtedHp;
        [DontInitilize] [NetworkProperty] public int RecoverHp;
        [DontInitilize] [NetworkProperty] public int InHurtedCount;
        [DontInitilize] [NetworkProperty] public int BuffRemainTime;

        [DontInitilize] public int LastLifeState; // PlayerLifeState
        [DontInitilize] [NetworkProperty] public int LifeState; // PlayerLifeState
        [DontInitilize] public bool IsStandPosture;

        [DontInitilize] [NetworkProperty] public int LifeStateChangeTime; // ServerTime

        [DontInitilize] public EVehicleCollisionState VehicleCollisionState;
        [DontInitilize] public EntityKey CollidedVehicleKey;

        [DontInitilize] [NetworkProperty] public int GameState;
        [DontInitilize] public int ClientState;

        [DontInitilize] [NetworkProperty] public int PlayerState;
        [DontInitilize] public int UIState;
        [DontInitilize] public bool UIStateUpdate;
        [DontInitilize] [NetworkProperty] public int CastState;

        [DontInitilize] [NetworkProperty] public int CameraEntityId;
        [DontInitilize] [NetworkProperty] public bool BeingObserved;

        [DontInitilize] [NetworkProperty] public int Energy;
        [DontInitilize] [NetworkProperty] public int BagWeight;

        [DontInitilize] [NetworkProperty] public int CurArmor;
        [DontInitilize] [NetworkProperty] public int MaxArmor;
        [DontInitilize] [NetworkProperty] public int CurHelmet;
        [DontInitilize] [NetworkProperty] public int MaxHelmet;
        [DontInitilize] [NetworkProperty] public int ArmorLv;
        [DontInitilize] [NetworkProperty] public int HelmetLv;

        [DontInitilize] [NetworkProperty] public bool IsSave;
        [DontInitilize] [NetworkProperty] public bool IsBeSave;
        [DontInitilize] [NetworkProperty] public int SaveTime;
        [DontInitilize] [NetworkProperty] public EntityKey SavePlayerKey;

        [DontInitilize] [NetworkProperty] public bool CoverInit;                    //是否开始执行用户指令
       
        [DontInitilize] public short LastViewModeByCmd;                         //LastViewMode: 记录由指令驱动的人称转换，用于人物复活时恢复人称状态

        [DontInitilize] [NetworkProperty] public bool TipHideStatus;
        [NetworkProperty] public bool Visibility;
        public bool ClientVisibility;
        [NetworkProperty] public int JobAttribute { get; set; }
        [DontInitilize] [NetworkProperty] public bool ChickenHxWin;

        [DontInitilize] public int UseEntityId;

        [NetworkProperty] public bool Witness = false; // 观战

        [DontInitilize] public int ActiveMask;

        [NetworkProperty] public bool HudView = false; // Hud可见

        public void ChangeLifeState(EPlayerLifeState state, int time)
        {
            if (LifeState != (int) state)
            {
                LastLifeState = LifeState;
                LifeState = (int) state;
                LifeStateChangeTime = time;
                if (state == EPlayerLifeState.Dying)
                {
                    InHurtedHp = 100;
                    InHurtedCount ++;
                    BuffRemainTime = 0;
                }
            }
        }

        public void ChangeNewRoleId(int roleId)
        {
            if(NewRoleId == roleId) return;
            LastNewRoleId = NewRoleId;
            NewRoleId = roleId;
        }

        public bool HasNewRoleIdChangedFlag()
        {
            return NewRoleId != LastNewRoleId;
        }

        public void ClearNewRoleIdChangedFlag()
        {
            LastNewRoleId = NewRoleId;
        }

        public string GetLifeStateString()
        {
            return LifeStateString[LifeState];
        }

        /// <summary>
        /// 更新状态后需要调用
        /// </summary>
        public void ClearLifeStateChangedFlag()
        {
            LastLifeState = LifeState;
        }

        public bool IsLifeChangeOlderThan(int time)
        {
            return LifeStateChangeTime < time;
        }
        public bool IsLifeState(EPlayerLifeState state)
        {
            return LifeState == (int) state;
        }

        public bool IsAlive()
        {
            return LifeState != (int)EPlayerLifeState.Dead;
        }

        public bool IsLastLifeState(EPlayerLifeState state)
        {
            return LastLifeState == (int)state;
        }
        
        public bool HasLifeStateChangedFlag()
        {
             return LastLifeState != LifeState; 
        }

        public bool IsVehicleCollisionState(EVehicleCollisionState state)
        {
            return VehicleCollisionState == state;
        }

        public bool IsDead()
        {
            return LifeState == (int) EPlayerLifeState.Dead;
        }

        public bool IsHitDown()
        {
            return LifeState == (int) EPlayerLifeState.Dying && LastLifeState == (int) EPlayerLifeState.Alive;
        }

        public float CurModeHp
        {
            get
            {
                if (IsLifeState(EPlayerLifeState.Dying))
                    return InHurtedHp;
                else
                    return CurHp;
            }
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerGamePlay;
        }

        public void CopyFrom(object target)
        {
            var hp = target as GamePlayComponent;
            CurHp = hp.CurHp;
            NewRoleId = hp.NewRoleId;
            LifeState = hp.LifeState;
            IsStandPosture = hp.IsStandPosture;
            LifeStateChangeTime = hp.LifeStateChangeTime;
            MaxHp = hp.MaxHp;
            GameState = hp.GameState;
            InHurtedHp = hp.InHurtedHp;
            RecoverHp = hp.RecoverHp;
            InHurtedCount = hp.InHurtedCount;
            BuffRemainTime = hp.BuffRemainTime;
            PlayerState = hp.PlayerState;
            CastState = hp.CastState;
            Energy = hp.Energy;
            CameraEntityId = hp.CameraEntityId;
            BeingObserved = hp.BeingObserved;

            ArmorLv = hp.ArmorLv;
            HelmetLv = hp.HelmetLv;
            CurArmor = hp.CurArmor;
            MaxArmor = hp.MaxArmor;
            CurHelmet = hp.CurHelmet;
            MaxHelmet = hp.MaxHelmet;
            IsSave = hp.IsSave;
            IsBeSave = hp.IsBeSave;
            SaveTime = hp.SaveTime;
            SavePlayerKey = hp.SavePlayerKey;

            CoverInit = hp.CoverInit;
            Visibility = hp.Visibility;
            TipHideStatus = hp.TipHideStatus;
            JobAttribute = hp.JobAttribute;
            Witness = hp.Witness;
            ChickenHxWin = hp.ChickenHxWin;
        }

        public bool IsMatchJob(int jobAttribute) {
            switch (JobAttribute) {
                case (int)EJobAttribute.EJob_EveryMan:
                case (int)EJobAttribute.EJob_Hero:
                    return jobAttribute == (int)EJobAttribute.EJob_EveryMan || jobAttribute == (int)EJobAttribute.EJob_Hero;
                case (int)EJobAttribute.EJob_Variant:
                case (int)EJobAttribute.EJob_Matrix:
                    return jobAttribute == (int)EJobAttribute.EJob_Variant || jobAttribute == (int)EJobAttribute.EJob_Matrix;
            }
            return false;
        }

        public bool IsMatrix()
        {
            return (JobAttribute == (int)EJobAttribute.EJob_Matrix);
        }
        public bool IsVariant()
        {
            return (JobAttribute == (int)EJobAttribute.EJob_Variant || JobAttribute == (int)EJobAttribute.EJob_Matrix);
        }

        public bool CanAutoPick()
        {
            return (JobAttribute == (int)EJobAttribute.EJob_EveryMan);
        }

        public float DecreaseHp(float damage)
        {
            float ret = 0;
            float oldHp = 0;
            if (IsLifeState(EPlayerLifeState.Alive))
            {
                oldHp = CurHp;
                CurHp = Math.Max(CurHp - damage, 0);
                ret = oldHp - CurHp;
            }
            else if (IsLifeState(EPlayerLifeState.Dying))
            {
                oldHp = InHurtedHp;
                InHurtedHp = Math.Max(InHurtedHp - damage, 0);
                ret = oldHp - InHurtedHp;
            }
            return ret;
        }
        public bool IsInterpolateEveryFrame(){ return true; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            CopyFrom(left);
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public bool IsObserving()
        {
            return CameraEntityId != 0;
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.GamePlayComponent;
        }

        public void Reset()
        {
            LastNewRoleId = -1;
            NewRoleId = -1;
        }
    }
}
