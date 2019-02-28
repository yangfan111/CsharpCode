using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.unit;
using Sharpen;
using UnityEngine;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [Serializable]
    public abstract class AbstractFreeMove : IFreeMove
    {
        protected IMoveSpeed speed;

        protected IPosSelector startPos;

        protected int iniSpeed;

        [NonSerialized]
        protected long startTime;

        protected IMoveSpeed GetSpeed(IEventArgs args, FreeMoveEntity entity)
        {
            if (speed == null)
            {
                speed = new AlwaysOneSpeed((float)iniSpeed / 100f);
            }

            return speed;
        }

        [NonSerialized]
        private UnitPosition _entityPosition;

        protected UnitPosition GetEntityPosition(FreeMoveEntity entity)
        {
            if (_entityPosition == null)
            {
                _entityPosition = new UnitPosition();
            }

            _entityPosition.SetX(entity.position.Value.x);
            _entityPosition.SetY(entity.position.Value.y);
            _entityPosition.SetZ(entity.position.Value.z);

            return _entityPosition;
        }

        public abstract bool Frame(FreeRuleEventArgs args, FreeMoveEntity entity, int interval);

        public virtual void Start(FreeRuleEventArgs args, FreeMoveEntity entity)
        {
            startTime = args.Rule.ServerTime;

            UnitPosition up = startPos.Select(args);
            entity.position.Value = new Vector3(up.GetX(), up.GetY(), up.GetZ());

            StartMove(args, entity);
        }

        public abstract void StartMove(FreeRuleEventArgs args, FreeMoveEntity entity);

    }
}
