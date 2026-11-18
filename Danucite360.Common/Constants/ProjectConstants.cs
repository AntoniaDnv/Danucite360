namespace Danucite360.Common.Constants
{
    public static class ProjectStatuses
    {
        public const string InPlanning = "In Planning";
        public const string Ongoing = "Ongoing";
        public const string Completed = "Completed";
    }

    public static class ProjectTypes
    {
        public const string Infrastructure = "Infrastructure";
        public const string Education = "Education";
        public const string Healthcare = "Healthcare";
        public const string Energy = "Energy";
        public const string Digitalization = "Digitalization";
        public const string Environment = "Environment";
    }

    public static class FundingSources
    {
        public const string EuCohesionFund = "EU Cohesion Fund";
        public const string EuRecoveryPlan = "EU Recovery & Resilience Plan";
        public const string NationalBudget = "National Budget";
        public const string Municipality = "Municipal Co-financing";
    }
}

namespace Danucite360.Common.Constants
{
    /// <summary>Bulgarian display labels for the English keys stored in the DB.</summary>
    public static class Labels
    {
        private static readonly Dictionary<string, string> Bg = new()
        {
            [ProjectStatuses.InPlanning] = "В подготовка",
            [ProjectStatuses.Ongoing] = "В изпълнение",
            [ProjectStatuses.Completed] = "Завършен",
            [ProjectTypes.Infrastructure] = "Инфраструктура",
            [ProjectTypes.Education] = "Образование",
            [ProjectTypes.Healthcare] = "Здравеопазване",
            [ProjectTypes.Energy] = "Енергетика",
            [ProjectTypes.Digitalization] = "Дигитализация",
            [ProjectTypes.Environment] = "Околна среда",
            [FundingSources.EuCohesionFund] = "Кохезионен фонд на ЕС",
            [FundingSources.EuRecoveryPlan] = "План за възстановяване и устойчивост",
            [FundingSources.NationalBudget] = "Държавен бюджет",
            [FundingSources.Municipality] = "Общинско съфинансиране",
        };

        public static string ToBg(string? key) =>
            key != null && Bg.TryGetValue(key, out var label) ? label : key ?? string.Empty;
    }
}
