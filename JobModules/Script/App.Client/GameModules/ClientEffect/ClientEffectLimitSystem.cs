using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.Network;
using Entitas;
using UnityEngine;

namespace App.Client.GameModules.ClientEffect
{
    public class ClientEffectEntityComparer : IComparer<ClientEffectEntity>
    {
        public int Compare(ClientEffectEntity x, ClientEffectEntity y)
        {
            return x.lifeTime.CreateTime.CompareTo( y.lifeTime.CreateTime);
        }
    }

    public class ClientEffectLimitSystem:IGamePlaySystem
    {
        private IGroup<ClientEffectEntity> _group;
        private List<ClientEffectEntity> _list = new List<ClientEffectEntity>();
        private ClientEffectEntityComparer _comparer = new ClientEffectEntityComparer();
        private readonly int _maxCount = 30;
        private float _lastTime = 0;
        private const float CheckLimitTime = 0.2f;
        public ClientEffectLimitSystem(Contexts contexts)
        {
            _group = contexts.clientEffect.GetGroup(ClientEffectMatcher.AllOf(ClientEffectMatcher.Assets, ClientEffectMatcher.LifeTime)
                .NoneOf(ClientEffectMatcher.FlagDestroy));
            _list = new List<ClientEffectEntity>(128);
        }

        public void OnGamePlay()
        {
            var now = Time.time;
            if (now - _lastTime > CheckLimitTime)
            {
                _lastTime = now;
                var es = _group.GetEntities();
                _list.Clear();
                _list.AddRange(es);
                if (_list.Count > _maxCount)
                {
                   
                    _list.Sort(_comparer);
                    for (var i = 0; i < _list.Count - _maxCount; i++)
                    {
                        _list[i].isFlagDestroy = true;
                    }
                }
            }

        }
    }
}