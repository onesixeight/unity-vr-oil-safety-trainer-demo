using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using OilSafetyTrainer;

namespace OilSafetyTrainer.Editor
{
    public static class SafetyTrainerProjectBuilder
    {
        [MenuItem("Oil Safety Trainer/Rebuild Demo Scene")]
        public static void BuildProject()
        {
            BuildScenarioScene(SafetyTrainerPaths.ScenePath, SafetyTrainerScenarioBuilder.CreateScenarioConfig());
        }

        [MenuItem("Oil Safety Trainer/Rebuild Maintenance Demo Scene")]
        public static void BuildMaintenanceProject()
        {
            BuildScenarioScene(SafetyTrainerPaths.MaintenanceScenePath, SafetyTrainerScenarioBuilder.CreateMaintenanceScenarioConfig());
        }

        private static void BuildScenarioScene(string scenePath, SafetyScenarioConfig scenarioConfig)
        {
            SafetyTrainerPaths.EnsureFolders();
            SafetyTrainerTextMeshProResources.EnsureImported();

            var materials = SafetyTrainerMaterialFactory.CreateMaterialSet();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = System.IO.Path.GetFileNameWithoutExtension(scenePath);

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
            var manager = SafetyTrainerScenarioBuilder.CreateManager(ui, playerStart, scenarioConfig);

            var playerRig = SafetyTrainerScenarioBuilder.CreatePlayer(playerStart.position, playerStart.rotation, out var desktopController);
            var ppeStations = SafetyTrainerScenarioBuilder.CreatePpeStations(materials, geometry, scenarioConfig);
            var hazards = SafetyTrainerScenarioBuilder.CreateHazards(materials, geometry, scenarioConfig);
            manager.ConfigureRuntimeReferences(ppeStations, hazards, playerRig, desktopController);
            SafetyTrainerScenarioBuilder.CreateFinalStation(materials, geometry);
            SafetyTrainerScenarioBuilder.CreateGate(playerStart, materials, geometry);
            SafetyTrainerEnvironmentBuilder.CreateInstructionBoards(materials, geometry);

            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.ImportAsset(scenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath, true) };
            PlayerSettings.productName = "Oil Safety Trainer VR Demo";
            PlayerSettings.companyName = "Codex Demo";
            PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
            PlayerSettings.defaultIsNativeResolution = true;
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Standalone, "com.codexdemo.oilsafetytrainer");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Oil safety trainer demo scene generated at {scenePath}");
        }
    }
}
