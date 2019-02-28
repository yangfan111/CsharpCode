using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.SessionState;

namespace App.Client.Tools
{
    /// <summary>
    /// 最小渲染集合系统，用于禁用掉所有不可见物体测试性能提上限
    /// </summary>
    public class MinRendererSetSystem : AbstractStepExecuteSystem
    {
        public MinRendererSetSystem(Contexts contexts)
        {
            GameObject gameController = GameObject.Find("GameController");
            MinRendererSetSampler sampler = gameController.AddComponent<MinRendererSetSampler>();
            sampler.playerContext = contexts.player;
        }

        protected override void InternalExecute() { }
    }
}
