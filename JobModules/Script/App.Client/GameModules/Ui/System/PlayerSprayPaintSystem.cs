﻿using App.Client.GameModules.Ui.UiAdapter;
using App.Shared;
using App.Shared.Audio;
using App.Shared.Player;
using App.Shared.Util;
using Core;
using Core.Free;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Free.framework;
using System;
using UnityEngine;
using Utils.Appearance.Bone;
using Utils.Configuration;
using XmlConfig;

namespace App.Client.GameModules.Ui.System
{
    public class PlayerSprayPaintSystem : AbstractUserCmdExecuteSystem
    {
        private Contexts _contexts;
        private GameObject[] affectedObjects;
        private bool isSprayPainting = false;
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerSprayPaintSystem));

        public PlayerSprayPaintSystem(Contexts contexts) {
            _contexts = contexts;
        }

        private void SendMarkMessage(Vector3 position, Vector3 forward, Vector3 head, int SprayPrintSpriteId) {
            SimpleProto sprayPaint = FreePool.Allocate();
            sprayPaint.Key = FreeMessageConstant.PlayerSprayPaint;
            /*位置*/
            sprayPaint.Fs.Add(position.x);
            sprayPaint.Fs.Add(position.y);
            sprayPaint.Fs.Add(position.z);
            /*朝向*/
            sprayPaint.Fs.Add(forward.x);
            sprayPaint.Fs.Add(forward.y);
            sprayPaint.Fs.Add(forward.z);
            /*方向*/
            sprayPaint.Fs.Add(head.x);
            sprayPaint.Fs.Add(head.y);
            sprayPaint.Fs.Add(head.z);
            /*ID*/
            sprayPaint.Ins.Add(SprayPrintSpriteId);
            var config = IndividuationConfigManager.GetInstance().GetConfigById(SprayPrintSpriteId);
            int lifeTime;
            if (null == config || 0 == config.LifeTime) {
                lifeTime = int.MaxValue;
            }
            else {
                lifeTime = config.LifeTime;
            }
            /*LifeTime*/
            sprayPaint.Ins.Add(lifeTime);
            if (_contexts.session.clientSessionObjects.NetworkChannel != null) {
                _contexts.session.clientSessionObjects.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, sprayPaint);
            }
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            if (!cmd.IsSprayPaint)
            {
                isSprayPainting = false;
                return;
            }
            if (isSprayPainting)
            {
                return;
            }
            isSprayPainting = true;
            var root = _contexts.player.flagSelfEntity.RootGo();
            Transform cameraTran = _contexts.player.cameraObj.MainCamera.transform;
            Transform headTran = BoneMount.FindChildBoneFromCache(root, BoneName.CharacterHeadBoneName); /*头部*/
            if (headTran == null || cameraTran == null) {
                _logger.DebugFormat("error headTran or cameraTran  !" );
                return;
            }
            Vector3 forward = cameraTran.forward;
            Vector3 head = root.transform.eulerAngles;

            var paintIdList = _contexts.ui.uI.PaintIdList;
            var selectedPaintIndex = _contexts.ui.uI.SelectedPaintIndex;
            if (0 == selectedPaintIndex)
            {
                selectedPaintIndex = PaintUiAdapter.SelectValidIndex(paintIdList);
                _contexts.ui.uI.SelectedPaintIndex = selectedPaintIndex;
            }
            if (paintIdList == null || paintIdList.Count <= selectedPaintIndex || paintIdList[selectedPaintIndex] == 0)
            {
                _logger.DebugFormat("error paintIdList or selectedPaintIndex : " + selectedPaintIndex);
                //TODO 未装备喷漆提示
                return;
            }

            try
            {
                PlaySprayAudio();
            }
            catch (Exception e)
            {
                _logger.DebugFormat(e.Message);
            }

            RaycastHit raycastHit;
            Ray ray = new Ray(headTran.position, cameraTran.forward);
            if (Physics.Raycast(ray, out raycastHit, 3.0f)) {
                if (!IsIgnore(raycastHit.collider.gameObject))
                {
                    _logger.DebugFormat("SendSprayMessage");
                    SendMarkMessage(raycastHit.point, raycastHit.normal, head, paintIdList[selectedPaintIndex]);
                }
            }
        }

        private void PlaySprayAudio()
        {
            Vector3 position = _contexts.player.flagSelfEntity.position.Value;
            GameAudioMedia.PlayUniqueEventAudio(position, EAudioUniqueId.UI_battle_spray);
            /*_contexts.player.flagSelfEntity.AudioController().PlaySimpleAudio(EAudioUniqueId.UI_battle_spray, false);*/
        }

        protected bool IsIgnore(GameObject obj) {
            if (obj == null) return true;
            // Layer
            int layer = obj.layer;
            string name = obj.name;
            bool ignore = (layer == LayerMask.NameToLayer("Player") ||
                layer == LayerMask.NameToLayer("Vehicle") ||
                layer == LayerMask.NameToLayer("VehicleTrigger") ||
                layer == LayerMask.NameToLayer("UserInputRaycast"));
            ignore = (ignore ||
                name.ToLower().IndexOf("door'") > 0);
            return ignore;
        }



        protected override bool Filter(PlayerEntity playerEntity)
        {
            ActionKeepInConfig ac = playerEntity.stateInterface.State.GetActionKeepState();
            PostureInConfig mc = playerEntity.stateInterface.State.GetCurrentPostureState();

            return !(ac == ActionKeepInConfig.Drive ||
                mc == PostureInConfig.Swim ||
                mc == PostureInConfig.Dive ||
                mc == PostureInConfig.DyingTransition ||
                mc == PostureInConfig.Dying ||
                mc == PostureInConfig.Climb);
        }
    }
}
