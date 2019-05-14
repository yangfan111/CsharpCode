using App.Client.GameModules.Ui.UiAdapter.Interface.Common;
using App.Client.Utility;
using App.Shared;
using App.Shared.Components;
using Assets.Sources.Free.UI;
using Core.Free;
using Free.framework;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.UiAdapter.Common
{

    public class DeadUiAdapter : UIAdapter, IDeadUiAdapter
    {
        private Contexts _contexts;

        public DeadUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        public bool IsGameOver
        {
            get { return _contexts.ui.uI.GameResult != Core.Enums.EUIGameResultType.None; }
        }

        public bool DeadButtonShow
        {
            get
            {
                return GameRules.IsChicken(_contexts.session.commonSession.RoomInfo.ModeId ) && !IsGameOver;
            }
        }

        public bool HaveAliveTeammate
        {
            get
            {
                var teamInfoList = _contexts.ui.map.TeamInfos;
                foreach (var it in teamInfoList)
                {
                    if (!it.IsDead)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void BackToHall()
        {
            HallUtility.GameOver();
        }

        public void Observe()
        {
            SimpleProto data = FreePool.Allocate();
            data.Key = FreeMessageConstant.ObservePlayer;
            data.Ins.Add(_contexts.player.flagSelfEntity.entityKey.Value.EntityId);
            SingletonManager.Get<FreeUiManager>().Contexts1.session.clientSessionObjects.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, data);

            data = FreePool.Allocate();
            data.Key = FreeUIEvent.EVENT_CLICK;
            data.Ss.Add("ObservePlayer");
            SingletonManager.Get<FreeUiManager>().Contexts1.session.clientSessionObjects.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, data);
        }

        public void SetCrossVisible(bool isVisible)
        {
            _contexts.ui.uI.IsShowCrossHair = isVisible;
        }

        public override bool Enable
        {
            get
            {
                return base.Enable && IsDead();

            }
            set { base.Enable = value; }
        }

        private bool IsDead()
        {
            if (_contexts.player == null) return false;
            return _contexts.player.flagSelfEntity.gamePlay.IsDead() && _contexts.player.flagSelfEntity.gamePlay.CameraEntityId == 0;
        }
    }

}
