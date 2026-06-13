using System.IO;
using OilSafetyTrainer;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OilSafetyTrainer.Editor
{
    public static class SafetyTrainerProjectBuilder
    {
        private const string ScenePath = "Assets/Scenes/OilSafetyTrainerDemo.unity";
        private const string MaterialFolder = "Assets/Materials";
        private const string TextureFolder = "Assets/Textures/PolyHaven";
        private const string ArtFolder = "Assets/Art";
        private const string HazardArtFolder = ArtFolder + "/Hazards";
        private const string PosterArtFolder = ArtFolder + "/UI/Posters";
        private const string PpeArtFolder = ArtFolder + "/UI/PPE";
        private const string TerminalArtFolder = ArtFolder + "/UI/Terminal";
        private const string ConcreteDiffusePath = TextureFolder + "/concrete_floor_02_diff_1k.jpg";
        private const string ConcreteNormalPath = TextureFolder + "/concrete_floor_02_nor_gl_1k.jpg";
        private const string AsphaltDiffusePath = TextureFolder + "/asphalt_floor_diff_1k.jpg";
        private const string AsphaltNormalPath = TextureFolder + "/asphalt_floor_nor_gl_1k.jpg";
        private const string SteelDiffusePath = TextureFolder + "/blue_metal_plate_diff_1k.jpg";
        private const string SteelNormalPath = TextureFolder + "/blue_metal_plate_nor_gl_1k.jpg";
        private const string CheckpointPosterPath = PosterArtFolder + "/checkpoint_poster_bg.png";
        private const string WorkZonePosterPath = PosterArtFolder + "/workzone_poster_bg.png";
        private const string FinalTerminalPath = TerminalArtFolder + "/final_terminal_bg.png";
        private const string PpeHelmetPath = PpeArtFolder + "/ppe_helmet.png";
        private const string PpeGogglesPath = PpeArtFolder + "/ppe_goggles.png";
        private const string PpeGlovesPath = PpeArtFolder + "/ppe_gloves.png";
        private const string PpeBootsPath = PpeArtFolder + "/ppe_boots.png";
        private const string HazardGuardrailPath = HazardArtFolder + "/hazard_guardrail_gap.png";
        private const string HazardOilSpillPath = HazardArtFolder + "/hazard_oil_spill.png";
        private const string HazardHotPipePath = HazardArtFolder + "/hazard_hot_pipe.png";
        private const string HazardGasWarningPath = HazardArtFolder + "/hazard_gas_warning.png";
        private const string HazardUnsafeValvePath = HazardArtFolder + "/hazard_unsafe_valve.png";

        [MenuItem("Oil Safety Trainer/Rebuild Demo Scene")]
        public static void BuildProject()
        {
            EnsureFolders();

            var concrete = CreateTexturedMaterial("Concrete", new Color(0.93f, 0.93f, 0.93f), ConcreteDiffusePath, ConcreteNormalPath, new Vector2(5f, 5f), 0f, 0.18f);
            var asphalt = CreateTexturedMaterial("Asphalt", Color.white, AsphaltDiffusePath, AsphaltNormalPath, new Vector2(2f, 7f), 0f, 0.08f);
            var safetyYellow = CreateMaterial("SafetyYellow", new Color(1f, 0.72f, 0.05f));
            var steel = CreateTexturedMaterial("Steel", Color.white, SteelDiffusePath, SteelNormalPath, new Vector2(1.5f, 1.5f), 0.38f, 0.32f);
            var pipeRed = CreateMaterial("HotPipeRed", new Color(0.75f, 0.12f, 0.08f));
            var oilBlack = CreateMaterial("OilBlack", new Color(0.02f, 0.02f, 0.018f));
            var green = CreateMaterial("SafeGreen", new Color(0.1f, 0.6f, 0.24f));
            var blue = CreateMaterial("InspectionBlue", new Color(0.1f, 0.5f, 0.95f));
            var white = CreateMaterial("WhitePaint", Color.white);
            var orange = CreateMaterial("WarningOrange", new Color(1f, 0.38f, 0.04f));
            var beacon = CreateEmissiveMaterial("BeaconWarning", new Color(1f, 0.38f, 0.04f), new Color(2.3f, 0.8f, 0.1f));
            var transparentGate = CreateTransparentMaterial("GateVolume", new Color(1f, 0.8f, 0f, 0.16f));

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "OilSafetyTrainerDemo";

            RenderSettings.ambientLight = new Color(0.62f, 0.66f, 0.7f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.63f, 0.67f, 0.71f);
            RenderSettings.fogDensity = 0.0125f;
            CreateLighting();

            var root = new GameObject("Oil Production Safety Trainer");
            var geometry = new GameObject("Generated Geometry").transform;
            geometry.SetParent(root.transform);

            CreatePrimitive(PrimitiveType.Cube, "Training Yard Floor", new Vector3(8f, -0.05f, 12f), new Vector3(26f, 0.1f, 28f), concrete, geometry);
            CreatePrimitive(PrimitiveType.Cube, "Dark Access Road", new Vector3(8f, 0.01f, 0f), new Vector3(7f, 0.04f, 24f), asphalt, geometry);
            CreatePrimitive(PrimitiveType.Cube, "Checkpoint Pad", new Vector3(0f, 0.02f, -8f), new Vector3(8f, 0.06f, 6f), safetyYellow, geometry);
            CreatePrimitive(PrimitiveType.Cube, "Final Assessment Pad", new Vector3(17f, 0.03f, 10f), new Vector3(5f, 0.06f, 4f), green, geometry);

            CreateFence(new Vector3(-5f, 0.8f, -2f), new Vector3(0.15f, 1.6f, 22f), steel, geometry, "Left Fence");
            CreateFence(new Vector3(21f, 0.8f, -2f), new Vector3(0.15f, 1.6f, 22f), steel, geometry, "Right Fence");
            CreateFence(new Vector3(8f, 0.8f, 13f), new Vector3(26f, 1.6f, 0.15f), steel, geometry, "Back Fence");
            CreatePrimitive(PrimitiveType.Cube, "Checkpoint Arch Left", new Vector3(4.4f, 1.7f, -4f), new Vector3(0.35f, 3.4f, 0.35f), safetyYellow, geometry);
            CreatePrimitive(PrimitiveType.Cube, "Checkpoint Arch Right", new Vector3(11.6f, 1.7f, -4f), new Vector3(0.35f, 3.4f, 0.35f), safetyYellow, geometry);
            CreatePrimitive(PrimitiveType.Cube, "Checkpoint Arch Top", new Vector3(8f, 3.25f, -4f), new Vector3(7.6f, 0.35f, 0.35f), safetyYellow, geometry);

            CreateCheckpointProps(steel, white, safetyYellow, geometry);
            CreateProcessEquipment(steel, pipeRed, orange, beacon, geometry);
            CreateFacilityBackdrop(steel, concrete, orange, geometry);

            var ui = CreateUi();
            var playerStart = new GameObject("Player Start").transform;
            playerStart.SetPositionAndRotation(new Vector3(-3.5f, 0.05f, -10.8f), Quaternion.Euler(0f, 24f, 0f));

            var managerObject = new GameObject("Safety Scenario Manager");
            var manager = managerObject.AddComponent<SafetyScenarioManager>();
            manager.requiredPpe = new[]
            {
                new SafetyScenarioManager.PpeRequirement { id = "helmet", label = "Каска" },
                new SafetyScenarioManager.PpeRequirement { id = "goggles", label = "Защитные очки" },
                new SafetyScenarioManager.PpeRequirement { id = "gloves", label = "Перчатки" },
                new SafetyScenarioManager.PpeRequirement { id = "boots", label = "Диэлектрические ботинки" },
            };
            manager.hazards = new[]
            {
                new SafetyScenarioManager.HazardRequirement { id = "guardrail_gap", label = "Разрыв ограждения" },
                new SafetyScenarioManager.HazardRequirement { id = "oil_spill", label = "Разлив нефти/масла" },
                new SafetyScenarioManager.HazardRequirement { id = "hot_pipe", label = "Горячая поверхность трубопровода" },
                new SafetyScenarioManager.HazardRequirement { id = "gas_warning", label = "Сигнал газоанализатора" },
                new SafetyScenarioManager.HazardRequirement { id = "unsafe_valve", label = "Открытый/непромаркированный клапан" },
            };
            manager.scorePanel = ui;
            manager.playerStart = playerStart;

            CreatePlayer(playerStart.position, playerStart.rotation);
            CreatePpeStations(safetyYellow, white, green, geometry);
            CreateHazards(oilBlack, pipeRed, orange, blue, steel, geometry);
            CreateFinalStation(green, white, geometry);
            CreateGate(playerStart, transparentGate, geometry);
            CreateInstructionBoards(white, safetyYellow, geometry);
            CreatePpePreviewPlacards(steel, white, geometry);
            CreateHazardReferenceGallery(steel, geometry);

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            PlayerSettings.productName = "Oil Safety Trainer VR Demo";
            PlayerSettings.companyName = "Codex Demo";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Standalone, "com.codexdemo.oilsafetytrainer");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Oil safety trainer demo scene generated at {ScenePath}");
        }

        private static void EnsureFolders()
        {
            Directory.CreateDirectory("Assets/Scenes");
            Directory.CreateDirectory(MaterialFolder);
            Directory.CreateDirectory(TextureFolder);
        }

        private static void CreateLighting()
        {
            var sun = new GameObject("Sun");
            var light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            light.color = new Color(1f, 0.96f, 0.9f);
            light.shadows = LightShadows.Soft;
            light.transform.rotation = Quaternion.Euler(50f, -35f, 0f);

            var fill = new GameObject("Soft Fill Light");
            var fillLight = fill.AddComponent<Light>();
            fillLight.type = LightType.Point;
            fillLight.intensity = 0.65f;
            fillLight.range = 22f;
            fillLight.transform.position = new Vector3(2f, 6f, -8f);

            var processFill = new GameObject("Process Area Fill Light");
            var processAreaLight = processFill.AddComponent<Light>();
            processAreaLight.type = LightType.Point;
            processAreaLight.intensity = 0.82f;
            processAreaLight.range = 26f;
            processAreaLight.color = new Color(1f, 0.92f, 0.85f);
            processAreaLight.transform.position = new Vector3(11f, 7f, 5f);
        }

        private static void CreateCheckpointProps(Material steel, Material white, Material accent, Transform parent)
        {
            CreatePrimitive(PrimitiveType.Cube, "PPE Rack Base", new Vector3(0f, 0.55f, -8.4f), new Vector3(7.5f, 1f, 1.9f), steel, parent);
            CreatePrimitive(PrimitiveType.Cube, "PPE Rack Top", new Vector3(0f, 1.55f, -8.4f), new Vector3(7.6f, 0.2f, 1.95f), accent, parent);
            CreatePrimitive(PrimitiveType.Cube, "Checkpoint Desk", new Vector3(5.8f, 0.7f, -6.4f), new Vector3(2.2f, 1.4f, 0.8f), steel, parent);
            CreatePrimitive(PrimitiveType.Cube, "Desk Monitor", new Vector3(5.8f, 1.45f, -6.75f), new Vector3(0.8f, 0.48f, 0.08f), white, parent);
            CreateDisplayPanel(
                "Checkpoint Desk Display",
                "CheckpointDeskDisplay",
                CheckpointPosterPath,
                new Vector3(5.8f, 1.45f, -6.8f),
                Quaternion.Euler(0f, 180f, 0f),
                new Vector3(0.72f, 0.42f, 1f),
                parent);
        }

        private static void CreateProcessEquipment(Material steel, Material pipeRed, Material warning, Material beacon, Transform parent)
        {
            CreatePrimitive(PrimitiveType.Cylinder, "Separator Vessel", new Vector3(8f, 1.2f, 5f), new Vector3(1.8f, 2.4f, 1.8f), steel, parent);
            CreatePrimitive(PrimitiveType.Cube, "Pump Skid", new Vector3(4f, 0.45f, 5.5f), new Vector3(3.2f, 0.9f, 1.8f), steel, parent);
            CreatePrimitive(PrimitiveType.Cylinder, "Hot Pipe Horizontal", new Vector3(10f, 1.1f, 2.5f), new Vector3(0.45f, 4.6f, 0.45f), pipeRed, parent).transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            CreatePrimitive(PrimitiveType.Cylinder, "Vertical Pipe", new Vector3(14f, 1.8f, 5f), new Vector3(0.45f, 3.6f, 0.45f), steel, parent);
            CreatePrimitive(PrimitiveType.Cube, "Valve Manifold Base", new Vector3(13f, 0.55f, 8f), new Vector3(3.5f, 1.1f, 1.2f), steel, parent);
            CreatePrimitive(PrimitiveType.Cylinder, "Valve Wheel", new Vector3(13f, 1.35f, 7.25f), new Vector3(1f, 0.12f, 1f), warning, parent).transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            CreatePrimitive(PrimitiveType.Cylinder, "Gas Warning Beacon Visual", new Vector3(15.2f, 1.8f, 2.8f), new Vector3(0.38f, 0.22f, 0.38f), beacon, parent);
            CreatePrimitive(PrimitiveType.Cube, "Pipe Support A", new Vector3(9.2f, 0.75f, 2.5f), new Vector3(0.25f, 1.5f, 0.8f), steel, parent);
            CreatePrimitive(PrimitiveType.Cube, "Pipe Support B", new Vector3(10.8f, 0.75f, 2.5f), new Vector3(0.25f, 1.5f, 0.8f), steel, parent);
            CreatePrimitive(PrimitiveType.Cylinder, "Secondary Pipe", new Vector3(6.4f, 0.95f, 6.5f), new Vector3(0.26f, 3f, 0.26f), steel, parent).transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            CreatePrimitive(PrimitiveType.Cube, "Equipment Platform", new Vector3(8f, 0.3f, 5.6f), new Vector3(9.8f, 0.12f, 4.6f), steel, parent);
        }

        private static void CreateFacilityBackdrop(Material steel, Material concrete, Material accent, Transform parent)
        {
            CreatePrimitive(PrimitiveType.Cylinder, "Storage Tank A", new Vector3(19f, 2.6f, 5f), new Vector3(2.2f, 5.2f, 2.2f), steel, parent);
            CreatePrimitive(PrimitiveType.Cylinder, "Storage Tank B", new Vector3(22.5f, 2.3f, 6.2f), new Vector3(1.8f, 4.6f, 1.8f), steel, parent);
            CreatePrimitive(PrimitiveType.Cube, "Tank Platform", new Vector3(20.8f, 0.16f, 5.6f), new Vector3(7.2f, 0.18f, 4.8f), concrete, parent);
            CreatePrimitive(PrimitiveType.Cube, "Pipe Rack Beam", new Vector3(15.5f, 3.2f, 1.2f), new Vector3(8.5f, 0.22f, 0.32f), steel, parent);
            CreatePrimitive(PrimitiveType.Cube, "Pipe Rack Leg A", new Vector3(12.2f, 1.6f, 1.2f), new Vector3(0.32f, 3.2f, 0.32f), steel, parent);
            CreatePrimitive(PrimitiveType.Cube, "Pipe Rack Leg B", new Vector3(18.8f, 1.6f, 1.2f), new Vector3(0.32f, 3.2f, 0.32f), steel, parent);
            CreatePrimitive(PrimitiveType.Cylinder, "Pipe Rack Line A", new Vector3(15.5f, 3.55f, 0.95f), new Vector3(0.18f, 4.1f, 0.18f), accent, parent).transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            CreatePrimitive(PrimitiveType.Cylinder, "Pipe Rack Line B", new Vector3(15.5f, 3.4f, 1.45f), new Vector3(0.14f, 4.1f, 0.14f), steel, parent).transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            CreatePrimitive(PrimitiveType.Cube, "Access Stairs Base", new Vector3(18f, 0.45f, 8.2f), new Vector3(1.2f, 0.9f, 2.8f), concrete, parent);
        }

        private static void CreatePpeStations(Material yellow, Material white, Material selected, Transform parent)
        {
            CreatePpe("PPE Helmet", "helmet", "Каска", new Vector3(-2.8f, 0.75f, -8.4f), PrimitiveType.Sphere, yellow, selected, parent);
            CreatePpe("PPE Goggles", "goggles", "Защитные очки", new Vector3(-1f, 0.75f, -8.4f), PrimitiveType.Cube, white, selected, parent);
            CreatePpe("PPE Gloves", "gloves", "Перчатки", new Vector3(0.8f, 0.75f, -8.4f), PrimitiveType.Capsule, yellow, selected, parent);
            CreatePpe("PPE Boots", "boots", "Диэлектрические ботинки", new Vector3(2.6f, 0.75f, -8.4f), PrimitiveType.Cube, white, selected, parent);
        }

        private static void CreatePpe(string name, string id, string label, Vector3 position, PrimitiveType primitive, Material material, Material selectedMaterial, Transform parent)
        {
            var pedestal = CreatePrimitive(PrimitiveType.Cube, $"{name} Pedestal", position + Vector3.down * 0.45f, new Vector3(1.2f, 0.25f, 1.2f), material, parent);
            var item = CreatePrimitive(primitive, name, position, new Vector3(0.65f, 0.65f, 0.65f), material, parent);
            var renderer = item.GetComponent<Renderer>();
            var station = item.AddComponent<PpeStation>();
            station.Configure(id, label, renderer, selectedMaterial.color);
            station.ConfigureInteraction(label, "Нажмите E", renderer);
            item.transform.SetParent(pedestal.transform, true);
        }

        private static void CreateHazards(Material oil, Material hot, Material warning, Material inspected, Material steel, Transform parent)
        {
            CreateHazard(
                "Hazard Guardrail Gap",
                "guardrail_gap",
                "Разрыв ограждения",
                "Остановитесь, выставьте временное ограждение и сообщите ответственному.",
                new Vector3(6f, 1f, 12.3f),
                new Vector3(2f, 1.8f, 0.25f),
                warning,
                inspected,
                parent);

            var spill = CreatePrimitive(PrimitiveType.Cylinder, "Oil Spill Visual", new Vector3(3.5f, 0.04f, 2.1f), new Vector3(2.2f, 0.04f, 1.35f), oil, parent);
            spill.transform.rotation = Quaternion.Euler(0f, 25f, 0f);
            AddHazardComponent(spill, "oil_spill", "Разлив нефти/масла", "Оградите место, используйте сорбент и сообщите о проливе.", inspected);

            CreateHazard(
                "Hazard Hot Pipe Marker",
                "hot_pipe",
                "Горячая поверхность трубопровода",
                "Не касайтесь трубы без допуска, проверьте термоизоляцию и предупреждающие знаки.",
                new Vector3(10f, 1.65f, 2.5f),
                new Vector3(1.1f, 1.1f, 1.1f),
                hot,
                inspected,
                parent);

            CreateHazard(
                "Hazard Gas Warning Beacon",
                "gas_warning",
                "Сигнал газоанализатора",
                "Покиньте опасную зону по ветру, включите оповещение и действуйте по плану эвакуации.",
                new Vector3(15.2f, 1.2f, 2.8f),
                new Vector3(0.75f, 1.5f, 0.75f),
                warning,
                inspected,
                parent);

            CreateHazard(
                "Hazard Unsafe Valve Marker",
                "unsafe_valve",
                "Открытый/непромаркированный клапан",
                "Не переключайте арматуру без наряда, проверьте бирку LOTO и схему трубопровода.",
                new Vector3(13f, 1.65f, 8f),
                new Vector3(1.1f, 1.1f, 1.1f),
                warning,
                inspected,
                parent);

            CreatePrimitive(PrimitiveType.Cube, "Temporary Missing Guardrail Visual", new Vector3(8.5f, 1.05f, 12.3f), new Vector3(2.2f, 0.18f, 0.18f), steel, parent);
        }

        private static void CreateHazard(
            string name,
            string id,
            string label,
            string mitigation,
            Vector3 position,
            Vector3 scale,
            Material material,
            Material inspected,
            Transform parent)
        {
            var marker = CreatePrimitive(PrimitiveType.Cube, name, position, scale, material, parent);
            AddHazardComponent(marker, id, label, mitigation, inspected);
        }

        private static void AddHazardComponent(GameObject marker, string id, string label, string mitigation, Material inspected)
        {
            var renderer = marker.GetComponent<Renderer>();
            var hazard = marker.AddComponent<HazardInspectionPoint>();
            hazard.Configure(id, label, mitigation, renderer, inspected.color);
            hazard.ConfigureInteraction(label, "Нажмите E", renderer);
        }

        private static void CreateFinalStation(Material green, Material white, Transform parent)
        {
            var terminal = CreatePrimitive(PrimitiveType.Cube, "Final Assessment Terminal", new Vector3(17f, 1f, 10f), new Vector3(1.2f, 2f, 0.7f), green, parent);
            terminal.AddComponent<FinalAssessmentStation>().ConfigureInteraction("Итоговая оценка", "Нажмите E", terminal.GetComponent<Renderer>());
            CreatePrimitive(PrimitiveType.Cube, "Terminal Screen", new Vector3(17f, 1.35f, 9.62f), new Vector3(0.85f, 0.55f, 0.05f), white, parent);
            CreateDisplayPanel(
                "Terminal Screen Display",
                "TerminalScreenDisplay",
                FinalTerminalPath,
                new Vector3(17f, 1.35f, 9.58f),
                Quaternion.Euler(0f, 180f, 0f),
                new Vector3(0.76f, 0.46f, 1f),
                parent);
        }

        private static void CreateGate(Transform returnPoint, Material material, Transform parent)
        {
            var gate = CreatePrimitive(PrimitiveType.Cube, "Work Zone PPE Gate Trigger", new Vector3(8f, 1.2f, -3.6f), new Vector3(25.8f, 2.4f, 0.2f), material, parent);
            var collider = gate.GetComponent<BoxCollider>();
            collider.isTrigger = true;
            gate.AddComponent<WorkZoneGate>().Configure(returnPoint);
        }

        private static void CreateInstructionBoards(Material white, Material yellow, Transform parent)
        {
            CreatePrimitive(PrimitiveType.Cube, "Board Checkpoint", new Vector3(-4.2f, 1.8f, -7.5f), new Vector3(0.1f, 2.2f, 3.4f), white, parent);
            CreatePrimitive(PrimitiveType.Cube, "Board Work Zone", new Vector3(18.8f, 1.8f, -1.8f), new Vector3(0.1f, 2.2f, 3.6f), yellow, parent);
            CreateDisplayPanel(
                "Checkpoint Poster Display",
                "CheckpointPosterDisplay",
                CheckpointPosterPath,
                new Vector3(-4.13f, 1.8f, -7.5f),
                Quaternion.Euler(0f, 90f, 0f),
                new Vector3(2.8f, 1.75f, 1f),
                parent);
            CreateDisplayPanel(
                "Work Zone Poster Display",
                "WorkZonePosterDisplay",
                WorkZonePosterPath,
                new Vector3(18.73f, 1.8f, -1.8f),
                Quaternion.Euler(0f, -90f, 0f),
                new Vector3(3f, 1.75f, 1f),
                parent);
            CreateWorldLabel("Board Checkpoint Text", "1. Наденьте все СИЗ\n2. Пройдите КПП", new Vector3(-4.08f, 2f, -7.5f), Quaternion.Euler(0f, 90f, 0f), 0.2f, Color.black, parent);
            CreateWorldLabel("Board Work Zone Text", "Найдите 5 опасностей\nи завершите обход", new Vector3(18.68f, 2f, -1.8f), Quaternion.Euler(0f, -90f, 0f), 0.2f, Color.black, parent);
            CreateWorldLabel("Final Station Label", "Терминал итоговой оценки", new Vector3(17f, 2.35f, 9.58f), Quaternion.identity, 0.16f, Color.black, parent);
        }

        private static void CreatePpePreviewPlacards(Material support, Material backing, Transform parent)
        {
            CreateReferencePlacard("PPE Helmet Placard", "PpeHelmetPlacard", PpeHelmetPath, new Vector3(-2.8f, 1.55f, -9.3f), Quaternion.Euler(0f, 180f, 0f), new Vector3(0.86f, 0.86f, 0.05f), support, backing, parent);
            CreateReferencePlacard("PPE Goggles Placard", "PpeGogglesPlacard", PpeGogglesPath, new Vector3(-1f, 1.55f, -9.3f), Quaternion.Euler(0f, 180f, 0f), new Vector3(0.86f, 0.86f, 0.05f), support, backing, parent);
            CreateReferencePlacard("PPE Gloves Placard", "PpeGlovesPlacard", PpeGlovesPath, new Vector3(0.8f, 1.55f, -9.3f), Quaternion.Euler(0f, 180f, 0f), new Vector3(0.86f, 0.86f, 0.05f), support, backing, parent);
            CreateReferencePlacard("PPE Boots Placard", "PpeBootsPlacard", PpeBootsPath, new Vector3(2.6f, 1.55f, -9.3f), Quaternion.Euler(0f, 180f, 0f), new Vector3(0.86f, 0.86f, 0.05f), support, backing, parent);
        }

        private static void CreateHazardReferenceGallery(Material support, Transform parent)
        {
            CreatePrimitive(PrimitiveType.Cube, "Hazard Reference Wall", new Vector3(20.55f, 1.9f, 7.5f), new Vector3(0.1f, 2.4f, 9.4f), support, parent);
            CreateDisplayPanel("Hazard Reference Guardrail", "HazardGuardrailReference", HazardGuardrailPath, new Vector3(20.48f, 2.45f, 10.8f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.3f, 0.74f, 1f), parent);
            CreateDisplayPanel("Hazard Reference Oil Spill", "HazardOilSpillReference", HazardOilSpillPath, new Vector3(20.48f, 1.95f, 8.9f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.3f, 0.74f, 1f), parent);
            CreateDisplayPanel("Hazard Reference Hot Pipe", "HazardHotPipeReference", HazardHotPipePath, new Vector3(20.48f, 1.45f, 7f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.3f, 0.74f, 1f), parent);
            CreateDisplayPanel("Hazard Reference Gas Warning", "HazardGasWarningReference", HazardGasWarningPath, new Vector3(20.48f, 0.95f, 5.1f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.3f, 0.74f, 1f), parent);
            CreateDisplayPanel("Hazard Reference Unsafe Valve", "HazardUnsafeValveReference", HazardUnsafeValvePath, new Vector3(20.48f, 0.45f, 3.2f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.3f, 0.74f, 1f), parent);
        }

        private static void CreatePlayer(Vector3 position, Quaternion rotation)
        {
            var player = new GameObject("Desktop PlayerRig");
            player.transform.SetPositionAndRotation(position, rotation);
            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.32f;
            controller.center = new Vector3(0f, 0.9f, 0f);
            var rig = player.AddComponent<PlayerRig>();
            player.AddComponent<DesktopPlayerController>();

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
        }

        private static ScorePanelController CreateUi()
        {
            CreateEventSystem();

            var canvasObject = new GameObject("Safety Trainer UI");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();
            var controller = canvasObject.AddComponent<ScorePanelController>();

            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            var objective = CreateText("Objective", canvasObject.transform, font, "VR-тренажёр: безопасный обход", 20, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -18f), new Vector2(720f, 52f), new Vector2(0f, 1f), true, 18, 20);
            var checklist = CreateText("Checklist", canvasObject.transform, font, string.Empty, 16, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -78f), new Vector2(560f, 420f), new Vector2(0f, 1f), true, 14, 16);
            var score = CreateText("Score", canvasObject.transform, font, string.Empty, 18, TextAnchor.UpperLeft, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(20f, 70f), new Vector2(360f, 42f), new Vector2(0f, 0f), true, 16, 18);
            var prompt = CreateText("Prompt", canvasObject.transform, font, string.Empty, 18, TextAnchor.LowerCenter, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 18f), new Vector2(1240f, 62f), new Vector2(0.5f, 0f), true, 15, 18);
            var message = CreateText("Message", canvasObject.transform, font, string.Empty, 18, TextAnchor.UpperRight, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-20f, -250f), new Vector2(580f, 170f), new Vector2(1f, 1f), true, 15, 18);

            var guidePanel = CreatePanel("Guide Panel", canvasObject.transform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-20f, -20f), new Vector2(412f, 220f), new Color(0.02f, 0.06f, 0.08f, 0.82f), new Vector2(1f, 1f));
            var guideGroup = guidePanel.AddComponent<CanvasGroup>();
            CreateText("Guide Title", guidePanel.transform, font, "Памятка", 21, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(18f, -18f), new Vector2(190f, 30f), new Vector2(0f, 1f), true, 18, 21);
            var guideText = CreateText("Guide Text", guidePanel.transform, font, string.Empty, 14, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(18f, -52f), new Vector2(376f, 150f), new Vector2(0f, 1f), true, 11, 14);

            var finalPanel = CreatePanel("Final Panel", canvasObject.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(760f, 430f), new Color(0f, 0f, 0f, 0.82f), new Vector2(0.5f, 0.5f));
            var group = finalPanel.AddComponent<CanvasGroup>();
            CreateText("Final Title", finalPanel.transform, font, "Результаты обхода", 27, TextAnchor.UpperCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -20f), new Vector2(520f, 36f), new Vector2(0.5f, 1f), true, 24, 27);
            var finalText = CreateText("Final Text", finalPanel.transform, font, string.Empty, 20, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -70f), new Vector2(712f, 250f), new Vector2(0f, 1f), true, 15, 20);
            var resetButton = CreateButton("Reset Button", finalPanel.transform, font, "Начать заново", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-118f, 28f), new Vector2(220f, 50f), new Color(0.12f, 0.5f, 0.22f, 0.96f), new Vector2(0.5f, 0f));
            var quitButton = CreateButton("Quit Button", finalPanel.transform, font, "Выйти", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(118f, 28f), new Vector2(220f, 50f), new Color(0.67f, 0.18f, 0.12f, 0.96f), new Vector2(0.5f, 0f));

            controller.Bind(objective, checklist, prompt, message, score, finalText, guideText, group, guideGroup, resetButton, quitButton);
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
            guideGroup.alpha = 1f;
            guideGroup.blocksRaycasts = false;
            guideGroup.interactable = false;
            return controller;
        }

        private static Text CreateText(string name, Transform parent, Font font, string text, int size, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, Vector2 pivot, bool bestFit = false, int minBestFitSize = 10, int maxBestFitSize = 20)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            var textComponent = textObject.AddComponent<Text>();
            textComponent.font = font;
            textComponent.text = text;
            textComponent.fontSize = size;
            textComponent.alignment = alignment;
            textComponent.color = Color.white;
            textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
            textComponent.verticalOverflow = VerticalWrapMode.Overflow;
            textComponent.resizeTextForBestFit = bestFit;
            textComponent.resizeTextMinSize = minBestFitSize;
            textComponent.resizeTextMaxSize = maxBestFitSize;

            var rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            return textComponent;
        }

        private static GameObject CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, Color color, Vector2 pivot)
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

        private static Button CreateButton(string name, Transform parent, Font font, string label, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, Color color, Vector2 pivot)
        {
            var buttonObject = CreatePanel(name, parent, anchorMin, anchorMax, anchoredPosition, sizeDelta, color, pivot);
            var button = buttonObject.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            CreateText("Label", buttonObject.transform, font, label, 18, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(-12f, -10f), new Vector2(0.5f, 0.5f), true, 14, 18);
            return button;
        }

        private static void CreateEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
            eventSystemObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            eventSystemObject.AddComponent<StandaloneInputModule>();
