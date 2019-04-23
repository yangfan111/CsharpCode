using UnityEngine;
using System.Collections.Generic;

public class DecalBuilder {

	private static List<Vector3> bufVertices = new List<Vector3>();
	private static List<Vector3> bufNormals = new List<Vector3>();
	private static List<Vector2> bufTexCoords = new List<Vector2>();
	private static List<int> bufIndices = new List<int>();
    private static Plane right = new Plane(Vector3.right, Vector3.right / 2f);
    private static Plane left = new Plane(-Vector3.right, -Vector3.right / 2f);
    private static Plane top = new Plane(Vector3.up, Vector3.up / 2f);
    private static Plane bottom = new Plane(-Vector3.up, -Vector3.up / 2f);
    private static Plane front = new Plane(Vector3.forward, Vector3.forward / 2f);
    private static Plane back = new Plane(-Vector3.forward, -Vector3.forward / 2f);
    private static DecalPolygon poly = new DecalPolygon();

    public static void BuildDecalForObject(GameObject decal, GameObject affectedObject, Texture2D texture) {
        Mesh affectedMesh = null;
        MeshCollider mc = affectedObject.GetComponent<MeshCollider>();
        if (mc != null) {
            affectedMesh = mc.sharedMesh;
        }
        if (affectedMesh == null) {
            affectedMesh = GenarateTerrainMesh(affectedObject);
         }

        if (affectedMesh == null) {
            MeshFilter mf = affectedObject.GetComponent<MeshFilter>();
            if (mf) {
                affectedMesh = mf.sharedMesh;
            }
        }
        if (affectedMesh == null) return;

		float maxAngle = /*decal.maxAngle*/90.0f;

		Vector3[] vertices = affectedMesh.vertices;
		int[] triangles = affectedMesh.triangles;
		int startVertexCount = bufVertices.Count;

		Matrix4x4 matrix = decal.transform.worldToLocalMatrix * affectedObject.transform.localToWorldMatrix;

		for(int i=0; i<triangles.Length; i+=3) {
			int i1 = triangles[i];
			int i2 = triangles[i+1];
			int i3 = triangles[i+2];
			
			Vector3 v1 = matrix.MultiplyPoint( vertices[i1] );
			Vector3 v2 = matrix.MultiplyPoint( vertices[i2] );
			Vector3 v3 = matrix.MultiplyPoint( vertices[i3] );

			Vector3 side1 = v2 - v1;
			Vector3 side2 = v3 - v1;
			Vector3 normal = Vector3.Cross(side1, side2).normalized;

			if( Vector3.Angle(-Vector3.forward, normal) >= maxAngle ) continue;

            poly.Clear();
            poly.SetVts(v1, v2, v3);

            //DecalPolygon poly = new DecalPolygon();
            //poly.SetVts(v1, v2, v3);

            if (!DecalPolygon.ClipPolygon(ref poly, right))
                continue;
            if (!DecalPolygon.ClipPolygon(ref poly, left))
                continue;

            if (!DecalPolygon.ClipPolygon(ref poly, top)) {
                continue;
            }
			if (!DecalPolygon.ClipPolygon(ref poly, bottom))
                continue;

            if (!DecalPolygon.ClipPolygon(ref poly, front))
                continue;
            if (!DecalPolygon.ClipPolygon(ref poly, back))
                continue;
			AddPolygon( poly, normal );
		}
		GenerateTexCoords(startVertexCount, texture);
	}

	private static void AddPolygon(DecalPolygon poly, Vector3 normal) {
		int ind1 = AddVertex( poly.vertices[0], normal );
		for(int i=1; i<poly.vertices.Count-1; i++) {
			int ind2 = AddVertex( poly.vertices[i], normal );
			int ind3 = AddVertex( poly.vertices[i+1], normal );

			bufIndices.Add( ind1 );
			bufIndices.Add( ind2 );
			bufIndices.Add( ind3 );
		}
	}

	private static int AddVertex(Vector3 vertex, Vector3 normal) {
		int index = FindVertex(vertex);
		if(index == -1) {
			bufVertices.Add( vertex );
			bufNormals.Add( normal );
			index = bufVertices.Count-1;
		} else {
			Vector3 t = bufNormals[ index ] + normal;
			bufNormals[ index ] = t.normalized;
		}
		return (int) index;
	}

	private static int FindVertex(Vector3 vertex) {
		for(int i=0; i<bufVertices.Count; i++) {
			if( Vector3.Distance(bufVertices[i], vertex) < 0.01f ) {
				return i;
			}
		}
		return -1;
	}

