using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JH.EnterpriseSystem.Report.Core.Models.Domain
{
    /// <summary>
    /// 週報 Domain Model。
    /// X 軸 = 日期，呈現一週趨勢。
    /// 對應舊系統 ToShow1/5/7/9/11/14 的資料結構。
    /// </summary>
    public class WeeklyProductionData
    {
        /// <summary>X 軸日期標籤，例如 ["03/06","03/07",...]</summary>
        public List<string> Dates { get; set; } = [];

        /// <summary>每日目標產量（對應舊系統 DispatchCount）</summary>
        public List<int> TargetQty { get; set; } = [];

        /// <summary>每日實際產量（對應舊系統 TotalCount）</summary>
        public List<int> ActualQty { get; set; } = [];

        /// <summary>每日達成率（%），例如 95.3</summary>
        public List<double> DailyRate { get; set; } = [];

        /// <summary>累計達成率（%）（對應舊系統 Mavg）</summary>
        public List<double> AccumulatedRate { get; set; } = [];

        public bool HasAccumulatedRate => AccumulatedRate.Count > 0;
    }
}
