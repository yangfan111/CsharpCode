using App.Server.GameModules.GamePlay.free.player;
using App.Shared.Components.Player;
using App.Shared.FreeFramework.framework.trigger;
using App.Shared.Player;
using com.wd.free.@event;
using Core.Free;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Prediction.UserPrediction.Cmd;
using Shared.Scripts.Effect;
using System.Collections.Generic;
using UnityEngine;

namespace App.Shared.GameModules.Player
{
    public class PlayerBioSwitchSystem : IUserCmdExecuteSystem
    {
        private Contexts _contexts;
        private int _jobAttribute;

        private const string ClearTargets = "ClearTargets";
        private const string AddTarget = "AddTarget";

        public PlayerBioSwitchSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        private AbstractEffectMonoBehaviour GlowOutlineComponent {
            get
            {
                UnityEngine.Camera main = UnityEngine.Camera.main;
                AbstractEffectMonoBehaviour component = main.GetComponent(EffectMonnoBehaviourManager.GetTypeByName("GlowOutlineCommandBuffer")) as AbstractEffectMonoBehaviour;
                if (null == component)
                {
                    component = main.gameObject.AddComponent(EffectMonnoBehaviourManager.GetTypeByName("GlowOutlineCommandBuffer")) as AbstractEffectMonoBehaviour;
                }
                return component;
            }
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = owner.OwnerEntity as PlayerEntity;
            if (player.hasGamePlay)
            {
                var jobAttribute = player.gamePlay.JobAttribute;
                if (jobAttribute != _jobAttribute)
                {
                    var sessionObjects = _contexts.session.clientSessionObjects;
                    var generator = sessionObjects.UserCmdGenerator;
                    if (jobAttribute == (int)EJobAttribute.EJob_Variant ||
                        jobAttribute == (int)EJobAttribute.EJob_Matrix ||
                        jobAttribute == (int)EJobAttribute.EJob_Hero)
                    {
                        generator.SwitchMode(EModeSwitch.Bio);
                        /*GlowOutlineComponent.enabled = true;*/
                    }
                    else
                    {
                        generator.SwitchMode(EModeSwitch.Normal);
                        /*GlowOutlineComponent.enabled = false;*/
                    }
                    _jobAttribute = jobAttribute;
                }

                /*UpdateGlowOutline();*/
            }
        }

        protected PlayerEntity GetEntityById(long playerId)
        {
            PlayerEntity[] entities = _contexts.player.GetEntities();
            for (int i = 0, maxi = entities.Length; i < maxi; i++) {
                PlayerEntity entity = entities[i];
                if (entity.playerInfo.PlayerId == playerId) {
                    return entity;
                }
            }
            return null;
        }

        private void UpdateGlowOutline()
        {
            GlowOutlineComponent.SetParam(ClearTargets, (object)null);
            PlayerEntity player = _contexts.player.flagSelfEntity;
            if (player.hasGamePlay && player.gamePlay.IsMatrix()) {
                List<long> heroIdList  = _contexts.ui.uI.HeroIdList;
                List<long> humanIdList = _contexts.ui.uI.HumanIdList;

                for (int i = 0, maxi = heroIdList.Count; i < maxi; i++) {
                    long playerId = heroIdList[i];
                    PlayerEntity pe = GetEntityById(playerId);
                    GlowOutlineComponent.SetParam(AddTarget, pe.RootGo());
                }

                for (int i = 0, maxi = humanIdList.Count; i < maxi; i++) {
                    long playerId = humanIdList[i];
                    PlayerEntity pe = GetEntityById(playerId);
                    GlowOutlineComponent.SetParam(AddTarget, (object)pe.RootGo());
                }
            }
        }
    }
}
