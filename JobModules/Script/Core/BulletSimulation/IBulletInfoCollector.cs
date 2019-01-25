using Core.Network;
using UnityEngine;

namespace Core.BulletSimulation
{
    public enum EHitType
    {
        Environment,
        Player,
        Vehicle,
    }

    public interface IBulletInfoCollector
    {
        void AddBulletData(int seq, Vector3 startPoint, Vector3 emitPoint, Vector3 startDir, Vector3 hitPoint, int hitType, INetworkChannel networkChannel);
        string GetStatisticData(int dataType);
    }
}