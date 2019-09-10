using System;
using App.Shared.Player;
using Core.Utils;
using System.Collections.Generic;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.GameModules.UserInput
{
    public class PointerProvider : AbstractProvider<PointerKeyHandler, PointerData>
    {
        LoggerAdapter _logger = new LoggerAdapter(typeof(PointerProvider));

        private List<UserInputKey> _resultList = new List<UserInputKey>();
        private int _raycastMask;
        private float _maxRaycastDistance;

        private PlayerContext player;

        public PointerProvider(Contexts contexts = null)
        {
            if(null != contexts) player = contexts.player;
        }

        protected override void SetRaycastTargetData(KeyData data, RayCastTarget target)
        {
            var pointerData = data as PointerData;
            if (null == pointerData)
            {
                return;
            }

            pointerData.Data = target.Data;
            pointerData.IdList = target.IdList;
        }

        private KeyData MakeKeyData(UserInputKey key, float axis, Vector3 pos)
        {
            var data = dataPool.GetData();
            data.Key = key;
            data.Position = pos;
            data.MouseX = Input.GetAxis("Mouse X");
            data.MouseY = Input.GetAxis("Mouse Y");
            return data;
        }

        public override void SetConfig(InputProviderConfig config)
        {
            var layerNames = config.RaycastLayer.Trim();
            var layerNameArray = layerNames.Split(',');
            _raycastMask = 0;
            for (int i = 0; i < layerNameArray.Length; i++)
            {
                var layer = LayerMask.NameToLayer(layerNameArray[i]);
                _raycastMask |= 1 << layer;
            }

            _maxRaycastDistance = config.RaycastMaskDistance;
        }

        private readonly RaycastHit[] results = new RaycastHit[64];
        private readonly List<RaycastHit> ignoreHit = new List<RaycastHit>();

        protected override void DoCollect()
        {
            foreach (var data in collection)
            {
                var userInputKeys = GetKeyList(data);
                foreach (var inputKey in userInputKeys)
                {
                    var keys = keyConverter.Convert(inputKey);
                    foreach (var keyConvertItem in keys)
                    {
                        var state = keyConvertItem.State;
                        //移动消息直接响应
                        if (state == UserInputState.PointerMove)
                        {
                            KeyDatas.Enqueue(MakeKeyData(inputKey, 0, Input.mousePosition));
                            break;
                        }

                        if (state == UserInputState.PointerRaycast)
                        {
                            ignoreHit.Clear();
                            RaycastHit hitTarget;
                            if (!RaycastSonar(keyConvertItem.FloatParam, out hitTarget, inputKey)) continue;
                            var target = hitTarget.transform.GetComponentInParent<RayCastTarget>();
                            if (null == target) continue;
                            var pointerData = MakeKeyData(inputKey, 0, hitTarget.point);
                            SetRaycastTargetData(pointerData, target);
                            KeyDatas.Enqueue(pointerData);
                        }
                    }
                }
            }
        }

        private bool RaycastSonar(float distance, out RaycastHit hitTarget, UserInputKey inputKey)
        {
            try
            {

           
            PlayerEntity playerEntity = player.flagSelfEntity;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var headRay = new Ray(playerEntity.position.Value, playerEntity.RootGo().transform.forward);
            var angle = 15;
            if (playerEntity.appearanceInterface.Appearance.IsFirstPerson)
                angle = 35;
            float angleCos = Mathf.Cos(Mathf.Deg2Rad * angle);
            GameObject resultObj = null;
            float maxDot = 0f;

            Collider[] cols = Physics.OverlapSphere(ray.origin, distance, _raycastMask);
            foreach (Collider col in cols)
            {
                float dot = dirDot(ray, col.transform.position);
                if (dot < angleCos) continue;
                float headDot = dirDot(headRay, col.transform.position);
                if (headDot < 0) continue;
                var rcTar = col.gameObject.GetComponentInParent<RayCastTarget>();
                if (null == rcTar || !rcTar.KeyList.Contains(inputKey)) continue;
                if (null == resultObj) {
                    resultObj = col.gameObject;
                    maxDot = dot;
                }
                else
                {
                    if(dot > maxDot)
                    {
                        resultObj = col.gameObject;
                        maxDot = dot;
                    }
                }
            }

            if (null != resultObj)
            {
                Vector3 dir = resultObj.transform.position - ray.origin;
                if (Physics.Raycast(ray.origin, dir, out hitTarget, dir.magnitude, _raycastMask))
                {
                    if (hitTarget.transform.gameObject == resultObj)
                    {
                        return true;
                    }
                    else
                    {
                        var rcTar = hitTarget.transform.GetComponentInParent<RayCastTarget>();
                        if (null != rcTar && rcTar.KeyList.Contains(inputKey))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            }
            catch (Exception e)
            {
               _logger.ErrorFormat("{0}",e );
            }
            hitTarget = new RaycastHit();
            return false;
        }

        private float dirDot (Ray ray, Vector3 pos)
        {
            Vector3 dir = pos - ray.origin;
            return Vector3.Dot(dir.normalized, ray.direction);
        }
    }
}