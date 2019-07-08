using App.Client.GameModules.GamePlay.Free.UI;
using App.Shared;
using UnityEngine;
using App.Shared.Configuration;
using App.Shared.Terrains;
using Assets.Sources.Free.Effect;
using Core.Components;
using Core.Ui.Map;
using Core.Utils;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class ParachuteStateUiAdapter : UIAdapter, IParachuteStateUiAdapter
    {
        private Contexts _contexts;
        private IMyTerrain _myTerrain;
        private float _curTerrainHeight;
        private bool _DEBUGTRACE = false;
        private FreeRenderObject _plane;
        private float _maxDistance = 10000;

        private PlayerEntity Player
        {
            get { return _contexts.ui.uI.Player; }
        }


        public ParachuteStateUiAdapter(Contexts contexts)
        {
            this._contexts = contexts;
            _myTerrain = SingletonManager.Get<TerrainManager>().GetCurrentTerrain();
        }

        public override bool Enable
        {
            get { return IsDrop && base.Enable; }
        }

        public bool IsDrop
        {
            get
            {
                return MinMapUpdater.PlayerIsDrop(Player);
            }
        }


        public float TerrainHeight
        {
            get
            {
                if (!Player.hasCharacterContoller) return 0;
                Vector3 fromV = Player.characterContoller.Value.transform.position;
                Vector3 toV = new Vector3(fromV.x, fromV.y - _maxDistance, fromV.z);

                Ray r = new Ray(fromV, new Vector3(toV.x - fromV.x, toV.y - fromV.y, toV.z - fromV.z));

                RaycastHit hitInfo;
                var layerMask = UnityLayers.SceneCollidableLayerMask;
                bool hited = Physics.Raycast(r, out hitInfo, _maxDistance, layerMask);

                if (hited)
                {
                    _curTerrainHeight = hitInfo.point.y - MapOrigin.Origin.y;
                }
                else
                {
                    _curTerrainHeight = 0;
                }
                if (_DEBUGTRACE)
                    Debug.LogFormat("TerrainHeight.....{0}", _curTerrainHeight);
                return _curTerrainHeight;
            }
        }

        public float DropSpeed
        {
            get
            {
                if (!Player.hasPlayerMove) return 0;
                float speed = Player.playerMove.Velocity.y * 3.6f;
	            speed = Mathf.Abs(speed);
				if (_DEBUGTRACE)
                    Debug.LogFormat("DropSpeed.....{0}", speed);
                return speed;
            }
        }

        public float CurHeight
        {
            get
            {
                if (!Player.hasCharacterContoller) return 0;
                var pos = Player.characterContoller.Value.transform.position;
                float height = new MapFixedVector3(WorldOrigin.WorldPosition(pos)).ShiftedUIVector3().y;
                if (_DEBUGTRACE)
                    Debug.LogFormat("CurHeight.....{0}", height);
                return height;
            }
        }

        public float TotalHeight
        {
            get
            {
                float height = 0;
                if (SharedConfig.IsOffline)
                {
                    height = SingletonManager.Get<MapConfigManager>().SceneParameters.PlayerBirthPosition.y - MapOrigin.Origin.y;
                }
                else
                {
                    if (null == _plane)
                    {
                        _plane = SingletonManager.Get<FreeEffectManager>().GetEffect("plane");
                    }
                    if (null != _plane)
                    {
//                        height = -_plane.model3D.y - MapOrigin.Origin.y;
                        height = new MapFixedVector3(WorldOrigin.WorldPosition(new Vector3(_plane.model3D.x, -_plane.model3D.y,
                            _plane.model3D.z))).ShiftedUIVector3().y;
                    }
                }
                
                if (_DEBUGTRACE)
                    Debug.LogFormat("TotalHeight.....{0}", height);
                return height;
            }
        }

        public float ForcedHeight
        {
            get
            {
	            float height = SingletonManager.Get<CharacterStateConfigManager>().SkyMoveConfig.MinParachuteHeight; //+ TerrainHeight;//- _myTerrain.InitPosition.y;
                if (_DEBUGTRACE)
                    Debug.LogFormat("ForcedHeight.....{0}", height);
                return height; 
            }
        }
    }
}
