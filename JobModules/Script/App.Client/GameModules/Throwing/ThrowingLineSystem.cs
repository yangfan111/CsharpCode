using System.Collections.Generic;
using App.Client.GameModules.Player;
using App.Shared.GameModules.Player;
using Utils.AssetManager;
using Core.Utils;
using Core.WeaponLogic;
using UnityEngine;
using Utils.Utils;
using Core.WeaponLogic.Throwing;
using App.Shared.Components.Player;

namespace App.Client.GameModules.Throwing
{
    public class ThrowingLineSystem : AbstractSelfPlayerRenderSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ThrowingLineSystem));
        private Contexts _contexts;
        private bool _isAddLine;
        private bool _isDrawLine;

        private float _lineWidth = 0.3f;
        private float _deltaTime = (float) 1/20;
        private float _totalLength = 20;
        private float _beginCollLength = 0;

        private Mesh _throwingMesh;
        private List<Vector3> _Vertices;
        private List<Vector2> _Uvs;

        private GameObject _throwingSphere;
        private int _layerMask;
        private bool _assetLoaded;

        public ThrowingLineSystem(Contexts contexts):base(contexts)
        {
            _contexts = contexts;
            _layerMask = UnityLayers.AllCollidableLayerMask;
        }

        public void OnLoadSucc(string source, UnityObject unityObj)
        {
            {
                var asset = unityObj.As<Material>();
                if (null == asset)
                {
                    return;
                }
                _contexts.player.flagSelfEntity.throwingLine.Go.GetComponent<MeshRenderer>().material = asset;
                _assetLoaded = true;
            }
        }

        public void OnGoLoadSucc(string source, UnityObject unityObj)
        {
            {
                var obj = unityObj.AsGameObject;
                if (null == obj)
                {
                    return;
                }
                _contexts.player.flagSelfEntity.AddThrowingSphere(unityObj);
                _throwingSphere = obj;
                SetSphereActive(false);
            }
        }

        private void SetSphereActive(bool active)
        {
            if (null != _throwingSphere)
            {
                _throwingSphere.SetActive(active);
            }
        }

        private void SetSpherePos(Vector3 pos)
        {
            if (null != _throwingSphere)
            {
                _throwingSphere.transform.position = pos;
                _throwingSphere.SetActive(true);
            }
        }

        protected override bool Filter(PlayerEntity player)
        {
            return player.hasThrowingAction && null != player.throwingAction.ActionInfo
                                            && player.throwingAction.ActionInfo.IsReady
                                            && !player.throwingAction.ActionInfo.IsThrow
                                            && player.gamePlay.IsLifeState(EPlayerLifeState.Alive);
        }

        public override void OnRender(PlayerEntity player)
        {
            if (!_isAddLine)
            {
                AddThrowingLine();
            }
            else
            {
                RefreshThrowingData(player);
                DrawThrowingLine(player.throwingAction.ActionInfo.Pos, player.throwingAction.ActionInfo.Vel,
                    player.throwingAction.ActionInfo.Gravity, player.throwingAction.ActionInfo.Decay,
                    _lineWidth, _deltaTime, _totalLength);
            }
        }

        public override void OnFilterRender()
        {
            
            if (_isDrawLine)
            {
                _throwingMesh.Clear();
                _isDrawLine = false;
                SetSphereActive(false);
            }
        }

        private void RefreshThrowingData(PlayerEntity player)
        {
            ThrowingActionInfo actionInfo = player.throwingAction.ActionInfo;
            var dir = BulletDirUtility.GetThrowingDir(player);
            if (actionInfo.IsNearThrow)
            {
                actionInfo.Vel = dir * actionInfo.Config.NearInitSpeed;
            }
            else
            {
                actionInfo.Vel = dir * actionInfo.Config.FarInitSpeed;
            }
            actionInfo.Pos = PlayerEntityUtility.GetThrowingEmitPosition(player);
            actionInfo.Gravity = actionInfo.Config.Gravity;
            actionInfo.Decay = actionInfo.Config.VelocityDecay;
            actionInfo.CountdownTime = actionInfo.Config.CountdownTime;
        }

        private void AddThrowingLine()
        {
            _isAddLine = true;
            _Vertices = new List<Vector3>();
            _Uvs = new List<Vector2>();

            PlayerEntity player = _contexts.player.flagSelfEntity;
            if (null != player && !player.hasThrowingLine)
            {
                GameObject go = new GameObject();
                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
                _throwingMesh = go.GetComponent<MeshFilter>().mesh;
                player.AddThrowingLine(go);

                _contexts.session.commonSession.AssetManager.LoadAssetAsync("ThrowingLineSystem", new AssetInfo("effect/common", "line_hong"), OnLoadSucc);
                _contexts.session.commonSession.AssetManager.LoadAssetAsync("ThrowingLineSystem", new AssetInfo("effect/common", "fresnel"), OnGoLoadSucc);
            }
        }

        private void DrawThrowingLine(Vector3 pos, Vector3 vel, float gravity, float decay, float lineWidth, float deltaTime, float length)
        {
            if (null == _throwingMesh || !_assetLoaded)
                return;

            Vector3 movePos;
            Vector3 moveVel = vel;
            float sumLength = 0;
            _Vertices.Clear();
            _Uvs.Clear();

            Vector3 offset = Vector3.Cross(new Vector3(0, 1, 0), vel);
            offset = offset.normalized * lineWidth / 2;

            Vector3 oldPos;
            Vector3 hitPoint = new Vector3();
            float moveLen = 0;
            bool isHit = false;

            while (sumLength < length)
            {
                oldPos = pos;
                movePos = moveVel * deltaTime;
                pos += movePos;
                _Vertices.Add(new Vector3(pos.x - offset.x, pos.y, pos.z - offset.z));
                _Vertices.Add(new Vector3(pos.x + offset.x, pos.y, pos.z + offset.z));
                _Uvs.Add(new Vector2(sumLength / length, 0));
                _Uvs.Add(new Vector2(sumLength / length, 1));
                moveLen = movePos.magnitude;
                sumLength += moveLen;
                moveVel.y = moveVel.y - gravity * deltaTime;
                moveVel = moveVel * Mathf.Pow(decay, deltaTime);
                isHit = false;
                if (sumLength > _beginCollLength)
                {
                    isHit = CommonMathUtil.Raycast(oldPos, pos, moveLen, _layerMask, out hitPoint);
                }
                if (isHit)
                {
                    break;
                }
            }

            //triangles
            int vertLen = _Vertices.Count - 2;
            int triLen = vertLen * 3;
            int[] triangles = new int[triLen];
            for (int i = 0, j = 0; i < triLen; i += 6, j += 2)
            {
                triangles[i] = j;
                triangles[i + 1] = j + 1;
                triangles[i + 2] = j + 3;

                triangles[i + 3] = j;
                triangles[i + 4] = j + 3;
                triangles[i + 5] = j + 2;
            }

            _throwingMesh.Clear();
            _throwingMesh.vertices = _Vertices.ToArray();
            _throwingMesh.triangles = triangles;
            _throwingMesh.uv = _Uvs.ToArray();

            //Sphere
            if (isHit)
            {
                SetSpherePos(hitPoint);
            }
            else
            {
                SetSphereActive(false);
            }

            _isDrawLine = true;
        }
    }
}
