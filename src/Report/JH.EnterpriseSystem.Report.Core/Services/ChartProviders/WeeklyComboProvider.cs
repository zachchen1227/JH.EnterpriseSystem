using JH.EnterpriseSystem.Report.Core.Interfaces;
using JH.EnterpriseSystem.Report.Core.Models.ExtraParams;

namespace JH.EnterpriseSystem.Report.Core.Services.ChartProviders
{
    public class WeeklyComboProvider : IChartProvider
    {
        private readonly IRepository _repo;
        private readonly IReportDataMapper _mapper;
        public string ChartType => "weekly-combo";

        public WeeklyComboProvider(IRepository repo, IReportDataMapper mapper)
        { _repo = repo; _mapper = mapper; }

        public async Task<object> GenerateDataAsync(
            string factory, string reportCode, string title,
            string queryDay, object? extraParams = null)
        {
            var extra = extraParams as WeeklyExtraParams ?? new WeeklyExtraParams();

            var eDay = DateTime.TryParse(queryDay, out var d) ? d : DateTime.Today.AddDays(-1);
            var sDay = eDay.AddDays(-(extra.Days - 1));

            var raw = await _repo.GetWeeklyRawDataAsync(factory, reportCode,
                          sDay.ToString("yyyy/MM/dd"), eDay.ToString("yyyy/MM/dd"));
            var data = _mapper.MapWeeklyProduction(raw, extra);

            bool hasAccRate = data.Series.ContainsKey("AccumulatedRate");

            // 把 List<object> 轉成純數值陣列供前端使用
            static List<double> ToDoubles(object v) =>
                v is List<double> ld ? ld : ((List<int>)v).Select(x => (double)x).ToList();

            static List<int> ToInts(object v) =>
                v is List<int> li ? li : ((List<double>)v).Select(x => (int)x).ToList();

            return new
            {
                meta = new
                {
                    reportCode,
                    title = $"{title} - {factory}",
                    chartType = "weekly-combo",
                    isDaily = false,
                    hasAccumulatedRate = hasAccRate,
                    rateWarningThreshold = 95.0,
                },
                data = new
                {
                    categories = data.Dates,
                    actualQty = data.Series.TryGetValue("ActualQty", out var aq) ? ToInts(aq) : new List<int>(),
                    targetQty = data.Series.TryGetValue("TargetQty", out var tq) ? ToInts(tq) : new List<int>(),
                    dailyRate = data.Series.TryGetValue("DailyRate", out var dr) ? ToDoubles(dr) : new List<double>(),
                    accumulatedRate = data.Series.TryGetValue("AccumulatedRate", out var ar) ? ToDoubles(ar) : new List<double>(),
                }
            };
        }
    }
}