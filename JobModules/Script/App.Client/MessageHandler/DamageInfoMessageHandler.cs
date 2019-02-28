using App.Client.Console.MessageHandler;
using App.Protobuf;
using App.Shared.Components.Ui;
using Core.Utils;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace App.Client.MessageHandler
{
    public class DamageInfoMessageHandler : AbstractClientMessageHandler<PlayerDamageInfoMessage>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DamageInfoMessageHandler));
        private UiContext _uiContext;
        private PlayerContext _playerContext;
        public DamageInfoMessageHandler(PlayerContext playerContext, UiContext uiContext)
        {
            _uiContext = uiContext;
            _playerContext = playerContext;
        }

        public override void DoHandle(int messageType, PlayerDamageInfoMessage messageBody)
        {
            Logger.DebugFormat("Handle message {0} {1} {2} {3}", messageBody.EntityId, messageBody.Damage, messageBody.PosX, messageBody.PosZ);
            var selfPlayer = _playerContext.flagSelfEntity;
            if (null == selfPlayer)
            {
                Logger.Error("self player is null");
                return;
            }
            if (!selfPlayer.hasCameraObj)
            {
                Logger.Error("player has no camera obj");
                return;
            }
            if (!selfPlayer.hasPosition)
            {
                Logger.Error("player has no position");
                return;
            }
            var forword = selfPlayer.cameraObj.MainCamera.transform.forward;
            var forwordxz = new Vector2(forword.x, forword.z);
            var myPos = selfPlayer.position.Value;
            var damageSrcPos = new Vector3(messageBody.PosX, 0, messageBody.PosZ);
            var dir = damageSrcPos - myPos;
            var dirxz = new Vector2(dir.x, dir.z);
            var cross = dirxz.x * forwordxz.y - dirxz.y * forwordxz.x; 
            var angle = Vector2.Angle(dirxz, forwordxz) * -Mathf.Sign(cross);
            Logger.InfoFormat("myPos {0} srcPos {1} camForward {2} Angle is {3}",myPos, damageSrcPos, forword, angle);
            _uiContext.uI.HurtedDataList[messageBody.EntityId] = new CrossHairHurtedData(messageBody.Damage, angle, new Vector2(damageSrcPos.x, damageSrcPos.z));
        }
    }
}