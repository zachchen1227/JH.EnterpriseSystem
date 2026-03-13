using JH.EnterpriseSystem.Report.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Models
{
    /// <summary>
    /// 廠區圖表配色 + 業務規則。
    ///
    /// 設計為 record，支援 with 運算子覆寫：
    ///   var theme = ChartThemeRegistry.Default("BU1")
    ///                   with { ActualColor = "#FF0000" };
    ///
    /// 現階段所有廠區使用同一預設值（來自舊系統 jhOptions）。
    /// 未來若特定廠區需要差異化，只需在 ChartThemeRegistry 用 with 覆寫。
    /// </summary>
    public record FactoryChartTheme
    {
        public string FactoryName { get; init; } = string.Empty;

        // ── 各系列顏色 ────────────────────────────────────────
        /// <summary>實際產量長條色。對應舊系統 ColorA = '#3A7BD5'</summary>
        public string ActualColor { get; init; } = "#3A7BD5";

        /// <summary>目標產量散點色（菱形）。對應舊系統 ColorB = '#FF7F0E'</summary>
        public string TargetColor { get; init; } = "#FF7F0E";

        /// <summary>達成率折線 / 散點色。對應舊系統 ColorC = '#2CA02C'</summary>
        public string RateLineColor { get; init; } = "#2CA02C";

        /// <summary>累計達成率虛線色（週報專用）。對應舊系統 ColorD = '#17BECF'</summary>
        public string AccumulatedRateColor { get; init; } = "#17BECF";

        /// <summary>達成率警示色（低於閾值時的點色）。對應舊系統 ColorE = '#D62728'</summary>
        public string RateWarningColor { get; init; } = "#D62728";

        /// <summary>第二系列備用色（雙廠比較圖）</summary>
        public string Series2Color { get; init; } = "#AA4643";

        // ── 業務規則 ──────────────────────────────────────────
        /// <summary>
        /// 達成率警示閾值（%）。低於此值的點改用 RateWarningColor。
        /// 對應舊系統 fontColorUnder95，預設 95。
        /// </summary>
        public double RateWarningThreshold { get; init; } = 95.0;

        /// <summary>
        /// 左 Y 軸放大倍率，避免長條頂到圖頂。
        /// 對應舊系統 jhOptions.yLeft.mag = 1.5。
        /// </summary>
        public double YLeftMagnification { get; init; } = 1.5;

        /// <summary>右 Y 軸上限（%）。對應舊系統 yRight.ceiling = 120。</summary>
        public double YRightCeiling { get; init; } = 120.0;

        /// <summary>右 Y 軸下限（%）。對應舊系統 yRight.floor = 0。</summary>
        public double YRightFloor { get; init; } = 0.0;

        /// <summary>
        /// 右 Y 軸刻度間隔（%）。對應舊系統 yRight.gap = 15。
        /// tickAmount 由前端算：Math.round((ceiling-floor)/gap)+1。
        /// </summary>
        public double YRightGap { get; init; } = 15.0;

        /// <summary>
        /// 特定報表的左 Y 軸強制上限（覆寫倍率計算）。
        /// 對應舊系統 BU1YLeftMax_RPT25013 / BU2YLeftMax_RPT25013。
        /// Key = reportCode，Value = 最大值。
        /// 預設空字典，表示全部報表用倍率計算。
        /// </summary>
        public IReadOnlyDictionary<string, double> ReportYLeftMaxOverride { get; init; }
            = new Dictionary<string, double>();

        // ── 輔助方法 ──────────────────────────────────────────
        /// <summary>
        /// 給 Highcharts chart.colors 全域陣列使用的有序清單。
        /// 順序固定：[實際, 備用, 目標, 達成率]
        /// </summary>
        public List<string> ToColorArray() =>
        [
            ActualColor,
            Series2Color,
            TargetColor,
            RateLineColor
        ];
    }
}