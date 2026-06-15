using System;
using System.Collections.Generic;
using System.Linq;
using OilSafetyTrainer;
using UnityEditor;
using UnityEngine;

namespace OilSafetyTrainer.Editor
{
    internal static class SafetyTrainerScenarioBuilder
    {
        public static Transform CreatePlayerStart()
        {
            var playerStart = new GameObject("Player Start").transform;
            playerStart.SetPositionAndRotation(new Vector3(0.6f, 0.05f, -10.75f), Quaternion.Euler(0f, 12f, 0f));
            return playerStart;
        }

        public static SafetyScenarioManager CreateManager(ScorePanelController ui, Transform playerStart)
        {
            return CreateManager(ui, playerStart, CreateScenarioConfig());
        }

        public static SafetyScenarioManager CreateManager(ScorePanelController ui, Transform playerStart, SafetyScenarioConfig scenarioConfig)
        {
            var managerObject = new GameObject("Safety Scenario Manager");
            var manager = managerObject.AddComponent<SafetyScenarioManager>();
            manager.ConfigureScenario(scenarioConfig);
            manager.scorePanel = ui;
            manager.playerStart = playerStart;
            return manager;
        }

        public static SafetyScenarioConfig CreateScenarioConfig()
        {
            var config = AssetDatabase.LoadAssetAtPath<SafetyScenarioConfig>(SafetyTrainerPaths.ScenarioConfigPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<SafetyScenarioConfig>();
                AssetDatabase.CreateAsset(config, SafetyTrainerPaths.ScenarioConfigPath);

                config.Configure(
                    SafetyTrainerScenarioDefaults.CreateRequiredPpe(),
                    SafetyTrainerScenarioDefaults.CreateHazards(),
                    SafetyTrainerScenarioDefaults.BaseScore,
                    SafetyTrainerScenarioDefaults.MissingPpePenalty,
                    SafetyTrainerScenarioDefaults.UninspectedHazardPenalty,
                    SafetyTrainerScenarioDefaults.ScenarioName,
                    SafetyTrainerText.Objective,
                    SafetyTrainerText.GuideText,
                    SafetyTrainerText.StartMessage);

                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssetIfDirty(config);
            }
            else if (config.EnsureInstructionText(
                SafetyTrainerScenarioDefaults.ScenarioName,
                SafetyTrainerText.Objective,
                SafetyTrainerText.GuideText,
                SafetyTrainerText.StartMessage))
            {
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssetIfDirty(config);
            }

            return config;
        }

        public static SafetyScenarioConfig CreateMaintenanceScenarioConfig()
        {
            var config = AssetDatabase.LoadAssetAtPath<SafetyScenarioConfig>(SafetyTrainerPaths.MaintenanceScenarioConfigPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<SafetyScenarioConfig>();
                AssetDatabase.CreateAsset(config, SafetyTrainerPaths.MaintenanceScenarioConfigPath);

                config.Configure(
                    SafetyTrainerScenarioDefaults.CreateMaintenanceRequiredPpe(),
                    SafetyTrainerScenarioDefaults.CreateMaintenanceHazards(),
                    SafetyTrainerScenarioDefaults.BaseScore,
                    SafetyTrainerScenarioDefaults.MaintenanceMissingPpePenalty,
                    SafetyTrainerScenarioDefaults.MaintenanceUninspectedHazardPenalty,
                    SafetyTrainerScenarioDefaults.MaintenanceScenarioName,
                    SafetyTrainerScenarioDefaults.MaintenanceObjective,
                    SafetyTrainerScenarioDefaults.MaintenanceGuideText,
                    SafetyTrainerScenarioDefaults.MaintenanceStartMessage);

                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssetIfDirty(config);
            }
            else if (config.EnsureInstructionText(
                SafetyTrainerScenarioDefaults.MaintenanceScenarioName,
                SafetyTrainerScenarioDefaults.MaintenanceObjective,
                SafetyTrainerScenarioDefaults.MaintenanceGuideText,
                SafetyTrainerScenarioDefaults.MaintenanceStartMessage))
            {
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssetIfDirty(config);
            }

            return config;
        }

        public static PlayerRig CreatePlayer(Vector3 position, Quaternion rotation, out DesktopPlayerController desktopController)
        {
            var player = new GameObject("Desktop PlayerRig");
            player.transform.SetPositionAndRotation(position, rotation);
            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.32f;
            controller.center = new Vector3(0f, 0.9f, 0f);
            var rig = player.AddComponent<PlayerRig>();
            desktopController = player.AddComponent<DesktopPlayerController>();

            var cameraObject = new GameObject("Player Camera");
            cameraObject.transform.SetParent(player.transform);
            cameraObject.transform.localPosition = new Vector3(0f, 1.62f, 0f);
            cameraObject.transform.localRotation = Quaternion.identity;
            var camera = cameraObject.AddComponent<Camera>();
            camera.nearClipPlane = 0.03f;
            camera.farClipPlane = 120f;
            camera.fieldOfView = 70f;
            cameraObject.AddComponent<AudioListener>();
            rig.Configure(cameraObject.transform, cameraObject.transform);
            return rig;
        }

