using App.Shared;
using App.Shared.Components.FreeMove;
using Assets.Sources.Free.Auto;
using Assets.Sources.Free.UI;
using Core.EntityComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.EntityComponent;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Auto
{
    class AutoScaleValue : IAutoValue
    {
        private bool started;

        private int id;

        public bool Started { get { return started; } }

        public object Frame(int frameTime)
        {
            if (started)
            {
                FreeMoveEntity move = SingletonManager.Get<FreeUiManager>().Contexts1.freeMove.GetEntityWithEntityKey(new EntityKey(id, (int)EEntityType.FreeMove));
                if (move != null &&
                    move.hasFreeData)
                {
                    return new Vector3(move.freeData.ScaleX, move.freeData.ScaleY, move.freeData.ScaleZ);
                }
            }

            return null;
        }

        public IAutoValue Parse(string config)
        {
            AutoScaleValue auto = new AutoScaleValue();
            var ss = config.Split('|');
            if (ss.Length >= 2 && ss[0] == "scale")
            {
                auto.id = int.Parse(ss[1]);
            }

            return auto;
        }

        public void SetValue(params object[] value)
        {
        }

        public void Start()
        {
            started = true;
        }

        public void Stop()
        {
            started = false;
        }
    }
}
