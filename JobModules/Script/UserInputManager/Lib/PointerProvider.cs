using Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UserInputManager.Lib
{
    public  class PointerProvider : AbstractProvider<PointerReceiver, PointerData>
    {
        LoggerAdapter _logger = new LoggerAdapter(typeof(PointerProvider));

        private int _raycastMask;
        private float _maxRaycastDistance;

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

        private KeyData MakeKeyData(UserInputKey key, Vector3 pos)
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
                            KeyDatas.Enqueue(MakeKeyData(inputKey, Input.mousePosition));
                            break;
                        }

                        if (state == UserInputState.PointerRaycast)
                        {
                            ignoreHit.Clear();
                            RaycastHit hitTarget;
                            if (!GetHitTarget(keyConvertItem.FloatParam, out hitTarget, inputKey)) continue;
                            var target = hitTarget.transform.GetComponent<RayCastTarget>();
                            if (null == target) continue;
                            //if (!target.KeyList.Contains(inputKey)) continue;
                            var pointerData = MakeKeyData(inputKey, hitTarget.point);
                            SetRaycastTargetData(pointerData, target);
                            KeyDatas.Enqueue(pointerData);
                        }
                    }
                }
            }
        }

        private bool GetHitTarget(float distance, out RaycastHit hitTarget, UserInputKey inputKey)
        {
            hitTarget = new RaycastHit();
            try
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int resultsCount = Physics.RaycastNonAlloc(ray, results, distance > 0.01f ? distance : _maxRaycastDistance, _raycastMask);

                if (resultsCount == 0)
                {
                    return false;
                }

                bool hasC4 = false;
                for (int i = 0; i < resultsCount; i++)
                {
                    var raycastHit = results[i];
                    var keyTarget = raycastHit.transform.GetComponent<RayCastTarget>();
                    //忽略不可操作的物体
                    if (keyTarget == null || !keyTarget.KeyList.Contains(inputKey))
                    {
                        ignoreHit.Add(raycastHit);
                    }
                    else
                    {
                        if (keyTarget.IdList.Count >= 3 && keyTarget.IdList[2] == 46)
                        {
                            hitTarget = raycastHit;
                            hasC4 = true;
                            break;
                        }
                        //忽略玩家装备的武器、配件的触发盒，避免玩家触发范围比实际大很多
                        if (raycastHit.collider.gameObject.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.Player) &&
                           raycastHit.collider.gameObject.name.ToLower().Equals("NormalCollider".ToLower()))
                        {
                            ignoreHit.Add(raycastHit);
                        }
                    }
                }

                if (ignoreHit.Count == resultsCount)
                {
                    return false;
                }

                if (!hasC4)
                {
                    for (int i = 0; i < resultsCount; i++)
                    {
                        var raycastHit = results[i];
                        if (!ignoreHit.Contains(raycastHit))
                        {
                            if (hitTarget.distance <= 0f || IsFirstCloserToSecond(raycastHit, hitTarget))
                                hitTarget = raycastHit;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("{0}",e);
                return false;
            }

            return true;
        }

        private bool IsFirstCloserToSecond(RaycastHit hit1, RaycastHit hit2)
        {
            /*float dis1 = Vector3.Distance(hit1.point, hit1.transform.position);
            float dis2 = Vector3.Distance(hit2.point, hit2.transform.position);
            return dis1 < dis2;*/
            return hit1.distance < hit2.distance;
        }
    }
}