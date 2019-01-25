using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shared.Scripts.SceneManagement;
using UnityEngine;

public class HierarchyRecorder
{
    public bool NeedLevelStreaming { get; private set; }

    private readonly StreamingScene _streamingScene = new StreamingScene();

    public StreamingScene StreamingScene
    {
        get { return _streamingScene; }
    }

    public void Record(string bundleName, string assetName, StreamingObjectCategory cat, Transform transform)
    {
        NeedLevelStreaming = true;
        var obj = new StreamingObject
        {
            BundleName = bundleName,
            AssetName = assetName,
            Category = cat,
            //Position = transform.position,
            //Rotation = transform.rotation.eulerAngles,
            //Scale = transform.lossyScale
        };

        _streamingScene.Objects.Add(obj);
    }
}