    private static void GenerateTexCoords(int start, Texture2D texture)
    {
        Rect rect = new Rect(0, 0 , 1, 1);
        // rect.x /= sprite.texture.width;
        // rect.y /= sprite.texture.height;
        // rect.width /= sprite.texture.width;
        // rect.height /= sprite.texture.height;
        for (int i = start; i < bufVertices.Count; i++)
        {
            Vector3 vertex = bufVertices[i];
            Vector2 uv = new Vector2(vertex.x + 0.5f, vertex.y + 0.5f);
            uv.x = Mathf.Lerp(rect.xMin, rect.xMax, uv.x);
            uv.y = Mathf.Lerp(rect.yMin, rect.yMax, uv.y);

            bufTexCoords.Add(uv);
        }
    }

    public static void Push(float distance) {
		for(int i=0; i<bufVertices.Count; i++) {
			Vector3 normal = bufNormals[i];
			bufVertices[i] += normal * distance;
		}
	}
    protected static Mesh GenarateTerrainMesh(GameObject terrainObj)
    {
        var terrain = terrainObj.GetComponent<Terrain>();
        if (terrain == null)
        {
            return null;
        }

        var terrainData = terrain.terrainData;
        if (terrainData == null)
        {
            return null;
        }

        int vertexCountScale = 4;       // [dev] 将顶点数稀释 vertexCountScale*vertexCountScale 倍
        int w = terrainData.heightmapWidth;
        int h = terrainData.heightmapHeight;
        Vector3 size = terrainData.size;
        float[,,] alphaMapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        Vector3 meshScale = new Vector3(size.x / (w - 1f) * vertexCountScale, 1, size.z / (h - 1f) * vertexCountScale);
        Vector2 uvScale = new Vector2(1f / (w - 1f), 1f / (h - 1f)) * vertexCountScale * (size.x / terrainData.splatPrototypes[0].tileSize.x);     // [dev] 此处有问题，若每个图片大小不一，则出问题。日后改善

        w = (w - 1) / vertexCountScale + 1;
        h = (h - 1) / vertexCountScale + 1;
        Vector3[] vertices = new Vector3[w * h];
        Vector2[] uvs = new Vector2[w * h];
        Vector4[] alphasWeight = new Vector4[w * h];            // [dev] 只支持4张图片

        // 顶点，uv，每个顶点每个图片所占比重
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                int index = j * w + i;
                float z = terrainData.GetHeight(i * vertexCountScale, j * vertexCountScale);
                vertices[index] = Vector3.Scale(new Vector3(i, z, j), meshScale);
                uvs[index] = Vector2.Scale(new Vector2(i, j), uvScale);

                // alpha map
                int i2 = (int)(i * terrainData.alphamapWidth / (w - 1f));
                int j2 = (int)(j * terrainData.alphamapHeight / (h - 1f));
                i2 = Mathf.Min(terrainData.alphamapWidth - 1, i2);
                j2 = Mathf.Min(terrainData.alphamapHeight - 1, j2);
                var alpha0 = alphaMapData[j2, i2, 0];
                var alpha1 = alphaMapData[j2, i2, 1];
                var alpha2 = alphaMapData[j2, i2, 2];
                var alpha3 = alphaMapData[j2, i2, 3];
                alphasWeight[index] = new Vector4(alpha0, alpha1, alpha2, alpha3);
            }
        }

        int[] triangles = new int[(w - 1) * (h - 1) * 6];
        int triangleIndex = 0;
        for (int i = 0; i < w - 1; i++)
        {
            for (int j = 0; j < h - 1; j++)
            {
                int a = j * w + i;
                int b = (j + 1) * w + i;
                int c = (j + 1) * w + i + 1;
                int d = j * w + i + 1;

                triangles[triangleIndex++] = a;
                triangles[triangleIndex++] = b;
                triangles[triangleIndex++] = c;

                triangles[triangleIndex++] = a;
                triangles[triangleIndex++] = c;
                triangles[triangleIndex++] = d;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.tangents = alphasWeight;       // 将地形纹理的比重写入到切线中

        return mesh;
    }

    public static Mesh CreateMesh() {
		if(bufIndices.Count == 0) {
			return null;
		}
		Mesh mesh = new Mesh();

		mesh.vertices = bufVertices.ToArray();
		mesh.normals = bufNormals.ToArray();
		mesh.uv = bufTexCoords.ToArray();
		mesh.uv2 = bufTexCoords.ToArray();
		mesh.triangles = bufIndices.ToArray();

		bufVertices.Clear();
		bufNormals.Clear();
		bufTexCoords.Clear();
		bufIndices.Clear();

		return mesh;
	}

}
