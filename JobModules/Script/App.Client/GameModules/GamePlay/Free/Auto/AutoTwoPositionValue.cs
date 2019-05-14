using System;
using App.Shared;
using Assets.Sources.Free.UI;
using Assets.Sources.Free.Utility;
using Core.EntityComponent;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using Object = System.Object;

namespace Assets.Sources.Free.Auto
{
    public class AutoTwoPositionValue : IAutoValue
    {


        private int source;
        private int target;
        private float distance;
        private float height;
        private bool toSource;

        private bool started;


        public Object Frame(int frameTime)
        {
            var v = new Vector3();

            //            var battleModel:BattleModel = GameModelLocator.getInstance().gameModel;

            //			var ceS:ClientEntity = battleModel.getClientEntityFormId(source);
            //			var ceT:ClientEntity = battleModel.getClientEntityFormId(target);
            //			
            //			if(ceS == null){
            //				ceS = battleModel.getCurrentSelfClientEntity();
            //			}
            //			
            //			if(ceT == null){
            //				ceT = battleModel.getCurrentSelfClientEntity();
            //			}

            var ceS = SingletonManager.Get<FreeUiManager>().Contexts1.player.GetEntityWithEntityKey(new EntityKey(source, (int)EEntityType.Player));
            if (ceS == null)
                ceS = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;

            var ceT = SingletonManager.Get<FreeUiManager>().Contexts1.player.GetEntityWithEntityKey(new EntityKey(target, (int)EEntityType.Player));
            if (ceT == null)
                ceT = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;

            var sub = ceT.position.Value - ceS.position.Value;
            if (toSource)
            {
                sub = ceS.position.Value - ceT.position.Value;
            }
            var av = new Vector3();
            FreeMathUtility.VectorToAngles(sub, ref av);
            var angle = av.x;

            var dir = new Vector3();
            var end = new Vector3();

            var mins = new Vector3();
            var maxs = new Vector3();
            mins.Set(-10, -10, -10);
            maxs.Set(10, 10, 10);

            FreeMathUtility.AnglesToVector(angle, 0, ref dir);

            if (toSource)
            {
                FreeMathUtility.Vector3DMA(ceS.position.Value, distance, dir, ref end);
            }
            else
            {
                FreeMathUtility.Vector3DMA(ceT.position.Value, distance, dir, ref end);
            }
            v.x = end.x;
            v.y = end.y + height;
            v.z = end.z;

            return v;
        }

        public IAutoValue Parse(string config)
        {
            var ss = config.Split("|");
            if (ss.Length >= 6 && ss[0] == "tposition")
            {
                var at = new AutoTwoPositionValue();
                at.source = Convert.ToInt32(ss[1]);
                at.target = Convert.ToInt32(ss[2]);
                at.distance = Convert.ToSingle(ss[3]);
                at.height = Convert.ToSingle(ss[4]);
                at.toSource = ss[5] == "true";

                return at;
            }
            return null;
        }

        public void Start()
        {

            this.started = true;

        }

        public bool Started
        {
            get
            {
                return started;
            }
        }

        public void Stop()
        {

            this.started = false;

        }

        public void SetValue(params object[] v)
        {

        }
    }
}
