using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using XmlConfig;

namespace App.Shared.Components.Ui
{
    [Ui, Unique]
    public class MapComponent : IComponent
    {
        [DontInitilize] public MiniMapTeamPlayInfo CurPlayer;   //当前玩家
        [DontInitilize] public List<MiniMapTeamPlayInfo> TeamInfos;

        [DontInitilize] public DuQuanInfo CurDuquan; //当前毒圈
        [DontInitilize] public DuQuanInfo NextDuquan;  //下一个毒圈
        [DontInitilize] public BombAreaInfo BombArea; //轰炸区

        [DontInitilize] public AirPlaneData PlaneData;

        [DontInitilize] public List<TeamPlayerMarkInfo> TeamPlayerMarkInfos;
        [DontInitilize] public Dictionary<long, MiniMapPlayMarkInfo> MapMarks;
        public void AddMapMark(long playerId, int playerNum, float mx, float my)
        {
            RemoveMapMark(playerId);
            MapMarks.Add(playerId, new MiniMapPlayMarkInfo(new Vector2(mx, my), playerNum));
        }
        public void RemoveMapMark(long playerId)
        {
            if (MapMarks.ContainsKey(playerId))
            {
                MapMarks.Remove(playerId);
            }
        }


        [DontInitilize] public bool IsShowRouteLine;  //是否显示航线
        [DontInitilize] public Vector2 RouteLineStartPoint;    //跳伞开始位置
        [DontInitilize] public Vector2 RouteLineEndPoint;      //跳伞结束位置


        [DontInitilize] public int OffLineLevel; //离线模式下的一个level
    }
}