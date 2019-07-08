using App.Client.GameModules.Player;
using Core.GameModule.Interface;

namespace App.Client.GameModules.Throwing
{
    public class ThrowingCountDownSystem : AbstractSelfPlayerRenderSystem
    {
        private bool _isShow;
        private Contexts _contexts;

        public ThrowingCountDownSystem(Contexts contexts) : base(contexts)
        {
            _contexts = contexts;
        }

        protected override bool Filter(PlayerEntity playerEntity)
        {
            return playerEntity.hasThrowingAction && null != playerEntity.throwingAction.ActionData &&
                   _contexts.ui.uI != null;
        }

        public override void OnRender(PlayerEntity player)
            {
                var uidata = _contexts.ui.uI;

                if (player.throwingAction.ActionData.ShowCountdownUI)
                {
                    if (!_isShow)
                    {
                        _isShow = true;
//                        adapter.SetCountDown(true, (float)player.throwingAction.ActionInfo.CountdownTime / 1000);
                        uidata.CountingDown = true;
                        uidata.CountDownNum = (float) player.throwingAction.ActionData.CountdownTime / 1000;
                    }
                }
                else if (_isShow)
                {
                    _isShow = false;
//                    adapter.SetCountDown(false, 0);
                    uidata.CountingDown = false;
                    uidata.CountDownNum = 0;
                    uidata.HaveCompletedCountDown = true;
                }
            }
        }
}