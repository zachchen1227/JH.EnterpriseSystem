using JH.EnterpriseSystem.Report.Core.Enums;
using JH.EnterpriseSystem.Report.Core.Models;

namespace JH.EnterpriseSystem.Report.Core.Registry
{
    /// <summary>
    /// 報表定義集中管理。
    /// ChartType 格式："{granularity}-{visual}"
    /// weekly-combo / daily-combo / daily-bar
    /// </summary>
    public static class ReportRegistry
    {
        private static readonly Dictionary<ReportCode, List<ChartDefinition>> Registry = new()
        {
            [ReportCode.RPT25001] = [new ChartDefinition
            {
                ChartType   = "weekly-combo",
                Title       = "成型生產週報表",
                ExtraParams = new { Days = 6 }
            }],
            [ReportCode.RPT25002] = [new ChartDefinition
            {
                ChartType   = "daily-combo",
                Title       = "每日成型產量達成狀況表",
                ExtraParams = new { TargetDataType = "1271", ActualDataType = "1273" }
            }],

            [ReportCode.RPT25003] = [new ChartDefinition
            {
                ChartType   = "weekly-combo",
                Title       = "成型RFT週報表",
                ExtraParams = null
            }],
            [ReportCode.RPT25004] = [new ChartDefinition
            {
                ChartType   = "daily-combo",
                Title       = "成型RFT日報表",
                ExtraParams = null
            }],

            [ReportCode.RPT25005] = [new ChartDefinition
            {
                ChartType   = "weekly-combo",
                Title       = "組底週報表",
                ExtraParams = new { Days = 6 }
            }],
            [ReportCode.RPT25006] = [new ChartDefinition
            {
                ChartType   = "daily-combo",
                Title       = "每日組底產量達成狀況表",
                ExtraParams = new { TargetDataType = "1292", ActualDataType = "1209A" }
            }],

            [ReportCode.RPT25007] = [new ChartDefinition
            {
                ChartType   = "weekly-combo",
                Title       = "針車週報表",
                ExtraParams = new { Days = 6 }
            }],
            [ReportCode.RPT25008] = [new ChartDefinition
            {
                ChartType   = "daily-combo",
                Title       = "每日針車產量達成狀況表",
                ExtraParams = new { TargetDataType = "1261", ActualDataType = "1209A" }
            }],

            [ReportCode.RPT25009] = [new ChartDefinition
            {
                ChartType   = "weekly-combo",
                Title       = "噴漆週報表",
                ExtraParams = new { Days = 6 }
            }],
            [ReportCode.RPT25010] = [new ChartDefinition
            {
                ChartType   = "daily-combo",
                Title       = "每日噴漆產量達成狀況表",
                ExtraParams = new { TargetDataType = "1281", ActualDataType = "1209A" }
            }],

            [ReportCode.RPT250091] = [new ChartDefinition
            {
                ChartType   = "weekly-combo",
                Title       = "印刷_高週波週報表",
                ExtraParams = new { Days = 6 }
            }],
            [ReportCode.RPT250101] = [new ChartDefinition
            {
                ChartType   = "daily-combo",
                Title       = "每日印刷_高週波產量達成狀況表",
                ExtraParams = new { TargetDataType = "1281", ActualDataType = "1209A" }
            }],

            [ReportCode.RPT25011] = [new ChartDefinition
            {
                ChartType   = "weekly-combo",
                Title       = "裁斷週報表",
                ExtraParams = new { Days = 6 }
            }],
            [ReportCode.RPT25012] = [new ChartDefinition
            {
                ChartType   = "daily-combo",
                Title       = "每日裁斷產量達成狀況表",
                ExtraParams = new { TargetDataType = "1251", ActualDataType = "1209A" }
            }],

            [ReportCode.RPT25013] = [new ChartDefinition
            {
                ChartType   = "weekly-combo",
                Title       = "成行配套週報表",
                ExtraParams = new { Days = 6 }
            }],
            [ReportCode.RPT25014] = [new ChartDefinition
            {
                ChartType   = "daily-combo",
                Title       = "當日成型時段產量",
                ExtraParams = null
            }],
        };

        public static List<ChartDefinition> GetCharts(ReportCode code) =>
            Registry.GetValueOrDefault(code, []);

        public static IEnumerable<ReportCode> GetAllCodes() => Registry.Keys;
    }
}