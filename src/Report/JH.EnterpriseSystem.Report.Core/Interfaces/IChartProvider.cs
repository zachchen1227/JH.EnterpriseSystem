using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Interfaces
{
    /// <summary>
    /// 圖表 Provider 介面。
    /// 每個實作 = 一種時間粒度 × 一種圖表類型的組合。
    ///
    /// 命名規則：{時間粒度}{圖表類型}Provider
    ///   例：WeeklyComboProvider、DailyBarProvider、DailyHeatmapProvider
    ///
    /// ChartType 格式："{granularity}-{visual}"
    ///   例："weekly-combo"、"daily-combo"、"daily-bar"
    /// </summary>
    public interface IChartProvider
    {
        /// <summary>
        /// 圖表類型識別碼，對應 ReportRegistry 的 ChartDefinition.ChartType。
        /// 格式："{granularity}-{visual}"，例如 "weekly-combo"。
        /// </summary>
        string ChartType { get; }

        Task<object> GenerateDataAsync(
            string factory,
            string reportCode,
            string queryDay,
            object? extraParams = null);
    }
}
