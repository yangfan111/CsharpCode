using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.Components.Ui;
using App.Shared.Terrains;
using Assets.Sources.Free.UI;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class SuoDuUiAdapter : UIAdapter, ISuoDuUiAdapter
    {
        private Contexts _contexts;
        private Vector3 _playerPos;

        public SuoDuUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
            _playerPos = new Vector3(0, 0, 0);
        }

        public Vector3 _CurPosition
        {
            get
            {
                Vector3 pos = _contexts.player.flagSelfEntity.position.Value;
                _playerPos.Set(pos.x - TerrainCommonData.leftMinPos.x, pos.y, pos.z - TerrainCommonData.leftMinPos.z);
                return _playerPos;
            }
        }

        public DuQuanInfo CurDuquan   //当前毒圈数据
        {   get
            {
                var map = SingletonManager.Get<FreeUiManager>().Contexts1.ui.map;
                return map.CurDuquan;
            }
        }

        public DuQuanInfo NextDuquan   //下一个毒圈数据
        {
            get
            {
                var map = SingletonManager.Get<FreeUiManager>().Contexts1.ui.map;
                return map.NextDuquan;
            }
        }

        public int OffLineNum { get { return _contexts.ui.map.OffLineLevel; } }
    }
}