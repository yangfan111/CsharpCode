using Core.GameModule.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Util;
using Assets.Utils.Configuration;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core.CameraControl;
using Core.Appearance;
using UnityEngine;
using Utils.Compare;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Client.GameModules.Player
{
    class PlayerFirstPersonHandRotationSystem : AbstractUserCmdExecuteSystem
    {
        class FirstHandWeaponRotation
        {
            private const float VelThrehold = 20;

            public RotationParameter HorizontalParam { get; set; }
            public RotationParameter VerticalParam { get; set; }

            public void UpdateParam(FirstPersonRotationConfig config, float scale = 1.0f)
            {
                HorizontalParam.Init(config.HorizontalParam.UpperLimit * scale, config.HorizontalParam.RestoreVel * scale, config.HorizontalParam.VelCoefficient * scale, HorizontalParam.Shift * scale);
                VerticalParam.Init(config.VerticalParam.UpperLimit * scale, config.VerticalParam.RestoreVel * scale, config.VerticalParam.VelCoefficient * scale, VerticalParam.Shift * scale);
            }

            private void SetVelocity(float horizontal, float vertical)
            {
                HorizontalParam.SetVel(horizontal);
                VerticalParam.SetVel(vertical);
            }

            public void SetAngularVelocity(float horizontal, float vertical)
            {
                HorizontalParam.AngularVelocity = horizontal;
                VerticalParam.AngularVelocity = vertical;

                SetVelocity(horizontal, vertical);
            }

            public void SetShift(float horizontal, float vertical)
            {
                HorizontalParam.Shift = horizontal;
                VerticalParam.Shift = vertical;
            }

            public void Rotate(float deltaTime)
            {
                Rotate(HorizontalParam, deltaTime);
                Rotate(VerticalParam, deltaTime);
            }

            private void Rotate(RotationParameter param, float deltaTime)
            {
                if (Math.Abs(param.AngularVelocity) >= VelThrehold)
                {
                    var upperBound = AmplitudeLimit(param.AngularVelocity, param);
                    var lowerBound = -upperBound;
                    param.Shift = Mathf.Clamp(param.Shift + param.GetAverageVel() * deltaTime, lowerBound, upperBound);
                }
                else
                {
                    if (param.Shift != 0)
                    {
                        // Restore
                        var restoreVel = param.Shift > 0 ? -param.RestoreVel : param.RestoreVel;
                        var newShift = param.Shift + restoreVel * deltaTime;
                        if (newShift * param.Shift < 0)
                        {
                            param.Shift = 0;
                        }
                        else
                        {
                            param.Shift = newShift;
                        }
                    }
                }
            }

            /// <summary>
            /// 返回幅度的最大值
            /// </summary>
            /// <param name="vel"></param>
            /// <param name="param"></param>
            /// <returns></returns>
            private float AmplitudeLimit(float vel, RotationParameter param)
            {
                float baseAmplitude = Mathf.Abs(vel / 10);
                return Mathf.Min(baseAmplitude, param.UpperLimit);
            }
        }


        class RotationParameter
        {
            public float UpperLimit { get; private set; }
            public float RestoreVel { get; private set; }
            public float VelCoefficient { get; private set; }
            public float Shift { get; set; }
            public float AngularVelocity { get; set; }
            private List<float> _velHistory = new List<float>(5) { 0, 0, 0, 0, 0 };
            private int _index = 0;

            public RotationParameter(float upperLimit, float restoreVel, float velCoefficient, float shift = 0.0f)
            {
                Init(upperLimit, restoreVel, velCoefficient, shift);   
            }

            public void Init(float upperLimit, float restoreVel, float velCoefficient, float shift = 0.0f)
            {
                UpperLimit = upperLimit;
                RestoreVel = restoreVel;
                VelCoefficient = velCoefficient;
                Shift = shift;
                _index = 0;
                for (int i = 0; i < _velHistory.Count; ++i)
                {
                    _velHistory[i] = 0.0f;
                }
            }

            public void SetVel(float vel)
            {
                _velHistory[_index] = vel;
                _index = (_index + 1) % _velHistory.Count;
            }

            public float GetAverageVel()
            {
                float vel = 0;
                _velHistory.ForEach((x) => vel += x * 0.2f);
                return vel * VelCoefficient;
            }

            public void RestelHistory()
            {
                _index = 0;
                for (int i = 0; i < _velHistory.Count; ++i)
                {
                    _velHistory[i] = 0.0f;
                }
            }

        }

        private static LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerFirstPersonHandRotationSystem));

        private FirstHandWeaponRotation firstHandRotation;
        private FirstHandWeaponRotation sightRotation;

        private int _currentWeapon = -1;
        private float _currentScopeScale = 1.0f;

        public PlayerFirstPersonHandRotationSystem()
        {
            firstHandRotation = new FirstHandWeaponRotation()
            {
                HorizontalParam = new RotationParameter(4, 20, 1, 0),
                VerticalParam = new RotationParameter(2, 10, 0.95f, 0)
            };
            sightRotation = new FirstHandWeaponRotation()
            {
                HorizontalParam = new RotationParameter(4, 20, 1, 0),
                VerticalParam = new RotationParameter(2, 10, 0.95f, 0)
            };
        }

        protected override bool Filter(PlayerEntity playerEntity)
        {
            return playerEntity.hasAppearanceInterface && playerEntity.hasCharacterBoneInterface && playerEntity.hasStateInterface;
        }
        // 问题，执行在PlayerAppearanceUpdateSystem,因为执行在PlayerAppearanceUpdateSystem添加在UserCmdGameModule中，PlayerFirstPersonHandRotationSystem添加在ClientPlayerModule中
        // 程序先添加UserCmdGameModule再添加ClientPlayerModule，导致player.appearanceInterface.Appearance.FirstPersonFastMoveShift设置的是下一帧的数据
        protected override void ExecuteUserCmd(PlayerEntity player, IUserCmd cmd)
        {
           
            if (!player.appearanceInterface.Appearance.IsFirstPerson || !player.hasPredictedAppearance || !player.hasStateInterface)
            {
                firstHandRotation.SetShift(0,0);
                firstHandRotation.SetAngularVelocity(0,0);
                sightRotation.SetShift(0,0);
                sightRotation.SetAngularVelocity(0,0);
                return;
            }


            var weaponInHand = player.appearanceInterface.Appearance.GetWeaponIdInHand();
            var weaponScope = player.appearanceInterface.Appearance.GetScopeIdInCurrentWeapon();

            var scopeScale = GetScopeScale(IsSight(player), weaponInHand, weaponScope);
            
            if (_currentWeapon != weaponInHand || !CompareUtility.IsApproximatelyEqual(scopeScale, _currentScopeScale))
            {
                _currentWeapon = weaponInHand;
                _currentScopeScale = scopeScale;
                if (_currentWeapon > 0)
                {
                    if (Logger.IsDebugEnabled)
                    {
                        Logger.DebugFormat("change first person offset config to weapon:{0}, scopeScale:{1}", _currentWeapon, _currentScopeScale);
                    }
                    firstHandRotation.UpdateParam(SingletonManager.Get<FirstPersonOffsetConfigManager>().GetFirstPersonRotationConfig(_currentWeapon), _currentScopeScale);
                    sightRotation.UpdateParam(SingletonManager.Get<FirstPersonOffsetConfigManager>().GetFirstPersonSightRotationConofig(_currentWeapon), _currentScopeScale);
                }
            }

            float deltaTime = cmd.FrameInterval * 0.001f;

            firstHandRotation.SetAngularVelocity(cmd.DeltaYaw / deltaTime, cmd.DeltaPitch / deltaTime);
            sightRotation.SetAngularVelocity(cmd.DeltaYaw / deltaTime, cmd.DeltaPitch / deltaTime);

            firstHandRotation.Rotate(deltaTime);
            sightRotation.Rotate(deltaTime);

            player.characterBoneInterface.CharacterBone.FirstPersonFastMoveShift(
                firstHandRotation.HorizontalParam.Shift, firstHandRotation.VerticalParam.Shift, 
                sightRotation.HorizontalParam.Shift, sightRotation.VerticalParam.Shift);
        }

       

        private float GetScopeScale(bool isSight, int weaponId, int scopeId)
        {
            var ret = 1.0f;

            if (scopeId > 0)
            {
                if (isSight)
                {
                    //瞄准镜缩放
                    ret = SingletonManager.Get<WeaponPartsConfigManager>().GetSightModelScale(scopeId);
                }
            }
            else if (isSight)
            {
                ret = SingletonManager.Get<WeaponResourceConfigManager>().GetAimModelScale(weaponId);
            }
            
            return ret;
        }

        private bool IsSight(PlayerEntity player)
        {
            var state = player.stateInterface.State;
            return state.GetActionKeepState() == ActionKeepInConfig.Sight ||
                   state.GetNextActionKeepState() == ActionKeepInConfig.Sight;
        }
    }
}
