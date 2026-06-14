using UnityEditor;
using UnityEngine;

namespace OilSafetyTrainer.Editor
{
    internal readonly struct SafetyTrainerMaterialSet
    {
        public SafetyTrainerMaterialSet(
            Material concrete,
            Material asphalt,
            Material safetyYellow,
            Material steel,
            Material pipeRed,
            Material oilBlack,
            Material safeGreen,
            Material inspectionBlue,
            Material white,
            Material warningOrange,
            Material beaconWarning,
            Material gateVolume)
        {
            Concrete = concrete;
            Asphalt = asphalt;
            SafetyYellow = safetyYellow;
            Steel = steel;
            PipeRed = pipeRed;
            OilBlack = oilBlack;
            SafeGreen = safeGreen;
            InspectionBlue = inspectionBlue;
            White = white;
            WarningOrange = warningOrange;
            BeaconWarning = beaconWarning;
            GateVolume = gateVolume;
        }

        public Material Concrete { get; }
        public Material Asphalt { get; }
        public Material SafetyYellow { get; }
        public Material Steel { get; }
        public Material PipeRed { get; }
        public Material OilBlack { get; }
        public Material SafeGreen { get; }
        public Material InspectionBlue { get; }
        public Material White { get; }
        public Material WarningOrange { get; }
        public Material BeaconWarning { get; }
        public Material GateVolume { get; }
    }

    internal static class SafetyTrainerMaterialFactory
    {
        public static SafetyTrainerMaterialSet CreateMaterialSet()
        {
            return new SafetyTrainerMaterialSet(
                CreateTexturedMaterial("Concrete", new Color(0.93f, 0.93f, 0.93f), SafetyTrainerPaths.ConcreteDiffusePath, SafetyTrainerPaths.ConcreteNormalPath, new Vector2(5f, 5f), 0f, 0.18f),
                CreateTexturedMaterial("Asphalt", Color.white, SafetyTrainerPaths.AsphaltDiffusePath, SafetyTrainerPaths.AsphaltNormalPath, new Vector2(2f, 7f), 0f, 0.08f),
                CreateMaterial("SafetyYellow", new Color(1f, 0.72f, 0.05f)),
                CreateTexturedMaterial("Steel", Color.white, SafetyTrainerPaths.SteelDiffusePath, SafetyTrainerPaths.SteelNormalPath, new Vector2(1.5f, 1.5f), 0.38f, 0.32f),
                CreateMaterial("HotPipeRed", new Color(0.75f, 0.12f, 0.08f)),
                CreateMaterial("OilBlack", new Color(0.02f, 0.02f, 0.018f)),
                CreateMaterial("SafeGreen", new Color(0.1f, 0.6f, 0.24f)),
                CreateMaterial("InspectionBlue", new Color(0.1f, 0.5f, 0.95f)),
                CreateMaterial("WhitePaint", Color.white),
                CreateMaterial("WarningOrange", new Color(1f, 0.38f, 0.04f)),
                CreateEmissiveMaterial("BeaconWarning", new Color(1f, 0.38f, 0.04f), new Color(2.3f, 0.8f, 0.1f)),
                CreateTransparentMaterial("GateVolume", new Color(1f, 0.8f, 0f, 0.16f)));
        }

        public static Material CreateDisplayMaterial(string name, string texturePath)
        {
            if (string.IsNullOrWhiteSpace(texturePath))
            {
                return null;
            }

            var texture = LoadDisplayTexture(texturePath);
            if (texture == null)
            {
                return null;
            }

            var path = $"{SafetyTrainerPaths.MaterialFolder}/{name}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            var shader = Shader.Find("Unlit/Transparent") ?? Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Texture");
            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, path);
            }
            else if (material.shader != shader)
            {
                material.shader = shader;
            }

            material.mainTexture = texture;
            if (material.HasProperty("_Color"))
            {
                material.color = Color.white;
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material CreateMaterial(string name, Color color)
        {
            var path = $"{SafetyTrainerPaths.MaterialFolder}/{name}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Standard"));
                AssetDatabase.CreateAsset(material, path);
            }

            material.color = color;
            material.SetFloat("_Metallic", 0f);
            material.SetFloat("_Glossiness", 0.2f);
            material.DisableKeyword("_EMISSION");
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material CreateTexturedMaterial(string name, Color tint, string diffusePath, string normalPath, Vector2 tiling, float metallic, float glossiness)
        {
            var material = CreateMaterial(name, tint);
            var diffuse = LoadTexture(diffusePath, false);
            if (diffuse != null)
            {
                material.SetTexture("_MainTex", diffuse);
                material.mainTextureScale = tiling;
            }

            var normal = LoadTexture(normalPath, true);
            if (normal != null)
            {
                material.EnableKeyword("_NORMALMAP");
                material.SetTexture("_BumpMap", normal);
                material.SetTextureScale("_BumpMap", tiling);
            }
            else
            {
                material.DisableKeyword("_NORMALMAP");
                material.SetTexture("_BumpMap", null);
            }

            material.SetFloat("_Metallic", metallic);
            material.SetFloat("_Glossiness", glossiness);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material CreateEmissiveMaterial(string name, Color color, Color emissionColor)
        {
            var material = CreateMaterial(name, color);
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", emissionColor);
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            material.SetFloat("_Glossiness", 0.35f);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material CreateTransparentMaterial(string name, Color color)
        {
            var material = CreateMaterial(name, color);
            material.SetFloat("_Mode", 3f);
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Texture2D LoadTexture(string path, bool normalMap)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture == null)
            {
                return null;
            }

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                var dirty = false;
                if (normalMap && importer.textureType != TextureImporterType.NormalMap)
                {
                    importer.textureType = TextureImporterType.NormalMap;
                    dirty = true;
                }

                if (!normalMap && importer.textureType != TextureImporterType.Default)
                {
                    importer.textureType = TextureImporterType.Default;
                    dirty = true;
                }

                if (dirty)
                {
                    importer.SaveAndReimport();
                }
            }

            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static Texture2D LoadDisplayTexture(string path)
        {
            var texture = LoadTexture(path, false);
            if (texture == null)
            {
                return null;
            }

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                var dirty = false;
                if (!importer.alphaIsTransparency)
                {
                    importer.alphaIsTransparency = true;
                    dirty = true;
                }

                if (importer.wrapMode != TextureWrapMode.Clamp)
                {
                    importer.wrapMode = TextureWrapMode.Clamp;
                    dirty = true;
                }

                if (dirty)
                {
                    importer.SaveAndReimport();
                }
            }

            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
    }
}
