using JH.EnterpriseSystem.Report.Core.Interfaces;
using JH.EnterpriseSystem.Report.Core.Models.ExtraParams;

namespace JH.EnterpriseSystem.Report.Core.Services.ChartProviders
{
    public class DailyComboProvider : IChartProvider
    {
        private readonly IRepository _repo;
        private readonly IReportDataMapper _mapper;
        public string ChartType => "daily-combo";

        public DailyComboProvider(IRepository repo, IReportDataMapper mapper)
        { _repo = repo; _mapper = mapper; }

        public async Task<object> GenerateDataAsync(
            string factory, string reportCode, string title,
            string queryDay, object? extraParams = null)
        {
            var extra = extraParams as DailyExtraParams ?? new DailyExtraParams();

            var raw = await _repo.GetDailyRawDataAsync(factory, reportCode, queryDay);
            var data = _mapper.MapDailyTeamProduction(raw, extra);

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
                    chartType = "daily-combo",
                    isDaily = true,
                    hasAccumulatedRate = false,
                    rateWarningThreshold = 95.0,
                },
                data = new
                {
                    categories = data.Teams,
                    actualQty = data.Series.TryGetValue("ActualQty", out var aq) ? ToInts(aq) : new List<int>(),
                    targetQty = data.Series.TryGetValue("TargetQty", out var tq) ? ToInts(tq) : new List<int>(),
                    dailyRate = data.Series.TryGetValue("Rates", out var dr) ? ToDoubles(dr) : new List<double>(),
                }
            };
        }
    }
}