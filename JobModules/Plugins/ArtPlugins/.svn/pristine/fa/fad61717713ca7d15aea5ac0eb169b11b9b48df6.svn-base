using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ArtPlugins;

namespace DigitalOpus.MB.Core
{
    internal class MB3_TextureCombinerPackerMeshBaker : MB3_TextureCombinerPackerRoot
    {
        /// <summary>
        /// 执行具体的图集创建过程
        /// </summary>
        public override IEnumerator CreateAtlases(ProgressUpdateDelegate progressInfo,
            MB3_TextureCombinerPipeline.TexturePipelineData data, MB3_TextureCombiner combiner,
            AtlasPackingResult packedAtlasRects,
            Texture2D[] atlases, MB2_EditorMethodsInterface textureEditorMethods,
            MB2_LogLevel LOG_LEVEL)
        {
            Rect[] uvRects = packedAtlasRects.rects;

            int atlasSizeX = packedAtlasRects.atlasX;       // 记录图集的宽度
            int atlasSizeY = packedAtlasRects.atlasY;       // 记录图集的高度

            if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("Generated atlas will be " + atlasSizeX + "x" + atlasSizeY);

            for (int propIdx = 0; propIdx < data.numAtlases; propIdx++)             // 对于每一个纹理属性要创建的图集
            {
                Texture2D atlas = null;
                ShaderTextureProperty property = data.texPropertyNames[propIdx];    // 记录Shader中的纹理属性信息

                if (!MB3_TextureCombinerPipeline._ShouldWeCreateAtlasForThisProperty(propIdx, data._considerNonTextureProperties, data.allTexturesAreNullAndSameColor))         // 无须为此属性创建图集
                {
                    atlas = null;
                    if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("=== Not creating atlas for " + property.name + " because textures are null and default value parameters are the same.");
                }
                else                // 为此纹理属性创建图集
                {
                    if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("=== Creating atlas for " + property.name);

                    GC.Collect();

                    CreateTemporaryTexturesForAtlas(data.distinctMaterialTextures, combiner, propIdx, data);

                    //use a jagged array because it is much more efficient in memory
                    Color[][] atlasPixels = new Color[atlasSizeY][];                    // 创建图集像素的锯齿矩阵
                    for (int j = 0; j < atlasPixels.Length; j++)
                    {
                        atlasPixels[j] = new Color[atlasSizeX];
                    }

                    bool isNormalMap = false;
                    if (property.isNormalMap) isNormalMap = true;

                    for (int texSetIdx = 0; texSetIdx < data.distinctMaterialTextures.Count; texSetIdx++)           // 对于待合并的每一个材质
                    {
                        MB_TexSet texSet = data.distinctMaterialTextures[texSetIdx];
                        MeshBakerMaterialTexture matTex = texSet.ts[propIdx];                                       // 取得该材质的指定纹理属性
                        string s = "Creating Atlas '" + property.name + "' texture " + matTex.GetTexName();
                        if (progressInfo != null) progressInfo(s, .01f);
                        if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log(string.Format("Adding texture {0} to atlas {1} for texSet {2} srcMat {3}", matTex.GetTexName(), property.name, texSetIdx, texSet.matsAndGOs.mats[0].GetMaterialName()));
                        Rect r = uvRects[texSetIdx];
                        Texture2D t = texSet.ts[propIdx].GetTexture2D();
                        int x = Mathf.RoundToInt(r.x * atlasSizeX);
                        int y = Mathf.RoundToInt(r.y * atlasSizeY);
                        int ww = Mathf.RoundToInt(r.width * atlasSizeX);
                        int hh = Mathf.RoundToInt(r.height * atlasSizeY);
                        if (ww == 0 || hh == 0) Debug.LogError("Image in atlas has no height or width " + r);
                        if (progressInfo != null) progressInfo(s + " set ReadWrite flag", .01f);
                        // if (textureEditorMethods != null) textureEditorMethods.SetReadWriteFlag(t, true, true);
                        if (progressInfo != null) progressInfo(s + "Copying to atlas: '" + matTex.GetTexName() + "'", .02f);
                        DRect samplingRect = texSet.ts[propIdx].GetEncapsulatingSamplingRect();
                        Debug.Assert(!texSet.ts[propIdx].isNull, string.Format("Adding texture {0} to atlas {1} for texSet {2} srcMat {3}", matTex.GetTexName(), property.name, texSetIdx, texSet.matsAndGOs.mats[0].GetMaterialName()));
                        // yield return CopyScaledAndTiledToAtlas(texSet.ts[propIdx], texSet, property, samplingRect, x, y, ww, hh, packedAtlasRects.padding[texSetIdx], atlasPixels, isNormalMap, data, combiner, progressInfo, LOG_LEVEL);
                        yield return CopyScaledAndTiledToAtlasUseMagick(texSet.ts[propIdx], texSet, property, samplingRect, x, y, ww, hh, packedAtlasRects.padding[texSetIdx], atlasPixels, isNormalMap, data, combiner, progressInfo, LOG_LEVEL);
                    }

                    yield return data.numAtlases;

                    // 设置图集像素
                    if (progressInfo != null) progressInfo("Applying changes to atlas: '" + property.name + "'", .03f);
                    atlas = new Texture2D(atlasSizeX, atlasSizeY, TextureFormat.ARGB32, true);
                    for (int j = 0; j < atlasPixels.Length; j++)
                    {
                        atlas.SetPixels(0, j, atlasSizeX, 1, atlasPixels[j]);
                    }
                    atlas.Apply();

                    if (LOG_LEVEL >= MB2_LogLevel.debug) Debug.Log("Saving atlas " + property.name + " w=" + atlas.width + " h=" + atlas.height);
                }

                atlases[propIdx] = atlas;                   // 记录创建好的图集列表
                if (progressInfo != null) progressInfo("Saving atlas: '" + property.name + "'", .04f);
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                if (data._saveAtlasesAsAssets && textureEditorMethods != null)                  // 保存图集并设置到最终材质球上
                {
                    textureEditorMethods.SaveAtlasToAssetDatabase(atlases[propIdx], data.texPropertyNames[propIdx], propIdx, data.resultMaterial);
                }
                else                                                                            // 将图集直接设置到最终材质球上
                {
                    data.resultMaterial.SetTexture(data.texPropertyNames[propIdx].name, atlases[propIdx]);
                }

                // 设置最终材质的offset和scale
                data.resultMaterial.SetTextureOffset(data.texPropertyNames[propIdx].name, Vector2.zero);
                data.resultMaterial.SetTextureScale(data.texPropertyNames[propIdx].name, Vector2.one);

                combiner._destroyTemporaryTextures(data.texPropertyNames[propIdx].name);        // 删除临时的纹理列表
            }

            yield break;
        }

