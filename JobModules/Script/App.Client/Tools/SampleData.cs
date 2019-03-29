using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Client.Tools
{
    /// <summary>
    /// 记录采样数据
    /// </summary>
    public class SampleData
    {
        private int _x;
        private int _y;
        private float _frameRate;

        private int _batches;
        private int _setPassCalls;
        private int _triangles;
        private int _vertices;
        private int _shadowCaster;
        private float _frameTime;
        private float _renderTime;

        private float _camDirX;
        private float _camDirY;
        private float _camDirZ;

        private float _camUpX;
        private float _camUpY;
        private float _camUpZ;

        private float _camPosX;
        private float _camPosY;
        private float _camPosZ;

        private float _camFOV;
        private float _camNear;
        private float _camFar;

        private int _camOC;
        private int _camHDR;
        private int _camMSAA;

        public SampleData(int posX, int posZ, float frameRate, int batches, int setPassCalls, int triangles, int vertices,
            Vector3 camDir, Vector3 camUp, Vector3 camPos, float camFOV, float camNear, float camFar, int camOC, int camHDR,
            int camMSAA, int shadowCaster, float frameTime, float renderTime)
        {
            _x = posX;
            _y = posZ;
            _frameRate = frameRate;
            _batches = batches;
            _setPassCalls = setPassCalls;
            _triangles = triangles;
            _vertices = vertices;
            _camDirX = camDir.x;
            _camDirY = camDir.y;
            _camDirZ = camDir.z;
            _camUpX = camUp.x;
            _camUpY = camUp.y;
            _camUpZ = camUp.z;
            _camPosX = camPos.x;
            _camPosY = camPos.y;
            _camPosZ = camPos.z;
            _camFOV = camFOV;
            _camNear = camNear;
            _camFar = camFar;
            _camOC = camOC;
            _camHDR = camHDR;
            _camMSAA = camMSAA;
            _shadowCaster = shadowCaster;
            _frameTime = frameTime;
            _renderTime = renderTime;
        }

        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }

        public float FrameRate
        {
            get { return _frameRate; }
        }

        public int Batches
        {
            get { return _batches; }
        }

        public int SetPassCalls
        {
            get { return _setPassCalls; }
        }

        public int Triangles
        {
            get { return _triangles; }
        }

        public int Vertices
        {
            get { return _vertices; }
        }

        public float CamDirX
        {
            get { return _camDirX; }
        }

        public float CamDirY
        {
            get { return _camDirY; }
        }

        public float CamDirZ
        {
            get { return _camDirZ; }
        }

        public float CamUpX
        {
            get { return _camUpX; }
        }

        public float CamUpY
        {
            get { return _camUpY; }
        }

        public float CamUpZ
        {
            get { return _camUpZ; }
        }

        public float CamPosX
        {
            get { return _camPosX; }
        }

        public float CamPosY
        {
            get { return _camPosY; }
        }

        public float CamPosZ
        {
            get { return _camPosZ; }
        }

        public float CamFOV
        {
            get { return _camFOV; }
        }

        public float CamNear
        {
            get { return _camNear; }
        }

        public float CamFar
        {
            get { return _camFar; }
        }

        public int camOC
        {
            get { return _camOC; }
        }

        public int camHDR
        {
            get { return _camHDR; }
        }

        public int camMSAA
        {
            get { return _camMSAA; }
        }

        public int shadowCaster
        {
            get { return _shadowCaster; }
        }

        public float frameTime
        {
            get { return _frameTime; }
        }

        public float renderTime
        {
            get { return _renderTime; }
        }
    }
}
