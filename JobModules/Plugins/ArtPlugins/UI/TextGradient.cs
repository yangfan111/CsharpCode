using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponent.Extension
{
    [AddComponentMenu("UI/Effects/TextGradient")]
    public class TextGradient : BaseMeshEffect
    {
        public Color32 topColor = Color.white;
        public Color32 bottomColor = Color.black;

        private static void setColor(List<UIVertex> verts, int index, Color32 c)
        {
            UIVertex vertex = verts[index];
            vertex.color = c;
            verts[index] = vertex;
        }

        private void ModifyVertices(List<UIVertex> verts)
        {
            for (int i = 0; i < verts.Count; i += 6)
            {
                setColor(verts, i + 0, topColor);
                setColor(verts, i + 1, topColor);
                setColor(verts, i + 2, bottomColor);
                setColor(verts, i + 3, bottomColor);

                setColor(verts, i + 4, bottomColor);
                setColor(verts, i + 5, topColor);
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!this.IsActive())
            {
                return;
            }
            List<UIVertex> verts = new List<UIVertex>(vh.currentVertCount);
            vh.GetUIVertexStream(verts);

            ModifyVertices(verts);

            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
        }
    }

}