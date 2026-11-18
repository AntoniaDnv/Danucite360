namespace Danucite360.Common.Constants
{
    /// <summary>Steps in a project's evidence trail, from the budget line to the finished work.</summary>
    public static class EvidenceStages
    {
        public const string Allocation = "allocation";
        public const string Procurement = "procurement";
        public const string Contract = "contract";
        public const string Payment = "payment";
        public const string Progress = "progress";

        public static string Label(string stage) => stage switch
        {
            Allocation => "Средства в бюджета",
            Procurement => "Обществена поръчка",
            Contract => "Договор",
            Payment => "Плащане",
            Progress => "Отчет за напредък",
            _ => stage
        };
    }
}
