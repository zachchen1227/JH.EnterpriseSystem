using JH.EnterpriseSystem.Report.Core.Enums;
using JH.EnterpriseSystem.Report.Core.Interfaces;
using JH.EnterpriseSystem.Report.Core.Models.ExtraParams;
using JH.EnterpriseSystem.Report.Core.Registry;

namespace JH.EnterpriseSystem.Report.Core.Services.ChartProviders
{
    public class DailyComboProvider : IChartProvider
    {
        private readonly IRepository _repo;
        private readonly IReportDataMapper _mapper;
        private readonly IChartBuilder _builder;
        public string ChartType => "daily-combo";

        public DailyComboProvider(
            IRepository repo, IReportDataMapper mapper, IChartBuilder builder)
        { _repo = repo; _mapper = mapper; _builder = builder; }

        public async Task<object> GenerateDataAsync(
                  string factory, string reportCode, string title,
                  string queryDay, object? extraParams = null)
        {
            var extra = extraParams as DailyExtraParams ?? new DailyExtraParams();
            var theme = ChartThemeRegistry.Get(Enum.Parse<FactoryCode>(factory, true));

            var raw = await _repo.GetDailyRawDataAsync(factory, reportCode, queryDay);
            var data = _mapper.MapDailyTeamProduction(raw, extra);

            return _builder.BuildDaily(data, theme, title, reportCode, extra.SeriesConfig);
        }
    }
}