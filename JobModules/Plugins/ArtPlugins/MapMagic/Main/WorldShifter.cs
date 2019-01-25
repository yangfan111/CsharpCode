
// based on the works of Peter Stirling and Tony Lovell
// http://wiki.unity3d.com/index.php/Floating_Origin

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MapMagic
{

	public static class WorldShifter
	{
		public static void Update (int shiftThreshold)
		{
			if (Camera.main == null) return;
			Vector3 camPos = Camera.main.transform.position;
			
			if (camPos.x < -shiftThreshold) { ShiftObjects(shiftThreshold, 0); ShiftParticles(shiftThreshold, 0); }
			if (camPos.x > shiftThreshold) { ShiftObjects(-shiftThreshold, 0); ShiftParticles(-shiftThreshold, 0); }
			if (camPos.z < -shiftThreshold) { ShiftObjects(0, shiftThreshold); ShiftParticles(0, shiftThreshold); }
			if (camPos.z > shiftThreshold) { ShiftObjects(0, -shiftThreshold); ShiftParticles(0, -shiftThreshold); }	
		}

		public static void ShiftObjects (float x, float z)
		{
			//in case of moving camera in scene view (or changing pos vars)
			#if UNITY_EDITOR
			UnityEditor.Selection.objects = new Object[0];
			UnityEditor.EditorGUI.FocusTextInControl("");
			#endif

			#if UNITY_5_5_OR_NEWER
			GameObject[] allObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
			for (int i=0; i<allObjects.Length; i++)
			{
				Vector3 oldPos = allObjects[i].transform.position;
				allObjects[i].transform.position = new Vector3(oldPos.x+x, oldPos.y, oldPos.z+z);
			}			
			#else
			Transform[] allTransforms = GameObject.FindObjectsOfType<Transform>();
            for (int i=0; i<allTransforms.Length; i++)
			{
				Transform tfm = allTransforms[i];
                if (tfm.parent == null)
				{
					Vector3 oldPos = tfm.position;
                    tfm.position = new Vector3(oldPos.x+x, oldPos.y, oldPos.z+z);
				}
            }
			#endif
		}

		public static void ShiftParticles (float x, float z)
		{
			ParticleSystem[] allParticles = GameObject.FindObjectsOfType<ParticleSystem>();	

			ParticleSystem.Particle[] tempParticles = null;

			for (int i=0; i<allParticles.Length; i++)
            {
                ParticleSystem particles = allParticles[i];

				#if UNITY_5_5_OR_NEWER
				if (particles.main.simulationSpace != ParticleSystemSimulationSpace.World) continue;

				int maxParticles = particles.main.maxParticles;
				if (maxParticles <= 0) continue;
				#else
				if (particles.simulationSpace != ParticleSystemSimulationSpace.World) continue;

				int maxParticles = particles.maxParticles;
				if (maxParticles <= 0) continue;
				#endif

				//pausing
				bool wasPaused = particles.isPaused;
				bool wasPlaying = particles.isPlaying;
				if (!wasPaused) particles.Pause();

				//shifting particles
				if (tempParticles==null || tempParticles.Length < maxParticles) tempParticles = new ParticleSystem.Particle[maxParticles];

				int numParticles = particles.GetParticles(tempParticles);
				for (int j=0; j<numParticles; j++) 
				{
					Vector3 oldPosition = tempParticles[j].position;
					tempParticles[j].position = new Vector3(oldPosition.x+x, oldPosition.y, oldPosition.z+z);
				}
				particles.SetParticles(tempParticles, numParticles);

				//resuming
				if (wasPlaying) particles.Play();
			}
		}


	}

}//namespace
