using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OilSafetyTrainer.Editor
{
    internal static class SafetyTrainerPrimitiveFactory
    {
        public static GameObject CreatePrimitive(PrimitiveType type, string name, Vector3 position, Vector3 scale, Material material, Transform parent)
        {
            var gameObject = GameObject.CreatePrimitive(type);
            gameObject.name = name;
            gameObject.transform.SetParent(parent);
            gameObject.transform.position = position;
            gameObject.transform.localScale = scale;
            if (material != null)
            {
                gameObject.GetComponent<Renderer>().sharedMaterial = material;
            }

            return gameObject;
        }

        public static void CreateFence(Vector3 position, Vector3 scale, Material material, Transform parent, string name)
        {
            CreatePrimitive(PrimitiveType.Cube, name, position, scale, material, parent);
        }

        public static void CreateWorldLabel(string name, string text, Vector3 position, Quaternion rotation, float characterSize, Color color, Transform parent)
        {
            var labelObject = new GameObject(name);
            labelObject.transform.SetParent(parent);
            labelObject.transform.SetPositionAndRotation(position, rotation);
            labelObject.transform.localScale = Vector3.one * characterSize;

            var textMesh = labelObject.AddComponent<TextMeshPro>();
            textMesh.text = text;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.fontSize = 3.2f;
            textMesh.color = color;
            textMesh.enableWordWrapping = false;
            textMesh.rectTransform.sizeDelta = new Vector2(8f, 1.4f);
        }

        public static void CreateDisplayPanel(string name, string materialName, string texturePath, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent, bool flipToCamera = true)
        {
            var material = SafetyTrainerMaterialFactory.CreateDisplayMaterial(materialName, texturePath);
            if (material == null)
            {
                return;
            }

            var finalRotation = flipToCamera ? rotation * Quaternion.Euler(0f, 180f, 0f) : rotation;
            CreateDisplayQuad(name, position, finalRotation, scale, material, parent);
        }

        public static void CreateDisplayQuad(string name, Vector3 position, Quaternion rotation, Vector3 scale, Material material, Transform parent)
        {
            var display = GameObject.CreatePrimitive(PrimitiveType.Quad);
            display.name = name;
            display.transform.SetParent(parent);
            display.transform.SetPositionAndRotation(position, rotation);
            display.transform.localScale = scale;

            var collider = display.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }

            var renderer = display.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
        }

        public static void CreateInteractionProxy(string name, Vector3 position, Quaternion rotation, Vector3 size, Transform parent)
        {
            var proxy = new GameObject(name);
            proxy.transform.SetParent(parent);
            proxy.transform.SetPositionAndRotation(position, rotation);

            var collider = proxy.AddComponent<BoxCollider>();
            collider.size = size;
            collider.isTrigger = false;
        }

        public static TextMeshProUGUI CreateText(string name, Transform parent, string text, int size, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, Vector2 pivot, bool bestFit = false, int minBestFitSize = 10, int maxBestFitSize = 20)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            var uiText = textObject.AddComponent<TextMeshProUGUI>();
            uiText.text = text;
            uiText.fontSize = size;
            uiText.color = Color.white;
            uiText.alignment = ToTextMeshProAlignment(alignment);
            uiText.textWrappingMode = TextWrappingModes.Normal;
            uiText.overflowMode = TextOverflowModes.Truncate;
            uiText.enableAutoSizing = bestFit;
            uiText.fontSizeMin = minBestFitSize;
            uiText.fontSizeMax = maxBestFitSize;

            var rect = uiText.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            return uiText;
        }

        private static TextAlignmentOptions ToTextMeshProAlignment(TextAnchor alignment)
        {
            return alignment switch
            {
                TextAnchor.UpperLeft => TextAlignmentOptions.TopLeft,
                TextAnchor.UpperCenter => TextAlignmentOptions.Top,
                TextAnchor.UpperRight => TextAlignmentOptions.TopRight,
                TextAnchor.MiddleLeft => TextAlignmentOptions.Left,
                TextAnchor.MiddleCenter => TextAlignmentOptions.Center,
                TextAnchor.MiddleRight => TextAlignmentOptions.Right,
                TextAnchor.LowerLeft => TextAlignmentOptions.BottomLeft,
                TextAnchor.LowerCenter => TextAlignmentOptions.Bottom,
                TextAnchor.LowerRight => TextAlignmentOptions.BottomRight,
                _ => TextAlignmentOptions.Left
            };
        }

        public static GameObject CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, Color color, Vector2 pivot)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            var image = panel.AddComponent<Image>();
            image.color = color;
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            return panel;
        }
    }
}
