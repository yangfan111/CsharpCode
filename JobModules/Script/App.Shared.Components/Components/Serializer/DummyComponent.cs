using Core.Playback;
using System.Diagnostics;
using Core.Components;
using Core.EntityComponent;
using Core.Prediction;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;

namespace App.Shared.Components.Serializer
{
    /// <summary>
    /// 单元测试中会用到的组件，因为要利用代码生成工具生成序列化类，所以放在这个项目中
    /// </summary>
    
    public class DummyComponent  : IGameComponent, IUserPredictionComponent, IPlaybackComponent
    {
        [NetworkProperty] public int Age;
        [NetworkProperty] public int Height;
        [NetworkProperty] public int ChildNum;


        public int GetComponentId()
        {
            return (int) EComponentIds.DummyObject;
        }

        public void CopyFrom(object rightComponent)
        {
            var comp = rightComponent as DummyComponent;
            Debug.Assert(comp != null);
            Age = comp.Age;
            Height = comp.Height;
            ChildNum = comp.ChildNum;
        }

        public bool IsApproximatelyEqual(object right)
        {
            return true;
        }
        public bool IsInterpolateEveryFrame(){ return false; }
        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            
        }

        public void RewindTo(object rightComponent)
        {
           
        }
    }
}