        internal static IEnumerator CopyScaledAndTiledToAtlas(MeshBakerMaterialTexture source, MB_TexSet sourceMaterial,
            ShaderTextureProperty shaderPropertyName, DRect srcSamplingRect, int targX, int targY, int targW, int targH,
            AtlasPadding padding, Color[][] atlasPixels, bool isNormalMap, MB3_TextureCombinerPipeline.TexturePipelineData data,
            MB3_TextureCombiner combiner, ProgressUpdateDelegate progressInfo = null, MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info)
        {
            //HasFinished = false;
            Texture2D t = source.GetTexture2D();                // 待合并的源纹理
            if (LOG_LEVEL >= MB2_LogLevel.debug)
            {
                Debug.Log(String.Format("CopyScaledAndTiledToAtlas: {0} inAtlasX={1} inAtlasY={2} inAtlasW={3} inAtlasH={4} paddX={5} paddY={6} srcSamplingRect={7}",
                    t, targX, targY, targW, targH, padding.leftRight, padding.topBottom, srcSamplingRect));
            }
            float newWidth = targW;                             // 在图集中的宽度
            float newHeight = targH;                            // 在图集中的高度
            float scx = (float)srcSamplingRect.width;           // 源图片采样宽度
            float scy = (float)srcSamplingRect.height;          // 源图片采样高度
            float ox = (float)srcSamplingRect.x;                // 源图片采样x轴起始处
            float oy = (float)srcSamplingRect.y;                // 源图片采样y轴起始处
            int w = (int)newWidth;
            int h = (int)newHeight;

            if (data._considerNonTextureProperties)
            {
                t = combiner._createTextureCopy(shaderPropertyName.name, t);
                t = data.nonTexturePropertyBlender.TintTextureWithTextureCombiner(t, sourceMaterial, shaderPropertyName);
            }

            // 图集区域的像素设置
            for (int i = 0; i < w; i++)
            {
                if (progressInfo != null && w > 0) progressInfo("CopyScaledAndTiledToAtlas " + (((float)i / (float)w) * 100f).ToString("F0"), .2f);
                for (int j = 0; j < h; j++)
                {
                    float u = i / newWidth * scx + ox;
                    float v = j / newHeight * scy + oy;
                    atlasPixels[targY + j][targX + i] = t.GetPixelBilinear(u, v);
                }
            }

            //bleed the border colors into the padding 上下左右的边界的像素设置
            for (int i = 0; i < w; i++)
            {
                for (int j = 1; j <= padding.topBottom; j++)
                {
                    //top margin
                    atlasPixels[(targY - j)][targX + i] = atlasPixels[(targY)][targX + i];
                    //bottom margin
                    atlasPixels[(targY + h - 1 + j)][targX + i] = atlasPixels[(targY + h - 1)][targX + i];
                }
            }
            for (int j = 0; j < h; j++)
            {
                for (int i = 1; i <= padding.leftRight; i++)
                {
                    //left margin
                    atlasPixels[(targY + j)][targX - i] = atlasPixels[(targY + j)][targX];
                    //right margin
                    atlasPixels[(targY + j)][targX + w + i - 1] = atlasPixels[(targY + j)][targX + w - 1];
                }
            }

            //corners
            for (int i = 1; i <= padding.leftRight; i++)
            {
                for (int j = 1; j <= padding.topBottom; j++)
                {
                    atlasPixels[(targY - j)][targX - i] = atlasPixels[targY][targX];                                        // 左下角像素设置
                    atlasPixels[(targY + h - 1 + j)][targX - i] = atlasPixels[(targY + h - 1)][targX];                      // 左上角像素设置
                    atlasPixels[(targY + h - 1 + j)][targX + w + i - 1] = atlasPixels[(targY + h - 1)][targX + w - 1];      // 右上角像素设置
                    atlasPixels[(targY - j)][targX + w + i - 1] = atlasPixels[targY][targX + w - 1];                        // 右下角像素设置
                    yield return null;
                }
                yield return null;
            }
            // Debug.Log("copyandscaledatlas finished too!");
            // HasFinished = true;
            yield break;
        }

