using UnityEngine;
using System.Collections;
using UnityEditor;


namespace MultiTerrain
{
/// <summary>
/// Splat texture editor.
/// </summary>
	public class SplatTextureEditor : EditorWindow
	{
		/// <summary>
		/// The multi terrain editor.
		/// </summary>
		public MultiTerrainEditor multiTerrainEditor;
		/// <summary>
		/// The add.
		/// </summary>
		public bool add = true;
		/// <summary>
		/// The splat texture.
		/// </summary>
		public Texture2D splatTexture;
		/// <summary>
		/// The normal texture.
		/// </summary>
		public Texture2D normalTexture;
		/// <summary>
		/// The size of the tile.
		/// </summary>
		public Vector2 tileSize = new Vector2 (15, 15);
		/// <summary>
		/// The tile offset.
		/// </summary>
		public Vector2 tileOffset;
		/// <summary>
		/// The metallic.
		/// </summary>
		public float metallic;
		/// <summary>
		/// The smoothness.
		/// </summary>
		public float smoothness;

		/// <summary>
		/// Gets the splat prototype.
		/// </summary>
		/// <returns>The splat prototype.</returns>
		public SplatPrototype GetSplatPrototype ()
		{
			SplatPrototype splat = new SplatPrototype ();
			splat.texture = splatTexture;
			splat.normalMap = normalTexture;
			splat.tileSize = tileSize;
			splat.tileOffset = tileOffset;
			splat.metallic = metallic;
			splat.smoothness = smoothness;
			return splat;
		}


		/// <summary>
		/// Raises the GUI event.
		/// </summary>
		void OnGUI ()
		{
			splatTexture = (Texture2D)EditorGUILayout.ObjectField ("Splat texture:", splatTexture, typeof(Texture2D), false);
			normalTexture = (Texture2D)EditorGUILayout.ObjectField ("Normal texture:", normalTexture, typeof(Texture2D), false);

			metallic = EditorGUILayout.Slider ("Metallic", metallic, 0, 1);
			smoothness = EditorGUILayout.Slider ("Smoothness", smoothness, 0, 1);

			tileSize = EditorGUILayout.Vector2Field ("Tile size:", tileSize);
			tileOffset = EditorGUILayout.Vector2Field ("Tile offset:", tileOffset);

			if (add) {
				if (GUILayout.Button ("Add Texture")) {
					multiTerrainEditor.AddSplatPrototype (GetSplatPrototype ());
					this.Close ();
				}
//			if (GUILayout.Button ("Set All Terrain Splat Textures")) {
//				multiTerrainEditor.AddSplatPrototype (GetSplatPrototype ());
//				multiTerrainEditor.SetTerrainSplatPrototypes ();
//				
//				this.Close ();
//			}

			} else {
				if (GUILayout.Button ("Edit Texture")) {
					multiTerrainEditor.EditSplatPrototype (GetSplatPrototype ());
					this.Close ();
				}
//			if (GUILayout.Button ("Set All Terrain Splat Textures")) {
//				multiTerrainEditor.EditSplatPrototype (GetSplatPrototype ());
//				multiTerrainEditor.SetTerrainSplatPrototypes ();
//				
//				this.Close ();
//			}

			}
		

		}
	}
}