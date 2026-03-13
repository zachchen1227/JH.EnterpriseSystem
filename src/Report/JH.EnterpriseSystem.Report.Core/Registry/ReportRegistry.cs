using JH.EnterpriseSystem.Report.Core.Enums;
using JH.EnterpriseSystem.Report.Core.Models;
using JH.EnterpriseSystem.Report.Core.Models.ExtraParams;

namespace JH.EnterpriseSystem.Report.Core.Registry
{
    public static class ReportRegistry
    {
        // ── 預設 ColumnMappings（大多數週報共用）──────────────
        private static Dictionary<string, string> DefaultWeeklyColumns => new()
        {
            ["ActualQty"] = "TotalCount",
            ["TargetQty"] = "DispatchCount",
            ["AccumulatedRate"] = "Mavg",
        };

        // ── 無累計達成率的 ColumnMappings ─────────────────────
        private static Dictionary<string, string> NoAccWeeklyColumns => new()
        {
            ["ActualQty"] = "TotalCount",
            ["TargetQty"] = "DispatchCount",
        };

        private static readonly Dictionary<ReportCode, List<ChartDefinition>> Registry = new()
        {
            // ── 成型 ──────────────────────────────────────────
            [ReportCode.RPT25001] = [new ChartDefinition
            {
                ChartType   = "weekly-combo",
                Title       = "成型生產週報表",
                ExtraParams = new WeeklyExtraParams
                {
                    Days             = 6,
                    ColumnMappings   = DefaultWeeklyColumns,
                    ComputeDailyRate = true,
                    // SeriesConfig = null → 使用預設（長條+散點+折線+累計虛線）
                }
            }],
            [ReportCode.RPT25002] = [new ChartDefinition
            {
                ChartType   = "daily-combo",
                Title       = "每日成型產量達成狀況表",
                ExtraParams = new DailyExtraParams
                {                  
                    TargetDataType = "1271",
                    ActualDataType = "1273",
                }
            }],

            // ── 組底 ──────────────────────────────────────────
            [ReportCode.RPT25005] = [new ChartDefinition
            {
                ChartType   = "weekly-combo",
                Title       = "組底週報表",
                ExtraParams = new WeeklyExtraParams
                {
                    Days             = 6,
                    ColumnMappings   = NoAccWeeklyColumns,  // 無累計達成率
                    ComputeDailyRate = true,
                }
            }],
            [ReportCode.RPT25006] = [new ChartDefinition
            {
                ChartType   = "daily-combo",
                Title       = "每日組底產量達成狀況表",
                ExtraParams = new DailyExtraParams
                {
                    TargetDataType = "1292",
                    ActualDataType = "1209A",
                }
            }],

            // ── 針車 ──────────────────────────────────────────
            [ReportCode.RPT25007] = [new ChartDefinition
            {
                ChartType   = "weekly-combo",
                Title       = "針車週報表",
                ExtraParams = new WeeklyExtraParams
                {
                    Days             = 6,
                    ColumnMappings   = DefaultWeeklyColumns,
                    ComputeDailyRate = true,
                }
            }],
            [ReportCode.RPT25008] = [new ChartDefinition
            {
                ChartType   = "daily-bar",
                Title       = "每日針車產量達成狀況表",
                ExtraParams = new DailyExtraParams
                {
                    TargetDataType = "1261",
                    ActualDataType = "1209A",
                }
            }],

            // ── 噴漆 ────────────────────────────────────
            [ReportCode.RPT25009] = [new ChartDefinition
            {
                ChartType   = "weekly-combo",
                Title       = "噴漆週報表",
                ExtraParams = new WeeklyExtraParams
                {
                    Days             = 6,
                    ColumnMappings   = NoAccWeeklyColumns,
                    ComputeDailyRate = true,
                }
            }],
            [ReportCode.RPT25010] = [new ChartDefinition
            {
                ChartType   = "daily-combo",
                Title       = "每日噴漆產量達成狀況表",
                ExtraParams = new DailyExtraParams
                {
                    TargetDataType = "1281",
                    ActualDataType = "1209A",
                }
            }],

            // ── 印刷_高週波 ────────────────────────────────────
            [ReportCode.RPT250091] = [new ChartDefinition
            {
                ChartType   = "weekly-combo",
                Title       = "印刷_高週波週報表",
                ExtraParams = new WeeklyExtraParams
                {
                    Days             = 6,
                    ColumnMappings   = NoAccWeeklyColumns,
                    ComputeDailyRate = true,
                }
            }],
            [ReportCode.RPT250101] = [new ChartDefinition
            {
                ChartType   = "daily-combo",
                Title       = "每日印刷_高週波產量達成狀況表",
                ExtraParams = new DailyExtraParams
                {                    
                    TargetDataType = "1281",
                    ActualDataType = "1209A",
                }
            }],

            // ── 裁斷 ──────────────────────────────────────────
            [ReportCode.RPT25011] = [new ChartDefinition
            {
                ChartType   = "weekly-combo",
                Title       = "裁斷週報表",
                ExtraParams = new WeeklyExtraParams
                {
                    Days             = 6,
                    ColumnMappings   = NoAccWeeklyColumns,
                    ComputeDailyRate = true,
                }
            }],
            [ReportCode.RPT25012] = [new ChartDefinition
            {
                ChartType   = "daily-combo",
                Title       = "每日裁斷產量達成狀況表",
                ExtraParams = new DailyExtraParams
                {             
                    TargetDataType = "1251",
                    ActualDataType = "1209A",
                }
            }],
        };

        public static List<ChartDefinition> GetCharts(ReportCode code) =>
            Registry.GetValueOrDefault(code, []);

        public static IEnumerable<ReportCode> GetAllCodes() => Registry.Keys;
    }
}