using System;
using System.Collections.Generic;
using App.Client.SceneManagement.DistanceCulling.Factory;
using App.Shared.SceneManagement.Streaming;
using Core.SceneManagement;
using Core.Utils;
using Shared.Scripts.SceneManagement;
using UnityEngine;
using Utils.AssetManager;

namespace App.Client.SceneManagement.DistanceCulling
{
    class OctreeNode : ICacheableElement
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(OctreeNode));
        
        private readonly Action<OctreeNode> _reuseHandler;
        private readonly Func<OctreeNode> _getHandler;

        enum CullState
        {
            NotSure,
            Culled,
            NotCulled
        }

        private const int AxisX = 1;
        private const int AxisY = 2;
        private const int AxisZ = 4;

        private int _height;
        private OctreeNode _parent;
        private OctreeNode[] _children = new OctreeNode[8];

        private Vector3 _halfSize;
        private Vector3 _center;
        private Vector3[] _extremeDir = new Vector3[8];
        
        private List<StreamingGo> _streamingGoes = new List<StreamingGo>();
        private Dictionary<StreamingGoKey, StreamingGo> _streamingGoMap = new Dictionary<StreamingGoKey, StreamingGo>(StreamingGoKey.Instance);

        private readonly int[] _cullingCount;
        private readonly int[] _cullingCountOverall;
        private readonly CullState[] _cullingState;

        private OctreeNode[] _recursiveStack;

        // oc enable
        private bool _isOcTestLevel;

        public OctreeNode(Action<OctreeNode> reuseHandler, Func<OctreeNode> getHandler)
        {
            _reuseHandler = reuseHandler;
            _getHandler = getHandler;

            _cullingCount = new int[Constants.DistCullingCatCount];
            _cullingCountOverall = new int[Constants.DistCullingCatCount];
            _cullingState = new CullState[Constants.DistCullingCatCount];

            for (int i = 0; i < _cullingState.Length; i++)
                _cullingState[i] = CullState.Culled;
        }

        public void SetHeight(int value)
        {
            _height = value;
            _recursiveStack = new OctreeNode[_height + 1];
            for (int i = 0; i <= _height; i++)
                _recursiveStack[i] = null;
        }

        #region ICacheableElement
        
        public object Clone()
        {
            return new OctreeNode(_reuseHandler, _getHandler);
        }
        
        public void Free()
        {
            _reuseHandler(this);
        }

        public void Reset()
        {
            _recursiveStack = null;

            var count = _streamingGoes.Count;
            for (int i = 0; i < count; i++)
            {
                _streamingGoes[i].Free();
            }

            _streamingGoes.Clear();
            _streamingGoMap.Clear();

            count = Constants.DistCullingCatCount;
            for (int i = 0; i < count; i++)
            {
                _cullingState[i] = CullState.Culled;
                _cullingCount[i] = 0;
                _cullingCountOverall[i] = 0;
            }
            
            _parent = null;

            count = _children.Length;
            for (int i = 0; i < count; i++)
            {
                if (_children[i] != null)
                {
                    _children[i].Free();
                    _children[i] = null;
                }
            }
        }

        #endregion

        public void InsertStreamingGo(StreamingGo streamingGo, Vector3 position, Vector3 size)
        {
            if (_height > 0 && FitInOneChild(size))
            {
                var index = GetChildIndex(position);
                if (_children[index] == null)
                {
                    _children[index] = _getHandler();

                    Vector3 childCenter = new Vector3(
                        0 == (index & AxisX) ? _center.x - _halfSize.x * 0.5f : _center.x + _halfSize.x * 0.5f,
                        0 == (index & AxisY) ? _center.y - _halfSize.y * 0.5f : _center.y + _halfSize.y * 0.5f,
                        0 == (index & AxisZ) ? _center.z - _halfSize.z * 0.5f : _center.z + _halfSize.z * 0.5f);

                    _children[index].SetAabb(childCenter, _halfSize);
                    _children[index]._height = _height - 1;
                    _children[index]._parent = this;
                }
                
                _children[index].InsertStreamingGo(streamingGo, position, size);
            }
            else
            {
                _streamingGoes.Add(streamingGo);
                _streamingGoMap.Add(streamingGo.Key, streamingGo);
                ChangeCullingCount(streamingGo.Cat, 1);
            }
        }

        public void AssignGo(UnityObject unityObj, CullingHandler handler, StreamingGoKey key,
            Vector3 position, Vector3 size, IStreamingResourceHandler resHandler)
        {
            if (_height > 0 && FitInOneChild(size))
            {
                var index = GetChildIndex(position);
                _children[index].AssignGo(unityObj, handler, key, position, size, resHandler);
            }
            else
            {
                var streamingGo = _streamingGoMap[key];
                if (streamingGo.SetGo(unityObj))
                {
                    while (handler != null)
                    {
                        handler.StateChanged(_cullingState[(int) handler.Category] != CullState.Culled);
                        var sibling = handler.Sibling;
                        handler.Sibling = null;

                        streamingGo.AddHandler(handler);
                        ChangeCullingCount(handler.Category, 1);

                        handler = sibling;
                    }
                }
                else
                {
                    resHandler.UnloadGo(unityObj, key.SceneIndex);
                    while (handler != null)
                    {
                        var sibling = handler.Sibling;
                        handler.Free();
                        handler = sibling;
                    }
                }
            }
        }

        public void Delete(IStreamingResourceHandler handler)
        {
            var count = _streamingGoes.Count;
            for (int i = 0; i < count; i++)
            {
                var streamingGo = _streamingGoes[i];
                if (streamingGo.Go != null)
                    handler.UnloadGo(streamingGo.Go, streamingGo.Key.SceneIndex);
            }

            count = _children.Length;
            for (int i = 0; i < count; i++)
            {
                if (_children[i] != null)
                    _children[i].Delete(handler);
            }
        }

        public void SetRootAabb(Vector3 center, Vector3 size)
        {
            _center = center;
            _halfSize = size * 0.5f;

            SetExtremeDir(_halfSize);
        }

        private void SetAabb(Vector3 center, Vector3 size)
        {
            _center = center;
            _halfSize = size * 0.5f;
            
            SetExtremeDir(size);

            var minDimension = Math.Min(size.x, Math.Min(size.y, size.z));
            _isOcTestLevel = minDimension <= Constants.CloseToArticlesStandard
                && 2 * minDimension > Constants.CloseToArticlesStandard;
        }

        private void SetExtremeDir(Vector3 halfSize)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        _extremeDir[i * AxisX + j * AxisY + k * AxisZ] = new Vector3(
                            halfSize.x * (2 * i - 1),
                            halfSize.y * (2 * j - 1),
                            halfSize.z * (2 * k - 1));
                    }
                }
            }
        }

        private void ChangeCullingCount(DistCullingCat cat, int step)
        {
            var index = (int) cat;
            _cullingCount[index] += step;
            _cullingCountOverall[index] += step;

            if (_cullingCountOverall[index] == 0)
                _cullingState[index] = CullState.Culled;
            
            var itor = _parent;
            while (itor != null)
            {
                itor._cullingCountOverall[index] += step;
                if (itor._cullingCountOverall[index] == 0)
                    itor._cullingState[index] = CullState.Culled;
                    
                itor = itor._parent;
            }
        }

        private bool FitInOneChild(Vector3 size)
        {
            return size.x <= _halfSize.x
                   && size.y <= _halfSize.y
                   && size.z <= _halfSize.z;
        }

        private int GetChildIndex(Vector3 center)
        {
            return (center.x > _center.x ? AxisX : 0)
                   | (center.y > _center.y ? AxisY : 0)
                   | (center.z > _center.z ? AxisZ : 0);
        }

        private int GetRemoteChildIndex(Vector3 center)
        {
            return (center.x > _center.x ? 0 : AxisX)
                   | (center.y > _center.y ? 0 : AxisY)
                   | (center.z > _center.z ? 0 : AxisZ);
        }

        #region Culling

        private int _childTraverseIndex;

        public void OnCameraMovement(Vector3 cameraPos, float scale, IStreamingResourceHandler handler)
        {
            var index = -1;
            _recursiveStack[++index] = this;

            while (index >= 0)
            {
                var node = _recursiveStack[index];
                node._childTraverseIndex = 0;

                float nearestDist = -1f;
                float farthestDist = -1f;

                var toCenterX = cameraPos.x - node._center.x;
                var toCenterY = cameraPos.y - node._center.y;
                var toCenterZ = cameraPos.z - node._center.z;
                
                if (Math.Abs(toCenterX) > node._extremeDir[7].x || Math.Abs(toCenterY) > node._extremeDir[7].y
                    || Math.Abs(toCenterZ) > node._extremeDir[7].z)
                {
                    var childIndex = node.GetChildIndex(cameraPos);
                    var remoteChildIndex = node.GetRemoteChildIndex(cameraPos);
                
                    var distX = toCenterX - node._extremeDir[childIndex].x;
                    var distY = toCenterY - node._extremeDir[childIndex].y;
                    var distZ = toCenterZ - node._extremeDir[childIndex].z;

                    var scaleSquare = scale * scale;

                    nearestDist = (distX * distX + distY * distY + distZ * distZ) * scaleSquare;
                
                    distX = toCenterX - node._extremeDir[remoteChildIndex].x;
                    distY = toCenterY - node._extremeDir[remoteChildIndex].y;
                    distZ = toCenterZ - node._extremeDir[remoteChildIndex].z;

                    farthestDist = (distX * distX + distY * distY + distZ * distZ) * scaleSquare;
                }

                if (node.DistanceCull(nearestDist, farthestDist, handler))
                    node._childTraverseIndex = node._children.Length;

                bool findNotTraversedNode = false;

                while (!findNotTraversedNode)
                {
                    while (node._childTraverseIndex < node._children.Length)
                    {
                        if (node._children[node._childTraverseIndex] != null)
                        {
                            _recursiveStack[++index] = node._children[node._childTraverseIndex];
                            ++node._childTraverseIndex;
                            findNotTraversedNode = true;
                            break;
                        }
                        
                        ++node._childTraverseIndex;
                    }

                    if (!findNotTraversedNode)
                    {
                        if (index == 0)
                            return;

                        node = _recursiveStack[--index];
                    }
                }
            }
        }

        private bool DistanceCull(float nearestDistSquare, float farthestDistSquare, IStreamingResourceHandler handler)
        {
            bool allowExitAhead = true;
            // camera in this cell
            if (nearestDistSquare < 0)
            {
                for (int i = 0; i < Constants.DistCullingCatCount; i++)
                {
                    if (_cullingState[i] != CullState.NotCulled)
                    {
                        _cullingState[i] = CullState.NotCulled;

                        SetActivation(i, true, handler);
                    }
                }

                allowExitAhead = false;
            }
            else
            {
                // 分若干种距离判断是否需要提前退出
                for (int i = 0; i < Constants.DistCullingCatCount; i++)
                {
                    var standard = Constants.DistCullingCats[i].Dist;

                    if (_cullingCountOverall[i] != 0)
                    {
                        CullState state = CullState.NotSure;
                        if (nearestDistSquare >= standard)
                            state = CullState.Culled;
                        else if (farthestDistSquare < standard)
                            state = CullState.NotCulled;

                        // 结果为剔除，上次也为剔除，提前退出
                        // 结果为必可见，上次也为必可见，提前退出
                        if (_cullingState[i] != state || state == CullState.NotSure)
                            allowExitAhead = false;

                        // 状态变化时，处理可见性
                        if (_cullingState[i] != state)
                        {
                            _cullingState[i] = state;
                            
                            SetActivation(i, _cullingState[i] == CullState.NotCulled, handler);
                        }
                    }
                }
            }

            return allowExitAhead;
        }

        private void SetActivation(int index, bool value, IStreamingResourceHandler handler)
        {
            if (_cullingCount[index] != 0)
            {
                var count = _streamingGoes.Count;
                for (int i = 0; i < count; i++)
                {
                    if (_streamingGoes[i].SetGoActivatiton((DistCullingCat) index, value, handler))
                    {
                        for (int j = 0; j < Constants.DistCullingCatCount; j++)
                        {
                            var itor = _streamingGoes[i].Handlers[j];
                            while (itor != null)
                            {
                                ChangeCullingCount((DistCullingCat) j, -1);
                                itor = itor.Sibling;
                            }
                        }
                    }

                    _streamingGoes[i].SetHandlerActivation(index, value, handler);
                }
            }
        }

        private bool IsOutside(Vector3 pos)
        {
            var dirX = Math.Abs(pos.x - _center.x);
            var dirY = Math.Abs(pos.y - _center.y);
            var dirZ = Math.Abs(pos.z - _center.z);

            return Math.Abs(dirX) > _extremeDir[7].x || Math.Abs(dirY) > _extremeDir[7].y
                || Math.Abs(dirZ) > _extremeDir[7].z;
        }

        #endregion

        public void Traverse(Vector3 cameraPos, OriginStatus status)
        {
            if (_parent == null && IsOutside(cameraPos))
                return;

            if (_isOcTestLevel)
            {
                var count = 0;
                for (int i = 0; i < _cullingCountOverall.Length; i++)
                {
                    count += _cullingCountOverall[i];
                }

                status.CloseToBuilding = count > Constants.ArticlesCountStandard;
                return;
            }

            var childIndex = GetChildIndex(cameraPos);
            if (_children[childIndex] != null)
                _children[childIndex].Traverse(cameraPos, status);
            else
                status.CloseToBuilding = false;
        }
        
        public void Log()
        {
            for (int i = 0; i < Constants.DistCullingCatCount; i++)
            {
                if (_cullingState[i] == CullState.NotCulled)
                    StreamingGoByDistance.Show[_height, i]++;

                StreamingGoByDistance.Total[_height, i]++;
            }
            
            var count = _children.Length;
            for (int i = 0; i < count; i++)
            {
                if (_children[i] != null)
                {
                    _children[i].Log();
                }
            }
        }
    }
}