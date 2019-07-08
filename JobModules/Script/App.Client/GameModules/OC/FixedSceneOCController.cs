using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using Core.OC;
using OC;
using OC.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Client.GameModules.OC
{
    public class FixedOCParam: OCParam
    {
        public string SceneName;
    }

    public class FixedSceneOCController : IOcclusionCullingController
    {
        private string[] CommonSceneNames = {"ClientScene", "DontDestroyOnLoad"};

        private FixedOCParam _param;
        private SingleScene _scene;
        public FixedSceneOCController(OCParam param)
        {
            OcclusionRunningState.HasPVSData = true;
            OcclusionRunningState.OcclusionEnabled = SharedConfig.EnableCustomOC;

            _param = param as FixedOCParam;
            _scene = new SingleScene(String.Empty, _param.SceneName, Index.InValidIndex);
            _scene.Load(_param.OCData, false);
        }

        public void DoCulling(Vector3 position)
        {
            if (SharedConfig.EnableCustomOC)
            {
                _scene.DoCulling(position);
            }
            else
            {
                _scene.UndoCulling();
            }
        }

        public void Dispose()
        {
            
        }
    }
}
