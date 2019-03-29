using App.Shared.Components.Player;
using Core.Statistics;


namespace App.Client.GameModules.Ui.UiAdapter
{
    public class TechStatUiAdapter : UIAdapter,ITechStatUiAdapter
    {
        private Contexts contexts;

        public TechStatUiAdapter(Contexts contexts)
        {
            this.contexts = contexts;
        }

        public BattleData BattleData
        {
            get
            {
                return contexts.player.flagSelfEntity.statisticsData.Battle;
            }
        }

        public override bool IsReady()
        {
            return null != contexts.player.flagSelfEntity && contexts.player.flagSelfEntity.hasStatisticsData;
        }

        public override bool Enable
        {
            get
            {
                if (!IsDead())
                {
                    return false;
                }
                else
                {
                    return base.Enable;
                }
            }
        }

        private bool IsDead()
        {
            if (contexts.player == null) return false;
            return contexts.player.flagSelfEntity.gamePlay.IsDead() && contexts.player.flagSelfEntity.gamePlay.CameraEntityId == 0;
        }
    }
}
