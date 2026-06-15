using System.IO;

namespace OilSafetyTrainer.Editor
{
    internal static class SafetyTrainerPaths
    {
        public const string ScenePath = "Assets/Scenes/OilSafetyTrainerDemo.unity";
        public const string ScenarioFolder = "Assets/Scenarios";
        public const string ScenarioConfigPath = ScenarioFolder + "/OilSafetyTrainerScenario.asset";
        public const string MaterialFolder = "Assets/Materials";
        public const string TextureFolder = "Assets/Textures/PolyHaven";
        public const string ArtFolder = "Assets/Art";
        public const string HazardArtFolder = ArtFolder + "/Hazards";
        public const string PosterArtFolder = ArtFolder + "/UI/Posters";
        public const string PpeArtFolder = ArtFolder + "/UI/PPE";
        public const string TerminalArtFolder = ArtFolder + "/UI/Terminal";

        public const string ConcreteDiffusePath = TextureFolder + "/concrete_floor_02_diff_1k.jpg";
        public const string ConcreteNormalPath = TextureFolder + "/concrete_floor_02_nor_gl_1k.jpg";
        public const string AsphaltDiffusePath = TextureFolder + "/asphalt_floor_diff_1k.jpg";
        public const string AsphaltNormalPath = TextureFolder + "/asphalt_floor_nor_gl_1k.jpg";
        public const string SteelDiffusePath = TextureFolder + "/blue_metal_plate_diff_1k.jpg";
        public const string SteelNormalPath = TextureFolder + "/blue_metal_plate_nor_gl_1k.jpg";

        public const string CheckpointPosterPath = PosterArtFolder + "/checkpoint_poster_bg.png";
        public const string WorkZonePosterPath = PosterArtFolder + "/workzone_poster_bg.png";
        public const string FinalTerminalPath = TerminalArtFolder + "/final_terminal_bg.png";

        public const string PpeHelmetPath = PpeArtFolder + "/ppe_helmet.png";
        public const string PpeGogglesPath = PpeArtFolder + "/ppe_goggles.png";
        public const string PpeGlovesPath = PpeArtFolder + "/ppe_gloves.png";
        public const string PpeBootsPath = PpeArtFolder + "/ppe_boots.png";

        public const string HazardGuardrailPath = HazardArtFolder + "/hazard_guardrail_gap.png";
        public const string HazardOilSpillPath = HazardArtFolder + "/hazard_oil_spill.png";
        public const string HazardHotPipePath = HazardArtFolder + "/hazard_hot_pipe.png";
        public const string HazardGasWarningPath = HazardArtFolder + "/hazard_gas_warning.png";
        public const string HazardUnsafeValvePath = HazardArtFolder + "/hazard_unsafe_valve.png";

        public static void EnsureFolders()
        {
            Directory.CreateDirectory("Assets/Scenes");
            Directory.CreateDirectory(ScenarioFolder);
            Directory.CreateDirectory(MaterialFolder);
            Directory.CreateDirectory(TextureFolder);
        }
    }
}
