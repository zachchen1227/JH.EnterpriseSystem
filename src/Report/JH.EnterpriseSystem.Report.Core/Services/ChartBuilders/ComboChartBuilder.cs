using JH.EnterpriseSystem.Report.Core.Interfaces;
using JH.EnterpriseSystem.Report.Core.Models;
using JH.EnterpriseSystem.Report.Core.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Services.ChartBuilders
{
    /// <summary>
    /// Combo 圖建構器。
    ///
    /// 使用 FactoryChartTheme 的具名屬性，不依賴 index：
    ///   theme.ActualColor          → 實際產量長條色
    ///   theme.TargetColor          → 目標產量散點色
    ///   theme.RateLineColor        → 達成率折線色
    ///   theme.AccumulatedRateColor → 累計達成率虛線色
    ///   theme.RateWarningColor     → 達成率低於閾值時的警示色
    ///   theme.RateWarningThreshold → 警示閾值（%）
    ///   theme.YLeft/YRight 業務規則 → 直接傳給前端，前端不需要 if/else
    /// </summary>
    public class ComboChartBuilder : IChartBuilder
    {
        // ════════════════════════════════════════════════════════
        //  週報：X 軸 = 日期
        //    實際產量長條 + 目標菱形散點 + 達成率折線 + 累計達成率虛線
        // ════════════════════════════════════════════════════════
        public object BuildWeekly(
            WeeklyProductionData data,
            FactoryChartTheme theme,
            string title,
            string reportCode)
        {
            var series = new List<object>
            {
                BuildActualColumn(theme.ActualColor, data.ActualQty),
                BuildTargetScatter(theme.TargetColor, data.TargetQty),
                BuildRateSeries(
                    theme.RateLineColor,
                    theme.RateWarningColor,
                    theme.RateWarningThreshold,
                    data.DailyRate,
                    isLine: true),         // 週報用折線
            };

            // 週報專用：累計達成率虛線
            if (data.HasAccumulatedRate)
                series.Add(BuildAccumulatedLine(
                    theme.AccumulatedRateColor,
                    data.AccumulatedRate));

            return BuildPayload(
                categories: data.Dates,
                series, theme, title, reportCode, isDaily: false);
        }

        // ════════════════════════════════════════════════════════
        //  日報：X 軸 = 組別
        //    實際產量長條 + 目標產量長條 + 達成率散點
        // ════════════════════════════════════════════════════════
        public object BuildDaily(
            DailyTeamProductionData data,
            FactoryChartTheme theme,
            string title,
            string reportCode)
        {
            var series = new List<object>
            {
                BuildActualColumn(theme.ActualColor, data.ActualQty),
                BuildTargetColumn(theme.TargetColor, data.TargetQty),  // 日報目標改用長條
                BuildRateSeries(
                    theme.RateLineColor,
                    theme.RateWarningColor,
                    theme.RateWarningThreshold,
                    data.Rates,
                    isLine: false),        // 日報用散點
            };

            return BuildPayload(
                categories: data.Teams,
                series, theme, title, reportCode, isDaily: true);
        }

        // ════════════════════════════════════════════════════════
        //  私有：各系列建構
        // ════════════════════════════════════════════════════════

        /// <summary>實際產量長條（週報 + 日報共用）</summary>
        private static object BuildActualColumn(string color, List<int> data) => new
        {
            name = "實際產量",
            type = "column",
            color,          // theme.ActualColor
            yAxis = 0,
            data = data.Cast<object>().ToList()
        };

        /// <summary>目標產量菱形散點（週報專用）</summary>
        private static object BuildTargetScatter(string color, List<int> data) => new
        {
            name = "目標產量",
            type = "scatter",
            color,          // theme.TargetColor
            yAxis = 0,
            data = data.Cast<object>().ToList()
        };

        /// <summary>目標產量長條（日報專用）</summary>
        private static object BuildTargetColumn(string color, List<int> data) => new
        {
            name = "目標產量",
            type = "column",
            color,          // theme.TargetColor
            yAxis = 0,
            data = data.Cast<object>().ToList()
        };

        /// <summary>
        /// 達成率系列（週報=折線，日報=散點）。
        /// 每個點帶 isWarning 旗標，前端依此旗標決定顏色，
        /// 後端同時帶入 color 讓 Highcharts 可直接渲染（雙保險）。
        /// </summary>
        private static object BuildRateSeries(
            string rateColor,
            string warningColor,
            double threshold,
            List<double> rates,
            bool isLine) => new
            {
                name = "達成率",
                type = isLine ? "line" : "scatter",
                color = rateColor,  // theme.RateLineColor
                yAxis = 1,
                data = rates.Select(r => (object)new
                {
                    y = r,
                    isWarning = r < threshold,          // 前端可依此旗標著色
                    color = r < threshold            // Highcharts 直接用此色
                                    ? warningColor       // theme.RateWarningColor
                                    : rateColor          // theme.RateLineColor
                }).ToList()
            };

        /// <summary>累計達成率虛線（週報專用）</summary>
        private static object BuildAccumulatedLine(string color, List<double> data) => new
        {
            name = "累計達成率",
            type = "line",
            color,          // theme.AccumulatedRateColor
            yAxis = 1,
            dashStyle = "ShortDash",
            data = data.Cast<object>().ToList()
        };

        // ════════════════════════════════════════════════════════
        //  私有：統一包裝 { config, data }
        //  業務規則全部從 theme 取，前端不需要任何 if/else
        // ════════════════════════════════════════════════════════
        private static object BuildPayload(
            List<string> categories,
            List<object> series,
            FactoryChartTheme theme,
            string title,
            string reportCode,
            bool isDaily)
        {
            // 特定報表的左 Y 軸強制上限（如 RPT25013 成型配套）
            double? yLeftMax = theme.ReportYLeftMaxOverride
                .TryGetValue(reportCode, out var v) ? v : null;

            return new
            {
                config = new
                {
                    type = isDaily ? "daily-combo" : "weekly-combo",
                    title = $"{title} - {theme.FactoryName}",
                    isDaily,

                    // ── Highcharts chart.colors 全域陣列 ─────────────
                    // 順序由 ToColorArray() 保證：[實際, 備用, 目標, 達成率]
                    colors = theme.ToColorArray(),

                    // ── 各系列具名顏色（前端直接用名稱，不靠 index）──
                    seriesColors = new
                    {
                        actual = theme.ActualColor,
                        target = theme.TargetColor,
                        rate = theme.RateLineColor,
                        accumulated = theme.AccumulatedRateColor,
                        warning = theme.RateWarningColor,
                    },

                    // ── 業務規則 ──────────────────────────────────────
                    // 前端依此值決定警示顏色，不寫死 95 或 "#D62728"
                    rateWarningThreshold = theme.RateWarningThreshold,

                    // ── Y 軸業務設定（前端只讀此值，不寫死任何數字）──
                    yAxes = new object[]
                    {
                        // 左軸（產量）
                        new
                        {
                            title         = new { text = "實際/目標產量" },
                            opposite      = false,
                            magnification = theme.YLeftMagnification,
                            // null = 前端用倍率自動計算；有值 = 強制上限（如 RPT25013）
                            maxOverride   = yLeftMax,
                        },
                        // 右軸（達成率）
                        // tickAmount 由前端算：Math.round((ceiling-floor)/gap)+1
                        // 對應舊系統 get tickAmount() { return Math.round(...) + 1; }
                        new
                        {
                            title    = new { text = "達成率" },
                            opposite = true,
                            ceiling  = theme.YRightCeiling,   // 業務值：120
                            floor    = theme.YRightFloor,     // 業務值：0
                            gap      = theme.YRightGap,       // 業務值：15
                        }
                    }
                },
                data = new { categories, series }
            };
        }
    }
}