        public static PpeStation[] CreatePpeStations(SafetyTrainerMaterialSet materials, Transform parent)
        {
            return CreatePpeStations(materials, parent, CreateScenarioConfig());
        }

        public static PpeStation[] CreatePpeStations(SafetyTrainerMaterialSet materials, Transform parent, SafetyScenarioConfig scenarioConfig)
        {
            return (scenarioConfig?.RequiredPpe ?? Array.Empty<SafetyScenarioConfig.PpeItem>())
                .Select(item =>
                {
                    var definition = SafetyTrainerVisualCatalog.GetPpe(item.id);
                    var material = ResolveMaterial(materials, definition.MaterialRole);
                    return CreatePpePlacard(definition.Name, item.id, item.label, definition.TexturePath, definition.Position, material, materials.SafeGreen, parent);
                })
                .ToArray();
        }

        public static HazardInspectionPoint[] CreateHazards(SafetyTrainerMaterialSet materials, Transform parent)
        {
            return CreateHazards(materials, parent, CreateScenarioConfig());
        }

        public static HazardInspectionPoint[] CreateHazards(SafetyTrainerMaterialSet materials, Transform parent, SafetyScenarioConfig scenarioConfig)
        {
            var hazards = new List<HazardInspectionPoint>();
            foreach (var item in scenarioConfig?.Hazards ?? Array.Empty<SafetyScenarioConfig.HazardItem>())
            {
                var definition = SafetyTrainerVisualCatalog.GetHazard(item.id);
                if (definition.Kind == HazardVisualKind.OilSpillSurface)
                {
                    hazards.Add(CreateOilSpillHazard(item, definition, materials, parent));
                    continue;
                }

                hazards.Add(CreateHazardPlacard(
                    definition.Name,
                    item.id,
                    item.label,
                    item.recommendation,
                    definition.TexturePath,
                    definition.Position,
                    definition.Rotation,
                    ResolveMaterial(materials, definition.MaterialRole),
                    materials.InspectionBlue,
                    parent));
            }

            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Temporary Missing Guardrail Visual", new Vector3(8.5f, 1.05f, 12.3f), new Vector3(2.2f, 0.18f, 0.18f), materials.Steel, parent);
            return hazards.ToArray();
        }

        private static HazardInspectionPoint CreateOilSpillHazard(SafetyScenarioConfig.HazardItem item, HazardVisualDefinition definition, SafetyTrainerMaterialSet materials, Transform parent)
        {
            var spill = SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Oil Spill Visual", new Vector3(3.5f, 0.03f, 2.1f), new Vector3(2.4f, 0.02f, 1.6f), materials.OilBlack, parent);
            spill.transform.rotation = Quaternion.Euler(0f, 25f, 0f);
            var spillDisplayMaterial = SafetyTrainerMaterialFactory.CreateDisplayMaterial("HazardOilSpillSurface", definition.TexturePath);
            if (spillDisplayMaterial != null)
            {
                spill.GetComponent<Renderer>().sharedMaterial = spillDisplayMaterial;
            }

            SafetyTrainerPrimitiveFactory.CreateDisplayPanel("Oil Spill Image", "HazardOilSpillSurface", definition.TexturePath, definition.Position, definition.Rotation, new Vector3(2.25f, 1.5f, 1f), spill.transform, false);
            return ConfigureHazard(spill, item.id, item.label, item.recommendation, materials.InspectionBlue);
        }

        private static Material ResolveMaterial(SafetyTrainerMaterialSet materials, SafetyTrainerMaterialRole role)
        {
            return role switch
            {
                SafetyTrainerMaterialRole.SafetyYellow => materials.SafetyYellow,
                SafetyTrainerMaterialRole.White => materials.White,
                SafetyTrainerMaterialRole.WarningOrange => materials.WarningOrange,
                SafetyTrainerMaterialRole.PipeRed => materials.PipeRed,
                _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
            };
        }

        public static void CreateFinalStation(SafetyTrainerMaterialSet materials, Transform parent)
        {
            var terminal = SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Final Assessment Terminal", new Vector3(17f, 1f, 10f), new Vector3(1.2f, 2f, 0.7f), materials.SafeGreen, parent);
            terminal.AddComponent<FinalAssessmentStation>().ConfigureInteraction("Итоговая оценка", "Нажмите E", terminal.GetComponent<Renderer>());
            SafetyTrainerPrimitiveFactory.CreateDisplayPanel("Terminal Screen Display", "TerminalScreenDisplay", SafetyTrainerPaths.FinalTerminalPath, new Vector3(17f, 1.38f, 9.62f), Quaternion.identity, new Vector3(0.82f, 0.52f, 1f), terminal.transform);
            SafetyTrainerPrimitiveFactory.CreateWorldLabel("Final Station Label", "Терминал итоговой оценки", new Vector3(17f, 2.35f, 9.58f), Quaternion.identity, 0.16f, Color.black, parent);
        }

