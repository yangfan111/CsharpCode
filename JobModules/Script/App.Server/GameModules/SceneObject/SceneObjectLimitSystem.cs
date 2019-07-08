using App.Shared;
using App.Shared.Components;
using Core.GameModule.Interface;
using Entitas;
using System.Collections.Generic;

namespace App.Server.GameModules.SceneObject
{
    public class SceneObjectLimitSystem : IGamePlaySystem
    {
        private Contexts _contexts;
        private IGroup<SceneObjectEntity> _group;
        private List<SceneObjectEntity> _list;

        private SceneObjectEntityComparer _comparer = new SceneObjectEntityComparer();
        private readonly int _maxCount = 40;
        private long _lastTime = 0L;
        private const long CheckLimitTime = 500L;

        public SceneObjectLimitSystem(Contexts contexts)
        {
            _contexts = contexts;
            _group = contexts.sceneObject.GetGroup(SceneObjectMatcher.AllOf(SceneObjectMatcher.WeaponObject, SceneObjectMatcher.LifeTime)
                .NoneOf(SceneObjectMatcher.FlagDestroy));
            _list = new List<SceneObjectEntity>(128);
        }

        public void OnGamePlay()
        {
            if (GameRules.IsChicken(_contexts.session.commonSession.RoomInfo.ModeId)) return;
            if (_contexts.session.commonSession.FreeArgs.Rule.ServerTime - _lastTime < CheckLimitTime) return;

            _list.Clear();
            _list.AddRange(_group.GetEntities());

            if (_list.Count > _maxCount)
            {
                _list.Sort(_comparer);
                for (var i = 0; i < _list.Count - _maxCount; i++)
                {
                    if(!WeaponUtil.IsC4p(_list[i].weaponObject.ConfigId)) //不回收C4
                        _list[i].isFlagDestroy = true;
                }
            }

            _lastTime = _contexts.session.commonSession.FreeArgs.Rule.ServerTime;
        }
    }

    public class SceneObjectEntityComparer : IComparer<SceneObjectEntity>
    {
        public int Compare(SceneObjectEntity x, SceneObjectEntity y)
        {
            return x.lifeTime.CreateTime.CompareTo(y.lifeTime.CreateTime);
        }
    }
}
