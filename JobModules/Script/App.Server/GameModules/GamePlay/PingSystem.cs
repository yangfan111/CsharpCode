using System.Threading;
using Entitas;
using UnityEngine;

namespace App.Server.GameModules.GamePlay
{
    public class PingSystem : IExecuteSystem
    {
        private readonly Contexts _contexts;
     

        public PingSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        private float _lastime;

        public void Execute()
        {
            var serverStatus = _contexts.session.serverSessionObjects.FpsSatatus;
            var time = Time.time;
            var delta = time - _lastime;
            _lastime = time;
            serverStatus.Tick(time, delta);
        }
    }
}