        public static void CreateGate(Transform returnPoint, SafetyTrainerMaterialSet materials, Transform parent)
        {
            var gate = new GameObject("PPE Work Zone Gate");
            gate.transform.SetParent(parent);
            gate.transform.position = new Vector3(8f, 1f, -4.1f);
            var collider = gate.AddComponent<BoxCollider>();
            collider.size = new Vector3(26f, 2.3f, 0.5f);
            collider.isTrigger = true;
            gate.AddComponent<WorkZoneGate>().Configure(returnPoint);

            var visual = SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Gate Visual", gate.transform.position, collider.size, materials.GateVolume, parent);
            visual.transform.SetParent(gate.transform, true);
            UnityEngine.Object.DestroyImmediate(visual.GetComponent<Collider>());
        }

        private static PpeStation CreatePpePlacard(string name, string id, string label, string texturePath, Vector3 position, Material material, Material selectedMaterial, Transform parent)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent);
            root.transform.position = position;

            var pedestal = SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, $"{name} Pedestal", position + new Vector3(0f, -0.62f, 0f), new Vector3(1.05f, 0.18f, 0.7f), material, parent);
            pedestal.transform.SetParent(root.transform, true);
            var stand = SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, $"{name} Stand", position + new Vector3(0f, -0.2f, 0.12f), new Vector3(0.07f, 0.72f, 0.07f), material, parent);
            stand.transform.SetParent(root.transform, true);
            var board = SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, $"{name} Board", position + new Vector3(0f, 0.06f, 0f), new Vector3(0.72f, 0.72f, 0.05f), material, parent);
            board.transform.SetParent(root.transform, true);
            SafetyTrainerPrimitiveFactory.CreateDisplayPanel($"{name} Image", $"{name.Replace(" ", string.Empty)}Display", texturePath, position + new Vector3(0f, 0.06f, -0.075f), Quaternion.Euler(0f, 180f, 0f), new Vector3(0.64f, 0.64f, 1f), root.transform);
            SafetyTrainerPrimitiveFactory.CreateInteractionProxy($"{name} Interaction Zone", position + new Vector3(0f, 0.06f, -0.12f), Quaternion.identity, new Vector3(0.92f, 0.92f, 0.28f), root.transform);

            var renderer = board.GetComponent<Renderer>();
            var station = root.AddComponent<PpeStation>();
            station.Configure(id, label, renderer, selectedMaterial.color);
            station.ConfigureInteraction(label, "Нажмите E", renderer);
            return station;
        }

        private static HazardInspectionPoint CreateHazardPlacard(string name, string id, string label, string mitigation, string texturePath, Vector3 position, Quaternion rotation, Material material, Material inspected, Transform parent)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent);
            root.transform.SetPositionAndRotation(position, rotation);

            var stand = SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, $"{name} Stand", position + rotation * new Vector3(0f, -0.55f, 0.04f), new Vector3(0.08f, 1.1f, 0.08f), material, parent);
            stand.transform.rotation = rotation;
            stand.transform.SetParent(root.transform, true);
            var board = SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, $"{name} Board", position, new Vector3(1.2f, 0.82f, 0.06f), material, parent);
            board.transform.rotation = rotation;
            board.transform.SetParent(root.transform, true);
            SafetyTrainerPrimitiveFactory.CreateDisplayPanel($"{name} Image", $"{name.Replace(" ", string.Empty)}Display", texturePath, position + rotation * new Vector3(0f, 0f, -0.11f), rotation, new Vector3(1.08f, 0.68f, 1f), root.transform, false);
            SafetyTrainerPrimitiveFactory.CreateInteractionProxy($"{name} Interaction Zone", position + rotation * new Vector3(0f, 0f, -0.18f), rotation, new Vector3(1.35f, 0.96f, 0.35f), root.transform);

            return ConfigureHazard(root, id, label, mitigation, inspected, board.GetComponent<Renderer>());
        }

        private static HazardInspectionPoint ConfigureHazard(GameObject marker, string id, string label, string mitigation, Material inspected, Renderer statusRenderer = null)
        {
            var renderer = statusRenderer != null ? statusRenderer : marker.GetComponentInChildren<Renderer>();
            var hazard = marker.AddComponent<HazardInspectionPoint>();
            hazard.Configure(id, label, mitigation, renderer, inspected.color);
            hazard.ConfigureInteraction(label, "Нажмите E", renderer);
            return hazard;
        }
    }
}
