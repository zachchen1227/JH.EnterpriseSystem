using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Models.ExtraParams
{
    /// <summary>
    /// 一條 Series 的定義。
    /// Builder 依此決定：畫幾條、每條怎麼畫。
    /// </summary>
    public class SeriesDefinition
    {
        /// <summary>顯示名稱，例如 "實際產量"</summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// 對應 WeeklyProductionData.Series 或
        /// DailyTeamProductionData.Series 的 Key。
        /// 找不到 → Builder 自動跳過此條線。
        /// </summary>
        public string DataField { get; init; } = string.Empty;

        /// <summary>Highcharts type：column / line / scatter / spline</summary>
        public string VisualType { get; init; } = "line";

        /// <summary>0 = 左軸（產量），1 = 右軸（達成率）</summary>
        public int YAxis { get; init; } = 0;

        /// <summary>
        /// 對應 FactoryChartTheme 屬性名稱：
        /// ActualColor / TargetColor / RateLineColor /
        /// AccumulatedRateColor / RateWarningColor / Series2Color
        /// </summary>
        public string ColorSource { get; init; } = "ActualColor";

        /// <summary>true = 低於 RateWarningThreshold 的點改警示色</summary>
        public bool ApplyWarningColor { get; init; } = false;

        /// <summary>null = 實線，"ShortDash" = 虛線</summary>
        public string? DashStyle { get; init; } = null;
    }
}
