using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Client.StartUp.Offline
{
    public class OfflineSimulationSystem : MonoBehaviour
    {
        private float timer;

        void Update()
        {
#if UNITY_2017
            if (Physics.autoSimulation)
                return;

            timer += Time.deltaTime;

            // Catch up with the game time.
            // Advance the physics simulation in portions of Time.fixedDeltaTime
            // Note that generally, we don't want to pass variable delta to Simulate as that leads to unstable results.
            while (timer >= Time.fixedDeltaTime)
            {
                timer -= Time.fixedDeltaTime;
                Physics.Simulate(Time.fixedDeltaTime);
            }
#endif
        }
    }
}
