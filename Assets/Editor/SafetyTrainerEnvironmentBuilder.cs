using UnityEngine;

namespace OilSafetyTrainer.Editor
{
    internal static class SafetyTrainerEnvironmentBuilder
    {
        public static void ConfigureRenderSettings()
        {
            RenderSettings.ambientLight = new Color(0.62f, 0.66f, 0.7f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.63f, 0.67f, 0.71f);
            RenderSettings.fogDensity = 0.0125f;
        }

        public static void CreateLighting()
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

        public static void CreateYard(SafetyTrainerMaterialSet materials, Transform parent)
        {
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Training Yard Floor", new Vector3(8f, -0.05f, 5f), new Vector3(34f, 0.1f, 42f), materials.Concrete, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Dark Access Road", new Vector3(8f, 0.01f, 0f), new Vector3(7f, 0.04f, 24f), materials.Asphalt, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Checkpoint Pad", new Vector3(0f, 0.02f, -8f), new Vector3(8f, 0.06f, 6f), materials.SafetyYellow, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Final Assessment Pad", new Vector3(17f, 0.03f, 10f), new Vector3(5f, 0.06f, 4f), materials.SafeGreen, parent);

            SafetyTrainerPrimitiveFactory.CreateFence(new Vector3(-5f, 0.8f, -2f), new Vector3(0.15f, 1.6f, 22f), materials.Steel, parent, "Left Fence");
            SafetyTrainerPrimitiveFactory.CreateFence(new Vector3(21f, 0.8f, -2f), new Vector3(0.15f, 1.6f, 22f), materials.Steel, parent, "Right Fence");
            SafetyTrainerPrimitiveFactory.CreateFence(new Vector3(8f, 0.8f, 13f), new Vector3(26f, 1.6f, 0.15f), materials.Steel, parent, "Back Fence");
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Checkpoint Arch Left", new Vector3(4.4f, 1.7f, -4f), new Vector3(0.35f, 3.4f, 0.35f), materials.SafetyYellow, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Checkpoint Arch Right", new Vector3(11.6f, 1.7f, -4f), new Vector3(0.35f, 3.4f, 0.35f), materials.SafetyYellow, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Checkpoint Arch Top", new Vector3(8f, 3.25f, -4f), new Vector3(7.6f, 0.35f, 0.35f), materials.SafetyYellow, parent);
        }

