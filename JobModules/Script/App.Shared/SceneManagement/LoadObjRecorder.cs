using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Utils.AssetManager;
using Utils.Singleton;

namespace App.Shared.SceneManagement
{
    public class LoadObjRecorder:DisposableSingleton<LoadObjRecorder>
    {

        private Dictionary<int, List<GameObject>> _recorder = new Dictionary<int, List<GameObject>>();
        private Dictionary<int, int> _lastRecordeFrame = new Dictionary<int, int>();
        private int _recordNum = 20;
        
        public LoadObjRecorder()
        {
            for (int i = 0; i < _recordNum; i++)
            {
                var list = new List<GameObject>();
                _recorder.Add(i, list);
                _lastRecordeFrame[i] = 0;
            }
        }
        
        protected override void OnDispose()
        {
            for (int i = 0; i < _recordNum; i++)
            {
                _recorder[i].Clear();
            }
            _recorder.Clear();
            _lastRecordeFrame.Clear();
        }

        public void Restore(GameObject obj, int frame)
        {
            if (obj == null) return;
            var recordFrame = _lastRecordeFrame[frame % _recordNum];
            var recordMessage = _recorder[frame % _recordNum];
            if (frame != recordFrame)
            {
                recordMessage.Clear();
            }
            recordMessage.Add(obj);
            _lastRecordeFrame[frame % _recordNum] = frame;
        }

        public string ReadRecord()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" UnityObj loaded in last 5 frames: \n");
            int sum = 0;
            for (int i = 0; i < _recordNum; i++)
            {
                sb.Append(_lastRecordeFrame[i]);
                sb.Append("recordColliderLine : \t");
                foreach (var obj in _recorder[i])
                {
                    sb.Append(obj.name);
                    sb.Append("(");
                    var num = obj.GetComponentsInChildren<Collider>(true).Length;
                    sb.Append(num);
                    sb.Append(")\t");
                    sum += num;
                }
                sb.Append("\n");
            }
            sb.Append("sum of collider: " + sum);
            return sb.ToString();
        }
    }
}