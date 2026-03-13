using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Models
{
    /// <summary>
    /// 報表圖表定義。
    /// ChartType 格式："{granularity}-{visual}"
    ///   weekly-combo / daily-combo / daily-bar
    /// </summary>
    public class ChartDefinition
    {
        public string ChartType { get; set; } = "weekly-combo";
        public string Title { get; set; } = string.Empty;
        public object? ExtraParams { get; set; } = null;
    }

}
