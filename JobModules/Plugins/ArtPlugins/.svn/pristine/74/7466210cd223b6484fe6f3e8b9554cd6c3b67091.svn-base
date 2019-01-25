using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Utils.Appearance
{
    public class BlinkEye : MonoBehaviour
    {
        private Transform eyeBone;

        public float blinkScale = 40;                        //幅度
        public float blinkPeriod = 1.0f;                       //周期
        public float waitTimeBase = 1;                          //最小等待时间
        public float waitTimeRandom = 2;                        //等待时间波动

        public AnimationCurve curve;
        public bool IsActive;
        public Vector3 initEularAngle;
        public string boneName = "BlinkLocator";                    //眨眼用骨骼
        public string recordFile = "C:\\Users\\Administrator\\Desktop\\KeyValue.txt";    //关键帧输出文件

        public bool IsBlinking = false;                              //眨眼状态
        public float nextTime;                                      //下次状态更新时间
        public float curStartTime;                                  //当前眨眼起始时间

        void Start()
        {
            foreach (var child in transform.GetComponentsInChildren<Transform>())
            {
                if (child.name == boneName)
                {
                    eyeBone = child;
                    IsActive = true;
                    break;
                }
            }

            if (!IsActive)
            {
                this.enabled = false;
                //Debug.Log("Can't find BlinkLocator");
                return;
            }

            initEularAngle = eyeBone.transform.localEulerAngles;

            curve = new AnimationCurve();

            AddKey();
        }

        private void AddKey()
        {
            for (int i = 0; i < keyNode.Length / 4; i++)
            {
                //Debug.Log(i + ": " + keyNode[i, 0]);
                curve.AddKey(keyNode[i, 0], keyNode[i, 1]);
                var ck = curve.keys[i];
                ck.inTangent = keyNode[i, 2];
                ck.outTangent = keyNode[i, 3];
                curve.MoveKey(i, ck);
            }
        }

        public void PlayerDead()
        {
            Vector3 curR = initEularAngle;
            curR.x += blinkScale;
            eyeBone.transform.localEulerAngles = curR;
            this.enabled = false;
        }

        public void PlayerRelive()
        {
            this.enabled = true;
        }
        
        private void PrintKey()
        {
            StreamWriter sw = new StreamWriter(recordFile, true);
            sw.WriteLine(" ");
            foreach (var V in curve.keys)
            {
                sw.WriteLine("{ " + V.time + "f ," + V.value + "f ," + V.inTangent + "f ," + V.outTangent + "f },");
            }
            sw.WriteLine(" ");
            sw.Flush();
            sw.Close();
        }

        void Update()
        {
            if (Time.time >= nextTime)
            {
                if (IsBlinking) nextTime = Time.time + Random.Range(waitTimeBase, waitTimeBase + waitTimeRandom);
                else
                {
                    curStartTime = Time.time;
                    nextTime = curStartTime + blinkPeriod;
                }
                IsBlinking = !IsBlinking;
            }
            else
            {
                if (IsBlinking)
                {
                    float curAngle = curve.Evaluate((Time.time - curStartTime) / blinkPeriod) * blinkScale;
                    Vector3 curR = initEularAngle;
                    curR.x += curAngle;
                    eyeBone.transform.localEulerAngles = curR;
                    //PrintKey();
                }
            }
        }

        private static float[,] keyNode = new float[,]
        {
            { 0f ,0f ,36.23363f ,36.23363f },
            { 0.06f ,1.000001f ,-0.002670818f ,-0.002670818f },
            { 0.15f ,-9.536743E-06f ,-1.834811f ,-1.834811f },
            { 0.25f ,0f ,-2.663457f ,-2.663457f }
        };
    }
}