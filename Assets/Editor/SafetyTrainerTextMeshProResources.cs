using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace OilSafetyTrainer.Editor
{
    internal static class SafetyTrainerTextMeshProResources
    {
        public static void EnsureImported()
        {
            if (AssetDatabase.FindAssets("t:TMP_Settings", new[] { "Assets" }).Length > 0 &&
                AssetDatabase.FindAssets("t:TMP_StyleSheet", new[] { "Assets" }).Length > 0)
            {
                return;
            }

            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(TMP_Text).Assembly);
            if (packageInfo == null)
            {
                Debug.LogError("Unable to locate the TextMeshPro package for importing essential resources.");
                return;
            }

            var packagePath = Path.Combine(
                packageInfo.resolvedPath,
                "Package Resources",
                "TMP Essential Resources.unitypackage");

            if (!IsUsableUnityPackage(packagePath))
            {
                packagePath = Path.Combine(
                    EditorApplication.applicationContentsPath,
                    "Resources",
                    "PackageManager",
                    "BuiltInPackages",
                    "com.unity.ugui",
                    "Package Resources",
                    "TMP Essential Resources.unitypackage");
            }

            if (!IsUsableUnityPackage(packagePath))
            {
                Debug.LogError($"TMP Essential Resources package was not found at {packagePath}.");
                return;
            }

            Debug.Log($"Importing TMP Essential Resources from {packagePath}");
            AssetDatabase.ImportPackage(packagePath, false);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private static bool IsUsableUnityPackage(string packagePath)
        {
            return File.Exists(packagePath) && new FileInfo(packagePath).Length > 1024;
        }
    }
}
