using System;
using Assets.Sources.Free.UI;
using Assets.Sources.Free.Utility;
using Core.EntityComponent;
using App.Shared;
using Core.Utils;
using Utils.Singleton;

namespace Assets.Sources.Free.Auto
{
    public class AutoPlayerValue : IAutoValue
    {
        private bool _started;
        private int _currentValue;

        private string _field;

        private int _id;

        private bool _idChange;


        public object Frame(int frameTime)
        {
            return GetPlayerField();
        }

        private int GetPlayerField()
        {

            var playerEntity = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;
            if (playerEntity == null)
            {
                FreeMoveEntity move = SingletonManager.Get<FreeUiManager>().Contexts1.freeMove.GetEntityWithEntityKey(new EntityKey(_id, (int)EEntityType.FreeMove));
                if(move != null)
                {
                    if("scale" == _field)
                    {
                        if (move.hasFreeData)
                        {

                        }
                    }
                }
                return 0;
            }
            else
            {
                if ("speed" == _field)
                {
                    var speedVector3D = playerEntity.playerMove.Velocity;
                    speedVector3D.z = 0;
                    return (int) speedVector3D.magnitude;
                }
                if ("ping" == _field)
                {
                    return PlayerStatus.GetPing((int)_id);
                }

                if (_field == "hp")
                    return (int)playerEntity.gamePlay.CurHp;
                if (_field == "maxHp")
                    return playerEntity.gamePlay.MaxHp;
                if (_field == "maxSpeed")
                    return 0;
                if (_field == "x")
                    return (int)playerEntity.position.Value.x;
                if (_field == "y")
                    return (int)playerEntity.position.Value.y;
                if (_field == "z")
                    return (int)playerEntity.position.Value.z;
                if (_field == "pitch")
                    return (int)playerEntity.orientation.Pitch;
                if (_field == "yaw")
                    return (int)playerEntity.orientation.Yaw;

                return 0;
            }
        }

        public bool Started
        {
            get
            {
                return _started;
            }
        }

        public IAutoValue Parse(string config)
        {
            var ss = config.Split("|");

            if (ss.Length >= 4 && ss[0] == "player")
            {
                var at = new AutoPlayerValue();
                at._id = int.Parse(ss[1]);
                at._field = ss[2];
                at._idChange = "true" == ss[3];

                return at;
            }

            return null;
        }

        public void Start()
        {
            _started = true;
        }

        public void Stop()
        {

            _started = false;

        }

        public void SetValue(params object[] v)
        {
            if (_idChange && Convert.ToInt32(v[0]) > 0)
                _id = Convert.ToInt32(v[0]);
            else
                _currentValue = Convert.ToInt32(v[0]);
        }
    }
}
