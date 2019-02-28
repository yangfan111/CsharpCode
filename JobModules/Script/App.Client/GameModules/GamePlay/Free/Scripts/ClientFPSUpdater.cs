using App.Client.GameModules.Free;
using Assets.Sources.Free;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Scripts
{
    public class ClientFPSUpdater : Singleton<ClientFPSUpdater>
    {
        public float fpsMeasuringDelta = 0.5f;

        private float timePassed;
        private int m_FrameCount;

        public int Fps;

//        // Use this for initialization
//        void Start()
//        {
//            timePassed = 0.0f;
//        }

        public void Update()
        {
            m_FrameCount = m_FrameCount + 1;
            timePassed = timePassed + Time.deltaTime;

            if (timePassed > fpsMeasuringDelta)
            {
                Fps = (int)(m_FrameCount / timePassed);

                timePassed = 0.0f;
                m_FrameCount = 0;
            }
        }
    }
}
