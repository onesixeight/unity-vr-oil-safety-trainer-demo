using OilSafetyTrainer;

namespace OilSafetyTrainer.Editor
{
    internal static class SafetyTrainerScenarioDefaults
    {
        public const int BaseScore = 100;
        public const int MissingPpePenalty = 12;
        public const int UninspectedHazardPenalty = 8;

        public static SafetyScenarioConfig.PpeItem[] CreateRequiredPpe()
        {
            return new[]
            {
                new SafetyScenarioConfig.PpeItem { id = "helmet", label = "Каска" },
                new SafetyScenarioConfig.PpeItem { id = "goggles", label = "Защитные очки" },
                new SafetyScenarioConfig.PpeItem { id = "gloves", label = "Перчатки" },
                new SafetyScenarioConfig.PpeItem { id = "boots", label = "Диэлектрические ботинки" },
            };
        }

        public static SafetyScenarioConfig.HazardItem[] CreateHazards()
        {
            return new[]
            {
                new SafetyScenarioConfig.HazardItem
                {
                    id = "guardrail_gap",
                    label = "Разрыв ограждения",
                    recommendation = "Остановитесь, выставьте временное ограждение и сообщите ответственному."
                },
                new SafetyScenarioConfig.HazardItem
                {
                    id = "oil_spill",
                    label = "Разлив нефти/масла",
                    recommendation = "Оградите место, используйте сорбент и сообщите о проливе."
                },
                new SafetyScenarioConfig.HazardItem
                {
                    id = "hot_pipe",
                    label = "Горячая поверхность трубопровода",
                    recommendation = "Не касайтесь трубы без допуска, проверьте термоизоляцию и предупреждающие знаки."
                },
                new SafetyScenarioConfig.HazardItem
                {
                    id = "gas_warning",
                    label = "Сигнал газоанализатора",
                    recommendation = "Покиньте опасную зону по ветру, включите оповещение и действуйте по плану эвакуации."
                },
                new SafetyScenarioConfig.HazardItem
                {
                    id = "unsafe_valve",
                    label = "Открытый/непромаркированный клапан",
                    recommendation = "Не переключайте арматуру без наряда, проверьте бирку LOTO и схему трубопровода."
                }
            };
        }
    }
}
