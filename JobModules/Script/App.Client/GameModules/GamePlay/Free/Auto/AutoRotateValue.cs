using Assets.Sources.Free.Utility;
using System;
using App.Shared;
using Assets.Sources.Free.UI;
using Core.EntityComponent;
using Core.EntityComponent;
using Utils.Singleton;

namespace Assets.Sources.Free.Auto
{
    public class AutoRotateValue : IAutoValue
    {

        private bool started;
        private int id;
        private int angle;

        public Object Frame(int frameTime)
        {
            //            var battleModel = GameModelLocator.getInstance().gameModel;
            //            var ce:ClientEntity = battleModel.getClientEntityFormId(id);
            //
            //            if (ce == null)
            //            {
            //                ce = battleModel.getCurrentSelfClientEntity();
            //            }
            //
            //            return angle + PlayerEntity(ce.currentState).viewYaw;

            var player = SingletonManager.Get<FreeUiManager>().Contexts1.player.GetEntityWithEntityKey(new EntityKey(id, (int)EEntityType.Player));
            if (player == null)
            {
                player = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;
            }
            if (player == null)
                return (float)angle;

            return angle + player.orientation.Yaw;
        }

        public IAutoValue Parse(string config)
        {
            var ss = config.Split("|");
            if (ss.Length == 3 && ss[0] == "rotate")
            {
                var at = new AutoRotateValue();
                at.id = Convert.ToInt32(ss[1]);
                at.angle = Convert.ToInt32(ss[2]);

                return at;
            }

            return null;
        }

        public bool Started
        {
            get
            {
                return true;
            }
        }

        public void Start()
        {
            started = true;
        }

        public void Stop()
        {

            this.started = false;

        }

        public void SetValue(params object[] vs)
        {
        }

    }
}
