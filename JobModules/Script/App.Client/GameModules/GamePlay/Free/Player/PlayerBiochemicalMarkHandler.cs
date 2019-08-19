using Assets.Sources.Free;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Free.framework;
using System;
using Core.Free;
using Utils.Singleton;
using Assets.Sources.Free.UI;
using Assets.App.Client.GameModules.Ui;
using Shared.Scripts.Effect;
using App.Shared.Player;

namespace App.Client.GameModules.GamePlay.Free.Player
{
    public class PlayerBiochemicalMarkHandler : ISimpleMesssageHandler
    {
        private const string ClearCharacters = "ClearCharacters";
        private const string AddCharacter = "AddCharacter";
        private const string RemoveCharacter = "RemoveCharacter";

        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.PlayerBiochemicalMark;
        }

        private bool IsMatrix(PlayerEntity entity)
        {
            return (entity.hasGamePlay && entity.gamePlay.IsMatrix());
        }

        private bool IsVariant(PlayerEntity entity) {
            return (entity.hasGamePlay && entity.gamePlay.IsVariant());
        }

        private AbstractEffectMonoBehaviour _glowOutlineComponent = null;

        private AbstractEffectMonoBehaviour GlowOutlineComponent
        {
            get
            {
                Camera main = Camera.main;
                if (null == _glowOutlineComponent)
                {
                    _glowOutlineComponent = main.GetComponent(EffectMonnoBehaviourManager.GetTypeByName("GlowOutlineCommandBuffer")) as AbstractEffectMonoBehaviour;
                    if (null == _glowOutlineComponent)
                    {
                        _glowOutlineComponent = main.gameObject.AddComponent(EffectMonnoBehaviourManager.GetTypeByName("GlowOutlineCommandBuffer")) as AbstractEffectMonoBehaviour;
                    }
                }
                return _glowOutlineComponent;
            }
        }

        protected PlayerEntity GetEntityById(Contexts contexts , long playerId)
        {
            PlayerEntity[] entities = contexts.player.GetEntities();
            for (int i = 0, maxi = entities.Length; i < maxi; i++)
            {
                PlayerEntity entity = entities[i];
                if (entity.playerInfo.PlayerId == playerId)
                {
                    return entity;
                }
            }
            return null;
        }

        public void Handle(SimpleProto data)
        {
            Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;
            var selfEntity = contexts.player.flagSelfEntity;
            var ui = contexts.ui.uI;
            /*contexts.ui.uISession.UiState[UiNameConstant.BiochemicalMarkModel] = data.Bs[0];*/
            var type = data.Ins[0];
            var add = data.Bs[0];
            var playerId = data.Ls[0];
            var motherIdList = contexts.ui.uI.MotherIdList;
            var heroIdList = contexts.ui.uI.HeroIdList;
            var humanIdList = contexts.ui.uI.HumanIdList;
            switch (type) {
                case 0:
                    motherIdList.Clear();
                    heroIdList.Clear();
                    humanIdList.Clear();
                    /*if (IsMatrix(selfEntity))*/
                    GlowOutlineComponent.SetParam(ClearCharacters, (object)null);
                    GlowOutlineComponent.enabled = false;
                    break;
                case 1:
                    if (add) motherIdList.Add(playerId);
                    else motherIdList.Remove(playerId);
                    break;
                case 2:
                    PlayerEntity pe = GetEntityById(contexts, playerId);
                    if (add) {
                        /*heroIdList.Add(playerId);*/
                        if (IsVariant(selfEntity)) {
                            if (!GlowOutlineComponent.enabled)
                            {
                                GlowOutlineComponent.SetParam(ClearCharacters, (object)null);
                                GlowOutlineComponent.enabled = true;
                            }
                            GlowOutlineComponent.SetParam(AddCharacter, (object)pe.RootGo());
                        }
                    } else {
                        /*heroIdList.Remove(playerId);*/
                        if (IsVariant(selfEntity)) {
                            GlowOutlineComponent.SetParam(RemoveCharacter, (object)pe.RootGo());
                        }
                    }
                    break;
                case 3:
                    pe = GetEntityById(contexts, playerId);
                    if (add) {
                        /*humanIdList.Add(playerId);*/
                        if (IsVariant(selfEntity)) {
                            if (!GlowOutlineComponent.enabled)
                            {
                                GlowOutlineComponent.SetParam(ClearCharacters, (object)null);
                                GlowOutlineComponent.enabled = true;
                            }
                            GlowOutlineComponent.SetParam(AddCharacter, (object)pe.RootGo());
                        }
                    } else {
                        /*humanIdList.Remove(playerId);*/
                        if (IsVariant(selfEntity)) {
                            GlowOutlineComponent.SetParam(RemoveCharacter, (object)pe.RootGo());
                        }
                    }
                    break;
            }

        }
    }
}