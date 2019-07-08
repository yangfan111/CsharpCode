using App.Client.GameModules.Ui.UiAdapter;
using App.Shared.Components.Ui;
using App.Shared.Terrains;
using Assets.Sources.Free.UI;
using Core.Ui.Map;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class SuoDuUiAdapter : UIAdapter, ISuoDuUiAdapter
    {
        private Contexts _contexts;
        private MapFixedVector3 _playerPos;

        public SuoDuUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
            _playerPos = new MapFixedVector3(0, 0, 0);
        }

        public MapFixedVector3 CurPosition
        {
            get
            {
                _playerPos.Set(_contexts.ui.uI.Player.position.FixedVector3);
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