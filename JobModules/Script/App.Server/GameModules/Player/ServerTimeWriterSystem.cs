using App.Shared.Util;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Server.GameModules.Player
{
    
    ///
    /// <summary>
    /// 服务端动画模块更新服务端时间
    /// <a href="http://192.168.0.6:8090/pages/viewpage.action?pageId=14299301#id-%E8%A7%92%E8%89%B2%E7%9B%B8%E5%85%B3%E4%BB%A3%E7%A0%81-4%E5%8A%A8%E7%94%BB%E7%8A%B6%E6%80%81%E6%9C%BA(Fsm)">link</a>
    /// </summary>
    /// 
    /// <stage>
    /// 命令执行
    /// </stage>
    /// <author>
    /// yzx
    /// </author>
    /// 
    public class ServerTimeWriterSystem : AbstractUserCmdExecuteSystem
    {
        private readonly Contexts _contexts;
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ServerTimeWriterSystem));

        public ServerTimeWriterSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        protected override bool filter(PlayerEntity playerEntity)
        {
            return playerEntity.hasNetworkAnimator && playerEntity.hasNetworkAnimatiorServerTime;
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            if (playerEntity.networkAnimator.NeedChangeServerTime)
            {
                //Logger.InfoFormat("server set base serverTime, name:{0}, seq:{1}, currentTime:{2}, prevTIme:{3}", playerEntity.entityKey.ToString(),
                //    cmd.Seq,
                //    _serverSession.sessionObjects.CurrentTime,
                //    playerEntity.networkAnimator.BaseServerTime);
                playerEntity.networkAnimator.BaseServerTime = _contexts.session.currentTimeObject.CurrentTime;
                playerEntity.networkAnimatiorServerTime.ServerTime = _contexts.session.currentTimeObject.CurrentTime;
            }
            else
            {
                playerEntity.networkAnimator.BaseServerTime = playerEntity.networkAnimatiorServerTime.ServerTime;
            }
        }
    }
}