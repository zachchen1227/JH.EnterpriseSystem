using JH.EnterpriseSystem.Report.Core.Interfaces;
using JH.EnterpriseSystem.Report.Core.Models;
using JH.EnterpriseSystem.Report.Core.Models.Domain;
using JH.EnterpriseSystem.Report.Core.Models.ExtraParams;

namespace JH.EnterpriseSystem.Report.Core.Services.ChartBuilders
{
    public class ComboChartBuilder : IChartBuilder
    {
        // ── 預設 Series 組合 ──────────────────────────────────
        private static readonly List<SeriesDefinition> DefaultWeekly =
        [
            new() { Name="實際產量",   DataField="ActualQty",
                    VisualType="column",  YAxis=0, ColorSource="ActualColor" },
            new() { Name="目標產量",   DataField="TargetQty",
                    VisualType="scatter", YAxis=0, ColorSource="TargetColor" },
            new() { Name="達成率",     DataField="DailyRate",
                    VisualType="line",    YAxis=1, ColorSource="RateLineColor",
                    ApplyWarningColor=true },
            new() { Name="累計達成率", DataField="AccumulatedRate",
                    VisualType="line",    YAxis=1, ColorSource="AccumulatedRateColor",
                    DashStyle="ShortDash" },
        ];

        private static readonly List<SeriesDefinition> DefaultDaily =
        [
            new() { Name="實際產量", DataField="ActualQty",
                    VisualType="column",  YAxis=0, ColorSource="ActualColor" },
            new() { Name="目標產量", DataField="TargetQty",
                    VisualType="scatter",  YAxis=0, ColorSource="TargetColor" },
            new() { Name="達成率",   DataField="Rates",
                    VisualType="scatter", YAxis=1, ColorSource="RateLineColor",
                    ApplyWarningColor=true },
        ];

        // ════════════════════════════════════════════════════════
        //  週報
        // ════════════════════════════════════════════════════════
        public object BuildWeekly(
            WeeklyProductionData data, FactoryChartTheme theme,
            string title, string reportCode,
            List<SeriesDefinition>? seriesConfig = null)
        {
            var series = Build(seriesConfig ?? DefaultWeekly, data.Series, theme);
            return Payload(data.Dates, series, theme, title, reportCode, isDaily: false);
        }

        // ════════════════════════════════════════════════════════
        //  日報
        // ════════════════════════════════════════════════════════
        public object BuildDaily(
            DailyTeamProductionData data, FactoryChartTheme theme,
            string title, string reportCode,
            List<SeriesDefinition>? seriesConfig = null)
        {
            var series = Build(seriesConfig ?? DefaultDaily, data.Series, theme);
            return Payload(data.Teams, series, theme, title, reportCode, isDaily: true);
        }

        // ── 核心：依 SeriesDefinition + 通用字典組裝 ──────────
        private static List<object> Build(
            List<SeriesDefinition> config,
            Dictionary<string, object> dataMap,
            FactoryChartTheme theme)
        {
            var result = new List<object>();
            foreach (var def in config)
            {
                if (!dataMap.TryGetValue(def.DataField, out var raw)) continue;
                if (raw is List<double> dl && dl.Count == 0) continue;
                if (raw is List<int> il && il.Count == 0) continue;

                var color = ColorFrom(theme, def.ColorSource);
                var points = ToPoints(raw, def, color, theme);

                result.Add(def.DashStyle is not null
                    ? (object)new
                    {
                        name = def.Name,
                        type = def.VisualType,
                        color,
                        yAxis = def.YAxis,
                        dashStyle = def.DashStyle,
                        data = points
                    }
                    : (object)new
                    {
                        name = def.Name,
                        type = def.VisualType,
                        color,
                        yAxis = def.YAxis,
                        data = points
                    });
            }
            return result;
        }

        private static List<object> ToPoints(
            object raw, SeriesDefinition def,
            string color, FactoryChartTheme theme)
        {
            if (def.ApplyWarningColor && raw is List<double> rates)
                return rates.Select(r => (object)new
                {
                    y = r,
                    isWarning = r < theme.RateWarningThreshold,
                    color = r < theme.RateWarningThreshold
                                    ? theme.RateWarningColor : color
                }).ToList();

            if (raw is List<double> d) return d.Cast<object>().ToList();
            if (raw is List<int> i) return i.Cast<object>().ToList();
            return [];
        }

