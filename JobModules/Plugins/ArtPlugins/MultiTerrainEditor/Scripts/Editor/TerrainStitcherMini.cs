using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MultiTerrain
{
	/// <summary>
	/// Terrain stitcher mini.
	/// </summary>
	public class TerrainStitcherMini
	{
		/// <summary>
		/// The size of the blur.
		/// </summary>
		public static float blurSize = 5;

	
		/// <summary>
		/// The length of the stitch check.
		/// </summary>
		public static int checkLength = 100;
	
		/// <summary>
		/// The power of average function.
		/// </summary>
		public static float power = 2.0f;


	
		public enum Side
		{
			Left,
			Right,
			Top,
			Bottom
		}

		/// <summary>
		/// Average the specified first and second value.
		/// </summary>
		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
		public static float Average (float first, float second)
		{
		
			return (first + second) * 0.5f;//Mathf.Pow ((Mathf.Pow (first, power) + Mathf.Pow (second, power)) / 2.0f, 1 / power);


		}

		/// <summary>
		/// Stitchs the terrains.
		/// </summary>
		/// <param name="terrain">Terrain.</param>
		/// <param name="second">Second.</param>
		/// <param name="side">Side.</param>
		/// <param name="xBase">X base.</param>
		/// <param name="yBase">Y base.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="range">Range.</param>
		/// <param name="smooth">If set to <c>true</c> smooth.</param>
		public static void StitchTerrains (Terrain terrain, Terrain second, Side side, int xBase, int yBase, int width, int height, int range, Color[,] bigPicture, float opacity, bool switchImageX = false, bool switchImageY = false)
		{		
			float imagePixel = 0;
			TerrainData terrainData = terrain.terrainData;
			TerrainData secondData = second.terrainData;
				
			if (side == Side.Right) {
				
				float[,] heights = terrainData.GetHeights (terrainData.heightmapWidth - width, yBase, width, height);
				float[,] secondHeights = secondData.GetHeights (0, yBase, range > 0 ? range : 1, height);
							

				int dimmensionX = heights.GetLength (0);
				int dimmensionY = heights.GetLength (1);
				int dimmensionX2 = secondHeights.GetLength (0);
				int dimmensionY2 = secondHeights.GetLength (1);
						 
				int dimmensionXAll = dimmensionX;
				int dimmensionYAll = dimmensionY + dimmensionY2;



				checkLength = width;

				int yp = width - 1;
				int xp = 0;
			
				int y2 = 0;

											
				for (xp = 0; xp < dimmensionX; xp++) {
					
					heights [xp, yp] = Average (heights [xp, yp], secondHeights [xp, y2]);
				
					secondHeights [xp, y2] = heights [xp, yp];

									
				}


				float[,] allHeights = new float[dimmensionXAll, dimmensionYAll];
				float[,] allHeightsCalculated = new float[dimmensionXAll, dimmensionYAll];

				for (int x = 0; x < dimmensionX; x++) {
					for (int y = 0; y < dimmensionY; y++) {
						allHeights [x, y] = heights [x, y];
						allHeightsCalculated [x, y] = heights [x, y];
					}
				}

				for (int x = 0; x < dimmensionX2; x++) {
					for (int y = 0; y < dimmensionY2; y++) {
						
						allHeights [x, y + dimmensionY] = secondHeights [x, y];
						allHeightsCalculated [x, y + dimmensionY] = secondHeights [x, y];

					}
				}

				float sum = 0;
				float count = 0;
				for (int i = 0; i < dimmensionXAll; i++) {
					for (int h = 0; h < dimmensionYAll; h++) {
						sum += allHeights [i, h];
						count++;
					}
				}

				float average = sum / (float)count;


				for (int i = 0; i < dimmensionXAll; i++) {
					for (int h = 0; h < dimmensionYAll; h++) {
						float bigX = 0;
						float bigY = 0;

						try {
							if (!switchImageX)
								bigX = (dimmensionXAll - i + 1);
							else
								bigX = i;
							if (!switchImageY)
								bigY = (dimmensionYAll - h + 1);
							else
								bigY = h;
							imagePixel = bigPicture [(int)bigX, (int)bigY].a;
						

						} catch {

							imagePixel = 0f;
							//continue;
						}

						if (imagePixel < 0.1f)
							continue;
					
						sum = 0;
						count = 0;

						for (int xBlur = (i - blurSize * 0.5f) >= 0 ? (int)(i - blurSize * 0.5f) : i; (xBlur < blurSize * 0.5f + i && xBlur < dimmensionXAll); xBlur++) {
							for (int yBlur = (h - blurSize * 0.5f) >= 0 ? (int)(h - blurSize * 0.5f) : h; (yBlur < blurSize * 0.5f + h && yBlur < dimmensionYAll); yBlur++) {
								sum += allHeights [xBlur, yBlur];
								count++;
							}
						}

						float averageLocal = sum / (float)count;
						allHeightsCalculated [i, h] = Mathf.Lerp (allHeights [i, h], average * 0.1f + averageLocal * 0.9f, imagePixel * opacity * 0.001f);//

					}

				}

				for (int x = 0; x < dimmensionX; x++) {
					for (int y = 0; y < dimmensionY; y++) {

						heights [x, y] = allHeightsCalculated [x, y];
					}
				}

				for (int x = 0; x < dimmensionX2; x++) {
					for (int y = 0; y < dimmensionY2; y++) {
						secondHeights [x, y] = allHeightsCalculated [x, y + dimmensionY];
					}
				}




				terrainData.SetHeightsDelayLOD (terrainData.heightmapWidth - width, yBase, heights);
				terrain.terrainData = terrainData;

				secondData.SetHeightsDelayLOD (0, yBase, secondHeights);
				second.terrainData = secondData;

			} else {
				
				if (side == Side.Top) {
					
					float[,] heights = terrainData.GetHeights (xBase, terrainData.heightmapHeight - height, width, height);

					float[,] secondHeights = secondData.GetHeights (xBase, 0, width, range > 0 ? range : 1);

				


					int dimmensionX = heights.GetLength (0);
					int dimmensionY = heights.GetLength (1);
					int dimmensionX2 = secondHeights.GetLength (0);
					int dimmensionY2 = secondHeights.GetLength (1);

					int dimmensionXAll = dimmensionX + dimmensionX2;
					int dimmensionYAll = dimmensionY;

					checkLength = height;
					int yp = 0;
					int xp = height - 1;
				

					int x2 = 0;
				
					for (yp = 0; yp < dimmensionY; yp++) {

						heights [xp, yp] = Average (heights [xp, yp], secondHeights [x2, yp]);
						secondHeights [x2, yp] = heights [xp, yp];

					
					
					}


					float[,] allHeights = new float[dimmensionXAll, dimmensionYAll];
					float[,] allHeightsCalculated = new float[dimmensionXAll, dimmensionYAll];

					for (int x = 0; x < dimmensionX; x++) {
						for (int y = 0; y < dimmensionY; y++) {
							allHeights [x, y] = heights [x, y];
							allHeightsCalculated [x, y] = heights [x, y];
						}
					}

					for (int x = 0; x < dimmensionX2; x++) {
						for (int y = 0; y < dimmensionY2; y++) {
						
							allHeights [x + dimmensionX, y] = secondHeights [x, y];
							allHeightsCalculated [x + dimmensionX, y] = secondHeights [x, y];

						}
					}

					float sum = 0;
					float count = 0;
					for (int i = 0; i < dimmensionXAll; i++) {
						for (int h = 0; h < dimmensionYAll; h++) {
							sum += allHeights [i, h];
							count++;
						}
					}

					float average = sum / (float)count;

					for (int i = 0; i < dimmensionXAll; i++) {
						for (int h = 0; h < dimmensionYAll; h++) {
							float bigX = 0;
							float bigY = 0;

							try {
								
								if (!switchImageX)
									bigX = (dimmensionXAll - i + 1);
								else
									bigX = i;
								if (!switchImageY)
									bigY = (dimmensionYAll - h + 1);
								else
									bigY = h;
								
								imagePixel = bigPicture [(int)bigX, (int)bigY].a;


							} catch {

								imagePixel = 0;
								//continue;
							}
							if (imagePixel < 0.1f)
								continue;

							sum = 0;
							count = 0;

							for (int xBlur = (i - blurSize * 0.5f) >= 0 ? (int)(i - blurSize * 0.5f) : i; (xBlur < blurSize * 0.5f + i && xBlur < dimmensionXAll); xBlur++) {
								for (int yBlur = (h - blurSize * 0.5f) >= 0 ? (int)(h - blurSize * 0.5f) : h; (yBlur < blurSize * 0.5f + h && yBlur < dimmensionYAll); yBlur++) {
									sum += allHeights [xBlur, yBlur];
									count++;
								}
							}

							float averageLocal = sum / (float)count;
							allHeightsCalculated [i, h] = Mathf.Lerp (allHeights [i, h], average * 0.1f + averageLocal * 0.9f, imagePixel * opacity * 0.001f);// imagePixel * 0.05f;//

						}

					}

					for (int x = 0; x < dimmensionX; x++) {
						for (int y = 0; y < dimmensionY; y++) {

							heights [x, y] = allHeightsCalculated [x, y];
						}
					}

					for (int x = 0; x < dimmensionX2; x++) {
						for (int y = 0; y < dimmensionY2; y++) {
							secondHeights [x, y] = allHeightsCalculated [x + dimmensionX, y];
						}
					}



					terrainData.SetHeightsDelayLOD (xBase, terrainData.heightmapHeight - height, heights);
					terrain.terrainData = terrainData;

					secondData.SetHeightsDelayLOD (xBase, 0, secondHeights);
					second.terrainData = secondData;

				}
			}
		
		
	
		
			terrain.Flush ();
			second.Flush ();
		
		
		}

		/// <summary>
		/// Stitchs the terrains and repairs errors.
		/// </summary>
		/// <param name="terrain">Terrain.</param>
		/// <param name="second">Second.</param>
		/// <param name="side">Side.</param>
		/// <param name="xBase">X base.</param>
		/// <param name="yBase">Y base.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public static void StitchTerrainsRepair (Terrain terrain, Terrain second, Side side, int xBase, int yBase, int width, int height)
		{

		
			TerrainData terrainData = terrain.terrainData;
			TerrainData secondData = second.terrainData;
		
		
			if (side == Side.Right) {

				float[,] heights = terrainData.GetHeights (terrainData.heightmapWidth - 1, yBase, 1, height);
				float[,] secondHeights = secondData.GetHeights (0, yBase, 1, height);

				int dimmensionX = heights.GetLength (0);
		
				int y = 0;
				int x = 0;
			
				int y2 = 0;
			
				for (x = 0; x < dimmensionX; x++) {

					if (heights [x, y] == secondHeights [x, y2])
						continue;

					heights [x, y] = Average (heights [x, y], secondHeights [x, y2]);

				
					secondHeights [x, y2] = heights [x, y];

				
				} 

				terrainData.SetHeightsDelayLOD (terrainData.heightmapWidth - 1, yBase, heights);
				terrain.terrainData = terrainData;

				secondData.SetHeightsDelayLOD (0, yBase, secondHeights);
				second.terrainData = secondData;
			} else {
				
				if (side == Side.Top) {
					//Debug.Log ("top");
					float[,] heights = terrainData.GetHeights (xBase, terrainData.heightmapHeight - 1, width, 1);

					float[,] secondHeights = secondData.GetHeights (xBase, 0, width, 1);

					int dimmensionY = heights.GetLength (1);

					int y = 0;
					int x = 0;
				
					int x2 = 0;
				
					for (y = 0; y < dimmensionY; y++) {
						
						if (heights [x, y] == secondHeights [x2, y])
							continue;
					
						heights [x, y] = Average (heights [x, y], secondHeights [x2, y]);
				
					
						secondHeights [x2, y] = heights [x, y];

					}

					terrainData.SetHeightsDelayLOD (xBase, terrainData.heightmapHeight - 1, heights);
					terrain.terrainData = terrainData;

					secondData.SetHeightsDelayLOD (xBase, 0, secondHeights);
					second.terrainData = secondData;

				}

			
			}
		
		
		
		
			terrain.Flush ();
			second.Flush ();
		
		
		}

		/// <summary>
		/// Stitchs the terrains corners.
		/// </summary>
		/// <param name="terrain11">Terrain11.</param>
		/// <param name="terrain21">Terrain21.</param>
		/// <param name="terrain12">Terrain12.</param>
		/// <param name="terrain22">Terrain22.</param>
		public static void StitchTerrainsRepairCorner (Terrain terrain11, Terrain terrain21, Terrain terrain12, Terrain terrain22)
		{
			int size = 0;

			if (terrain11 != null)
				size = terrain11.terrainData.heightmapHeight - 1;
			else if (terrain21 != null)
				size = terrain21.terrainData.heightmapHeight - 1;
			else if (terrain12 != null)
				size = terrain12.terrainData.heightmapHeight - 1;
			else if (terrain22 != null)
				size = terrain22.terrainData.heightmapHeight - 1;
			

			int size0 = 0;
			List<float> heights = new List<float> ();
		
			if (terrain11 != null)
				heights.Add (terrain11.terrainData.GetHeights (size, size0, 1, 1) [0, 0]);
			if (terrain21 != null)
				heights.Add (terrain21.terrainData.GetHeights (size0, size0, 1, 1) [0, 0]);
			if (terrain12 != null)
				heights.Add (terrain12.terrainData.GetHeights (size, size, 1, 1) [0, 0]);
			if (terrain22 != null)
				heights.Add (terrain22.terrainData.GetHeights (size0, size, 1, 1) [0, 0]);
		
		
			float[,] height = new float[1, 1];
			height [0, 0] = heights.Max ();

			if (terrain11 != null)
				terrain11.terrainData.SetHeightsDelayLOD (size, size0, height);
			if (terrain21 != null)
				terrain21.terrainData.SetHeightsDelayLOD (size0, size0, height);
			if (terrain12 != null)
				terrain12.terrainData.SetHeightsDelayLOD (size, size, height);
			if (terrain22 != null)
				terrain22.terrainData.SetHeightsDelayLOD (size0, size, height);

			if (terrain11 != null)
				terrain11.Flush ();
			if (terrain12 != null)
				terrain12.Flush ();
			if (terrain21 != null)
				terrain21.Flush ();
			if (terrain22 != null)
				terrain22.Flush ();
		
		
		}
	}
}
