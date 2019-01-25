using UnityEngine;

#if UNITY_5_5_OR_NEWER
using Include = UnityEngine.SerializeField;
using Exclude = System.NonSerializedAttribute;
#else
using TinyJSON;
using Include = TinyJSON.Include;
using Exclude = TinyJSON.Exclude;
#endif

namespace JesseStiller.TerrainFormerExtension {
    // TODO: I can't change this to the more correct "ToolSettings" since it will break Settings.tf files
    [System.Serializable]
    internal class ModeSettings {
        [Include]
        internal bool useFalloffForCustomBrushes = false;
        [Include]
        internal bool invertFalloff = false;
        [Include]
        internal string selectedBrushTab = "All";
        [Include]
        internal string selectedBrushId = BrushCollection.defaultFalloffBrushId;
        [Include]
        internal float brushSize = 35f;
        [Include]
        internal float brushSpeed = 1f;
        [Include]
        internal float brushRoundness = 1f;
        [Include]
        internal float brushAngle = 0f;

        [Exclude]
        internal AnimationCurve brushFalloff = new AnimationCurve(new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 1f, 0f, 1f));
#if !UNITY_5_5_OR_NEWER
        [Include]
        internal FauxKeyframe[] brushFalloffFauxFrames;
#endif

        // Random Spacing
        [Include]
        internal bool useBrushSpacing = false;
        [Include]
        internal float minBrushSpacing = 1f;
        [Include]
        internal float maxBrushSpacing = 50f;

        // Random Rotation
        [Include]
        internal bool useRandomRotation = false;
        [Include]
        internal float minRandomRotation = -180f;
        [Include]
        internal float maxRandomRotation = 180f;

        // Random Offset
        [Include]
        internal bool useRandomOffset = false;
        [Include]
        internal float randomOffset = 30f;

        [Include]
        internal bool invertBrushTexture = false;
    }
}
