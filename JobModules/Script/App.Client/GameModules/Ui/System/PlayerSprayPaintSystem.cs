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
using Core.Utils;

namespace App.Client.GameModules.Ui.System
{
    public class PlayerSprayPaintSystem : AbstractUserCmdExecuteSystem
    {
        private Contexts _contexts;
        private GameObject[] affectedObjects;
        private bool isSprayPainting = false;
        private Vector3 debugSize = Vector3.one;
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerSprayPaintSystem));

        public PlayerSprayPaintSystem(Contexts contexts) {
            _contexts = contexts;
        }

        private void SendMarkMessage(Vector3 position, Vector3 forward, Vector3 head) {
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
            if (_contexts.session.clientSessionObjects.NetworkChannel != null) {
                _contexts.session.clientSessionObjects.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, sprayPaint);
            }
        }

        int i, j;

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
            debugSize.x = 1.2f;
            debugSize.y = 4.0f;
            debugSize.z = 1.2f;
            var root = _contexts.player.flagSelfEntity.RootGo();
            Transform cameraTran = _contexts.player.cameraObj.MainCamera.transform;
            Transform headTran = BoneMount.FindChildBoneFromCache(root, BoneName.CharacterHeadBoneName); /*头部*/
            Vector3 forward = cameraTran.forward;
            Vector3 head = cameraTran.eulerAngles;
            Vector3 position = headTran.position + forward * debugSize.y * 0.6f + headTran.up * 0.2f;

            /*PlayerSprayPaintUtility.CreateSprayPaint(_contexts, debugSize, position, forward);*/
            _logger.DebugFormat("SendSprayMessage");
            SendMarkMessage(position, forward, head);
        }



        protected override bool filter(PlayerEntity playerEntity)
        {
            return true;
        }
    }
}
