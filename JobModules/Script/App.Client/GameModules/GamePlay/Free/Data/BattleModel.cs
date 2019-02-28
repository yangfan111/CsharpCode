using App.Client.GameModules.Free.Data;
using UnityEngine;

namespace Assets.Sources.Free.Data
{
    public class BattleModel
    {
        public ProfitModelWrapper ProfitModel;
        public RoomData RoomData;
        public bool IsMatch;
        public bool isObserver;
        public Vector3 shakeAngleOffect;

        public BattleModel()
        {
            ProfitModel = new ProfitModelWrapper();
        }
    }
}
