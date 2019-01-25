using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class CheckParticleCollider : MonoBehaviour
{
    public enum ColliderMode
    {
        Box,
        Sphere,
    }

    /// <summary>
    /// 记录检测到的碰撞器
    /// </summary>
    public List<Collider> colliders = new List<Collider>();

    public int cullingMask = -1;
    public ColliderMode colliderMode = ColliderMode.Box;
    public bool showBound = true;
    public float alpha = 0.1f;

    // Box
    public Vector3 boxCenter = Vector3.zero;
    public float boxSize = 1f;
    public Space boxRotationSpace = Space.Self;
    public Vector3 boxRotation = Vector3.zero;

    // Sphere
    public Vector3 sphereCenter = Vector3.zero;
    public float sphereRadius = 0.5f;

    private ParticleSystem[] ps = null;

    private void OnEnable()
    {
        ps = GetComponentsInChildren<ParticleSystem>(true);
    }

	public void ApplyTriggerMask(int mask)
	{
		cullingMask = mask;
		ApplyTrigger();
	}
	
    public void ApplyTrigger()
    {
        colliders.Clear();

        switch (colliderMode)
        {
            case ColliderMode.Box:
                {
                    Vector3 centerPos = transform.TransformPoint(boxCenter);
                    Quaternion rot = Quaternion.Euler(boxRotation);
                    if (boxRotationSpace == Space.Self)
                    {
                        rot = transform.rotation * rot;
                    }
                    var hits = Physics.BoxCastAll(centerPos, Vector3.one * boxSize / 2, Vector3.up, rot, 0f, cullingMask, QueryTriggerInteraction.Ignore);
                    foreach (var hit in hits)
                    {
                        if (hit.collider == null || hit.collider.gameObject == null) continue;
                        colliders.Add(hit.collider);
                    }
                }
                break;
            case ColliderMode.Sphere:
                {
                    Vector3 centerPos = transform.TransformPoint(sphereCenter);
                    var hits = Physics.SphereCastAll(centerPos, sphereRadius, Vector3.up, 0f, cullingMask, QueryTriggerInteraction.Ignore);
                    foreach (var hit in hits)
                    {
                        if (hit.collider == null || hit.collider.gameObject == null) continue;
                        colliders.Add(hit.collider);
                    }
                }
                break;
        }

        // apply colliders
        foreach (var p in ps)
        {
            if (p == null) continue;

            var trigger = p.trigger;
            trigger.enabled = true;
            trigger.enter = ParticleSystemOverlapAction.Kill;
            for (int i = 0; i < trigger.maxColliderCount; i++)
            {
                Collider c = i < colliders.Count ? colliders[i] : null;
                trigger.SetCollider(i, c);
            }
        }
    }

    public void CancelTrigger()
    {
        foreach (var p in ps)
        {
            if (p == null) continue;

            var trigger = p.trigger;
            trigger.enabled = false;
        }
    }
}
