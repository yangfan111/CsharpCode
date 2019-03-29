using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Components;
using Core.Playback;
using Core.Prediction.VehiclePrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using UnityEngine;

namespace App.Shared.Components.Vehicle
{
    [Vehicle]
    
    public class VehicleAssetInfoComponent : IVehiclePredictionComponent, INonSelfLatestComponent
    {
        [NetworkProperty]
        public int Id;

        [NetworkProperty]
        public int VType;

        [NetworkProperty]
        public string AssetBundleName;

        [NetworkProperty]
        public string ModelName;

        [NetworkProperty]
        public string TipName;

        [NetworkProperty]
        public int PostureId;

        public bool HasRadio;

        [NetworkProperty]
        public Vector3 CameraAnchorOffset;

        [NetworkProperty]
        public float CameraDistance;

        [NetworkProperty]
        public float CameraRotationDamping;

        public int GetComponentId()
        {
            return (int) EComponentIds.VehicleAssetInfo;
        }

        

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as VehicleAssetInfoComponent;
            Id = r.Id;
            VType = r.VType;
            AssetBundleName = r.AssetBundleName;
            ModelName = r.ModelName;
            TipName = r.TipName;
            PostureId = r.PostureId;
            HasRadio = r.HasRadio;
            CameraAnchorOffset = r.CameraAnchorOffset;
            CameraDistance = r.CameraDistance;
            CameraRotationDamping = r.CameraRotationDamping;
        }


        public bool IsApproximatelyEqual(object right)
        {
            return AssetBundleName != null && ModelName != null;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
}
