using OilSafetyTrainer;

namespace OilSafetyTrainer.Editor
{
    internal static class SafetyTrainerScenarioDefaults
    {
        public const int BaseScore = 100;
        public const int MissingPpePenalty = 12;
        public const int UninspectedHazardPenalty = 8;
        public const int MaintenanceMissingPpePenalty = 15;
        public const int MaintenanceUninspectedHazardPenalty = 10;

        public const string ScenarioName = "Обход нефтедобывающей площадки";
        public const string MaintenanceScenarioName = "Техническое обслуживание насосного блока";
        public const string MaintenanceObjective = "VR-тренажёр: безопасное обслуживание насосного блока";
        public const string MaintenanceStartMessage = "Цель: подготовьтесь к обслуживанию насосного блока и отметьте критичные риски.";
        public const string MaintenanceGuideText =
            "1. Наденьте 4 обязательных СИЗ перед работой у насосного блока.\n" +
            "2. Пройдите КПП и двигайтесь только по безопасной зоне.\n" +
            "3. Найдите 3 опасности обслуживания и отметьте их клавишей E.\n" +
            "4. Завершите обход у терминала итоговой оценки.\n\n" +
            "Управление: WASD - движение, мышь - обзор.\n" +
            "E - действие, H - памятка, R - сброс, Q - выход.\n";

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

        public static SafetyScenarioConfig.PpeItem[] CreateMaintenanceRequiredPpe()
        {
            return CreateRequiredPpe();
        }

        public static SafetyScenarioConfig.HazardItem[] CreateMaintenanceHazards()
        {
            return new[]
            {
                new SafetyScenarioConfig.HazardItem
                {
                    id = "oil_spill",
                    label = "Разлив нефти/масла",
                    recommendation = "Остановите работы, оградите место пролива, примените сорбент и сообщите ответственному."
                },
                new SafetyScenarioConfig.HazardItem
                {
                    id = "hot_pipe",
                    label = "Горячая поверхность трубопровода",
                    recommendation = "Не приступайте к обслуживанию до охлаждения участка и проверки предупреждающей маркировки."
                },
                new SafetyScenarioConfig.HazardItem
                {
                    id = "unsafe_valve",
                    label = "Открытый/непромаркированный клапан",
                    recommendation = "Проверьте бирку LOTO, схему трубопровода и разрешение перед переключением арматуры."
                }
            };
        }
    }
}
