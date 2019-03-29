using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Common;
using Core.GameModule.Interface;
using Core.Utils;
using NWH;
using Sharpen;
using UnityEngine;

namespace App.Client.ClientSystems
{
    public class RaycastTestSystem:IRenderSystem,IOnGuiSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RaycastTestSystem));
        private Contexts _contexts;
        GUIStyle _bb = new GUIStyle();
        List<string> _labels = new List<string>();
        private bool _display = false;
        private bool _lastButtonStat = false;
        private PlayerEntity _player;
        
        public RaycastTestSystem(Contexts contexts)
        {
            _contexts = contexts;
            
            _bb.normal.background = null; 
            _bb.normal.textColor = new Color(1.0f, 1f, 1.0f); 
            _bb.fontSize = 10; 
        }
        
        public void OnRender()
        {
            if (_player == null)
            {
                var playerEntity = _contexts.player.flagSelfEntity;
                if (playerEntity == null || !playerEntity.hasRaycastTest) return;
                _player = playerEntity;
            }

            var startPos = _player.cameraStateOutputNew.FinalArchorPosition;
            var startRot = Quaternion.Euler(_player.cameraStateOutputNew.ArchorEulerAngle +
                                            _player.cameraStateOutputNew.EulerAngle).Forward();
            var _testDistance = _player.raycastTest.Distance;
            
            _player.raycastTest.clear();
            
            RaycastHit[] hits;
            hits = Physics.RaycastAll(startPos, startRot, _testDistance, ~0);
            if(hits.Length<=0) return;

            _player.raycastTest.Num = hits.Length;
            
            foreach (var hit in hits)
            {
                RecordDetectObj(hit.collider.gameObject);
            }

        }

        public void OnGUI()
        {
            if (Input.GetKey(KeyCode.T))
            {
                if (!_lastButtonStat)
                    _display = !_display;
                _lastButtonStat = true;
            }
            else
            {
                _lastButtonStat = false;
            }

            ShowDetectedMessage();
        }

        private void ShowDetectedMessage()
        {
            if (!_display) return;
            
            _labels.Clear();
            if (_player == null) return;

            foreach (var item in _player.raycastTest.MapObjects)
            {
                _labels.Add(GetGameObjScriptMessage(item));
            }

            var h = 0;
            var w = Screen.width - 400;
            GUI.Box(new Rect(w - 4, h - 4, 400, 120), "");
            //居中显示FPS
            for (int i = 0; i < _labels.Count; i++)
            {
                if (_labels[i] != null)
                    GUI.Label(new Rect(w, h + i * 10, w + 400, h + 10 + i * 10), _labels[i], _bb);
            }
        }

        private string GetGameObjScriptMessage(GameObject gameObject)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append(gameObject.name);
            sb.Append("\n");
            
            sb.Append("\tReference\n");
            var reference = gameObject.GetComponent<EntityReference>();
            sb.Append("\t\t id:");
            sb.Append(reference.EntityKey.EntityId);
            sb.Append("\n\t\t Type:");
            sb.Append(reference.EntityKey.EntityType);
            sb.Append("\n");
            
            sb.Append("\tComponents:\n");
            var comps = gameObject.GetComponents<Component>();
            for(int i=0;i<comps.Length;i++)
            {
                sb.Append("\t\t");
                sb.Append(comps[i].GetType());
                sb.Append("\n");
            }

            return sb.ToString();
        }
        
        private void RecordDetectObj(GameObject gameObject)
        {
            var obj = gameObject.transform;
            while (obj.parent != null)
            {
                if (obj.GetComponent<EntityReference>() != null && !_player.raycastTest.MapObjects.Contains(obj.gameObject))
                {
                    _player.raycastTest.MapObjects.Add(obj.gameObject);
                    return;
                }
                obj = obj.parent;
            }
        }
    }
}