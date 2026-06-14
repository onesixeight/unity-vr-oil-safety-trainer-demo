using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OilSafetyTrainer.Editor
{
    public static class SafetyTrainerProjectBuilder
    {
        [MenuItem("Oil Safety Trainer/Rebuild Demo Scene")]
        public static void BuildProject()
        {
            SafetyTrainerPaths.EnsureFolders();

            var materials = SafetyTrainerMaterialFactory.CreateMaterialSet();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "OilSafetyTrainerDemo";

            SafetyTrainerEnvironmentBuilder.ConfigureRenderSettings();
            SafetyTrainerEnvironmentBuilder.CreateLighting();

            var root = new GameObject("Oil Production Safety Trainer");
            var geometry = new GameObject("Generated Geometry").transform;
            geometry.SetParent(root.transform);

            SafetyTrainerEnvironmentBuilder.CreateYard(materials, geometry);
            SafetyTrainerEnvironmentBuilder.CreateCheckpointProps(materials, geometry);
            SafetyTrainerEnvironmentBuilder.CreateProcessEquipment(materials, geometry);
            SafetyTrainerEnvironmentBuilder.CreateFacilityBackdrop(materials, geometry);

            var ui = SafetyTrainerUiBuilder.Create();
            var playerStart = SafetyTrainerScenarioBuilder.CreatePlayerStart();
            var manager = SafetyTrainerScenarioBuilder.CreateManager(ui, playerStart);

            SafetyTrainerScenarioBuilder.CreatePlayer(playerStart.position, playerStart.rotation);
            SafetyTrainerScenarioBuilder.CreatePpeStations(materials, geometry);
            SafetyTrainerScenarioBuilder.CreateHazards(materials, geometry);
            SafetyTrainerScenarioBuilder.CreateFinalStation(materials, geometry);
            SafetyTrainerScenarioBuilder.CreateGate(playerStart, materials, geometry);
            SafetyTrainerEnvironmentBuilder.CreateInstructionBoards(materials, geometry);

            EditorSceneManager.SaveScene(scene, SafetyTrainerPaths.ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(SafetyTrainerPaths.ScenePath, true) };
            PlayerSettings.productName = "Oil Safety Trainer VR Demo";
            PlayerSettings.companyName = "Codex Demo";
            PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
            PlayerSettings.defaultIsNativeResolution = true;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Standalone, "com.codexdemo.oilsafetytrainer");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Oil safety trainer demo scene generated at {SafetyTrainerPaths.ScenePath}");
        }
    }
}
