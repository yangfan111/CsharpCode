using App.Shared.Components.Player;
using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.Player;
using UnityEngine;
using XmlConfig;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class PlayerStateUiAdapter : UIAdapter, IPlayerStateUiAdapter
    {
        private PlayerEntity _playerEntity;

        public PlayerStateUiAdapter()
        {
        }
        public string PlayerName
        {
            get { return PlayerEntity.playerInfo.PlayerName; }
        }
        public int CurUIModel { get { return 1; }}             //1 默认模式 2可选方案
 
     
        //能量组
        public float CurPower
        {
            get
            {
                if (null != PlayerEntity)
                    return PlayerEntity.gamePlay.Energy;
                return 0;
            }
        }

        //buff 组（回血buff）
        public bool RecureBufActive { get { return CurPower > 0; }}
        //加速buff
        public bool SpeedBufActive { get { return CurPower > 60; }}

        //当前含氧量   最大100
        public float CurO2
        {
            get
            {
                if (null != PlayerEntity)
                    return PlayerEntity.oxygenEnergyInterface.Oxygen.CurrentOxygen;
                return 0;
            }
        }

        public float MaxCurO2
        {
            get
            {
                return 100;
            }
        }

        //血量组           
        public int CurrentHp          //非受伤状态的当前血量
        {
            get
            {
                if (PlayerEntity != null)
                {
                    return (int)PlayerEntity.gamePlay.CurHp;
                }
                else
                {
                    return 0;
                }
            }
        }
        public int MaxHp
        {
            get
            {
                if (PlayerEntity != null)
                    return PlayerEntity.gamePlay.MaxHp;
                else
                {
                    return 0;
                }
            }
        }
        public int CurrentHpInHurtedState     //受伤状态下 的当前血量
        {
            get
            {
                return (int)PlayerEntity.gamePlay.InHurtedHp;
            }
        }
        public bool IsInHurtedState { get { return PlayerEntity.gamePlay.IsLifeState(EPlayerLifeState.Dying); } }   //是否在受伤状态
        public int MayRecoverHp { get { return PlayerEntity.gamePlay.RecoverHp; } }          //可能恢复的血量 是+ 当前血量的总和


        //Pose 组
        public int CurPose
        {
            get
            {
                if (PlayerEntity != null)
                {
                    Core.CharacterState.ICharacterState state = PlayerEntity.stateInterface.State;
                    PostureInConfig postureInConfig = state.GetCurrentPostureState();
                    switch (postureInConfig)
                    {
                        case PostureInConfig.Stand:
                            return 1;
                        case PostureInConfig.Crouch:
                            return 2;
                        case PostureInConfig.Prone:
                            return 3;
                        case PostureInConfig.Jump:
                            return 4;
                        case PostureInConfig.Swim:
                            return 5;
                        case PostureInConfig.Dying:
                            return 6;
                        default:
                            break;
                    }

                    LeanInConfig leanInConfig = state.GetCurrentLeanState();
                    switch (leanInConfig)
                    {
                        case LeanInConfig.PeekLeft:
                            return 7;
                        case LeanInConfig.PeekRight:
                            return 8;
                        default:
                            break;
                    }
                }
                return 0;
            }
        }

        //1 第一视角 3 第三视角
        public int FirstOrThirdView
        {
            get
            {
                if (PlayerEntity != null && PlayerEntity.hasAppearanceInterface &&  PlayerEntity.appearanceInterface.Appearance.IsFirstPerson)
                    return 1;
                else
                    return 3;
            }
        }


        //装备组
        public bool IsDead
        {
            get { return PlayerEntity != null && PlayerEntity.gamePlay.IsDead(); }
        }
        //头盔
        public float maxHelmet
        {
            get
            {
                return PlayerEntity != null ? PlayerEntity.gamePlay.MaxHelmet : 0;
            }
        }

        public float curHelmet
        {
            get
            {
                return PlayerEntity != null ? PlayerEntity.gamePlay.CurHelmet : 0;
            }
        }


        //防弹衣
        public float maxArmor
        {
            get
            {
                return PlayerEntity != null ? PlayerEntity.gamePlay.MaxArmor : 0;
            }
        }      
        public float curArmor
        {
            get
            {
                return PlayerEntity != null ? PlayerEntity.gamePlay.CurArmor : 0;
            }
        }      


        public ParticleSystem MyParticle
        {
            get
            {
                PlayerEntity player = PlayerEntity;
                if (null != player && player.hasCameraFx && null != player.cameraFx.Poison)
                {
                    return player.cameraFx.Poison.GetComponent<ParticleSystem>();
                }
                return null;
            }
        }

        public PlayerEntity PlayerEntity
        {
            get
            {
                return _playerEntity;
            }

            set
            {
                _playerEntity = value;
            }
        }

       
        public override bool IsReady()
        {
            return _playerEntity != null;
        }
    }
}