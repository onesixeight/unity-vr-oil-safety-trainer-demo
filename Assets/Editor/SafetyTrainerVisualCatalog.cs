using System;
using UnityEngine;

namespace OilSafetyTrainer.Editor
{
    internal enum HazardVisualKind
    {
        Placard,
        OilSpillSurface
    }

    internal enum SafetyTrainerMaterialRole
    {
        SafetyYellow,
        White,
        WarningOrange,
        PipeRed
    }

    internal readonly struct PpePlacardDefinition
    {
        public PpePlacardDefinition(string name, string texturePath, Vector3 position, SafetyTrainerMaterialRole materialRole)
        {
            Name = name;
            TexturePath = texturePath;
            Position = position;
            MaterialRole = materialRole;
        }

        public string Name { get; }
        public string TexturePath { get; }
        public Vector3 Position { get; }
        public SafetyTrainerMaterialRole MaterialRole { get; }
    }

    internal readonly struct HazardVisualDefinition
    {
        public HazardVisualDefinition(
            string name,
            string texturePath,
            Vector3 position,
            Quaternion rotation,
            SafetyTrainerMaterialRole materialRole,
            HazardVisualKind kind)
        {
            Name = name;
            TexturePath = texturePath;
            Position = position;
            Rotation = rotation;
            MaterialRole = materialRole;
            Kind = kind;
        }

        public string Name { get; }
        public string TexturePath { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public SafetyTrainerMaterialRole MaterialRole { get; }
        public HazardVisualKind Kind { get; }
    }

    internal static class SafetyTrainerVisualCatalog
    {
        public static PpePlacardDefinition GetPpe(string id)
        {
            return id switch
            {
                "helmet" => new PpePlacardDefinition("PPE Helmet", SafetyTrainerPaths.PpeHelmetPath, new Vector3(-2.8f, 1.02f, -7.35f), SafetyTrainerMaterialRole.SafetyYellow),
                "goggles" => new PpePlacardDefinition("PPE Goggles", SafetyTrainerPaths.PpeGogglesPath, new Vector3(-1f, 1.02f, -7.35f), SafetyTrainerMaterialRole.White),
                "gloves" => new PpePlacardDefinition("PPE Gloves", SafetyTrainerPaths.PpeGlovesPath, new Vector3(0.8f, 1.02f, -7.35f), SafetyTrainerMaterialRole.SafetyYellow),
                "boots" => new PpePlacardDefinition("PPE Boots", SafetyTrainerPaths.PpeBootsPath, new Vector3(2.6f, 1.02f, -7.35f), SafetyTrainerMaterialRole.White),
                _ => throw new ArgumentException($"Unknown PPE id in scenario config: {id}", nameof(id))
            };
        }

        public static HazardVisualDefinition GetHazard(string id)
        {
            return id switch
            {
                "guardrail_gap" => new HazardVisualDefinition("Hazard Guardrail Gap", SafetyTrainerPaths.HazardGuardrailPath, new Vector3(6.2f, 1.1f, 11.85f), Quaternion.identity, SafetyTrainerMaterialRole.WarningOrange, HazardVisualKind.Placard),
                "oil_spill" => new HazardVisualDefinition("Oil Spill Visual", SafetyTrainerPaths.HazardOilSpillPath, new Vector3(3.5f, 0.07f, 2.1f), Quaternion.Euler(90f, 25f, 0f), SafetyTrainerMaterialRole.WarningOrange, HazardVisualKind.OilSpillSurface),
                "hot_pipe" => new HazardVisualDefinition("Hazard Hot Pipe Marker", SafetyTrainerPaths.HazardHotPipePath, new Vector3(10f, 1.55f, 1.55f), Quaternion.identity, SafetyTrainerMaterialRole.PipeRed, HazardVisualKind.Placard),
                "gas_warning" => new HazardVisualDefinition("Hazard Gas Warning Beacon", SafetyTrainerPaths.HazardGasWarningPath, new Vector3(15.2f, 1.45f, 3.75f), Quaternion.Euler(0f, 180f, 0f), SafetyTrainerMaterialRole.WarningOrange, HazardVisualKind.Placard),
                "unsafe_valve" => new HazardVisualDefinition("Hazard Unsafe Valve Marker", SafetyTrainerPaths.HazardUnsafeValvePath, new Vector3(13.9f, 1.58f, 8.35f), Quaternion.Euler(0f, -90f, 0f), SafetyTrainerMaterialRole.WarningOrange, HazardVisualKind.Placard),
                _ => throw new ArgumentException($"Unknown hazard id in scenario config: {id}", nameof(id))
            };
        }
    }
}
