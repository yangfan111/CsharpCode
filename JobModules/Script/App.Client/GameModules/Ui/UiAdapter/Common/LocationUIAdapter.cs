using System.Collections.Generic;
using App.Shared.Components.Ui;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class LocationUiAdapter : UIAdapter, ILocationUiAdapter
    {
        private Contexts _contexts;
        public LocationUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
        }

        public float _CurFaceDirection
        {
            get
            {
                //Vector2 curDireciton = new Vector2(_contexts.player.flagSelfEntity.orientation.Yaw, _contexts.player.flagSelfEntity.orientation.Pitch);
                // return Vector2.Angle(Vector3.forward, curDireciton);
                CurFaceDirection = -_contexts.player.flagSelfEntity.orientation.Yaw;
                return CurFaceDirection;
            }         
            set
            {
                CurFaceDirection = value;
            }
        }                                             //玩家当前面朝向 0 到360度  
        private float CurFaceDirection;

//        {
//            new TeamPlayerMarkInfo(20.0f, Color.red),
//            new TeamPlayerMarkInfo(30.0f, Color.yellow),
//            new TeamPlayerMarkInfo(60.0f, Color.green),
//        };
        public List<TeamPlayerMarkInfo> TeamPlayerMarkInfos
        {
            get { return _contexts.ui.map.TeamPlayerMarkInfos; }
        }
    }
}