#endif
        }

        private static void CreateFence(Vector3 position, Vector3 scale, Material material, Transform parent, string name)
        {
            CreatePrimitive(PrimitiveType.Cube, name, position, scale, material, parent);
        }

        private static void CreateWorldLabel(string name, string text, Vector3 position, Quaternion rotation, float characterSize, Color color, Transform parent)
        {
            var labelObject = new GameObject(name);
            labelObject.transform.SetParent(parent);
            labelObject.transform.SetPositionAndRotation(position, rotation);
            var textMesh = labelObject.AddComponent<TextMesh>();
            textMesh.text = text;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.characterSize = characterSize;
            textMesh.fontSize = 32;
            textMesh.color = color;
        }

        private static void CreateReferencePlacard(
            string name,
            string materialName,
            string texturePath,
            Vector3 position,
            Quaternion rotation,
            Vector3 backingScale,
            Material support,
            Material backing,
            Transform parent)
        {
            CreatePrimitive(PrimitiveType.Cube, $"{name} Stand", position + new Vector3(0f, -0.6f, 0.02f), new Vector3(0.08f, 1.2f, 0.08f), support, parent);
            CreatePrimitive(PrimitiveType.Cube, $"{name} Backing", position, backingScale, backing, parent);
            CreateDisplayPanel($"{name} Display", materialName, texturePath, position + new Vector3(0f, 0f, -0.03f), rotation, new Vector3(backingScale.x * 0.92f, backingScale.y * 0.92f, 1f), parent);
        }

        private static void CreateDisplayPanel(string name, string materialName, string texturePath, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent)
        {
            var material = CreateDisplayMaterial(materialName, texturePath);
            if (material == null)
            {
                return;
            }

            // Unity's built-in Quad front face points along -Z. Our placement rotations
            // were authored as if the front face pointed the opposite way, which made the
            // mirrored "backface" quad the only readable surface from gameplay angles.
            // Flip the authored rotation once and keep a single visible surface.
            CreateDisplayQuad(name, position, rotation * Quaternion.Euler(0f, 180f, 0f), scale, material, parent);
        }

        private static void CreateDisplayQuad(string name, Vector3 position, Quaternion rotation, Vector3 scale, Material material, Transform parent)
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

        private static GameObject CreatePrimitive(PrimitiveType type, string name, Vector3 position, Vector3 scale, Material material, Transform parent)
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

        private static Material CreateMaterial(string name, Color color)
        {
            var path = $"{MaterialFolder}/{name}.mat";
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

        private static Material CreateDisplayMaterial(string name, string texturePath)
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

            var path = $"{MaterialFolder}/{name}.mat";
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