        private static string ColorFrom(FactoryChartTheme t, string src) => src switch
        {
            "ActualColor" => t.ActualColor,
            "TargetColor" => t.TargetColor,
            "RateLineColor" => t.RateLineColor,
            "AccumulatedRateColor" => t.AccumulatedRateColor,
            "RateWarningColor" => t.RateWarningColor,
            "Series2Color" => t.Series2Color,
            _ => t.ActualColor
        };

        // ════════════════════════════════════════════════════════
        //  Payload：組裝完整 config 區塊（含新外觀設定）
        // ════════════════════════════════════════════════════════
        private static object Payload(
            List<string> categories, List<object> series,
            FactoryChartTheme theme, string title,
            string reportCode, bool isDaily)
        {
            double? yLeftMax = theme.ReportYLeftMaxOverride
                .TryGetValue(reportCode, out var v) ? v : null;

            return new
            {
                config = new
                {
                    type = isDaily ? "daily-combo" : "weekly-combo",
                    title = $"{title} - {theme.FactoryName}",
                    isDaily,

                    // ── 顏色 ──────────────────────────────────
                    colors = theme.ToColorArray(),
                    seriesColors = new
                    {
                        actual = theme.ActualColor,
                        target = theme.TargetColor,
                        rate = theme.RateLineColor,
                        accumulated = theme.AccumulatedRateColor,
                        warning = theme.RateWarningColor,
                    },
                    rateWarningThreshold = theme.RateWarningThreshold,

                    // ── Y 軸範圍設定 ──────────────────────────
                    yAxes = new object[]
                    {
                        new
                        {
                            title        = new { text = theme.YAxisTitleLeftText },
                            titleHtml    = theme.BuildYAxisTitleHtml(theme.YAxisTitleLeftText),
                            opposite     = false,
                            magnification  = theme.YLeftMagnification,
                            maxOverride    = yLeftMax,
                            labelColor   = theme.YLeftLabelColor,
                            titleOffsetX = 25,
                        },
                        new
                        {
                            title       = new { text = theme.YAxisTitleRightText },
                            titleHtml   = theme.BuildYAxisTitleHtml(theme.YAxisTitleRightText),
                            opposite    = true,
                            ceiling     = theme.YRightCeiling,
                            floor       = theme.YRightFloor,
                            gap         = theme.YRightGap,
                            // 刻度顏色與格式
                            labelColor  = theme.YRightLabelColor,
                            labelFormat = theme.YRightLabelFormat,
                            labelOffsetX = theme.YRightLabelOffsetX,
                            // 標題 x 偏移（對應舊系統 x:-2）
                            titleOffsetX = -2,
                        }
                    },

                    // ── X 軸外觀 ──────────────────────────────
                    xAxis = new
                    {
                        fontSize = isDaily ? theme.XAxisDailyFontSize : theme.XAxisWeeklyFontSize,
                        fontBold = theme.XAxisLabelBold,
                        labelRotation = isDaily ? theme.XAxisDailyLabelRotation : 0,
                    },

                    // ── Legend 外觀 ───────────────────────────
                    legend = new
                    {
                        verticalAlign = theme.LegendVerticalAlign,
                        fontSize = theme.LegendFontSize,
                        symbolSquare = theme.LegendSymbolSquare,
                        symbolWidth = theme.LegendSymbolWidth,
                        backgroundColor = theme.LegendBackgroundColor,
                    },

                    // ── Tooltip 外觀 ──────────────────────────
                    tooltip = new
                    {
                        transparent = theme.TooltipTransparent,
                        followPointer = theme.TooltipFollowPointer,
                        qtyUnit = theme.TooltipQtyUnit,
                    },

                    // ── DataLabel 外觀 ────────────────────────
                    dataLabel = new
                    {
                        columnInside = theme.ColumnDataLabelInside,
                        columnVerticalAlign = theme.ColumnDataLabelVerticalAlign,
                        noTextOutline = theme.DataLabelNoTextOutline,
                    },

                    // ── PlotOptions 行為 ──────────────────────
                    plotOptions = new
                    {
                        disableInactiveState = theme.DisableInactiveState,
                        enableZooming = theme.EnableZooming,
                        enableOverlapFix = theme.EnableDataLabelOverlapFix,
                    },

                    // ── 功能開關 ──────────────────────────────
                    enableExporting = theme.EnableExporting,
                },
                data = new { categories, series }
            };
        }
    }
}