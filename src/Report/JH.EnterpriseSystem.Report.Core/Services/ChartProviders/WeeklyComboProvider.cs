using JH.EnterpriseSystem.Report.Core.Enums;
using JH.EnterpriseSystem.Report.Core.Interfaces;
using JH.EnterpriseSystem.Report.Core.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Services.ChartProviders
{
    /// <summary>
    /// 週報 Combo 圖 Provider。
    /// 職責：撈週資料 → Mapper → ComboChartBuilder.BuildWeekly()
    /// </summary>
    public class WeeklyComboProvider : IChartProvider
    {
        private readonly IRepository _repo;
        private readonly IReportDataMapper _mapper;
        private readonly IChartBuilder _builder;

        public string ChartType => "weekly-combo";

        public WeeklyComboProvider(
            IRepository repo,
            IReportDataMapper mapper,
            IChartBuilder builder)
        {
            _repo = repo;
            _mapper = mapper;
            _builder = builder;
        }

        public async Task<object> GenerateDataAsync(
            string factory, string reportCode, string queryDay, object? extraParams = null)
        {
            dynamic? extra = extraParams;
            int days = (int?)((dynamic?)extra?.ExtraParams)?.Days ?? 7;
            string title = (extra?.Title as string) ?? reportCode;

            var fCode = Enum.Parse<FactoryCode>(factory, ignoreCase: true);
            var theme = ChartThemeRegistry.Get(fCode);

            // 計算週報區間
            var eDay = DateTime.TryParse(queryDay, out var d) ? d : DateTime.Today.AddDays(-1);
            var sDay = eDay.AddDays(-(days - 1));

            // Repository → 原始資料
            var rawDt = await _repo.GetWeeklyRawDataAsync(
                factory, reportCode,
                sDay.ToString("yyyy/MM/dd"),
                eDay.ToString("yyyy/MM/dd"));

            // Mapper → Domain Model
            var data = _mapper.MapWeeklyProduction(rawDt, hasAccumulatedRate: true);

            // Builder → Highcharts JSON
            return _builder.BuildWeekly(data, theme, title, reportCode);
        }
    }
}
