using System;
using System.Collections.Generic;
using System.Diagnostics;
using App.Shared;
using Core.Utils;
using Shared.Scripts.SceneManagement;
using UnityEngine;

namespace App.Client.SceneManagement.DistanceCulling
{
    class StreamingGoBuilder
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(StreamingGoBuilder));

        private readonly GenericPool<OctreeNode> _nodePool;
        private readonly GenericPool<StreamingGo> _streamingGoPool;

        private readonly Queue<StreamingScene> _buildingScenes = new Queue<StreamingScene>();
        private readonly Queue<OctreeNode> _buildingOctreeRoots = new Queue<OctreeNode>();
        private int _buildingGoIndex;
        private readonly Stopwatch _timer = new Stopwatch();

        private readonly Action<OctreeNode, string> _newOctreeRootCallback;
            
        public StreamingGoBuilder(Action<OctreeNode, string> newOctreeRootCallback)
        {
            _newOctreeRootCallback = newOctreeRootCallback;

            _nodePool = new GenericPool<OctreeNode>();
            _nodePool.SetMeta(new OctreeNode(_nodePool.Reuse, _nodePool.Get));
            
            _streamingGoPool = new GenericPool<StreamingGo>();
            _streamingGoPool.SetMeta(new StreamingGo(_streamingGoPool.Reuse));
        }

        public void CreateOctreeForScene(Vector3 center, float size, StreamingScene sceneStruct)
        {
            _buildingOctreeRoots.Enqueue(GetRoot(center, size));
            _buildingScenes.Enqueue(sceneStruct);
        }

        public OctreeNode GetOctreeForScene(Vector3 center, float size, StreamingScene sceneStruct)
        {
            var root = GetRoot(center, size);

            var count = sceneStruct.Objects.Count;
            for (int i = 0; i < count; i++)
            {
                var data = sceneStruct.Objects[i];
                if(!FilterStreamingObject(data))
                    root.InsertStreamingGo(GetStreamingGo(data, sceneStruct.Index, i), data.Position, data.Size);
            }

            return root;
        }

        public void Update()
        {
            if (_buildingScenes.Count > 0)
            {
                _timer.Reset();
                _timer.Start();

                var scene = _buildingScenes.Peek();
                var root = _buildingOctreeRoots.Peek();
                var goCount = scene.Objects.Count;

                while (_buildingGoIndex < goCount && _timer.ElapsedMilliseconds < 1)
                {
                    var data = scene.Objects[_buildingGoIndex];
                    if(!FilterStreamingObject(data))
                        root.InsertStreamingGo(GetStreamingGo(data, scene.Index, _buildingGoIndex), data.Position, data.Size);
                    
                    ++_buildingGoIndex;
                }

                if (_buildingGoIndex == goCount)
                {
                    _buildingGoIndex = 0;
                    _buildingScenes.Dequeue();
                    _buildingOctreeRoots.Dequeue();
                    
                    _newOctreeRootCallback.Invoke(root, scene.SceneName);
                }
                
                _timer.Stop();
            }
        }

        private bool FilterStreamingObject(StreamingObject so)
        {
            if (SharedConfig.IgnoreProp)
            {
                var tagValue = so.SceneTag;
                //filter inprop, out prop and inoutprop but wall, house and door, terrain

                if (MultiTagHelper.IsDoor(tagValue) || MultiTagHelper.IsHouse(tagValue)
                    || MultiTagHelper.IsWall(tagValue) || MultiTagHelper.IsTerrain(tagValue))
                    return false;

                if (MultiTagHelper.InDoor(tagValue) || MultiTagHelper.OutDoor(tagValue))
                    return true;
            }
            
            return false;
        }

        private DistCullingCat GetCullingCatForGo(StreamingObject data)
        {
            var cat = Constants.GetDistCullingCatForStreamingGo(data.Size);
            
            var catFromTag = Constants.GetDistCullingCatForTag(data.SceneTag);
            if ((int) catFromTag < (int) cat)
            {
                cat = catFromTag;
            }

            return cat;
        }

        private OctreeNode GetRoot(Vector3 center, float size)
        {
            var root = _nodePool.Get();
            root.SetRootAabb(center, new Vector3(size, size, size));
            root.SetHeight(7);

            return root;
        }

        private StreamingGo GetStreamingGo(StreamingObject data, int sceneIndex, int goIndex)
        {
            var go = _streamingGoPool.Get();

            go.Key.GoIndex = goIndex;
            go.Key.SceneIndex = sceneIndex;

            go.Status = StreamingGoStatus.NotLoaded;
            go.Cat = GetCullingCatForGo(data);

            go.AssetName = data.AssetName;
            go.Position = data.Position;
            go.Size = data.Size;
            

            return go;
        }
    }
}