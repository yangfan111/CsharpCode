using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class BoundsViewer : MonoBehaviour
{
    private Bounds bounds;

#if UNITY_EDITOR
    [MenuItem("CONTEXT/BoundsViewer/RebuildBounds")]
    private static void RebuildBounds(MenuCommand command)
    {
        BoundsViewer viewer = command.context as BoundsViewer;
        viewer.BuildBounds();

        if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.Repaint();
    }
#endif

    private void Awake()
    {
        BuildBounds();
    }

    private void OnDrawGizmosSelected()
    {
        if (transform.hasChanged) BuildBounds();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    private void BuildBounds()
    {
        bounds = new Bounds(Vector3.zero, Vector3.zero);
        MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();

        bool first = true;
        for (int i = 0; i < mrs.Length; i++)
        {
            MeshRenderer mr = mrs[i];
            if (mr == null) continue;

            MeshFilter mf = mr.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) continue;

            if (first)
            {
                bounds = mr.bounds;
                first = false;
            }
            else
            {
                bounds.Encapsulate(mr.bounds);
            }
        }
    }
}
