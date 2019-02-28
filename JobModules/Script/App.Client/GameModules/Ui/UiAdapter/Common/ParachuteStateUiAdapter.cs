using App.Client.GameModules.GamePlay.Free.UI;
using App.Shared;
using UnityEngine;
using App.Shared.Configuration;
using App.Shared.Terrains;
using Assets.Sources.Free.Effect;
using Core.Utils;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class ParachuteStateUiAdapter : UIAdapter, IParachuteStateUiAdapter
    {
        private Contexts contexts;
        private IMyTerrain _myTerrain;
        private float _curTerrainHeight;
        private bool _DEBUGTRACE = false;
        private FreeRenderObject _plane;
        private float _maxDistance = 10000;

        public ParachuteStateUiAdapter(Contexts contexts)
        {
            this.contexts = contexts;
            _myTerrain = SingletonManager.Get<TerrainManager>().GetCurrentTerrain();
        }

        public override bool Enable
        {
            get { return IsDrop && base.Enable; }
            set { base.Enable = value; }
        }

        public bool IsDrop
        {
            get
            {
                return MinMapUpdater.PlayerIsDrop(contexts.player.flagSelfEntity);
            }
        }

//        public bool PlayerIsDrop(PlayerEntity playerEntity)
//        {
//            if (null == playerEntity)
//                return false;
//            if (SharedConfig.IsOffline)
//            {
//                return playerEntity.hasStateInterface && (playerEntity.stateInterface.State.GetActionState() == ActionInConfig.Gliding
//                    || playerEntity.stateInterface.State.GetActionState() == ActionInConfig.Parachuting);
//            }
//            else
//            {
//                return playerEntity.hasGamePlay && (playerEntity.gamePlay.GameState == GameState.Gliding
//                    || playerEntity.gamePlay.GameState == GameState.JumpPlane);
//            }
//        }

        public float TerrainHeight
        {
            get
            {
                Vector3 fromV = contexts.player.flagSelfEntity.characterContoller.Value.transform.position;
                Vector3 toV = new Vector3(fromV.x, fromV.y - _maxDistance, fromV.z);

                Ray r = new Ray(fromV, new Vector3(toV.x - fromV.x, toV.y - fromV.y, toV.z - fromV.z));

                RaycastHit hitInfo;
                var layerMask = UnityLayers.SceneCollidableLayerMask;
                bool hited = Physics.Raycast(r, out hitInfo, _maxDistance, layerMask);

                if (hited)
                {
                    _curTerrainHeight = hitInfo.point.y - TerrainCommonData.leftMinPos.y;
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
                float speed = contexts.player.flagSelfEntity.playerMove.Velocity.y * 3.6f;
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
                float height = contexts.player.flagSelfEntity.characterContoller.Value.transform.position.y - TerrainCommonData.leftMinPos.y;
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
                    height = SingletonManager.Get<MapConfigManager>().SceneParameters.PlayerBirthPosition.y - TerrainCommonData.leftMinPos.y;
                }
                else
                {
                    if (null == _plane)
                    {
                        _plane = SingletonManager.Get<FreeEffectManager>().GetEffect("plane");
                    }
                    if (null != _plane)
                    {
                        height = -_plane.model3D.y - TerrainCommonData.leftMinPos.y;
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
