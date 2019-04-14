using System;
using App.Shared;
using App.Shared.Util;
using Core.Free;
using Core.Prediction.UserPrediction.Cmd;
using Free.framework;
using UnityEngine;
using App.Shared.Player;
using Utils.Appearance;
using App.Client.ClientSystems;

namespace App.Client.GameModules.Ui.System
{
    public class PlayerSprayPaintSystem : AbstractUserCmdExecuteSystem
    {
        private Contexts _contexts;
        private GameObject[] affectedObjects;

        public PlayerSprayPaintSystem(Contexts contexts) {
            _contexts = contexts;
        }

        private void SendMarkMessage(Vector3 position, Vector3 forward, Vector3 vecSize) {
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
            /*大小*/
            sprayPaint.Fs.Add(vecSize.x);
            sprayPaint.Fs.Add(vecSize.y);
            sprayPaint.Fs.Add(vecSize.z);
            if (_contexts.session.clientSessionObjects.NetworkChannel != null) {
                _contexts.session.clientSessionObjects.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, sprayPaint);
            }
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            if (!cmd.IsSprayPaint) {
                return;
            }
            Vector3 debugSize = new Vector3(1.0f, 1.2f, 4.0f);
            var root = _contexts.player.flagSelfEntity.RootGo();
            Transform cameraTran = _contexts.player.cameraObj.MainCamera.transform;
            Transform headTran = BoneMount.FindChildBoneFromCache(root, BoneName.CharacterHeadBoneName); /*头部*/
            Vector3 forward = cameraTran.forward;
            Vector3 position = headTran.position + forward * debugSize.z * 0.5f + headTran.up * 0.2f;

            /*PlayerSprayPaintUtility.CreateSprayPaint(_contexts, debugSize, position, forward);*/
            SendMarkMessage(position, forward, debugSize);
        }



        protected override bool filter(PlayerEntity playerEntity)
        {
            return true;
        }
    }
}
