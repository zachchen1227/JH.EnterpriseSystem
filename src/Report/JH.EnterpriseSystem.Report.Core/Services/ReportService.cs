using JH.EnterpriseSystem.Report.Core.Enums;
using JH.EnterpriseSystem.Report.Core.Interfaces;
using JH.EnterpriseSystem.Report.Core.Registry;
using Microsoft.Extensions.Logging;

namespace JH.EnterpriseSystem.Report.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IReadOnlyDictionary<string, IChartProvider> _providers;
        private readonly ILogger<ReportService> _logger;

        public ReportService(
            IEnumerable<IChartProvider> providers,
            ILogger<ReportService> logger)
        {
            _providers = providers.ToDictionary(
                p => p.ChartType, StringComparer.OrdinalIgnoreCase);
            _logger = logger;
        }

        public async Task<object> GetChartDataAsync(
            string factory, string reportCode, string queryDay)
        {
            if (!Enum.TryParse<ReportCode>(reportCode, ignoreCase: true, out var code))
                throw new ArgumentException($"未知報表代碼：{reportCode}");

            var charts = ReportRegistry.GetCharts(code);
            if (charts.Count == 0)
                throw new InvalidOperationException($"報表 {reportCode} 尚未設定");

            var def = charts[0];

            if (!_providers.TryGetValue(def.ChartType, out var provider))
                throw new InvalidOperationException(
                    $"找不到 ChartType={def.ChartType} 的 Provider");

            _logger.LogInformation(
                "Chart factory={Factory} code={Code} type={Type} provider={Provider}",
                factory, reportCode, def.ChartType, provider.GetType().Name);

            return await provider.GenerateDataAsync(
                 factory, reportCode, def.Title, queryDay, def.ExtraParams);
        }
    }
}