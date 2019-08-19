using Assets.Sources.Free.Utility;
using System;
using App.Shared;
using Assets.Sources.Free.Data;
using Assets.Sources.Free.UI;
using Core.Components;
using Core.EntityComponent;
using Core.EntityComponent;
using UnityEngine;
using Utils.Singleton;
using Object = System.Object;

namespace Assets.Sources.Free.Auto
{
    public class AutoPositionValue : IAutoValue
    {

        //        private IEntitySelector selector;

        private int id;
        private float distance;
        private float height;
        private float pitch;
        private float _angle;

        private bool started;


        public Object Frame(int frameTime)
        {
            var v = new Vector3();

            //            var battleModel = GameModelLocator.getInstance().gameModel;
            //
            //            var ce = battleModel.getClientEntityFormId(id);
            //
            //            if (ce == null)
            //            {
            //                ce = battleModel.getCurrentSelfClientEntity();
            //            }
            //
            //            var angle = ce.lerpAngle.x + battleModel.shakeAngleOffect.x;
            var battleModel = GameModelLocator.GetInstance().GameModel;
            PlayerEntity player = SingletonManager.Get<FreeUiManager>().Contexts1.player.GetEntityWithEntityKey(new EntityKey(id, (int)EEntityType.Player));
            FreeMoveEntity move = SingletonManager.Get<FreeUiManager>().Contexts1.freeMove.GetEntityWithEntityKey(new EntityKey(id, (int)EEntityType.FreeMove));
           
            //if (player == null && move == null)
            //{
            //    player = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;
            //}

            var angle = this._angle;
            if (player != null)
            {
                angle = player.orientation.Yaw + this._angle + battleModel.shakeAngleOffect.x;
            }

            var dir = new Vector3();

            var end = new Vector3();


            FreeMathUtility.AnglesToVector(angle, pitch, ref dir);

            PositionComponent pos = null;
            if (player != null)
            {
                pos = player.position;
            }

            if (move != null)
            {
                if (move.hasPosition)
                {
                    pos = move.position;
                }
            }

            if (pos != null)
            {
                FreeMathUtility.Vector3DMA(pos.Value, distance, dir, ref end);

                v.x = end.x;
                v.y = end.y + height;
                v.z = end.z;
            }
            else
            {
                return null;
            }

            return v;
        }

        public bool Started
        {
            get
            {
                return started;
            }
        }

        public IAutoValue Parse(string config)
        {
            var ss = config.Split("|");
            if (ss.Length >= 6 && ss[0] == "position")
            {
                var at = new AutoPositionValue();
                at.id = Convert.ToInt32(ss[1]);
                at.distance = Convert.ToSingle(ss[2]);
                at.height = Convert.ToSingle(ss[3]);
                at.pitch = Convert.ToSingle(ss[4]);
                at._angle = Convert.ToSingle(ss[5]);

                return at;
            }
            return null;
        }

        public void Start()
        {

            this.started = true;

        }

        public void Stop()
        {

            this.started = false;

        }

        public void SetValue(params object[] ini)
        {

        }
    }
}
