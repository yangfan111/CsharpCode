using App.Client.GameModules.Terrain;
using App.Shared;
using UnityEngine;

namespace App.Client.Scripts.Scripts
{
    class ExitGame : MonoBehaviour
    {
        private string _text = "";

        void OnGUI()
        {
            if (!SharedConfig.IsShowTerrainTrace)
                return;

            if (GUI.Button(new Rect(200, 80, 100, 30), "退出游戏"))
            {
                var go = GameObject.Find("GameController");
                if (go == null)
                    return;
                var gameController = go.GetComponent("ClientGameController");
                if (gameController == null)
                    return;
                ((ClientGameController) gameController).OnGameOver();
            }

            _text = "Terrain:" + TerrainTestSystem.mapName + ",p" + TerrainTestSystem.pos + ",yaw:" + TerrainTestSystem.yaw + ",mark:" + TerrainTestSystem.mark + ",v"+ TerrainTestSystem.velocity + ",GF:" + TerrainTestSystem.gripFriction + ",DF:" + TerrainTestSystem.dragFriction + ",TId:" + TerrainTestSystem.textureId + ",tmCnt:" + TerrainTestSystem.teamCnt + ",tmNum:" + TerrainTestSystem.teamNum;
            GUI.Label(new Rect(320, 80, 850, 30), _text);
        }
    }
}
