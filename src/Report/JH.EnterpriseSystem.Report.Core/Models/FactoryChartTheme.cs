using JH.EnterpriseSystem.Report.Core.Enums;

namespace JH.EnterpriseSystem.Report.Core.Models
{
    /// <summary>
    /// 廠區圖表配色 + 業務規則 + 外觀設定（完整版）。
    ///
    /// 設計為 record，支援 with 運算子覆寫：
    ///   var theme = ChartThemeRegistry.Default("BU1")
    ///                   with { ActualColor = "#FF0000" };
    /// </summary>
    public record FactoryChartTheme
    {
        public string FactoryName { get; init; } = string.Empty;

        // ════════════════════════════════════════════════════════
        //  各系列顏色
        // ════════════════════════════════════════════════════════

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

        // ════════════════════════════════════════════════════════
        //  業務規則
        // ════════════════════════════════════════════════════════

        /// <summary>
        /// 達成率警示閾值（%）。低於此值的點改用 RateWarningColor。
        /// 對應舊系統 fontColorUnder95，預設 95。
        /// </summary>
        public double RateWarningThreshold { get; init; } = 95.0;

        /// <summary>
        /// 左 Y 軸放大倍率，避免長條頂到圖頂。
        /// 對應舊系統 maxx = Math.max(...) * 1.18 → 使用 1.2
        /// </summary>
        public double YLeftMagnification { get; init; } = 1.2;

        /// <summary>右 Y 軸上限（預設 120）</summary>
        public double YRightCeiling { get; init; } = 120;

        /// <summary>右 Y 軸下限（預設 0）</summary>
        public double YRightFloor { get; init; } = 0;

        /// <summary>右 Y 軸每格間距（決定 tickAmount）</summary>
        public double YRightGap { get; init; } = 15;

        /// <summary>特定報表的左 Y 軸強制上限。key = reportCode</summary>
        public Dictionary<string, double> ReportYLeftMaxOverride { get; init; } = [];

        // ════════════════════════════════════════════════════════
        //  【NEW】Y 軸標題外觀
        // ════════════════════════════════════════════════════════

        /// <summary>
        /// Y 軸標題文字大小（px）。
        /// 對應舊系統 font-size: 20px
        /// </summary>
        public int YAxisTitleFontSize { get; init; } = 20;

        /// <summary>
        /// Y 軸標題是否使用直書中文（writing-mode: vertical-lr; text-orientation: upright）。
        /// 對應舊系統 yLeftTitleHtml / yRightTitleHtml 的 inline style。
        /// </summary>
        public bool YAxisTitleVertical { get; init; } = true;

        /// <summary>
        /// 左 Y 軸標題文字顏色（預設繼承）。
        /// </summary>
        public string YAxisLeftTitleColor { get; init; } = "#333333";

        /// <summary>
        /// 右 Y 軸標題文字顏色（預設繼承）。
        /// </summary>
        public string YAxisRightTitleColor { get; init; } = "#333333";

        /// <summary>左 Y 軸標題文字（可依廠區覆寫）</summary>
        public string YAxisTitleLeftText { get; init; } = "實際/目標產量";

        /// <summary>右 Y 軸標題文字（可依廠區覆寫）</summary>
        public string YAxisTitleRightText { get; init; } = "達成率";

        /// <summary>
        /// 組裝 Y 軸標題 HTML（直書中文，對應舊系統 writing-mode: vertical-lr）
        /// </summary>
        public string BuildYAxisTitleHtml(string text)
        {
            if (!YAxisTitleVertical)
                return $"<div style='font-weight:bold;font-size:{YAxisTitleFontSize}px'>{text}</div>";

            return $"<div style='font-weight:bold;font-size:{YAxisTitleFontSize}px;" +
                   $"writing-mode:vertical-lr;text-orientation:upright'>{text}</div>";
        }

        // ════════════════════════════════════════════════════════
        //  【NEW】Y 軸刻度標籤外觀
        // ════════════════════════════════════════════════════════

        /// <summary>
        /// 左 Y 軸刻度標籤顏色。
        /// 對應舊系統 YLabelTextColorPrimary = "rgb(56,80,212)"
        /// </summary>
        public string YLeftLabelColor { get; init; } = "rgb(56,80,212)";

        /// <summary>
        /// 右 Y 軸刻度標籤顏色。
        /// 對應舊系統 YLabelTextColorSecondary = "rgb(74,102,255)"
        /// </summary>
        public string YRightLabelColor { get; init; } = "rgb(74,102,255)";

        /// <summary>
        /// 右 Y 軸標籤格式（含百分比符號）。
        /// 對應舊系統 pctFormat = "{value} %"
        /// </summary>
        public string YRightLabelFormat { get; init; } = "{value} %";

        /// <summary>
        /// 右 Y 軸標籤 X 偏移（px）。
        /// 對應舊系統 x: 13
        /// </summary>
        public int YRightLabelOffsetX { get; init; } = 13;

        // ════════════════════════════════════════════════════════
        //  【NEW】X 軸外觀
        // ════════════════════════════════════════════════════════

