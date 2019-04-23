using UnityEngine;
using System.Collections.Generic;

public class DecalPolygon {
	
	public List<Vector3> vertices = new List<Vector3>(9);

    private const int POSITIVE_NUM = 9;
    private static bool[] positive = new bool[POSITIVE_NUM];
    private static DecalPolygon tempPolygon = new DecalPolygon();

    public DecalPolygon(params Vector3[] vts) {
		vertices.AddRange( vts );
	}

    public void SetVts(params Vector3[] vts) {
        vertices.AddRange(vts);
    }

    public void CopyFrom(DecalPolygon value) {
        Clear();
        vertices.AddRange(value.vertices);
    }

    public void Clear() {
        vertices.Clear();
    }

    private static void ResetPositive() {
        for (int i = 0; i < POSITIVE_NUM; i++) {
            positive[i] = false;
        }
    }


    public static bool ClipPolygon (ref DecalPolygon polygon, Plane plane) {
        ResetPositive();
        int positiveCount = 0;

		for(int i = 0; i < polygon.vertices.Count; i++) {
			positive[i] = !plane.GetSide( polygon.vertices[i] );
			if(positive[i]) positiveCount++;
		}
		
		if(positiveCount == 0) return false;
		if(positiveCount == polygon.vertices.Count) return true;

        tempPolygon.Clear();
        for (int i = 0; i < polygon.vertices.Count; i++) {
			int next = i + 1;
			 next %= polygon.vertices.Count;

			if( positive[i] ) {
				tempPolygon.vertices.Add( polygon.vertices[i] );
			}

			if( positive[i] != positive[next] ) {
				Vector3 v1 = polygon.vertices[next];
				Vector3 v2 = polygon.vertices[i];
				
				Vector3 v = LineCast(plane, v1, v2);
				tempPolygon.vertices.Add( v );
			}
		}
        polygon.CopyFrom(tempPolygon);
        return true;
	}

	private static Vector3 LineCast(Plane plane, Vector3 a, Vector3 b) {
		float dis;
		Ray ray = new Ray(a, b-a);
		plane.Raycast( ray, out dis );
		return ray.GetPoint(dis);
	}
	
}