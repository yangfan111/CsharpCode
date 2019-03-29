using System;
using System.Linq;
using Core.GameModule.Step;
using UnityEngine;
using Utils.Singleton;

namespace Core.Utils
{
    public class VersionDisplay : MonoBehaviour
    {
        private bool _display = false;
        private bool _lastButtonStat = false;
        private float _lastTime;
        private GUIStyle _bb = new GUIStyle();

        public VersionDisplay()
        {
          
        }

        void Update()
        {
            SingletonManager.Get<DurationHelp>().Update();
        }

        string[] _labels = new string[6];

        void OnGUI()
        {
            if (Input.GetKey(KeyCode.H))
            {
                if (!_lastButtonStat)
                    _display = !_display;
                _lastButtonStat = true;
            }
            else
            {
                _lastButtonStat = false;
            }

            _lastTime += Time.deltaTime;
            if (_display)
            {
                if (_lastTime > 1)
                {
                    _lastTime = 0;
                    _labels[0] = string.Format("client :{0} asset;{1}", Core.Utils.Version.Instance.LocalVersion,
                        Core.Utils.Version.Instance.LocalAsset);;
                    _labels[1] = string.Format("server :{0} asset;{1}", Core.Utils.Version.Instance.RemoteVersion,
                        Core.Utils.Version.Instance.RemoteAsset);
                    _labels[2] = SingletonManager.Get<DurationHelp>().LastAvg;
                    _labels[3] = string.Format("cpuMax: {0}     Rewind:{1} time:{2}  pos:{3:N2} {4:N2} {5:N2}",
                        SingletonManager.Get<DurationHelp>().LastMax, SingletonManager.Get<DurationHelp>().RewindCount,
                        SingletonManager.Get<DurationHelp>().DriveTime,
                        SingletonManager.Get<DurationHelp>().Position.x,
                        SingletonManager.Get<DurationHelp>().Position.y,
                        SingletonManager.Get<DurationHelp>().Position.z
                        );
                    _labels[4] = string.Format( "{0}   Interval:{1} , Delta:{2} rTime:{3} sTime:{4} d:{5}",
                        StepExecuteManager.Instance.FpsString(), SingletonManager.Get<DurationHelp>().LastAvgInterpolateInterval, 
                        SingletonManager.Get<DurationHelp>().ServerClientDelta, SingletonManager.Get<DurationHelp>().RenderTime, SingletonManager.Get<DurationHelp>().LastServerTime, SingletonManager.Get<DurationHelp>().LastServerTime-SingletonManager.Get<DurationHelp>().RenderTime) ;
                    _labels[5] = string.Format("serverip: {0} ServerId: {1}",SingletonManager.Get<DurationHelp>().ServerInfo, SingletonManager.Get<Core.Utils.ServerInfo>().ServerId);
                    _bb.normal.background = null; //这是设置背景填充的
                    _bb.normal.textColor = new Color(1.0f, 1f, 1.0f); //设置字体颜色的
                    _bb.fontSize = 10; //当然，这是字体大小
                }

                var h = 0;
                var w = 300;
                GUI.Box(new Rect(w, h, 400, 70), "");
                //居中显示FPS
                for (int i = 0; i < _labels.Count(); i++)
                {
                    if (_labels[i] != null)
                        GUI.Label(new Rect(w + 5, h + 5 + i * 10, w + 400, h + 15 + i * 10), _labels[i], _bb);
                }
            }
        }
    }
}