        /// <summary>
        /// 週報 X 軸標籤字體大小（px）。
        /// 對應舊系統 fontSize: "15px"
        /// </summary>
        public int XAxisWeeklyFontSize { get; init; } = 15;

        /// <summary>
        /// 日報 X 軸標籤字體大小（px）。
        /// 對應舊系統 fontSize: "13.5px"
        /// </summary>
        public int XAxisDailyFontSize { get; init; } = 14;

        /// <summary>
        /// X 軸標籤是否粗體。
        /// 對應舊系統 fontWeight: "bold"
        /// </summary>
        public bool XAxisLabelBold { get; init; } = true;

        /// <summary>
        /// 日報 X 軸標籤旋轉角度（度）。
        /// 對應舊系統 rotation: -68
        /// </summary>
        public int XAxisDailyLabelRotation { get; init; } = -68;

        // ════════════════════════════════════════════════════════
        //  【NEW】Legend（圖例）外觀
        // ════════════════════════════════════════════════════════

        /// <summary>
        /// 圖例垂直對齊位置。
        /// 對應舊系統 verticalAlign: "top"
        /// </summary>
        public string LegendVerticalAlign { get; init; } = "top";

        /// <summary>
        /// 圖例 item 字體大小（px）。
        /// 對應舊系統 fontSize: "20px"
        /// </summary>
        public int LegendFontSize { get; init; } = 20;

        /// <summary>
        /// 圖例符號是否為方形（radius=0）。
        /// 對應舊系統 symbolRadius: 0
        /// </summary>
        public bool LegendSymbolSquare { get; init; } = true;

        /// <summary>
        /// 週報圖例符號寬度（px）。
        /// 對應舊系統 symbolWidth: 20
        /// </summary>
        public int LegendSymbolWidth { get; init; } = 20;

        /// <summary>
        /// 圖例背景色。
        /// 對應舊系統 "rgba(255,255,255,0.25)"
        /// </summary>
        public string LegendBackgroundColor { get; init; } = "rgba(255,255,255,0.25)";

        // ════════════════════════════════════════════════════════
        //  【NEW】Tooltip 外觀
        // ════════════════════════════════════════════════════════

        /// <summary>
        /// Tooltip 背景是否透明。
        /// 對應舊系統 backgroundColor: "transparent"
        /// </summary>
        public bool TooltipTransparent { get; init; } = true;

        /// <summary>
        /// Tooltip 是否跟隨滑鼠。
        /// 對應舊系統 followPointer: true
        /// </summary>
        public bool TooltipFollowPointer { get; init; } = true;

        /// <summary>
        /// Tooltip 的產量單位文字（Pair / 雙 / etc.）
        /// 對應舊系統 valueSuffix: " Pair"
        /// </summary>
        public string TooltipQtyUnit { get; init; } = "Pair";

        // ════════════════════════════════════════════════════════
        //  【NEW】DataLabel（資料標籤）外觀
        // ════════════════════════════════════════════════════════

        /// <summary>
        /// 長條圖資料標籤是否顯示在長條內部。
        /// 對應舊系統 inside: true
        /// </summary>
        public bool ColumnDataLabelInside { get; init; } = true;

        /// <summary>
        /// 長條圖資料標籤垂直對齊（"bottom" / "middle" / "top"）。
        /// 對應舊系統 verticalAlign: 'bottom'
        /// </summary>
        public string ColumnDataLabelVerticalAlign { get; init; } = "bottom";

        /// <summary>
        /// 是否關閉資料標籤文字描邊（PDF 輸出用）。
        /// 對應舊系統 textOutline: 'none'
        /// </summary>
        public bool DataLabelNoTextOutline { get; init; } = true;

        // ════════════════════════════════════════════════════════
        //  【NEW】PlotOptions 行為
        // ════════════════════════════════════════════════════════

        /// <summary>
        /// Hover 時是否停用其他系列淡化效果。
        /// 對應舊系統 series.states.inactive.enabled: false
        /// </summary>
        public bool DisableInactiveState { get; init; } = true;

        /// <summary>
        /// 是否開啟 XY 縮放功能。
        /// 對應舊系統 chart.zooming.type: "xy"
        /// </summary>
        public bool EnableZooming { get; init; } = false;

        /// <summary>
        /// 是否啟用 dataLabel 重疊防止（render function）。
        /// 對應舊系統 chart.events.render 防重疊邏輯
        /// </summary>
        public bool EnableDataLabelOverlapFix { get; init; } = true;

        // ════════════════════════════════════════════════════════
        //  【NEW】全螢幕 / 匯出功能
        // ════════════════════════════════════════════════════════

        /// <summary>
        /// 是否顯示全螢幕 / 匯出按鈕。
        /// 對應舊系統 exporting + viewFullscreen 設定
        /// </summary>
        public bool EnableExporting { get; init; } = true;

        // ═════════��══════════════════════════════════════════════
        //  Helper
        // ════════════════════════════════════════════════════════
        public string[] ToColorArray() =>
        [
            ActualColor, TargetColor, RateLineColor,
            AccumulatedRateColor, RateWarningColor, Series2Color
        ];
    }
}