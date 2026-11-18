namespace Danucite360.Common.Constants
{
    /// <summary>
    /// Citizen situations used to tag budget measures. Keys are stored in the DB
    /// (semicolon-separated); labels are the Bulgarian UI text.
    /// </summary>
    public static class MeasureAudiences
    {
        public const string Everyone = "everyone";
        public const string Employee = "employee";
        public const string SelfEmployed = "self-employed";
        public const string Parent = "parent";
        public const string Driver = "driver";
        public const string Pensioner = "pensioner";
        public const string Employer = "employer";

        public static readonly IReadOnlyList<(string Key, string Label)> Selectable = new[]
        {
            (Employee, "Служител"),
            (SelfEmployed, "Свободна професия / самоосигуряващ се"),
            (Parent, "Родител"),
            (Driver, "Шофьор"),
            (Pensioner, "Пенсионер"),
            (Employer, "Работодател / бизнес"),
        };

        public static string Label(string key) =>
            key == Everyone ? "Всички граждани" : Selectable.FirstOrDefault(a => a.Key == key).Label ?? key;
    }

    public static class MeasureAgeGroups
    {
        public const string All = "all";
        public const string Under30 = "under30";
        public const string From30To50 = "30-50";
        public const string From50To65 = "50-65";
        public const string Over65 = "over65";

        public static readonly IReadOnlyList<(string Key, string Label)> Selectable = new[]
        {
            (Under30, "До 30 г."),
            (From30To50, "30–50 г."),
            (From50To65, "50–65 г."),
            (Over65, "Над 65 г."),
        };

        public static string Label(string key) =>
            key == All ? "Всички възрасти" : Selectable.FirstOrDefault(a => a.Key == key).Label ?? key;
    }

    public static class MeasureStatuses
    {
        public const string Proposed = "Предложена";
        public const string Adopted = "Приета";
        public const string InForce = "В сила";
    }
}
