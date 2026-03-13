using JH.EnterpriseSystem.Report.Core.Enums;
using JH.EnterpriseSystem.Report.Core.Models;

namespace JH.EnterpriseSystem.Report.Core.Registry
{
    public static class ReportRegistry
    {
        private static readonly Dictionary<ReportCode, List<ChartDefinition>> Registry = new()
        {
            // 週報範例：使用同一套 Provider，僅參數不同
            [ReportCode.RPT25001] = new List<ChartDefinition> {
            new ChartDefinition {
                ChartType = "combo",
                Title = "成型生產週報表",
                ExtraParams = new { Days = 7 }
            }
        },
            // 日報範例
            [ReportCode.RPT25002] = new List<ChartDefinition> {
            new ChartDefinition {
                ChartType = "combo",
                Title = "成型生產日報表",
                ExtraParams = new { Days = 1 }
            }

        }
        };
        public static List<ChartDefinition> GetCharts(ReportCode code) => Registry.GetValueOrDefault(code, new());
    }
}