        /// <summary>
        /// 使用ImageMagick.Net版的自定义方法进行图集合并
        /// </summary>
        internal static IEnumerator CopyScaledAndTiledToAtlasUseMagick(MeshBakerMaterialTexture source, MB_TexSet sourceMaterial,
            ShaderTextureProperty shaderPropertyName, DRect srcSamplingRect, int targX, int targY, int targW, int targH,
            AtlasPadding padding, Color[][] atlasPixels, bool isNormalMap, MB3_TextureCombinerPipeline.TexturePipelineData data,
            MB3_TextureCombiner combiner, ProgressUpdateDelegate progressInfo = null, MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info)
        {
            //HasFinished = false;
            Texture2D t = source.GetTexture2D();                // 待合并的源纹理
            if (LOG_LEVEL >= MB2_LogLevel.debug)
            {
                Debug.Log(String.Format("CopyScaledAndTiledToAtlas: {0} inAtlasX={1} inAtlasY={2} inAtlasW={3} inAtlasH={4} paddX={5} paddY={6} srcSamplingRect={7}",
                    t, targX, targY, targW, targH, padding.leftRight, padding.topBottom, srcSamplingRect));
            }
            float newWidth = targW;                             // 在图集中的宽度
            float newHeight = targH;                            // 在图集中的高度
            float scx = (float)srcSamplingRect.width;           // 源图片采样宽度
            float scy = (float)srcSamplingRect.height;          // 源图片采样高度
            float ox = (float)srcSamplingRect.x;                // 源图片采样x轴起始处
            float oy = (float)srcSamplingRect.y;                // 源图片采样y轴起始处
            int w = (int)newWidth;
            int h = (int)newHeight;

            if (data._considerNonTextureProperties)             // 本项目中此项一直为false
            {
                t = combiner._createTextureCopy(shaderPropertyName.name, t);
                t = data.nonTexturePropertyBlender.TintTextureWithTextureCombiner(t, sourceMaterial, shaderPropertyName);
            }

            // 图集区域的像素设置
            // Debug.LogErrorFormat(t, "start read texture:{0} time:{1}", t.name, System.DateTime.Now.ToString());
            MagickTextureTool magicTex = new MagickTextureTool(t);
            for (int i = 0; i < w; i++)
            {
                if (progressInfo != null && w > 0) progressInfo("CopyScaledAndTiledToAtlas " + (((float)i / (float)w) * 100f).ToString("F0"), .2f);
                for (int j = 0; j < h; j++)
                {
                    float u = i / newWidth * scx + ox;
                    float v = j / newHeight * scy + oy;
                    // atlasPixels[targY + j][targX + i] = t.GetPixelBilinear(u, v);
                    atlasPixels[targY + j][targX + i] = magicTex.GetPixelBilinear(u, v);
                }
            }
            // Debug.LogErrorFormat(t, "finish read texture:{0} time:{1}", t.name, System.DateTime.Now.ToString());

            //bleed the border colors into the padding 上下左右的边界的像素设置
            for (int i = 0; i < w; i++)
            {
                for (int j = 1; j <= padding.topBottom; j++)
                {
                    //top margin
                    atlasPixels[(targY - j)][targX + i] = atlasPixels[(targY)][targX + i];
                    //bottom margin
                    atlasPixels[(targY + h - 1 + j)][targX + i] = atlasPixels[(targY + h - 1)][targX + i];
                }
            }
            for (int j = 0; j < h; j++)
            {
                for (int i = 1; i <= padding.leftRight; i++)
                {
                    //left margin
                    atlasPixels[(targY + j)][targX - i] = atlasPixels[(targY + j)][targX];
                    //right margin
                    atlasPixels[(targY + j)][targX + w + i - 1] = atlasPixels[(targY + j)][targX + w - 1];
                }
            }

            //corners
            for (int i = 1; i <= padding.leftRight; i++)
            {
                for (int j = 1; j <= padding.topBottom; j++)
                {
                    atlasPixels[(targY - j)][targX - i] = atlasPixels[targY][targX];                                        // 左下角像素设置
                    atlasPixels[(targY + h - 1 + j)][targX - i] = atlasPixels[(targY + h - 1)][targX];                      // 左上角像素设置
                    atlasPixels[(targY + h - 1 + j)][targX + w + i - 1] = atlasPixels[(targY + h - 1)][targX + w - 1];      // 右上角像素设置
                    atlasPixels[(targY - j)][targX + w + i - 1] = atlasPixels[targY][targX + w - 1];                        // 右下角像素设置
                    yield return null;
                }
                yield return null;
            }
            // Debug.Log("copyandscaledatlas finished too!");
            // HasFinished = true;
            yield break;
        }
    }
}
