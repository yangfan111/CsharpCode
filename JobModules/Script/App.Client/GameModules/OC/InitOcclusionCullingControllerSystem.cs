using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Configuration;
using Core.GameModule.Interface;
using Core.OC;
using Core.SessionState;
using UnityEngine;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Client.GameModules.OC
{
    public class InitOcclusionCullingControllerSystem : IModuleInitSystem
    {
        private static readonly string OCDataAssetBundleName = "oc";
        private ISessionState _sessionState;
        private Contexts _contexts;
        private LevelType _levelType;
        private AssetInfo _ocDataAssetInfo;
        private OCParam _ocParam;
        public InitOcclusionCullingControllerSystem(ISessionState sessionState, Contexts contexts)
        {

            _contexts = contexts;
            _sessionState = sessionState;
            _sessionState.CreateExitCondition(typeof(InitOcclusionCullingControllerSystem));


            var mapDesc = SingletonManager.Get<MapsDescription>();

            _levelType = mapDesc.CurrentLevelType;
            switch (_levelType)
            {
                case LevelType.SmallMap:
                    _ocDataAssetInfo = new AssetInfo(OCDataAssetBundleName, mapDesc.SmallMapParameters.AssetName + "_oc");
                    _ocParam = new FixedOCParam()
                    {
                        OCData = null,
                        SceneName = mapDesc.SmallMapParameters.AssetName,
                    };
                    break;
                case LevelType.BigMap: 
                    _ocDataAssetInfo = new AssetInfo(OCDataAssetBundleName, String.Format(mapDesc.BigMapParameters.TerrainNamePattern, "oc", "oc") + "_oc");
                    var initPosition = _contexts.session.commonSession.InitPosition;

                    _ocParam = new StreamOCParam()
                    {
                        OCData = null,
                        InitPosition = initPosition,
                        TerrainMin = mapDesc.BigMapParameters.TerrainMin,
                        TerrainDimension = mapDesc.BigMapParameters.TerrainDimension,
                        TerrainSize = mapDesc.BigMapParameters.TerrainSize,
                        TerrainNamePattern = mapDesc.BigMapParameters.TerrainNamePattern,

                        UnloadRadiusInGrid = 1.5f,
                        LoadRadiusInGrid = 2.0f,

                        LevelManager =  _contexts.session.commonSession.LevelManager,
                    };
                    break;
                default:
                    _ocDataAssetInfo = AssetInfo.EmptyInstance;
                    break;
            }
        }

        public void OnInitModule(IUnityAssetManager assetManager)
        {
            assetManager.LoadAssetAsync("InitOcclusionCullingControllerSystem", _ocDataAssetInfo, OnOCDataLoaded);
        }

        private void OnOCDataLoaded(string source, UnityObject unityObj)
        {
            var ocData = unityObj.As<TextAsset>();
            _ocParam.OCData = ocData == null ? null : ocData.bytes;

            var ocController = OcclisionCullingControllerFactory.CreateController(_levelType, _ocParam);
            _contexts.session.clientSessionObjects.OCController = ocController;

            _sessionState.FullfillExitCondition(typeof(InitOcclusionCullingControllerSystem));
        }

    }
}
