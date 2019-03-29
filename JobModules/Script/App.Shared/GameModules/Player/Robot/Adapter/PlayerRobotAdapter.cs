using System.Collections.Generic;
using App.Shared.Components.Player;
using App.Shared.Player;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.App.Shared.GameModules.Player.Robot.Adapter
{
    class PlayerRobotAdapter : IRobotPlayerWapper
    {
        public PlayerRobotAdapter(Entitas.IContexts contexts, PlayerEntity entity, NavMeshAgent navMeshAgent,
            IRobotUserCmdProvider robotUserCmdProvider, IRobotSpeedInfo robotSpeedInfo,
            IUserCmdGenerator userCmdGenerator, IRobotConfig robotConfig)
        {
            NavMeshAgent = navMeshAgent;
            Entity = entity;
            RobotUserCmdProvider = robotUserCmdProvider;
            RobotSpeedInfo = robotSpeedInfo;
            UserCmdGenerator = userCmdGenerator;
            RobotConfig = robotConfig;
            GameContexts = contexts;
        }

        public NavMeshAgent NavMeshAgent { get; private set; }
        public PlayerEntity Entity { get; private set; }
        public IRobotUserCmdProvider RobotUserCmdProvider { get; private set; }
        public IRobotSpeedInfo RobotSpeedInfo { get; private set; }
        public IUserCmdGenerator UserCmdGenerator { get; private set; }
        public IRobotConfig RobotConfig { get; private set; }
        public Vector3 Destination { get; set; }

        public GameObject TargetInSight(float value, float f, HashSet<GameObject> gameObjects)
        {
            return null;
        }

        public Entitas.IContexts GameContexts { get; private set; }
        public bool LastIsOnGround { get; set; }

        public int LastLifeState
        {
            get { return Entity.gamePlay.LifeState; }
        }

        public bool IsOnGround
        {
            get { return Entity.playerMove.IsGround; }
        }

        public int LifeState
        {
            get { return Entity.gamePlay.LifeState; }
        }

        public void StopNavMeshAgent()
        {
            if (NavMeshAgent.isOnNavMesh)
                NavMeshAgent.isStopped = true;
        }

        public IRobotWeaponStat GetCurrentWeaponStat()
        {
            throw new System.NotImplementedException();
        }

      

       

        public bool CanFire()
        {
            return true;
        }
        NavMeshHit m_NavMeshHit;
        RaycastHit hit;
        public bool LineOfSight(Vector3 position, PlayerEntity target, bool checkposition)
        {
            if ((position - target.position.Value).magnitude < 0.5) return true;

            var bonePosition = target.bones.Head;
            var offset = Vector3.zero;
            offset = target.RootGo().transform.InverseTransformPoint(target.cameraObj.FPCamera
                .transform.position);
            if (checkposition)
            {
                if (!NavMesh.SamplePosition(position, out m_NavMeshHit, 2f, NavMesh.AllAreas)) return false;
            }
            else
            {
                Physics.Linecast(position + offset, bonePosition.position, out hit, UnityLayerManager.GetLayerMask(EUnityLayerName.Player));
            }


            return true;
        }

        public Vector3 Position
        {
            get { return Entity.position.Value; }
        }
    }
}