        public static void CreateCheckpointProps(SafetyTrainerMaterialSet materials, Transform parent)
        {
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "PPE Rack Base", new Vector3(0f, 0.34f, -8.4f), new Vector3(7.5f, 0.58f, 1.9f), materials.Steel, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "PPE Rack Rear Rail", new Vector3(0f, 0.84f, -9.15f), new Vector3(7.3f, 0.08f, 0.28f), materials.SafetyYellow, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Checkpoint Desk", new Vector3(5.8f, 0.7f, -6.4f), new Vector3(2.2f, 1.4f, 0.8f), materials.Steel, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Desk Monitor", new Vector3(5.8f, 1.64f, -6.75f), new Vector3(0.8f, 0.48f, 0.08f), materials.White, parent);
            SafetyTrainerPrimitiveFactory.CreateDisplayPanel("Checkpoint Desk Display", "CheckpointDeskDisplay", SafetyTrainerPaths.CheckpointPosterPath, new Vector3(5.8f, 1.64f, -6.8f), Quaternion.Euler(0f, 180f, 0f), new Vector3(0.72f, 0.42f, 1f), parent);
        }

        public static void CreateProcessEquipment(SafetyTrainerMaterialSet materials, Transform parent)
        {
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cylinder, "Separator Vessel", new Vector3(8f, 1.2f, 5f), new Vector3(1.8f, 2.4f, 1.8f), materials.Steel, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Pump Skid", new Vector3(4f, 0.45f, 5.5f), new Vector3(3.2f, 0.9f, 1.8f), materials.Steel, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cylinder, "Hot Pipe Horizontal", new Vector3(10f, 1.1f, 2.5f), new Vector3(0.45f, 4.6f, 0.45f), materials.PipeRed, parent).transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cylinder, "Vertical Pipe", new Vector3(14f, 1.8f, 5f), new Vector3(0.45f, 3.6f, 0.45f), materials.Steel, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Valve Manifold Base", new Vector3(13f, 0.55f, 8f), new Vector3(3.5f, 1.1f, 1.2f), materials.Steel, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cylinder, "Valve Wheel", new Vector3(13f, 1.35f, 7.25f), new Vector3(1f, 0.12f, 1f), materials.WarningOrange, parent).transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cylinder, "Gas Warning Beacon Visual", new Vector3(15.2f, 1.8f, 2.8f), new Vector3(0.38f, 0.22f, 0.38f), materials.BeaconWarning, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Pipe Support A", new Vector3(9.2f, 0.75f, 2.5f), new Vector3(0.25f, 1.5f, 0.8f), materials.Steel, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Pipe Support B", new Vector3(10.8f, 0.75f, 2.5f), new Vector3(0.25f, 1.5f, 0.8f), materials.Steel, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cylinder, "Secondary Pipe", new Vector3(6.4f, 0.95f, 6.5f), new Vector3(0.26f, 3f, 0.26f), materials.Steel, parent).transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Equipment Platform", new Vector3(8f, 0.3f, 5.6f), new Vector3(9.8f, 0.12f, 4.6f), materials.Steel, parent);
        }

        public static void CreateFacilityBackdrop(SafetyTrainerMaterialSet materials, Transform parent)
        {
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cylinder, "Storage Tank A", new Vector3(19f, 2.6f, 5f), new Vector3(2.2f, 5.2f, 2.2f), materials.Steel, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cylinder, "Storage Tank B", new Vector3(22.5f, 2.3f, 6.2f), new Vector3(1.8f, 4.6f, 1.8f), materials.Steel, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Tank Platform", new Vector3(20.8f, 0.16f, 5.6f), new Vector3(7.2f, 0.18f, 4.8f), materials.Concrete, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Pipe Rack Beam", new Vector3(15.5f, 3.2f, 1.2f), new Vector3(8.5f, 0.22f, 0.32f), materials.Steel, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Pipe Rack Leg A", new Vector3(12.2f, 1.6f, 1.2f), new Vector3(0.32f, 3.2f, 0.32f), materials.Steel, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Pipe Rack Leg B", new Vector3(18.8f, 1.6f, 1.2f), new Vector3(0.32f, 3.2f, 0.32f), materials.Steel, parent);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cylinder, "Pipe Rack Line A", new Vector3(15.5f, 3.55f, 0.95f), new Vector3(0.18f, 4.1f, 0.18f), materials.WarningOrange, parent).transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cylinder, "Pipe Rack Line B", new Vector3(15.5f, 3.4f, 1.45f), new Vector3(0.14f, 4.1f, 0.14f), materials.Steel, parent).transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Access Stairs Base", new Vector3(18f, 0.45f, 8.2f), new Vector3(1.2f, 0.9f, 2.8f), materials.Concrete, parent);
        }

        public static void CreateInstructionBoards(SafetyTrainerMaterialSet materials, Transform parent)
        {
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Checkpoint Poster Board", new Vector3(-4.6f, 1.65f, -8.8f), new Vector3(0.08f, 1.8f, 2.6f), materials.White, parent);
            SafetyTrainerPrimitiveFactory.CreateDisplayPanel("Checkpoint Poster Display", "CheckpointPosterDisplay", SafetyTrainerPaths.CheckpointPosterPath, new Vector3(-4.54f, 1.65f, -8.8f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.55f, 2.25f, 1f), parent, false);
            SafetyTrainerPrimitiveFactory.CreatePrimitive(PrimitiveType.Cube, "Work Zone Poster Board", new Vector3(20.8f, 1.75f, 1.2f), new Vector3(0.08f, 2f, 3.2f), materials.SafetyYellow, parent);
            SafetyTrainerPrimitiveFactory.CreateDisplayPanel("Work Zone Poster Display", "WorkZonePosterDisplay", SafetyTrainerPaths.WorkZonePosterPath, new Vector3(20.74f, 1.75f, 1.2f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.75f, 2.75f, 1f), parent);
        }
    }
}
