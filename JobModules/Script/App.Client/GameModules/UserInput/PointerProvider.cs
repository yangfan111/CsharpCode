using System;
using System.Collections.Generic;
using System.Text;
using App.Shared.Player;
using Core.Utils;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.GameModules.UserInput
{
    public class UserPointerProvider : AbstractProvider<PointerReceiver, PointerData>
    {
        private readonly List<RaycastHit> ignoreHit = new List<RaycastHit>();

        private readonly RaycastHit[] results = new RaycastHit[64];
        LoggerAdapter _logger = new LoggerAdapter(typeof(UserPointerProvider));
        private float _maxRaycastDistance;
        private int _raycastMask;

        private List<UserInputKey> _resultList = new List<UserInputKey>();

        private PlayerContext player;

        public UserPointerProvider(Contexts contexts)
        {
            if (null != contexts) player = contexts.player;
        }


        protected override void SetRaycastTargetData(KeyData data, RayCastTarget target)
        {
            var pointerData = data as PointerData;
            if (null == pointerData)
            {
                return;
            }

            pointerData.Data   = target.Data;
            pointerData.IdList = target.IdList;
        }

        private KeyData MakeKeyData(UserInputKey key,Vector3 pos)
        {
            PointerData data = dataPool.GetData();
            data.Key      = key;
            data.Position = pos;
            data.MouseX   = Input.GetAxis("Mouse X");
            data.MouseY   = Input.GetAxis("Mouse Y");
            return data;
        }

        public override void SetConfig(InputProviderConfig config)
        {
            var layerNames     = config.RaycastLayer.Trim();
            var layerNameArray = layerNames.Split(',');
            _raycastMask = 0;
            for (int i = 0; i < layerNameArray.Length; i++)
            {
                var layer = LayerMask.NameToLayer(layerNameArray[i]);
                _raycastMask |= 1 << layer;
            }

            _maxRaycastDistance = config.RaycastMaskDistance;
        }
        StringBuilder s = new StringBuilder();

        protected override void DoCollect()
        {
            
            foreach (var data in collection)
            {
                s.Length = 0;
                List<UserInputKey> userInputKeys = GetKeyList(data);
                // foreach (var inputKey in userInputKeys)
                // {
                //     s.Append(inputKey + " ");
                // }
             //   DebugUtil.MyLog("[{0}]{1}",Time.frameCount,s.ToString());
                foreach (UserInputKey inputKey in userInputKeys)
                {
                    var keys = keyConverter.Convert(inputKey);
                    foreach (var keyConvertItem in keys)
                    {
                        var state = keyConvertItem.State;
                        //移动消息直接响应
                        if (state == UserInputState.PointerMove)
                        {
                            KeyDatas.Enqueue(MakeKeyData(inputKey, Input.mousePosition));
                            break;
                        }

                        if (state == UserInputState.PointerRaycast)
                        {
                            ignoreHit.Clear();
                            RaycastHit hitTarget;
                            if (!RaycastSonar(keyConvertItem.FloatParam, out hitTarget, inputKey)) continue;
                            var target = hitTarget.transform.GetComponentInParent<RayCastTarget>();
                            if (null == target) continue;
                            var pointerData = MakeKeyData(inputKey, hitTarget.point);
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
                var          ray          = Camera.main.ScreenPointToRay(Input.mousePosition);
                var headRay =
                                new Ray(playerEntity.position.Value, playerEntity.RootGo().transform.forward);
                var angle = 15;
                if (playerEntity.appearanceInterface.Appearance.IsFirstPerson)
                    angle = 35;
                float      angleCos  = Mathf.Cos(Mathf.Deg2Rad * angle);
                GameObject resultObj = null;
                float      maxDot    = 0f;

                Collider[] cols = Physics.OverlapSphere(ray.origin, distance, _raycastMask);
                foreach (Collider col in cols)
                {
                    float dot = dirDot(ray, col.transform.position);
                    if (dot < angleCos) continue;
                    float headDot = dirDot(headRay, col.transform.position);
                    if (headDot < 0) continue;
                    var rcTar = col.gameObject.GetComponentInParent<RayCastTarget>();
                    if (null == rcTar || !rcTar.KeyList.Contains(inputKey)) continue;
                    if (null == resultObj)
                    {
                        resultObj = col.gameObject;
                        maxDot    = dot;
                    }
                    else
                    {
                        if (dot > maxDot)
                        {
                            resultObj = col.gameObject;
                            maxDot    = dot;
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

                        var rcTar = hitTarget.transform.GetComponentInParent<RayCastTarget>();
                        if (null != rcTar && rcTar.KeyList.Contains(inputKey))
                        {
                            return true;
                        }

                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("{0}", e);
            }

            hitTarget = new RaycastHit();
            return false;
        }

        private float dirDot(Ray ray, Vector3 pos)
        {
            Vector3 dir = pos - ray.origin;
            return Vector3.Dot(dir.normalized, ray.direction);
        }
    }
}