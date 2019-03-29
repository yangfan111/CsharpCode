using App.Client.GameModules.GamePlay.Free.Scripts;
using App.Shared;
using App.Shared.Configuration;
using Assets.Sources.Free.UI;
using Core.GameModule.Interface;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay
{
    public class TestFrameSystem : IGamePlaySystem
    {
        static LoggerAdapter logger = new LoggerAdapter("FrameTest");

        private Contexts contexts;

        private int startX;
        private int startY;
        private int step;
        private long lastTime;

        private int state;

        private int x;
        private int y;

        private int max;
        private int tryTime;
        private int maxTry;
        private int tryFrame;

        private int moveTime;
        private int waitTime;

        private string testData;

        private bool running;

        public TestFrameSystem(Contexts contexts)
        {
            this.contexts = contexts;
            this.startX = -100;
            this.startY = -100;
            this.step = 10;
            this.max = 20;

            this.moveTime = 300;
            this.waitTime = 100;
        }

        public void OnGamePlay()
        {
            if (testData != SharedConfig.runTestRame)
            {
                testData = SharedConfig.runTestRame;
                if (string.IsNullOrEmpty(SharedConfig.runTestRame))
                {
                    this.running = false;
                }
                else
                {
                    if (contexts.session.clientSessionObjects.NetworkChannel.IsConnected)
                    {
                        contexts.session.clientSessionObjects.NetworkChannel.Disconnect();
                    }
                    string[] ss = SharedConfig.runTestRame.Split(',');
                    if (ss.Length >= 4)
                    {
                        this.startX = int.Parse(ss[0]);
                        this.startY = int.Parse(ss[1]);
                        this.step = int.Parse(ss[2]);
                        this.max = int.Parse(ss[3]);
                        this.x = 0;
                        this.y = 0;
                        this.running = true;

                        if(ss.Length >= 5)
                        {
                            this.maxTry = int.Parse(ss[4]);
                        }
                        else
                        {
                            this.maxTry = 5;
                        }
                        if(ss.Length >= 6)
                        {
                            this.tryFrame = int.Parse(ss[5]);
                        }
                        else
                        {
                            this.tryFrame = 40;
                        }
                    }
                }

            }

            if (y > max || contexts.player.flagSelfEntity == null || !running)
            {
                return;
            }
            switch (state)
            {
                case 0:
                    Act(0);
                    break;
                case 1:
                    Act(moveTime);
                    break;
                case 2:
                    Act(0);
                    break;
                case 3:
                    Act(waitTime);
                    break;
                case 4:
                    Act(0);
                    break;
                case 5:
                    Act(waitTime);
                    break;
                case 6:
                    Act(0);
                    break;
                case 7:
                    Act(waitTime);
                    break;
                case 8:
                    state = 0;
                    x++;
                    if (x > max)
                    {
                        x = 0;
                        y++;
                    }
                    break;
            }
        }

        private void Act(int time)
        {
            if (DateTime.Now.Ticks - lastTime > 10000 * time)
            {
                switch (state)
                {
                    case 0:
                        MoveTo(0);
                        break;
                    case 1:
                        Record();
                        break;
                    case 2:
                        MoveTo(90);
                        break;
                    case 3:
                        Record();
                        break;
                    case 4:
                        MoveTo(180);
                        break;
                    case 5:
                        Record();
                        break;
                    case 6:
                        MoveTo(270);
                        break;
                    case 7:
                        Record();
                        break;
                }

                lastTime = DateTime.Now.Ticks;
            }
        }

        private void Record()
        {
            //Debug.LogFormat("record {0}", state);
            ClientFPSUpdater fps = SingletonManager.Get<ClientFPSUpdater>();

            PlayerEntity player = SingletonManager.Get<FreeUiManager>().Contexts1.player.flagSelfEntity;

            if (fps.Fps < tryFrame && tryTime < maxTry)
            {
                tryTime++;
            }
            else
            {

#if UNITY_EDITOR
                logger.InfoFormat("{0},{1},{2},{3},{4} at ({5},{6}) with Yaw {7} atfer try {8}", fps.Fps, UnityStats.triangles, UnityStats.vertices, UnityStats.drawCalls, UnityStats.batches,
                    Math.Round(player.position.Value.x), Math.Round(player.position.Value.z), Math.Round(player.orientation.Yaw), tryTime);
#else
            logger.InfoFormat("{0} at ({1},{2}) with Yaw {3} after try {4}", fps.Fps, Math.Round(player.position.Value.x), Math.Round(player.position.Value.z), Math.Round(player.orientation.Yaw), tryTime);
#endif
                tryTime = 0;
                state++;
            }


        }

        private void MoveTo(int yaw)
        {

            //Debug.LogFormat("move {0}", yaw);
            state++;
            PlayerEntity p = contexts.player.flagSelfEntity;
            p.orientation.Pitch = 0;
            if (yaw != 0)
            {
                p.orientation.Yaw = yaw;
            }
            else
            {
                int currentX = startX + x * step;
                int currentY = startY + y * step;

                Vector3 fromV = new Vector3(currentX, 1000, currentY);

                Vector3 toV = new Vector3(currentX, -1000, currentY);

                Ray r = new Ray(fromV, new Vector3(toV.x - fromV.x, toV.y - fromV.y, toV.z - fromV.z));

                RaycastHit hitInfo;
                bool hited = Physics.Raycast(r, out hitInfo);

                if (hited)
                {
                    if (SingletonManager.Get<MapConfigManager>().InWater(new Vector3(hitInfo.point.x,
                                hitInfo.point.y + 0.1f, hitInfo.point.z)))
                    {
                        float sur = SingletonManager.Get<MapConfigManager>().DistanceAboveWater(fromV);
                        if (sur > 0)
                        {
                            hitInfo.point = new Vector3(fromV.x, fromV.y - sur, fromV.z);
                        }
                    }

                    p.position.Value = hitInfo.point;
                }
                else
                {
                    p.position.Value = new Vector3(currentX, 0, currentY);
                }
                p.orientation.Yaw = yaw;
            }
        }
    }
}
