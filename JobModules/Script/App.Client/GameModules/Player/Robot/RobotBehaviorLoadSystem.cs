using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared;
using App.Shared.GameModules.Player.Robot;
using App.Shared.Player;
using BehaviorDesigner.Runtime;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using Entitas.VisualDebugging.Unity;
using UnityEngine;
using UnityEngine.AI;

namespace App.Client.GameModules.Player.Robot
{
    class RobotBehaviorLoadSystem : ReactiveResourceLoadSystem<PlayerEntity>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RobotBehaviorLoadSystem));

        private Contexts _contexts;

        public RobotBehaviorLoadSystem(Contexts contexts) : base(contexts.player)
        {
            _contexts = contexts;
        }


        protected override ICollector<PlayerEntity> GetTrigger(IContext<PlayerEntity> context)
        {
            return context.CreateCollector(PlayerMatcher.Robot.Added());
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return entity.hasRobot;
        }


        public override void SingleExecute(PlayerEntity playerEntity)
        {
            AssetManager.LoadAssetAsync(playerEntity, AssetConfig.GetBehaviorAssetInfo(SharedConfig.RobotActionName),
                (behaviorSource, unityObj) =>
                {
                    var o = unityObj.AsGameObject;
                    
                    var playerRoot = ((PlayerEntity) behaviorSource).RootGo();
                    var b = o.GetComponent<BehaviorTree>().GetBehaviorSource();
                    _logger.ErrorFormat("b:{0}",b.behaviorName, b.TaskData.types.Count);
                    playerRoot.GetComponent<BehaviorTree>().SetBehaviorSource(b);
                    playerRoot.GetComponent<BehaviorTree>().GetBehaviorSource().Owner =
                        playerRoot.GetComponent<BehaviorTree>();
                    playerRoot.GetComponent<NavMeshAgent>().enabled = true;
                    playerRoot.GetComponent<BehaviorTree>().enabled = true;
                    GlobalVariables.Instance.SetVariableValue("GameContexts", _contexts);
                    o.DestroyGameObject();
                });
        }
    }
}