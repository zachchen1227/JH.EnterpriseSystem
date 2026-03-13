using JH.EnterpriseSystem.Report.Core.Enums;
using JH.EnterpriseSystem.Report.Core.Models;

namespace JH.EnterpriseSystem.Report.Core.Registry
{
    /// <summary>
    /// 廠區圖表主題集中管理。
    ///
    /// ── 設計說明 ─────────────────────────────────────────────
    /// 現階段：所有廠區共用同一套預設值（來自舊系統 jhOptions 前端設定）。
    /// 未來：若特定廠區需要差異化，只需在對應廠區用 with { } 覆寫需要的屬性，
    ///       其餘屬性自動繼承 Default。
    ///
    /// 範例（未來客製化時的寫法）：
    ///   FactoryCode.BU1 → Default("BU1") with
    ///   {
    ///       ActualColor          = "#1A6B8A",
    ///       ReportYLeftMaxOverride = new Dictionary...{ ["RPT25013"] = 200_000 }
    ///   }
    /// ─────────────────────────────────────────────────────────
    /// </summary>
    public static class ChartThemeRegistry
    {
        // ════════════════════════════════════════════════════════
        //  預設值（對應舊系統 jhOptions 的全域設定）
        //  ── 顏色對照 ──────────────────────────────────────────
        //  ColorA = '#3A7BD5' → ActualColor          (實際產量長條)
        //  ColorB = '#FF7F0E' → TargetColor           (目標產量菱形)
        //  ColorC = '#2CA02C' → RateLineColor         (達成率線)
        //  ColorD = '#17BECF' → AccumulatedRateColor  (累計達成率虛線)
        //  ColorE = '#D62728' → RateWarningColor      (低於閾值警示)
        //  ── 業務規則對照 ───────────────────────────────────────
        //  jhOptions.yLeft.mag     = 1.5   → YLeftMagnification
        //  jhOptions.yRight.ceiling = 120  → YRightCeiling
        //  jhOptions.yRight.floor   = 0    → YRightFloor
        //  jhOptions.yRight.gap     = 15   → YRightGap
        //  fontColorUnder95                → RateWarningThreshold = 95
        // ════════════════════════════════════════════════════════
        public static FactoryChartTheme Default(string factoryName) => new()
        {
            FactoryName = factoryName,

            // 顏色（來自舊系統 ColorA~E）
            ActualColor = "#3A7BD5",  // ColorA：實際產量長條（藍）
            TargetColor = "#FF7F0E",  // ColorB：目標產量菱形（橘）
            RateLineColor = "#2CA02C",  // ColorC：達成率線（綠）
            AccumulatedRateColor = "#17BECF",  // ColorD：累計達成率虛線（青）
            RateWarningColor = "#D62728",  // ColorE：低於閾值警示（紅）
            Series2Color = "#AA4643",  // 備用系列（暗紅）

            // 業務規則（來自舊系統 jhOptions）
            RateWarningThreshold = 95.0,       // fontColorUnder95
            YLeftMagnification = 1.5,        // yLeft.mag
            YRightCeiling = 120.0,      // yRight.ceiling
            YRightFloor = 0.0,        // yRight.floor
            YRightGap = 15.0,       // yRight.gap

            // 報表專屬 Y 軸覆寫（預設無）
            ReportYLeftMaxOverride = new Dictionary<string, double>(),
        };

        // ════════════════════════════════════════════════════════
        //  各廠區設定
        //  現階段全部使用 Default，預留 with { } 覆寫接口
        // ═════════════��══════════════════════════════════════════
        public static readonly IReadOnlyDictionary<FactoryCode, FactoryChartTheme> All
            = new Dictionary<FactoryCode, FactoryChartTheme>
            {

                // ── BU1 ───────────────────────────────────────────
                [FactoryCode.BU1] = Default("BU1") with
                {                  
                    ReportYLeftMaxOverride = new Dictionary<string, double>
                    {
                        ["RPT25013"] = 200_000
                    }
                },

                // ── BU2 ───────────────────────────────────────────
                [FactoryCode.BU2] = Default("BU2") with
                {
                    ReportYLeftMaxOverride = new Dictionary<string, double>
                    {
                        ["RPT25013"] = 80_000
                    }
                },

                // ── JT1 ───────────────────────────────────────────
                [FactoryCode.JT1] = Default("JT1"),

                // ── JT2 ───────────────────────────────────────────
                [FactoryCode.JT2] = Default("JT2"),
            };

        /// <summary>取得指定廠區的圖表主題</summary>
        public static FactoryChartTheme Get(FactoryCode factory) => All[factory];
    }
}
